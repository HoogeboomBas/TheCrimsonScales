using System.Collections.Generic;
using Godot;

public abstract class SavvasLavaflowAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/SavvasLavaflow/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard0>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard1>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard2>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard3>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard4>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard5>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard6>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard7>()
	];
}

public class SavvasLavaflowAbilityCard0 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 65;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 3, targets: 3, conditions: [Conditions.Curse])),
	];
}

public class SavvasLavaflowAbilityCard1 : SavvasLavaflowAbilityCard
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

public class SavvasLavaflowAbilityCard2 : SavvasLavaflowAbilityCard
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

public class SavvasLavaflowAbilityCard3 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 84;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, target: Target.Enemies | Target.TargetAll)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 4, conditions: [Conditions.Wound1])),
	];
}

public class SavvasLavaflowAbilityCard4 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 75;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, conditions: [Conditions.Poison1])),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 5, conditions: [Conditions.Immobilize])),
	];
}

public class SavvasLavaflowAbilityCard5 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 75;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -2, target: Target.Enemies | Target.TargetAll, conditions: [Conditions.Disarm])),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 3, targets: 2)),
	];
}

public class SavvasLavaflowAbilityCard6 : SavvasLavaflowAbilityCard
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

					// if(hex != null && await GameController.Instance.Map.CreateMonster(ModelDB.Monster<SavvasLavaflow>(), MonsterType.Normal, hex.Coords, true))
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

public class SavvasLavaflowAbilityCard7 : SavvasLavaflowAbilityCard
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