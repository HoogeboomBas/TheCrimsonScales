using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class RootedSubjugation : HierophantCardModel<RootedSubjugation.CardTop, RootedSubjugation.CardBottom>
{
	public override string Name => "Rooted Subjugation";
	public override int Level => 4;
	public override int Initiative => 30;
	protected override int AtlasIndex => 29 - 19;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(3, range: 3, pierce: 3)),

			new AbilityCardAbility(new OtherActiveAbility(
				async state =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					Figure target = attackAbilityState.UniqueTargetedFigures.First(figure => !figure.IsDead);

					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						parameters => parameters.RetaliatingFigure == target,
						async parameters =>
						{
							await GDTask.CompletedTask;

							parameters.SetRetaliateBlocked();
						}
					);

					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == target,
						parameters =>
						{
							parameters.Add(new FigureInfoTextExtraEffect.Parameters($"Attacks targeting this figure are unaffected by {Icons.Inline(Icons.Retaliate)} this round."));
						}
					);

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				},
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					return attackAbilityState.Performed && !attackAbilityState.UniqueTargetedFigures.TrueForAll(figure => figure.IsDead);
				}
			))
		];

		protected override IEnumerable<Element> Elements => [Element.Earth];
		protected override bool Round => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),

			new AbilityCardAbility(new GrantAbility(figure => [new RetaliateAbility(1, range: 2)]))
		];

		protected override bool Round => true;
	}
}