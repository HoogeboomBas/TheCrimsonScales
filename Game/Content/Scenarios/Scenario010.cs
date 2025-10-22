using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario010 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario010.tscn";
	public override int ScenarioNumber => 10;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<WondrousScenarioChain>();
	//public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario017>()];

	protected override ScenarioGoals CreateScenarioGoals() => new CustomScenarioGoals("Destroy at least 5 supply crates to win this scenario.");

	public override string BGSPath => null;

	private bool _fireConsumed;
	private bool _airConsumed;
	private bool _iceConsumed;
	private bool _earthConsumed;

	private int _crateKillCount = 0;

	public override async GDTask StartBeforeFirstRoomRevealed()
	{
		await base.StartBeforeFirstRoomRevealed();

		List<Objective> objectives = GameController.Instance.Map.GetChildrenOfType<Objective>();
		int objectiveHealth =
			2 * (GameController.Instance.SavedCampaign.Characters.Count + GameController.Instance.SavedScenario.ScenarioLevel + 1);
		foreach(Objective objective in objectives)
		{
			objective.Init(objectiveHealth, "Supply Crate");
		}

		UpdateScenarioText();

		GameController.Instance.Map.Treasures[0].SetItemLoot(AbilityCmd.GetRandomAvailableStone());

		//TODO: Scenario effect

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				if(_crateKillCount >= 5)
				{
					CustomScenarioGoals customScenarioGoals = (CustomScenarioGoals)ScenarioGoals;
					await customScenarioGoals.Win();
				}

				if(parameters.RoundNumber % 2 == 1)
				{
					// Odd round

					bool blackImpPresent = false;
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(figure is Monster monster && monster.MonsterModel == ModelDB.Monster<BlackImp>())
						{
							blackImpPresent = true;
						}
					}

					if(blackImpPresent)
					{
						if(_crateKillCount < 1)
						{
							await AbilityCmd.InfuseElement(Element.Fire, true);
						}

						if(_crateKillCount < 3)
						{
							await AbilityCmd.InfuseElement(Element.Air, true);
						}
					}
				}
				else
				{
					// Even round

					bool lurkerPresent = false;
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(figure is Monster monster && monster.MonsterModel == ModelDB.Monster<Lurker>())
						{
							lurkerPresent = true;
						}
					}

					if(lurkerPresent)
					{
						if(_crateKillCount < 2)
						{
							await AbilityCmd.InfuseElement(Element.Ice, true);
						}

						if(_crateKillCount < 4)
						{
							await AbilityCmd.InfuseElement(Element.Earth, true);
						}
					}
				}

				_fireConsumed = false;
				_airConsumed = false;
				_iceConsumed = false;
				_earthConsumed = false;
			}
		);

		ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
			parameters => parameters.Figure is Monster,
			async parameters =>
			{
				Monster monster = (Monster)parameters.Figure;
				if(monster.MonsterModel == ModelDB.Monster<BlackImp>())
				{
					if(_crateKillCount < 1 && await AbilityCmd.TryConsumeElement(Element.Fire))
					{
						_fireConsumed = true;
					}

					if(_crateKillCount < 3 && await AbilityCmd.TryConsumeElement(Element.Air))
					{
						_airConsumed = true;
					}
				}

				if(monster.MonsterModel == ModelDB.Monster<Lurker>())
				{
					if(_crateKillCount < 2 && await AbilityCmd.TryConsumeElement(Element.Ice))
					{
						_iceConsumed = true;
					}

					if(_crateKillCount < 4 && await AbilityCmd.TryConsumeElement(Element.Earth))
					{
						_earthConsumed = true;
					}

					if(_earthConsumed)
					{
						ActionState actionState = new ActionState(monster,
						[
							HealAbility.Builder()
								.WithHealValue(1)
								.WithTarget(Target.Self)
								.Build()
						]);
						await actionState.Perform();
					}
				}
			}
		);

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters => parameters.AbilityState is AttackAbility.State && parameters.Performer is Monster,
			async parameters =>
			{
				AttackAbility.State attackAbilityState = (AttackAbility.State)parameters.AbilityState;
				Monster monster = (Monster)parameters.Performer;
				if(monster.MonsterModel == ModelDB.Monster<BlackImp>())
				{
					if(_fireConsumed)
					{
						attackAbilityState.AbilityAdjustAttackValue(1);
					}
				}

				if(monster.MonsterModel == ModelDB.Monster<Lurker>())
				{
					if(_iceConsumed)
					{
						attackAbilityState.AbilityAdjustAttackValue(1);
					}
				}

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters => parameters.AbilityState is TargetedAbilityState && parameters.Performer is Monster,
			async parameters =>
			{
				TargetedAbilityState targetedAbilityState = (TargetedAbilityState)parameters.AbilityState;
				Monster monster = (Monster)parameters.Performer;
				if(monster.MonsterModel == ModelDB.Monster<BlackImp>())
				{
					if(_airConsumed)
					{
						targetedAbilityState.AbilityAdjustRange(2);
					}
				}

				await GDTask.CompletedTask;
			}, checkDuplicates: false
		);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Objective,
			async parameters =>
			{
				_crateKillCount++;

				UpdateScenarioText();

				await GDTask.CompletedTask;
			}
		);
	}

	private void UpdateScenarioText()
	{
		string text = _crateKillCount switch
		{
			0 => $"""
			      Destroy 5 more crates to win.

			      At the end of every odd round, if there is at least 1 Black Imp present, {Icons.Inline(Icons.GetElement(Element.Fire))} and {Icons.Inline(Icons.GetElement(Element.Air))} are infused.
			      At the end of every even round, if there is at least 1 Lurker present, {Icons.Inline(Icons.GetElement(Element.Ice))} and {Icons.Inline(Icons.GetElement(Element.Earth))} are infused.

			      During their turn, Black Imps can consume {Icons.Inline(Icons.GetElement(Element.Fire))} to add +1 {Icons.Inline(Icons.Attack)} to all attack abilities for the round and consume {Icons.Inline(Icons.GetElement(Element.Air))} to add +2 {Icons.Inline(Icons.Range)} to their ranged abilities for the round.
			      During their turn, Lurkers can consume {Icons.Inline(Icons.GetElement(Element.Ice))} to add +1 {Icons.Inline(Icons.Attack)} to all attack abilities for the round and consume {Icons.Inline(Icons.GetElement(Element.Earth))} to perform “{Icons.Inline(Icons.Heal)} 1, Self”.

			      As you destroy supply crates, {Icons.Inline(Icons.GetElement(Element.Fire))}, {Icons.Inline(Icons.GetElement(Element.Ice))}, {Icons.Inline(Icons.GetElement(Element.Air))} and {Icons.Inline(Icons.GetElement(Element.Earth))} will no longer be consumed or infused as per the special rules above (in that order).
			      """,
			1 => $"""
			      Destroy 4 more crates to win.

			      At the end of every odd round, if there is at least 1 Black Imp present, {Icons.Inline(Icons.GetElement(Element.Air))} is infused.
			      At the end of every even round, if there is at least 1 Lurker present, {Icons.Inline(Icons.GetElement(Element.Ice))} and {Icons.Inline(Icons.GetElement(Element.Earth))} are infused.

			      During their turn, Black Imps can consume {Icons.Inline(Icons.GetElement(Element.Air))} to add +2 {Icons.Inline(Icons.Range)} to their ranged abilities for the round.
			      During their turn, Lurkers can consume {Icons.Inline(Icons.GetElement(Element.Ice))} to add +1 {Icons.Inline(Icons.Attack)} to all attack abilities for the round and consume {Icons.Inline(Icons.GetElement(Element.Earth))} to perform “{Icons.Inline(Icons.Heal)} 1, Self”.

			      As you destroy more supply crates, {Icons.Inline(Icons.GetElement(Element.Ice))}, {Icons.Inline(Icons.GetElement(Element.Air))} and {Icons.Inline(Icons.GetElement(Element.Earth))} will no longer be consumed or infused as per the special rules above (in that order).
			      """,
			2 => $"""
			      Destroy 3 more crates to win.

			      At the end of every odd round, if there is at least 1 Black Imp present, {Icons.Inline(Icons.GetElement(Element.Air))} is infused.
			      At the end of every even round, if there is at least 1 Lurker present, {Icons.Inline(Icons.GetElement(Element.Earth))} is infused.

			      During their turn, Black Imps can consume {Icons.Inline(Icons.GetElement(Element.Air))} to add +2 {Icons.Inline(Icons.Range)} to their ranged abilities for the round.
			      During their turn, Lurkers can consume {Icons.Inline(Icons.GetElement(Element.Earth))} to perform “{Icons.Inline(Icons.Heal)} 1, Self”.

			      As you destroy more supply crates, {Icons.Inline(Icons.GetElement(Element.Air))} and {Icons.Inline(Icons.GetElement(Element.Earth))} will no longer be consumed or infused as per the special rules above (in that order).
			      """,
			3 => $"""
			      Destroy 2 more crates to win.

			      At the end of every even round, if there is at least 1 Lurker present, {Icons.Inline(Icons.GetElement(Element.Earth))} is infused.

			      During their turn, Lurkers can consume {Icons.Inline(Icons.GetElement(Element.Earth))} to perform “{Icons.Inline(Icons.Heal)} 1, Self”.

			      As you destroy more supply crates, {Icons.Inline(Icons.GetElement(Element.Earth))} will no longer be consumed or infused as per the special rules above.
			      """,
			4 => $"""
			      Destroy 1 more crate to win.
			      """,
			_ => null
		};

		UpdateScenarioText(text);
	}
}