using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Exterminator : TheCrimsonScalesBattleGoal
{
	public override string Title => "Exterminator";
	public override string Description => "Kill one or more enemies of each type that appears in the scenario.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override int MaxProgress => GameController.Instance.ScenarioModel.MonsterModels.Count;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		List<MonsterModel> monsterTypesToKill = [];
		List<MonsterModel> killedMonsterTypes = [];

		List<Figure> revealedMonsters = GameController.Instance.Map.Figures.Where(figure =>
			figure is Monster monster && 
			monster.EnemiesWith(character) && 
			!monster.Traits.Any(trait => trait is AllDamageImmunityTrait)).ToList();

		monsterTypesToKill.AddRange(revealedMonsters.Select(figure => figure as Monster).Select(monster => monster.MonsterModel).Distinct());
		
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
			parameters => 
				killedMonsterTypes.Count == monsterTypesToKill.Count &&
				!battleGoal.ProgressFull,
			async parameters =>
			{
				battleGoal.AdjustProgress(MaxProgress - killedMonsterTypes.Count);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}