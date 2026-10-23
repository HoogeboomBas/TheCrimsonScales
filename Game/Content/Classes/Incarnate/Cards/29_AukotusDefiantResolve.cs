using System.Collections.Generic;
using Godot;

public class AukotusDefiantResolve : IncarnateCardModel<AukotusDefiantResolve.CardTop, AukotusDefiantResolve.CardBottom>
{
	public override string Name => "Aukotu's Defiant Resolve";
	public override int Level => 9;
	public override int Initiative => 32;
	protected override int AtlasIndex => 29;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2, new PushCircle(this, new Vector2(0.35621542f, 0.2066482f)))
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(2)
				.WithConditions([Conditions.Rupture, Incarnate.Enfeeble])
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1, new HealDiamondPlus(this, new Vector2(0.38337782f, 0.29916897f)))
				.WithTarget(Target.TargetAll | Target.SelfOrAllies)
				.WithRange(2)
				.WithConditions(Incarnate.Empower)
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices =>
			[IncarnateSpirit.Ritualist, IncarnateSpirit.Conqueror, IncarnateSpirit.Reaver];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(new DynamicInt<MoveAbility.State>(state =>
				{
					if(state.Performer is IHasEnfeeble enfeebleCharacter)
					{
						return 15 - enfeebleCharacter.RemainingEnfeebleCount;
					}

					return 15;
				}), new MoveCircle(this, new Vector2(0.66897106f, 0.6241767f)))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(new DynamicInt<AttackAbility.State>(state =>
				{
					if(state.Performer is IHasEmpower empowerCharacter)
					{
						return 15 - empowerCharacter.RemainingEmpowerCount;
					}

					return 15;
				}), new AttackDiamond(this, new Vector2(0.66819495f, 0.80110806f)))
				.Build()),
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}