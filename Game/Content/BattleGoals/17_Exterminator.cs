using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Exterminator : TheCrimsonScalesBattleGoal
{
	public override string Title => "Exterminator";
	public override string Description => "Kill one or more enemies of each type that appears in the scenario.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override int MaxProgress => _monsterTypesCount;

	private int _monsterTypesCount = 0;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		_monsterTypesCount = 0;
		List<MonsterModel> monsterTypesToKill = [];
		List<MonsterModel> killedMonsterTypes = [];
		
		ScenarioEvents.FigureRegisteredEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Monster monster &&
				monster.EnemiesWith(character) &&
				!monster.Traits.Any(trait => trait is AllDamageImmunityTrait) &&
				!monsterTypesToKill.Contains(monster.MonsterModel),
			async parameters =>
			{
				Monster monster = parameters.Figure as Monster;
				monsterTypesToKill.Add(monster.MonsterModel);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => 
				parameters.Figure.EnemiesWith(character) &&
				parameters.PotentialKiller == character &&
				parameters.Figure is Monster monster &&
				!killedMonsterTypes.Contains(monster.MonsterModel),
			async parameters =>
			{
				Monster monster = parameters.Figure as Monster;
				killedMonsterTypes.Add(monster.MonsterModel);

				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.ScenarioEndedEvent.Subscribe(this,
			parameters => killedMonsterTypes.Count == monsterTypesToKill.Count,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}