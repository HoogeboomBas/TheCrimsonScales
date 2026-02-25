using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class RepurposeLeftovers : ArtificerCardModel<RepurposeLeftovers.CardTop, RepurposeLeftovers.CardBottom>
{
	public override string Name => "Repurpose Leftovers";
	public override int Level => 1;
	public override int Initiative => 68;
	protected override int AtlasIndex => 4;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5)
				.WithConditionalAbilityCheck(async state =>
				{
					ItemModel item = await AbilityCmd.SelectItem(state.Performer,
						((Character)state.Performer).Items.Where(itemModel =>
							itemModel.ItemState is not ItemState.Consumed &&
							itemModel.ItemType is ItemType.Head or ItemType.Feet or ItemType.OneHand or ItemType.TwoHands).ToList(),
						effectType: EffectType.Selectable,
						hintText: $"Select an item to {Icons.HintText(Icons.LoseCard)}");
					if(item == null)
					{
						return false;
					}

					await item.SetItemState(ItemState.Consumed);
					return true;
				})
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await GainScrapToken(state);
					await AbilityCmd.GainXP(state.Performer, 1);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						parameters => parameters.AbilityState is MoveAbility.State && state.Performer.AlliedWith(parameters.Performer, true) &&
						              RangeHelper.Distance(state.Performer.Hex, parameters.Performer.Hex) <= 1,
						async parameters =>
						{
							MoveAbility.State moveAbilityState = (MoveAbility.State)parameters.AbilityState;
							moveAbilityState.AdjustMoveValue(1);
							moveAbilityState.AddJump();
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build()),
			TimedTrack(
			[
				new UseSlot(new Vector2(0.39741865f, 0.80793643f)),
				new UseSlot(new Vector2(0.6059259f, 0.80793643f))
			])
		];

		public override Func<Figure, GDTask<bool>> OnCardSideStarted => async figure => await TryLoseScrapTokens(figure, 1);
		public override int XP => 1;
		public override bool Persistent => true;
	}
}