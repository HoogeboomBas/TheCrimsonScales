using System.Collections.Generic;
using System.Linq;
using Godot;

public class WieldedMemory : IncarnateCardModel<WieldedMemory.CardTop, WieldedMemory.CardBottom>
{
	public override string Name => "Wielded Memory";
	public override int Level => 1;
	public override int Initiative => 38;
	protected override int AtlasIndex => 13;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.6191026f, 0.24412134f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.ItemsUsed.Any(itemModel =>
							itemModel.ItemType is ItemType.OneHand or ItemType.TwoHands && itemModel.Owner == parameters.Performer),
						async parameters =>
						{
							//TODO: Need to expand when you get +attack for multi-target attacks
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(1, new PushCircle(this, new Vector2(0.40413168f, 0.7387121f)))
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(2)
				.WithConditions(Conditions.Rupture)
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}