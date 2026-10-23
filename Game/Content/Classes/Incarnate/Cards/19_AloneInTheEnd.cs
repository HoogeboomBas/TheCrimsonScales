using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AloneInTheEnd : IncarnateCardModel<AloneInTheEnd.CardTop, AloneInTheEnd.CardBottom>
{
	public override string Name => "Alone in the End";
	public override int Level => 4;
	public override int Initiative => 74;
	protected override int AtlasIndex => 19;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(1)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithMinRange(1)
				.WithRange(1)
				.WithDuringPushSubscriptions(
					InSpiritSubscription<ScenarioEvents.DuringPush.Parameters>(IncarnateSpirit.Reaver,
						async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Rupture);

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Fire);
						}))
				.WithConditionalAbilityCheck(async state =>
				{
					Hex hex = await AbilityCmd.SelectHex(state, hexes =>
					{
						hexes.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 1)
							.Where(hex => hex.GetFigures().Any(figure => state.Performer.EnemiesWith(figure))));
					}, hintText: "Designate one adjacent hex occupied by an enemy");

					if(hex == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Hex", hex);
					return true;
				})
				.WithCustomGetPerformHex(state => state.GetCustomValue<Hex>(this, "Hex"))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.30732313f, 0.42437676f)))
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<PushAbility.State>(0).GetCustomValue<Hex>(this, "Hex").GetFigures());
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6210548f, 0.634403f)))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Performer),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);

							await GDTask.CompletedTask;
						});

					state.ActionState.SetOverrideRound();

					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Ritualist];
	}
}