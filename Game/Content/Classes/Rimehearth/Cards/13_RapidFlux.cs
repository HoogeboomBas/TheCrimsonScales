using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class RapidFlux : RimehearthCardModel<RapidFlux.CardTop, RapidFlux.CardBottom>
{
	public override string Name => "Rapid Flux";
	public override int Level => 2;
	public override int Initiative => 25;
	protected override int AtlasIndex => 13;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6214308f, 0.19773795f)))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red)
						]
					),
					new AOEHexMark(Vector2I.Zero.Add(Direction.NorthWest), this, new Vector2(0.5113291f, 0.3052632f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.East).Add(Direction.NorthEast), this, new Vector2(0.71097875f, 0.3052632f)))
				.Build())
		];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, state.Performer,
						parameters => parameters.AbilityState.Target == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasDisadvantage();

							await GDTask.CompletedTask;
						}
					);

					ScenarioCheckEvents.DisadvantageCheckEvent.Subscribe(state, state.Performer,
						parameters => parameters.Target == state.Performer,
						parameters => parameters.SetDisadvantage(true)
					);

					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, state.Performer,
						parameters => state.Performer == parameters.Figure,
						parameters => parameters.Add(
							new InfoTextExtraEffect.Parameters(_ => "All attacks targeting this figure this round gain disadvantage."))
					);

					state.ActionState.SetOverridePersistent();

					await AbilityCmd.InfuseElement(state, Element.Ice);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, state.Performer);
					ScenarioCheckEvents.DisadvantageCheckEvent.Unsubscribe(state, state.Performer);
					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, state.Performer);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Fire,
					effectInfoText: $"All attacks targeting you this round gain disadvantage, {Icons.Inline(Icons.GetElement(Element.Ice))}"))
				.WithSkipConfirmation()
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, state.Performer,
						parameters => parameters.AbilityState.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();

							await GDTask.CompletedTask;
						}
					);

					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, state.Performer,
						parameters => state.Performer == parameters.Figure,
						parameters => parameters.Add(
							new InfoTextExtraEffect.Parameters(_ => "All this figure's attacks this round gain advantage."))
					);

					state.ActionState.SetOverridePersistent();

					await AbilityCmd.InfuseElement(state, Element.Fire);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, state.Performer);
					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, state.Performer);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state => !await AbilityCmd.HasPerformedAbility(state, 0) && await AbilityCmd.AskConsumeElement(
					state.Performer, Element.Ice,
					effectInfoText: $"All your attacks gain advantage this round, {Icons.Inline(Icons.GetElement(Element.Fire))}"))
				.WithSkipConfirmation()
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build())
		];
	}
}