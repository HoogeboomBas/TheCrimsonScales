using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class DoubleBarrelRailcaster : ArtificerCardModel<DoubleBarrelRailcaster.CardTop, DoubleBarrelRailcaster.CardBottom>
{
	public override string Name => "Double-Barrel Railcaster";
	public override int Level => 8;
	public override int Initiative => 37;
	protected override int AtlasIndex => 26;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackCircle(this, new Vector2(0.3874074f, 0.23174602f)))
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East).Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.East).Add(Direction.East), AOEHexType.Red)
						]
					),
					new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East).Add(Direction.East).Add(Direction.East), this,
						new Vector2(0.86296296f, 0.22857141f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.East).Add(Direction.East).Add(Direction.East), this,
						new Vector2(0.86296296f, 0.35185182f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Disarm)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(5);
							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.79470897f)))
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
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}
}