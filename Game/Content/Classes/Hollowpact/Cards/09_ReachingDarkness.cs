using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

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
				.WithDamage(2)
				.WithRange(5)
				.Build()),
			
			//TODO: Produce
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
						list.AddRange(RangeHelper.GetHexesInRange(targetedHex, 1).Where(hex => hex.IsUnoccupied()));
					}
				})
				.WithConditionalAbilityCheck(async state =>
				{
					//TODO: Consume &&
					
					return await AbilityCmd.HasPerformedAbility(state, 0);
				})
				.Build()),
			
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditions(Conditions.Stun)
				.WithConditionalAbilityCheck(async state =>
				{
					//TODO: Consume &&
					
					return await AbilityCmd.HasPerformedAbility(state, 1);
				})
				.WithCustomGetTargets((state, list) =>
				{
					list.AddRange(state.ActionState.GetAbilityState<SufferDamageAbility.State>(0).TargetedFigures);
				})
				.Build()),
		];
		
		public override int XP => 1;
		public override bool Loss => true;
		
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
	}
}