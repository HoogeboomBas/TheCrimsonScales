using System.Collections.Generic;
using Godot;

public abstract class AncientArtilleryAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/AncientArtillery/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<AncientArtilleryAbilityCard0>(),
		ModelDB.MonsterAbilityCard<AncientArtilleryAbilityCard1>(),
		ModelDB.MonsterAbilityCard<AncientArtilleryAbilityCard2>(),
		ModelDB.MonsterAbilityCard<AncientArtilleryAbilityCard3>(),
		ModelDB.MonsterAbilityCard<AncientArtilleryAbilityCard4>(),
		ModelDB.MonsterAbilityCard<AncientArtilleryAbilityCard5>(),
		ModelDB.MonsterAbilityCard<AncientArtilleryAbilityCard6>(),
		ModelDB.MonsterAbilityCard<AncientArtilleryAbilityCard7>()
	];
}

public class AncientArtilleryAbilityCard0 : AncientArtilleryAbilityCard
{
	public override int Initiative => 46;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, extraRange: 2)),
	];
}

public class AncientArtilleryAbilityCard1 : AncientArtilleryAbilityCard
{
	public override int Initiative => 71;
	public override int CardIndex => 1;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
				{
					List<Figure> sufferDamageTargets = new List<Figure>();
					foreach(Figure figure in RangeHelper.GetFiguresInRange(monster.Hex, 1))
					{
						if(state.Authority.EnemiesWith(figure))
						{
							sufferDamageTargets.Add(figure);
						}
					}

					foreach(Figure target in sufferDamageTargets)
					{
						await AbilityCmd.SufferDamage(null, target, 2);
					}

					state.SetPerformed();
				}
			)
			.Build())
	];
}

public class AncientArtilleryAbilityCard2 : AncientArtilleryAbilityCard
{
	public override int Initiative => 71;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
				{
					List<Figure> sufferDamageTargets = new List<Figure>();
					foreach(Figure figure in RangeHelper.GetFiguresInRange(monster.Hex, 1))
					{
						if(state.Authority.EnemiesWith(figure))
						{
							sufferDamageTargets.Add(figure);
						}
					}

					foreach(Figure target in sufferDamageTargets)
					{
						await AbilityCmd.SufferDamage(null, target, 2);
					}

					state.SetPerformed();
				}
			)
			.Build())
	];
}

public class AncientArtilleryAbilityCard3 : AncientArtilleryAbilityCard
{
	public override int Initiative => 37;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(PushAbility.Builder()
			.WithPush(1)
			.WithRange(1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, extraRange: -1,
			aoePattern: new AOEPattern(
				[
					new AOEHex(Vector2I.Zero, AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				]
			)
		))
	];
}

public class AncientArtilleryAbilityCard4 : AncientArtilleryAbilityCard
{
	public override int Initiative => 37;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(PushAbility.Builder()
			.WithPush(1)
			.WithRange(1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, extraRange: -1,
			aoePattern: new AOEPattern([
				new AOEHex(Vector2I.Zero, AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
			])
		)),
	];
}

public class AncientArtilleryAbilityCard5 : AncientArtilleryAbilityCard
{
	public override int Initiative => 95;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class AncientArtilleryAbilityCard6 : AncientArtilleryAbilityCard
{
	public override int Initiative => 17;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(PushAbility.Builder()
			.WithPush(2)
			.WithRange(1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(2).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -2))
	];
}

public class AncientArtilleryAbilityCard7 : AncientArtilleryAbilityCard
{
	public override int Initiative => 46;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1,
			aoePattern: new AOEPattern(
				[
					new AOEHex(Vector2I.Zero, AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				]
			), conditions: [Conditions.Immobilize]
		))
	];
}