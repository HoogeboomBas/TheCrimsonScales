using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class TheArmsOfOxcepi : IncarnateCardModel<TheArmsOfOxcepi.CardTop, TheArmsOfOxcepi.CardBottom>
{
	public override string Name => "The Arms of Oxcepi";
	public override int Level => 5;
	public override int Initiative => 56;
	protected override int AtlasIndex => 21;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.6169745f, 0.13518006f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach(ItemModel itemModel in attackAbilityState.ItemsUsed.Where(item => item.ItemState is ItemState.Spent))
					{
						await AbilityCmd.RefreshItem(itemModel);
						state.SetPerformed();
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.LongRestStartedEvent.Subscribe(state, this,
						parameters => parameters.Character == state.Performer,
						async parameters =>
						{
							await state.ActionState.RequestDiscardOrLose();

							ItemModel item = await AbilityCmd.SelectItem(parameters.Character,
								parameters.Character.Items.Where(item =>
									item.ItemState is ItemState.Consumed && item.ItemType is ItemType.OneHand or ItemType.TwoHands).ToList(),
								hintText: $"Select an item to {Icons.HintText(Icons.RecoverCard)}");

							if(item != null)
							{
								await AbilityCmd.RefreshItem(item);
							}
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.RecoverCard),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"{Icons.Inline(Icons.RecoverCard)} one of your lost {Icons.Inline(Icons.GetItem(ItemType.OneHand))} or {Icons.Inline(Icons.GetItem(ItemType.TwoHands))} items"));

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.LongRestStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Conqueror, IncarnateSpirit.Reaver];
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.5246223f, 0.6216067f)))
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.34427443f, 0.73593414f)))
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<MoveAbility.State>(0).Hexes.SelectMany(hex => hex.GetFigures()).Distinct());
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				//TODO: Items that affect a single attack apply to all attacks
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}