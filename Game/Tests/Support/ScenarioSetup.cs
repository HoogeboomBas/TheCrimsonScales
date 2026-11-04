using System;
using System.Collections.Generic;
using Godot;

namespace CrimsonScales.Tests;

/// <summary>A scenario-owned reference resolved against the active gameplay session.</summary>
public abstract class UnitHandle
{
	internal ScenarioSetup Setup { get; }
	internal string Name { get; }

	internal UnitHandle(ScenarioSetup setup, string name)
	{
		Setup = setup;
		Name = name;
	}

	internal Figure Resolve() => (Setup.ActiveGame
		?? throw new InvalidOperationException("Start the scenario before accessing its units; disposed units are unavailable."))
		.Resolve(this);
}

public sealed class UnitHandle<T> : UnitHandle where T : Figure
{
	internal UnitHandle(ScenarioSetup setup, string name) : base(setup, name) { }
	public T Unit => (T)Resolve();
}

public sealed class ScenarioSetup
{
	internal TestGame ActiveGame { get; set; }
	internal sealed record CharacterSpec(string Name, ClassModel Model, Vector2I Position)
	{
		public List<AbilityCardModel> Cards { get; } = [];
	}
	internal sealed record MonsterSpec(string Name, MonsterModel Model, Vector2I Position, int? Health, MonsterType Type);
	internal sealed record ConditionSpec(string Name, ConditionModel Model, string Giver);
	internal sealed record OverlaySpec(string ScenePath, Vector2I Position, int Rotation);

	internal List<CharacterSpec> Characters { get; } = [];
	internal List<MonsterSpec> Monsters { get; } = [];
	internal List<ConditionSpec> Conditions { get; } = [];
	internal List<OverlaySpec> Overlays { get; } = [];
	internal int Seed { get; private set; } = 42;
	internal bool NeutralModifiers { get; private set; }

	public static ScenarioSetup InTestRoom() => new();

	public ScenarioSetup WithSeed(int seed)
	{
		Seed = seed;
		return this;
	}

	public ScenarioSetup WithCharacter<TClass>(string name, Vector2I at) where TClass : ClassModel
	{
		ValidateFigure(name, at);
		Characters.Add(new CharacterSpec(name, ModelDB.Class<TClass>(), at));
		return this;
	}

	public ScenarioSetup WithCharacter<TClass>(out UnitHandle<Character> character, Vector2I at) where TClass : ClassModel
	{
		string name = NextName("character");
		WithCharacter<TClass>(name, at);
		character = new UnitHandle<Character>(this, name);
		return this;
	}

	public ScenarioSetup WithCard<TCard>(UnitHandle<Character> character) where TCard : AbilityCardModel
	{
		ValidateHandle(character);
		return WithCard<TCard>(character.Name);
	}

	public ScenarioSetup WithCard<TCard>(string character) where TCard : AbilityCardModel
	{
		CharacterSpec spec = Characters.Find(spec => spec.Name == character)
			?? throw new ArgumentException($"Declare character '{character}' before adding its cards.");
		spec.Cards.Add(ModelDB.AbilityCard<TCard>());
		return this;
	}

	public ScenarioSetup WithMonster<TMonster>(string name, Vector2I at, int? health = null, MonsterType type = MonsterType.Normal)
		where TMonster : MonsterModel
	{
		ValidateFigure(name, at);
		if(health <= 0 || type == MonsterType.None)
		{
			throw new ArgumentException("A test monster must be alive and have a standee type.");
		}
		Monsters.Add(new MonsterSpec(name, ModelDB.Monster<TMonster>(), at, health, type));
		return this;
	}

	public ScenarioSetup WithMonster<TMonster>(out UnitHandle<Monster> monster, Vector2I at, int? health = null,
		MonsterType type = MonsterType.Normal) where TMonster : MonsterModel
	{
		string name = NextName("monster");
		WithMonster<TMonster>(name, at, health, type);
		monster = new UnitHandle<Monster>(this, name);
		return this;
	}

	public ScenarioSetup WithCondition(UnitHandle figure, ConditionModel condition, UnitHandle giver = null)
	{
		ValidateHandle(figure);
		if(giver != null) ValidateHandle(giver);
		return WithCondition(figure.Name, condition, giver?.Name);
	}

	public ScenarioSetup WithCondition(string figure, ConditionModel condition, string giver = null)
	{
		Conditions.Add(new ConditionSpec(figure, condition, giver));
		return this;
	}

	/// <summary>Uses the real HexObject scene, including its footprint and map registration.</summary>
	public ScenarioSetup WithOverlay(string scenePath, Vector2I at, int rotation = 0)
	{
		ValidatePosition(at);
		Overlays.Add(new OverlaySpec(scenePath, at, rotation));
		return this;
	}

	/// <summary>Replace character modifier decks with +0 cards for exact damage assertions.</summary>
	public ScenarioSetup WithNeutralAttackModifiers()
	{
		NeutralModifiers = true;
		return this;
	}

	private string NextName(string prefix)
	{
		int index = 1;
		while(Characters.Exists(spec => spec.Name == $"{prefix} {index}") || Monsters.Exists(spec => spec.Name == $"{prefix} {index}"))
		{
			index++;
		}
		return $"{prefix} {index}";
	}

	private void ValidateHandle(UnitHandle handle)
	{
		if(handle == null || !ReferenceEquals(handle.Setup, this))
		{
			throw new ArgumentException("The unit handle belongs to a different scenario.", nameof(handle));
		}
	}

	private void ValidateFigure(string name, Vector2I at)
	{
		ValidatePosition(at);
		if(string.IsNullOrWhiteSpace(name) || Characters.Exists(spec => spec.Name == name) || Monsters.Exists(spec => spec.Name == name))
		{
			throw new ArgumentException($"Figure name '{name}' must be nonempty and unique.");
		}
		if(Characters.Exists(spec => spec.Position == at) || Monsters.Exists(spec => spec.Position == at))
		{
			throw new ArgumentException($"Hex {at} is already occupied by a figure.");
		}
	}

	private static void ValidatePosition(Vector2I at)
	{
		if(at.X < 0 || at.X >= 10 || at.Y < 0 || at.Y >= 10)
		{
			throw new ArgumentOutOfRangeException(nameof(at), "Test-room coordinates range from (0,0) to (9,9).");
		}
	}
}
