using System.Collections.Generic;
using System.Linq;
using Godot;

public class ReachingDarkness : HollowpactCardModel<ReachingDarkness.CardTop, ReachingDarkness.CardBottom>
{
	public override string Name => "Reaching Darkness";
	public override int Level => 1;
	public override int Initiative => 79;
	protected override int AtlasIndex => 9;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.6511111f, 0.2245968f)))
				.WithRange(5)
				.Build()),
			
			new AbilityCardAbility(GainVoidEnergyAbilityBuilder()
				.Build()),
		];
		
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(2)
				.WithRange(5)
				.Build()),
			
			new AbilityCardAbility(TeleportAbility.Builder()
				.WithCustomGetHexes((state, list) =>
				{
					foreach(Hex targetedHex in state.ActionState.GetAbilityState<SufferDamageAbility.State>(0).TargetedHexes)
					{
						list.AddRange(RangeHelper.GetHexesInRange(origin: targetedHex, range: 1, includeOrigin: false).Where(hex => hex.IsUnoccupied()));
					}
				})
				.WithConditionalAbilityCheck(async state =>
				{
					return 
						await AbilityCmd.HasPerformedAbility(state, 0) &&
						!state.ActionState.GetAbilityState<SufferDamageAbility.State>(0).UniqueTargetedFigures.First().IsDead &&
						await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 1, 
							new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Teleport)} to any hex adjacent to the enemy, then perform {Icons.Inline(Icons.Attack)}2, {Icons.Inline(Icons.GetCondition(Conditions.Stun))}"));;
				})
				.Build()),
			
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditions(Conditions.Stun)
				.WithConditionalAbilityCheck(async state =>
				{
					return await AbilityCmd.HasPerformedAbility(state, 1);
				})
				.Build()),
		];
		
		public override int XP => 1;
		public override bool Loss => true;
		
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
	}
}