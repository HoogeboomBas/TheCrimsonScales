using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class RecycleParts : ArtificerCardModel<RecycleParts.CardTop, RecycleParts.CardBottom>
{
	public override string Name => "Recycle Parts";
	public override int Level => 1;
	public override int Initiative => 24;
	protected override int AtlasIndex => 3;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					LootAbility.State lootState = state.ActionState.GetAbilityState<LootAbility.State>(0);
					for(int i = 0; i < Math.Min(2, lootState.TotalLootedCount); i++)
					{
						await GainScrapToken(state);
						state.SetPerformed();
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62128145f, 0.68961644f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
					Figure figure = await AbilityCmd.SelectFigure(state,
						figures => figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1)
							.Where(figure => figure is Character && figure.AlliedWith(state.Performer, true))),
						hintText: () => $"Select a figure to {Icons.HintText(Icons.RecoverCard)} one spent item");
					if(figure == null)
					{
						return;
					}

					ItemModel item = await AbilityCmd.SelectItem((Character)figure, ItemState.Spent,
						hintText: $"Select an item to {Icons.Inline(Icons.RecoverCard)}");
					if(item != null)
					{
						await AbilityCmd.RefreshItem(item);
					}
				})
				.WithConditionalAbilityCheck(async state => await LoseScrapTokensConditionalAbilityCheck(state.Performer, 1,
					new TextEffectInfoView.Parameters($"You or one adjacent ally may {Icons.Inline(Icons.RecoverCard)} one spent item")))
				.Build())
		];
	}
}