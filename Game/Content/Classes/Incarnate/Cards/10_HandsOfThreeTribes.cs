using System.Collections.Generic;
using System.Linq;
using Godot;

public class HandsOfThreeTribes : IncarnateCardModel<HandsOfThreeTribes.CardTop, HandsOfThreeTribes.CardBottom>
{
	public override string Name => "Hands of Three Tribes";
	public override int Level => 1;
	public override int Initiative => 70;
	protected override int AtlasIndex => 10;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.6185266f, 0.2465374f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					ItemModel item = await AbilityCmd.SelectItem(state.Performer,
						((Character)state.Performer).Items.Where(item =>
							item.ItemState is ItemState.Spent && item.ItemType is ItemType.OneHand or ItemType.TwoHands).ToList(),
						hintText: $"Select an item to {Icons.HintText(Icons.RecoverCard)}");

					if(item != null)
					{
						await AbilityCmd.RefreshItem(item);
						state.SetPerformed();
					}
				})
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1, new MoveCircle(this, new Vector2(0.62443507f, 0.72364575f)))
				.Build()),
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build())
		];
	}
}