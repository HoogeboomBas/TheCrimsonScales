using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class RansackClutter : ArtificerCardModel<RansackClutter.CardTop, RansackClutter.CardBottom>
{
	public override string Name => "Ransack Clutter";
	public override int Level => 4;
	public override int Initiative => 18;
	protected override int AtlasIndex => 18;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.44572595f, 0.2303878f)))
				.WithPierce(3, new PierceSquare(this, new Vector2(0.6696296f, 0.23048781f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					ItemModel item = await AbilityCmd.SelectItem(state.Performer,
						((Character)state.Performer).Items.Where(itemModel =>
							itemModel.ItemState is ItemState.Spent or ItemState.Consumed &&
							itemModel.ItemType is ItemType.Head or ItemType.Feet or ItemType.OneHand or ItemType.TwoHands).ToList(),
						effectType: EffectType.Selectable,
						hintText: $"Select an item to {Icons.HintText(Icons.RecoverCard)}");

					if(item != null)
					{
						await AbilityCmd.RefreshItem(item);
					}
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					AttackAbility.State attackState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					return attackState.KilledTargets.Count >= 1;
				})
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62232226f, 0.7094238f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					foreach(Hex hex in state.ActionState.GetAbilityState<MoveAbility.State>(0).Hexes)
					{
						await AbilityCmd.LootHex(state.Performer, hex);
						state.SetPerformed();
					}

					await GDTask.CompletedTask;
				})
				.Build()),
		];
	}
}