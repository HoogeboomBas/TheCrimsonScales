using System.Collections.Generic;
using Godot;

public abstract class WindDemonAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/WindDemon/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard0>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard1>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard2>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard3>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard4>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard5>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard6>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard7>()
	];
}

public class WindDemonAbilityCard0 : WindDemonAbilityCard
{
	public override int Initiative => 65;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 3, targets: 3, conditions: [Conditions.Curse])),
	];
}

public class WindDemonAbilityCard1 : WindDemonAbilityCard
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

public class WindDemonAbilityCard2 : WindDemonAbilityCard
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

public class WindDemonAbilityCard3 : WindDemonAbilityCard
{
	public override int Initiative => 84;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, target: Target.Enemies | Target.TargetAll)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 4, conditions: [Conditions.Wound1])),
	];
}

public class WindDemonAbilityCard4 : WindDemonAbilityCard
{
	public override int Initiative => 75;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, conditions: [Conditions.Poison1])),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 5, conditions: [Conditions.Immobilize])),
	];
}

public class WindDemonAbilityCard5 : WindDemonAbilityCard
{
	public override int Initiative => 75;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -2, target: Target.Enemies | Target.TargetAll, conditions: [Conditions.Disarm])),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 3, targets: 2)),
	];
}

public class WindDemonAbilityCard6 : WindDemonAbilityCard
{
	public override int Initiative => 96;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -2, range: 6)),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
				foreach(Figure target in attackAbilityState.UniqueTargetedFigures)
				{
					Hex hex = await AbilityCmd.SelectHex(state, list =>
					{
						foreach(Hex neighbourHex in target.Hex.Neighbours)
						{
							if(neighbourHex.IsEmpty())
							{
								list.Add(neighbourHex);
							}
						}
					});

					// if(hex != null && await GameController.Instance.Map.CreateMonster(ModelDB.Monster<WindDemon>(), MonsterType.Normal, hex.Coords, true))
					// {
					// 	state.SetPerformed();
					// 	break;
					// }
				}
			})
			.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
			.Build())
	];
}

public class WindDemonAbilityCard7 : WindDemonAbilityCard
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