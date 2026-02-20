using System.Linq;
using Fractural.Tasks;

public class Scenario038 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario038.tscn";
	public override int ScenarioNumber => 38;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<PersonalQuestScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() =>
		new CustomScenarioGoals("Kill all enemy monsters and destroy at least two altars to win this scenario.");

	private Door _door2;
	private Door _door3;
	private Objective _altarOfMystification;
	private Objective _altarOfDisorientation;
	private Objective _altarOfPerplexity;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<WovenPlateArmor>());

		_door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();
		_door3 = GameController.Instance.Map.GetMarker(Marker.Type._3).GetHexObject<Door>();
		_altarOfMystification = GameController.Instance.Map.GetMarker(Marker.Type.a).GetHexObject<Objective>();
		_altarOfDisorientation = GameController.Instance.Map.GetMarker(Marker.Type.b).GetHexObject<Objective>();
		_altarOfPerplexity = GameController.Instance.Map.GetMarker(Marker.Type.c).GetHexObject<Objective>();

		int firstThirdAltarHealth =
			(GameController.Instance.SavedScenario.ScenarioLevel + 3) * GameController.Instance.SavedCampaign.Characters.Count;
		int secondAltarHealth = (GameController.Instance.SavedScenario.ScenarioLevel + 4) * GameController.Instance.SavedCampaign.Characters.Count;
		_altarOfMystification.Init(firstThirdAltarHealth, "Altar of Mystification");
		_altarOfPerplexity.Init(firstThirdAltarHealth, "Altar of Perplexity");
		_altarOfDisorientation.Init(secondAltarHealth, "Altar of Disorientation");

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => KillAllEnemiesScenarioGoals.NoEnemiesRemaining(false) && (_altarOfMystification.IsDestroyed ? 1 : 0) +
				(_altarOfDisorientation.IsDestroyed ? 1 : 0) + (_altarOfPerplexity.IsDestroyed ? 1 : 0) >= 2,
			async parameters =>
			{
				await ((CustomScenarioGoals)ScenarioGoals).Win();
			}
		);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		UpdateScenarioText();
		if(parameters.Room == GameController.Instance.Map.Rooms[1])
		{
			foreach(Figure livingSpirit in GameController.Instance.Map.Figures.Where(figure =>
				        figure is Monster monster && monster.MonsterModel is LivingSpirit))
			{
				await AbilityCmd.AddCondition(null, livingSpirit, Conditions.Invisible);
			}

			ScenarioEvents.AfterRemoveConditionEvent.Subscribe(this,
				conditionParameters => conditionParameters.Figure is Monster monster && monster.MonsterModel is LivingSpirit &&
				                       conditionParameters.Condition == Conditions.Invisible,
				async conditionParameters =>
				{
					await AbilityCmd.AddCondition(null, conditionParameters.Figure, Conditions.Invisible);
				});

			ScenarioEvents.FigureKilledEvent.Subscribe(this, _altarOfMystification,
				figureKilledParameters => figureKilledParameters.Figure == _altarOfMystification,
				async figureKilledParameters =>
				{
					ScenarioEvents.AfterRemoveConditionEvent.Unsubscribe(this);
					await _door2.Unlock();
					UpdateScenarioText();
				});

			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				figureKilledParameters => figureKilledParameters.Figure is Monster monster && monster.MonsterModel is LivingSpirit,
				async figureKilledParameters =>
				{
					await _door3.Unlock();
					UpdateScenarioText();
				});
		}
		else if(parameters.Room == GameController.Instance.Map.Rooms[2])
		{
			ScenarioEvents.DuringAttackEvent.Subscribe(this, _altarOfDisorientation,
				attackParameters => RangeHelper.Distance(attackParameters.Performer.Hex, _altarOfDisorientation.Hex) == 1,
				async attackParameters =>
				{
					attackParameters.AbilityState.SingleTargetAdjustAttackValue(1);
					await GDTask.CompletedTask;
				});
			ScenarioEvents.FigureKilledEvent.Subscribe(this, _altarOfDisorientation,
				figureKilledParameters => figureKilledParameters.Figure == _altarOfDisorientation,
				async figureKilledParameters =>
				{
					UpdateScenarioText();
					await GDTask.CompletedTask;
				});
		}
		else if(parameters.Room == GameController.Instance.Map.Rooms[3])
		{
			ScenarioEvents.DuringAttackEvent.Subscribe(this, _altarOfPerplexity,
				attackParameters => RangeHelper.Distance(attackParameters.Performer.Hex, _altarOfPerplexity.Hex) == 1,
				async attackParameters =>
				{
					attackParameters.AbilityState.SingleTargetSetHasAdvantage();
					await GDTask.CompletedTask;
				});
			ScenarioEvents.FigureKilledEvent.Subscribe(this, _altarOfPerplexity,
				figureKilledParameters => figureKilledParameters.Figure == _altarOfPerplexity,
				async figureKilledParameters =>
				{
					UpdateScenarioText();
					await GDTask.CompletedTask;
				});
		}
	}

	private void UpdateScenarioText()
	{
		string text = "";
		if(_altarOfMystification.Hex.Revealed && !_altarOfMystification.IsDestroyed)
		{
			text +=
				$"""
				 The altar marked {Icons.InlineMarker(Marker.Type.a)} is the Altar of Mystification. Until the Altar of Mystification is destroyed, all Living Spirits are permanently {Icons.Inline(Icons.GetCondition(Conditions.Invisible))}

				 When the Altar of Mystification is destroyed, unlock door {Icons.InlineMarker(Marker.Type._2)}.


				 """;
		}

		if(GameController.Instance.Map.Rooms[1].Revealed && GameController.Instance.Map.Figures.Any(figure =>
			   figure is Monster monster && monster.MonsterModel is LivingSpirit))
		{
			text += $"When all Living Spirits have been killed, unlock door {Icons.InlineMarker(Marker.Type._3)}.\n\n";
		}

		if(_altarOfDisorientation.Hex.Revealed && !_altarOfDisorientation.IsDestroyed)
		{
			text +=
				$"The altar marked {Icons.InlineMarker(Marker.Type.b)} is the Altar of Disorientation. All figures add -1{Icons.Inline(Icons.Attack)} to all attacks performed while adjacent to the Altar of Disorientation.\n\n";
		}

		if(_altarOfPerplexity.Hex.Revealed && !_altarOfPerplexity.IsDestroyed)
		{
			text +=
				$"The altar marked {Icons.InlineMarker(Marker.Type.c)} is the Altar of Perplexity. All figures gain advantage on all attacks performed while adjacent to the Altar of Perplexity.\n\n";
		}

		text = text.TrimEnd('\n');
		base.UpdateScenarioText(text);
	}
}