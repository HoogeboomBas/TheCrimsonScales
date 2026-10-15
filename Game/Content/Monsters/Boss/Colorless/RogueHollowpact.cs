using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class RogueHollowpact : Colorless
{
	public override MonsterStats[] BossLevelStats =>
		base.BossLevelStats
			.Select(stats => stats with
			{
				Traits =
				[
					new ConditionImmunityTrait(Conditions.Stun), new ConditionImmunityTrait(Conditions.Immobilize),
					new ConditionImmunityTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Curse)
				]
			})
			.ToArray();

	public override string Name => "Rogue Hollowpact";

	// IBossMonsterModel
	public override string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Move)}+0, {Icons.Inline(Icons.Jump)}, {Icons.Inline(Icons.Attack)}+2, {Icons.Inline(Icons.Heal)}X, Self, where X is the number of Void Pit obstacles.
		 """;

	public override string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 Jump to an empty hex adjacent to a Void Pit obstacle furthest away from a character within {Icons.Inline(Icons.Range)}4.
		 {Icons.Inline(Icons.Attack)}+2, {Icons.Inline(Icons.Range)}4.
		 All enemies adjacent to a Void Pit obstacle suffer {Icons.Inline(Icons.Damage)}2.
		 """;

	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.MoveAbility(monster, +0, MoveType.Jump)),
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, +2)),

		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(0)
			.WithTarget(Target.Self)
			.WithOnAbilityStarted(async healState =>
			{
				healState.AbilityAdjustHealValue(GameController.Instance.Map.GetChildrenOfType<Objective>()
					.Count(objective => objective.DisplayName == "Void Pit" && !objective.IsDestroyed));

				await GDTask.CompletedTask;
			})
			.Build())
	];

	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(TeleportAbility.Builder()
			.WithCustomGetHexes((state, hexes) =>
			{
				// Find all void pits
				List<Objective> objectives = GameController.Instance.Map.GetChildrenOfType<Objective>()
					.Where(objective => objective.DisplayName == "Void Pit" && !objective.IsDestroyed).ToList();

				if(objectives.Count() == 0)
				{
					return;
				}

				Dictionary<Objective, int> objectiveDistanceToClosestCharacter = [];

				// Find the distance to the closest character for each
				foreach(Objective objective in objectives)
				{
					int closestCharacterRange = int.MaxValue;

					foreach(Hex objectiveHex in objective.Hexes)
					{
						foreach(Figure figure in GameController.Instance.Map.Figures)
						{
							if(figure is Character)
							{
								int range = RangeHelper.Distance(objectiveHex, figure.Hex);

								if(range < closestCharacterRange)
								{
									closestCharacterRange = range;
									objectiveDistanceToClosestCharacter[objective] = range;
								}
							}
						}
					}
				}

				// Sort the objectives by distance to the closest character in descending order
				objectives.Sort((objectiveA, objectiveB) =>
					objectiveDistanceToClosestCharacter[objectiveB].CompareTo(objectiveDistanceToClosestCharacter[objectiveA]));

				// Take the closest one that has an empty hex within range 4
				Objective targetObjective = objectives.First(objective =>
					objective.Hex.Neighbours.Any(hex => hex.IsEmpty() && Map.SimpleDistance(monster.Hex.Coords, hex.Coords) <= 4));

				hexes.AddRange(targetObjective.Hex.Neighbours.Where(hex => hex.IsEmpty() && Map.SimpleDistance(monster.Hex.Coords, hex.Coords) <= 4));
			})
			.Build()),
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, +2, range: 4)),
		new MonsterAbilityCardAbility(SufferDamageAbility.Builder()
			.WithDamage(2)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.WithCustomGetTargets((state, figures) =>
			{
				figures.AddRange(GameController.Instance.Map
					.GetChildrenOfType<Objective>()
					.Where(objective => objective.DisplayName == "Void Pit" && !objective.IsDestroyed)
					.SelectMany(objective => RangeHelper.GetFiguresInRange(objective, 1))
					.Distinct());
			})
			.Build())
	];
}