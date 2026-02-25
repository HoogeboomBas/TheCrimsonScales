using System.Collections.Generic;
using System.Linq;

public class AdaptiveEngineering : ArtificerCardModel<AdaptiveEngineering.CardTop, AdaptiveEngineering.CardBottom>
{
	public override string Name => "Adaptive Engineering";
	public override int Level => 1;
	public override int Initiative => 75;
	protected override int AtlasIndex => 9;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Character character = (Character)await AbilityCmd.SelectFigure(state,
						figures => figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1)
							.Where(figure => figure is Character && state.Performer.AlliedWith(figure, true))));
					if(character == null)
					{
						return;
					}

					ItemModel item = await AbilityCmd.SelectItem(character,
						character.Items.Where(itemModel =>
							itemModel.ItemState is ItemState.Spent or ItemState.Consumed &&
							itemModel.ItemType is ItemType.Head or ItemType.Feet or ItemType.OneHand or ItemType.TwoHands).ToList(),
						effectType: EffectType.Selectable,
						hintText: $"Select an item to {Icons.HintText(Icons.RecoverCard)}");
					if(item != null)
					{
						await AbilityCmd.RefreshItem(item);
					}

					if(state.GetCustomValue<bool>(this, "LostScrap"))
					{
						AbilityCard card = await AbilityCmd.SelectAbilityCard(character, CardState.Discarded,
							canSelectFunc: card => card.Model.Level == 1, hintText: $"Select a level 1 card to {Icons.HintText(Icons.RecoverCard)}");
						if(card != null)
						{
							await AbilityCmd.ReturnToHand(card);
						}
					}
				})
				.WithAbilityStartedSubscription(
					LoseScrapTokenSubscription<ScenarioEvents.AbilityStarted.Parameters>(1,
						async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "LostScrap", true);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						new TextEffectInfoView.Parameters(
							$"That figure may also {Icons.Inline(Icons.RecoverCard)} one level 1 card from their discard pile")))
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					Summon summon = state.GetCustomValue<Summon>(this, "DesignatedSummon");
					figures.AddRange(RangeHelper.GetFiguresInRange(summon.Hex, 2));
				})
				.WithOnAbilityStarted(async state =>
				{
					Figure summon = await AbilityCmd.SelectFigure(state, figures => figures.AddRange(((Character)state.Performer).Summons),
						hintText: () => "Designate one of your summons");
					if(summon == null)
					{
						state.SetBlocked();
						return;
					}

					state.SetCustomValue(this, "DesignatedSummon", summon);
				})
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Summon summon = state.ActionState.GetAbilityState<AttackAbility.State>(0).GetCustomValue<Summon>(this, "DesignatedSummon");
					await AbilityCmd.KillOrExhaust(state, summon);
					await GainScrapToken(state);
					await GainScrapToken(state);
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}