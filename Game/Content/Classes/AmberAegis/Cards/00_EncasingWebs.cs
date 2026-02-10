using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class EncasingWebs : AmberAegisCardModel<EncasingWebs.CardTop, EncasingWebs.CardBottom>
{
	public override string Name => "Encasing Webs";
	public override int Level => 1;
	public override int Initiative => 33;
	protected override int AtlasIndex => 0;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(5)
				.WithTarget(Target.Allies)
				.WithRange(3)
				.WithPull(2)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.Add(state.ActionState.GetAbilityState<HealAbility.State>(0).Target);
				})
				.WithTarget(Target.Allies)
				.WithMandatory(true)
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					RetaliateAbility.Builder().WithRetaliateValue(1).Build()
				])
				.WithCustomGetTargets((state, figures) =>
				{
					figures.Add(state.ActionState.GetAbilityState<HealAbility.State>(0).Target);
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Fire,
					effectInfoText: $"Grant the target of the heal ability {Icons.Inline(Icons.Retaliate)}1"))
				.WithOnAbilityEndedPerformed(async state =>
				{
					state.ActionState.SetOverrideRound();
					await GDTask.CompletedTask;
				})
				.Build())
		];
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.3749415f, 0.752968f)))
				.WithConditions(Conditions.Immobilize)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
					]
				))
				.Build())
		];

		public override int XP => 1;
	}
}