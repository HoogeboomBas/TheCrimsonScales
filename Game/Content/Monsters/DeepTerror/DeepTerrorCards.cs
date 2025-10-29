using System.Collections.Generic;
using Godot;

public abstract class DeepTerrorAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/DeepTerror/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<DeepTerrorAbilityCard0>(),
		ModelDB.MonsterAbilityCard<DeepTerrorAbilityCard1>(),
		ModelDB.MonsterAbilityCard<DeepTerrorAbilityCard2>(),
		ModelDB.MonsterAbilityCard<DeepTerrorAbilityCard3>(),
		ModelDB.MonsterAbilityCard<DeepTerrorAbilityCard4>(),
		ModelDB.MonsterAbilityCard<DeepTerrorAbilityCard5>(),
		ModelDB.MonsterAbilityCard<DeepTerrorAbilityCard6>(),
		ModelDB.MonsterAbilityCard<DeepTerrorAbilityCard7>()
	];
}

public class DeepTerrorAbilityCard0 : DeepTerrorAbilityCard
{
	public override int Initiative => 65;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 3, targets: 3, conditions: [Conditions.Curse])),
	];
}

public class DeepTerrorAbilityCard1 : DeepTerrorAbilityCard
{
	public override int Initiative => 60;
	public override int CardIndex => 1;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, pierce: 3,
			aoePattern: new AOEPattern([
				new AOEHex(Vector2I.Zero, AOEHexType.Gray),
				new AOEHex(new Vector2I(1, 0), AOEHexType.Red),
				new AOEHex(new Vector2I(2, 0), AOEHexType.Red),
				new AOEHex(new Vector2I(3, 0), AOEHexType.Red),
				new AOEHex(new Vector2I(4, 0), AOEHexType.Red),
				new AOEHex(new Vector2I(5, 0), AOEHexType.Red),
			])
		)),
	];
}

public class DeepTerrorAbilityCard2 : DeepTerrorAbilityCard
{
	public override int Initiative => 60;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, pierce: 3,
			aoePattern: new AOEPattern([
				new AOEHex(Vector2I.Zero, AOEHexType.Gray),
				new AOEHex(new Vector2I(1, 0), AOEHexType.Red),
				new AOEHex(new Vector2I(2, 0), AOEHexType.Red),
				new AOEHex(new Vector2I(3, 0), AOEHexType.Red),
				new AOEHex(new Vector2I(4, 0), AOEHexType.Red),
				new AOEHex(new Vector2I(5, 0), AOEHexType.Red),
			])
		)),
	];
}

public class DeepTerrorAbilityCard3 : DeepTerrorAbilityCard
{
	public override int Initiative => 84;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, target: Target.Enemies | Target.TargetAll)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 4, conditions: [Conditions.Wound1])),
	];
}

public class DeepTerrorAbilityCard4 : DeepTerrorAbilityCard
{
	public override int Initiative => 75;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, conditions: [Conditions.Poison1])),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 5, conditions: [Conditions.Immobilize])),
	];
}

public class DeepTerrorAbilityCard5 : DeepTerrorAbilityCard
{
	public override int Initiative => 75;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -2, target: Target.Enemies | Target.TargetAll, conditions: [Conditions.Disarm])),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 3, targets: 2)),
	];
}

public class DeepTerrorAbilityCard6 : DeepTerrorAbilityCard
{
	public override int Initiative => 96;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -2, range: 6)),
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<DeepTerror>())
			.WithMonsterType(MonsterType.Normal)
			.WithConditionalAbilityCheck(async state => await AbilityCmd.HasPerformedAbility(state, 0))
			.WithGetValidHexes((state, hexes) =>
			{
				AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
				foreach(Figure target in attackAbilityState.UniqueTargetedFigures)
				{
					RangeHelper.FindHexesInRange(target.Hex, 1, true, hexes);
				}

				for(int i = hexes.Count - 1; i >= 0; i--)
				{
					if(!hexes[i].IsEmpty())
					{
						hexes.RemoveAt(i);
					}
				}
			})
			.Build()),
	];
}

public class DeepTerrorAbilityCard7 : DeepTerrorAbilityCard
{
	public override int Initiative => 54;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Wound1, Conditions.Poison1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 4)),
	];
}