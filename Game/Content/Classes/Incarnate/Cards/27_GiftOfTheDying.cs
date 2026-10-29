using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class GiftOfTheDying : IncarnateCardModel<GiftOfTheDying.CardTop, GiftOfTheDying.CardBottom>
{
	public override string Name => "Gift of the Dying";
	public override int Level => 8;
	public override int Initiative => 78;
	protected override int AtlasIndex => 27;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.6169745f, 0.13905817f)))
				.Build()),
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Figure figure = await AbilityCmd.SelectFigure(state, figures =>
						{
							figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer, 2)
								.Where(figure => state.Performer.AlliedWith(figure) && figure is Character));
						},
						hintText: () =>
							$"Select an ally to {Icons.HintText(Icons.RecoverCard)} their spent {Icons.HintText(Icons.GetItem(ItemType.OneHand))} and {Icons.HintText(Icons.GetItem(ItemType.TwoHands))} items");

					foreach(ItemModel item in ((Character)state.Performer).Items.Where(item =>
						        item.ItemType is ItemType.OneHand or ItemType.TwoHands && item.ItemState is ItemState.Spent))
					{
						await AbilityCmd.RefreshItem(item);
						state.SetPerformed();
					}

					foreach(ItemModel item in ((Character)figure).Items.Where(item =>
						        item.ItemType is ItemType.OneHand or ItemType.TwoHands && item.ItemState is ItemState.Spent))
					{
						await AbilityCmd.RefreshItem(item);
						state.SetPerformed();
					}
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<LootAbility.State>(1).LootedObjects.Count >= 1;
				})
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Ritualist, IncarnateSpirit.Reaver];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.6186266f, 0.6518776f)))
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Reaver))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Conqueror];
	}
}