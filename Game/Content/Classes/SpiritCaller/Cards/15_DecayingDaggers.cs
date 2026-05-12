using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class DecayingDaggers : SpiritCallerCardModel<DecayingDaggers.CardTop, DecayingDaggers.CardBottom>
{
	public override string Name => "Decaying Daggers";
	public override int Level => 3;
	public override int Initiative => 28;
	protected override int AtlasIndex => 28 - 15;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.4038575f, 0.24590431f)))
				.WithTargets(2)
				.WithRange(3, new RangeSquare(this, new Vector2(0.70742154f, 0.24483776f)))
				.WithAfterTargetConfirmedSubscription(ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
					parameters =>
						RangeHelper.GetFiguresInRange(parameters.AbilityState.Target, 1, includeNonFigures: true)
							.Any(figure => Spirit.CountsAsSpirit(figure)),
					async parameters =>
					{
						parameters.AbilityState.SingleTargetAdjustAttackValue(1);

						await GDTask.CompletedTask;
					}))
				.Build())
		];
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters =>
							parameters.FromAttack &&
							parameters.TotalShield > 0 &&
							Spirit.CountsAsSpirit(parameters.PotentialAbilityState.Performer),
						async parameters =>
						{
							parameters.AdjustPierce(2);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.28975242f, 0.7408624f), GainXP),
					new UseSlot(new Vector2(0.49964964f, 0.7408624f), Air),
					new UseSlot(new Vector2(0.706647f, 0.7408624f), GainXP),
					new UseSlot(new Vector2(0.3969018f, 0.87083995f), Dark),
					new UseSlot(new Vector2(0.6038991f, 0.87083995f), GainXP),
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;

		private async GDTask Air(AbilityState state)
		{
			await AbilityCmd.InfuseElement(state, Element.Air);
		}

		private async GDTask Dark(AbilityState state)
		{
			await AbilityCmd.InfuseElement(state, Element.Dark);
		}
	}
}