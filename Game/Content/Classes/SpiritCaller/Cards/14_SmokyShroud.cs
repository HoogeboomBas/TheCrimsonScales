using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SmokyShroud : SpiritCallerCardModel<SmokyShroud.CardTop, SmokyShroud.CardBottom>
{
	public override string Name => "Smoky Shroud";
	public override int Level => 2;
	public override int Initiative => 81;
	protected override int AtlasIndex => 28 - 13;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Mantle of Darkness")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/mantle_of_darkness.png")
				.WithHealth(2)
				.WithMove(1)
				.WithAttack(2)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						parameters =>
							parameters.Figure == state.Performer &&
							parameters.Figure.Hex == state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit.Hex,
						async parameters =>
						{
							await AbilityCmd.AddCondition(state, state.Performer, Conditions.Invisible);
							state.SetCustomValue(this, "InvisibleGiven", true);
						}
					);

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						parameters =>
							state.GetCustomValue<bool>(this, "InvisibleGiven"),
						async parameters =>
						{
							await AbilityCmd.RemoveCondition(state.Performer, Conditions.Invisible, state);
							state.SetCustomValue(this, "InvisibleGiven", false);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);

					if(state.GetCustomValue<bool>(this, "InvisibleGiven"))
					{
						await AbilityCmd.RemoveCondition(state.Performer, Conditions.Invisible, state);
					}

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithMandatory(true)
				.WithSkipConfirmation()
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),

			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(1, new PushCircle(this, new Vector2(0.5121589f, 0.71031433f)))
				.WithRange(1)
				.WithDuringPushSubscriptions(
					ScenarioEvents.DuringPush.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AbilityAdjustPush(2);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Push)}")))
				.Build()),

			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithCustomGetTargets((state, list) =>
				{
					PushAbility.State pushAbilityState = state.ActionState.GetAbilityState<PushAbility.State>(1);
					foreach(SingleTargetState singleTargetState in pushAbilityState.SingleTargetStates)
					{
						foreach(Hex pushHex in singleTargetState.PushHexes)
						{
							if(Spirit.HasSpirit(pushHex))
							{
								list.Add(singleTargetState.Target);
								break;
							}
						}
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 1))
				.Build()),
		];
	}
}