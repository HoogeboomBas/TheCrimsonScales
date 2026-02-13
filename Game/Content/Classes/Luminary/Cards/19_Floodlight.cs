using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Floodlight : LuminaryCardModel<Floodlight.CardTop, Floodlight.CardBottom>
{
	public override string Name => "Floodlight";
	public override int Level => 4;
	public override int Initiative => 71;
	protected override int AtlasIndex => 19;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithConditions(Conditions.Poison1)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
					]
				))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithCustomGetTargets((state, targets) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					targets.AddRange(attackAbilityState.GetEmptyAOEHexes().SelectMany(hex => hex.GetHexObjectsOfType<Figure>()));
				})
				.WithConditionalAbilityCheck(async state =>
				{
					return state.ActionState.GetAbilityState<AttackAbility.State>(0).Performed &&
					       await AbilityCmd.AskConsumeElement(state.Performer, Element.Ice);
				})
				.Build()),
			Scuttle(2, [Element.Light]),
		];

		public override int XP => 1;
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						parameters => parameters.Target == state.Performer &&
						              AbilityCmd.CheckImmunity(parameters.ConditionModel, Conditions.Immobilize),
						async parameters =>
						{
							parameters.SetPrevented(true);

							await GDTask.CompletedTask;
						}
					);

					ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						parameters =>
						{
							parameters.AddImmunity(Conditions.Immobilize);
						}
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.InfuseElementEvent.Subscribe(state, this,
						parameters => parameters.Authority == state.Performer && parameters.Element == Element.Dark
						                                                      && parameters.AbilityState != state,
						async parameters =>
						{
							parameters.SetCanInfuse(false);
							await AbilityCmd.InfuseWildElement(state);
							Element element = state.UseSlotIndex == 0 ? Element.Light : (state.UseSlotIndex == 1) ? Element.Fire : Element.Ice;
							await AbilityCmd.InfuseElement(state, element);

							await state.AdvanceUseSlot();
						}
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.InfuseElementEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.16350047f, 0.8974989f), GainXP),
					new UseSlot(new Vector2(0.37350035f, 0.8974989f), GainXP),
					new UseSlot(new Vector2(0.58100003f, 0.8974989f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}