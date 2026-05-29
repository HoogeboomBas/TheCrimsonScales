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

	// IBossMonsterModel
	public string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 If the Icebound is occupying Room 1 (A2b tile), summon one Wind Demon in an empty hex closest to the hex marked {Icons.InlineMarker(Marker.Type.c, richTextParameters)}.
		 If the Icebound is occupying Room 2 (A3a tile), summon one Frost Demon in an empty hex closest to the hex marked {Icons.InlineMarker(Marker.Type.a, richTextParameters)}.
		 If the Icebound is occupying the G2a tile, summon one Stone Golem in an empty hex closest to the hex marked {Icons.InlineMarker(Marker.Type.b, richTextParameters)}.
		 Summons are normal for two characters, every other summon is elite for three characters, and all summons are elite for four characters.
		 The Icebound then performs ”{Icons.Inline(Icons.Heal)}3, Self”.
		 """;

	public string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Attack)}{monster.Stats.Attack + 0}, {Icons.Inline(Icons.Targets)}all enemies within {Icons.Inline(Icons.Range)}3
		 If the Icebound is occupying Room 1 (A2b tile), it immediately jumps into the nearest unoccupied hex adjacent to {Icons.InlineMarker(Marker.Type.e, richTextParameters)}. If the Icebound is occupying Room 2 (A3a tile), it immediately jumps into the nearest unoccupied hex adjacent to {Icons.InlineMarker(Marker.Type.d, richTextParameters)}.
		 {Icons.Inline(Icons.Attack)}{monster.Stats.Attack + 0}, {Icons.Inline(Icons.Targets)}all enemies within {Icons.Inline(Icons.Range)}3
		 """;

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
		new MonsterAbilityCardAbility(TeleportAbility.Builder()
			.WithCustomGetHexes((state, hexes) =>
			{
				Hex targetHex = CalculateJumpTarget(monster);

				if(targetHex == null)
				{
					return;
				}

				// First find hexes around the target that are closest to the Icebound
				int closestHexRange = int.MaxValue;

				foreach(Hex neighbourHex in targetHex.Neighbours)
				{
					if(!neighbourHex.IsEmpty())
					{
						continue;
					}

					// Teleporting so calculating direct distance
					int range = Map.SimpleDistance(neighbourHex.Coords, monster.Hex.Coords);

					if(range == closestHexRange)
					{
						hexes.Add(neighbourHex);
					}
					else if(range < closestHexRange)
					{
						closestHexRange = range;
						hexes.Clear();
						hexes.Add(neighbourHex);
					}
				}
			})
			.Build()),
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

	private Hex CalculateJumpTarget(Monster monster)
	{
		Hex hex = null;
		if(monster.Hex.Room == GameController.Instance.Map.Rooms[1])
		{
			hex = GameController.Instance.Map.GetMarker(Marker.Type.e).Hex;
		}
		else if(monster.Hex.Room == GameController.Instance.Map.Rooms[2])
		{
			hex = GameController.Instance.Map.GetMarker(Marker.Type.d).Hex;
		}

		return hex;
	}
}