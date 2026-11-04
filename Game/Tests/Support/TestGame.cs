using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;

namespace CrimsonScales.Tests;

/// <summary>A real gameplay scene with explicit setup and card-half execution, without the automatic round loop.</summary>
public sealed class TestGame : IAsyncDisposable
{
	private readonly Dictionary<string, Figure> _figures = new();
	private readonly GameController _controller;
	private readonly ScenarioSetup _setup;
	private readonly ScriptedChoices _choices;
	private bool _playing;
	private bool _disposed;

	public Map Map => _controller.Map;
	public Figure Figure(string name) => _figures.TryGetValue(name, out Figure figure)
		? figure : throw new ArgumentException($"Unknown test figure '{name}'.");
	public Character Character(string name) => Figure(name) as Character
		?? throw new ArgumentException($"'{name}' is not a character.");

	private TestGame(GameController controller, ScenarioSetup setup)
	{
		_controller = controller;
		_setup = setup;
		_choices = new ScriptedChoices(this);
		setup.ActiveGame = this;
	}

	internal Figure Resolve(UnitHandle handle)
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		if(handle == null || !ReferenceEquals(handle.Setup, _setup))
		{
			throw new ArgumentException("The unit handle belongs to a different scenario.", nameof(handle));
		}
		return Figure(handle.Name);
	}

	public static async Task<TestGame> Start(ScenarioSetup setup)
	{
		if(!ProjectSettings.GetSetting("testing/scenario_harness", false).AsBool())
		{
			throw new InvalidOperationException("Run this suite through the separate Game.Tests Godot project.");
		}
		if(GameController.Instance != null)
		{
			throw new InvalidOperationException("Dispose the previous TestGame before starting another. Gameplay tests run sequentially.");
		}
		if(setup.Characters.Count == 0)
		{
			throw new ArgumentException("A card scenario needs at least one character.");
		}

		SavedCampaign campaign = SavedCampaign.New("Scenario tests", StartingGroup.Militants);
		foreach(ScenarioSetup.CharacterSpec spec in setup.Characters)
		{
			campaign.AddCharacter(spec.Model, null, spec.Name);
			if(spec.Cards.Count != 0)
			{
				SavedCharacter saved = campaign.Characters[^1];
				saved.AvailableAbilityCards.Clear();
				saved.HandAbilityCardIndices.Clear();
				foreach(AbilityCardModel card in spec.Cards)
				{
					saved.HandAbilityCardIndices.Add(saved.AvailableAbilityCards.Count);
					saved.AvailableAbilityCards.Add(new SavedAbilityCard(card));
				}
			}
		}
		campaign.SetSavedScenario(new SavedScenario
		{
			Id = Guid.NewGuid(),
			ScenarioModelId = ModelDB.Scenario<TestScenario>().Id.ToString(),
			Seed = setup.Seed,
			ScenarioLevel = 1,
			IsOnline = false
		});

		GameController controller = SceneLoader.InstantiateScene<GameController>("res://Scenes/Scenario/Game.tscn");
		TestGame game = new TestGame(controller, setup);
		controller.Configure(new GameSceneRequest(campaign)
		{
			ScenarioScenePath = "res://Fixtures/TestRoom.tscn",
			RunScenarioPhases = false,
			DecisionSource = game._choices
		});
		try
		{
			((SceneTree)Engine.GetMainLoop()).Root.AddChild(controller);
			foreach(Room room in game.Map.Rooms)
			{
				await room.Reveal(null, null, true);
			}
			foreach(ScenarioSetup.OverlaySpec spec in setup.Overlays)
			{
				HexObject overlay = SceneLoader.InstantiateScene<HexObject>(spec.ScenePath);
				if(overlay is Door)
				{
					overlay.Free();
					throw new NotSupportedException("Doors require separate room topology; use obstacles or terrain in the initial single-room harness.");
				}
				game.Map.AddChild(overlay);
				await overlay.Init(game.Map.GetHex(spec.Position), spec.Rotation);
			}
			for(int i = 0; i < setup.Characters.Count; i++)
			{
				ScenarioSetup.CharacterSpec spec = setup.Characters[i];
				Character character = spec.Model.Scene.Instantiate<Character>();
				game.Map.AddChild(character);
				await character.Init(game.Map.GetHex(spec.Position));
				await character.Spawn(campaign.Characters[i], i);
				controller.CharacterManager.Characters.Add(character);
				game._figures.Add(spec.Name, character);
				await character.OnScenarioSetupCompleted();
				if(setup.NeutralModifiers)
				{
					AMDCardDeck deck = character.AMDCardDeck;
					deck.DrawPile.Clear();
					deck.DiscardPile.Clear();
					for(int j = 0; j < 20; j++)
					{
						deck.AddCard(new AMDCard(ModelDB.AMDCard<PlusZeroAMDCard>(), deck.Owner, character), false);
					}
				}
			}
			foreach(ScenarioSetup.MonsterSpec spec in setup.Monsters)
			{
				Monster monster = await game.Map.CreateMonster(spec.Model, spec.Type, spec.Position, false);
				if(monster == null)
				{
					throw new InvalidOperationException($"Could not spawn monster '{spec.Name}'.");
				}
				game._figures.Add(spec.Name, monster);
				if(spec.Health is int health)
				{
					monster.SetMaxHealth(health);
					monster.SetHealth(health);
				}
			}
			foreach(ScenarioSetup.ConditionSpec spec in setup.Conditions)
			{
				if(spec.Model.RequiresGiver && spec.Giver == null)
				{
					throw new ArgumentException($"Starting condition {spec.Model.Name} requires a giver.");
				}
				await game.Figure(spec.Name).AddCondition(spec.Model, spec.Giver == null ? null : game.Figure(spec.Giver));
			}
			game.Map.UpdateWallLines();
			controller.CameraController.SetProcess(false);
			controller.CameraController.Camera.GlobalPosition = game.Map.ContainerRect.GetCenter();
			controller.CameraController.Camera.Zoom = Vector2.One * 0.45f;
			GameController.SetFastForward(!ShowGame);
			await game.PauseForVisibleRun();
			return game;
		}
		catch
		{
			await game.DisposeAsync();
			throw;
		}
	}

	public Task PlayCard<TCard>(UnitHandle<Character> character, AbilityCardSection section, Action<ScriptedChoices> choices = null)
		where TCard : AbilityCardModel
	{
		Resolve(character);
		return PlayCard<TCard>(character.Name, section, choices);
	}

	public async Task PlayCard<TCard>(string characterName, AbilityCardSection section, Action<ScriptedChoices> choices = null)
		where TCard : AbilityCardModel
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		if(_playing)
		{
			throw new InvalidOperationException("Await the previous card before playing another.");
		}
		Character character = Character(characterName);
		AbilityCard card = character.Cards.FirstOrDefault(card => card.Model is TCard && card.CardState == CardState.Hand)
			?? throw new InvalidOperationException($"{characterName} has no {typeof(TCard).Name} in hand.");
		AbilityCardSide side = section switch
		{
			AbilityCardSection.Top => card.Top,
			AbilityCardSection.Bottom => card.Bottom,
			AbilityCardSection.BasicTop => card.BasicTop,
			AbilityCardSection.BasicBottom => card.BasicBottom,
			_ => throw new ArgumentOutOfRangeException(nameof(section))
		};
		_playing = true;
		try
		{
			choices?.Invoke(_choices);
			Map.SetTurnTaker(character);
			await card.SetCardState(CardState.Playing);
			await side.Perform(character).AsTask().WaitAsync(TimeSpan.FromSeconds(15));
			_choices.AssertConsumed();
			await PauseForVisibleRun();
		}
		finally
		{
			_playing = false;
			_choices.Clear();
		}
	}

	public async ValueTask DisposeAsync()
	{
		if(_disposed) return;
		_disposed = true;
		if(ReferenceEquals(_setup.ActiveGame, this)) _setup.ActiveGame = null;
		if(GodotObject.IsInstanceValid(_controller))
		{
			SignalAwaiter exited = _controller.ToSignal(_controller, Node.SignalName.TreeExited);
			_controller.QueueFree();
			await exited;
		}
	}

	private static bool ShowGame => System.Environment.GetEnvironmentVariable("CRIMSON_TEST_SHOW_GAME") == "1";
	private async Task PauseForVisibleRun()
	{
		if(ShowGame)
		{
			await _controller.ToSignal(_controller.GetTree().CreateTimer(0.8), SceneTreeTimer.SignalName.Timeout);
		}
	}
}
