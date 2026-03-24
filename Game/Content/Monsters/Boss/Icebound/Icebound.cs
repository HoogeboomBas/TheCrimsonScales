using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Icebound : SavvasIceStorm, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * CharacterCount,
				Traits = (stats.Traits ?? [])
				.Append(new ConditionImmunityTrait(Conditions.Stun))
				.Append(new ConditionImmunityTrait(Conditions.Disarm))
				.Append(ConditionImmunityTrait.PoisonImmunityTrait())
				.Append(new ConditionImmunityTrait(Conditions.Immobilize))
				.Append(new ForcedMovementImmunityTrait())
				.ToArray()
			})
			.ToArray();

	public override string Name => "Icebound";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<SavvasIceStorm>();

	private bool _summonElite;

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(CalculateMonsterModel(monster))
			.WithMonsterType(CharacterCount >= 4 || (CharacterCount == 3 && _summonElite) ? MonsterType.Elite : MonsterType.Normal)
			.WithGetValidHexes((state, list) =>
			{
				Hex spawnHex = CalculateSpawnPoint(monster);
				List<Hex> hexes = RangeHelper.GetHexesInRange(spawnHex, 100, requiresLineOfSight: false).ToList();
				hexes.Shuffle(GameController.Instance.StateRNG);
				hexes.Sort((otherHexA,
					otherHexB) => RangeHelper.Distance(spawnHex,
						otherHexA)
					.CompareTo(RangeHelper.Distance(spawnHex,
						otherHexB)));
				Hex firstHex = hexes.FirstOrDefault(hex => hex.IsEmpty());

				if(firstHex == null)
				{
					return;
				}

				int distance = RangeHelper.Distance(spawnHex,
					firstHex);

				list.AddRange(
					hexes.Where(h => h.IsEmpty() &&
					                 RangeHelper.Distance(spawnHex, h) == distance)
				);
			})
			.WithOnAbilityEndedPerformed(async state =>
			{
				_summonElite = !_summonElite;
				await GDTask.CompletedTask;
			})
			.Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(3)
			.WithTarget(Target.Self)
			.Build())
	];

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, +0, range: 3, target: Target.TargetAll | Target.Enemies)),
		//TODO: Teleport ability
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, +0, range: 3, target: Target.TargetAll | Target.Enemies))
	];

	private MonsterModel CalculateMonsterModel(Monster monster)
	{
		if(monster.Hex.Room == GameController.Instance.Map.Rooms[1])
		{
			return ModelDB.Monster<WindDemon>();
		}

		if(monster.Hex.Room == GameController.Instance.Map.Rooms[2])
		{
			return ModelDB.Monster<FrostDemon>();
		}

		return ModelDB.Monster<StoneGolem>();
	}

	private Hex CalculateSpawnPoint(Monster monster)
	{
		Marker marker;
		if(monster.Hex.Room == GameController.Instance.Map.Rooms[1])
		{
			marker = GameController.Instance.Map.GetMarker(Marker.Type.c);
		}
		else if(monster.Hex.Room == GameController.Instance.Map.Rooms[2])
		{
			marker = GameController.Instance.Map.GetMarker(Marker.Type.a);
		}
		else
		{
			marker = GameController.Instance.Map.GetMarker(Marker.Type.b);
		}

		return marker.Hex;
	}
}