using System.Collections.Generic;
using Godot;

public abstract class SavvasIceStormAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/SavvasIceStorm/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard0>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard1>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard2>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard3>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard4>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard5>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard6>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard7>()
	];
}

public class SavvasIceStormAbilityCard0 : SavvasIceStormAbilityCard
{
	public override int Initiative => 65;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 3, targets: 3, conditions: [Conditions.Curse])),
	];
}

public class SavvasIceStormAbilityCard1 : SavvasIceStormAbilityCard
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

public class SavvasIceStormAbilityCard2 : SavvasIceStormAbilityCard
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

public class SavvasIceStormAbilityCard3 : SavvasIceStormAbilityCard
{
	public override int Initiative => 84;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, target: Target.Enemies | Target.TargetAll)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 4, conditions: [Conditions.Wound1])),
	];
}

public class SavvasIceStormAbilityCard4 : SavvasIceStormAbilityCard
{
	public override int Initiative => 75;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, conditions: [Conditions.Poison1])),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 5, conditions: [Conditions.Immobilize])),
	];
}

public class SavvasIceStormAbilityCard5 : SavvasIceStormAbilityCard
{
	public override int Initiative => 75;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -2, target: Target.Enemies | Target.TargetAll, conditions: [Conditions.Disarm])),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 3, targets: 2)),
	];
}

public class SavvasIceStormAbilityCard6 : SavvasIceStormAbilityCard
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

					// if(hex != null && await GameController.Instance.Map.CreateMonster(ModelDB.Monster<SavvasIceStorm>(), MonsterType.Normal, hex.Coords, true))
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

public class SavvasIceStormAbilityCard7 : SavvasIceStormAbilityCard
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