using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class RootedSubjugation : HierophantLevelUpCardModel<RootedSubjugation.CardTop, RootedSubjugation.CardBottom>
{
	public override string Name => "Rooted Subjugation";
	public override int Level => 4;
	public override int Initiative => 30;
	protected override int AtlasIndex => 15 - 4;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.3761863f, 0.1670583f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.6018258f, 0.1670583f)))
				.WithPierce(3)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
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
							parameters.Add(new InfoTextExtraEffect.Parameters(
								$"Attacks targeting this figure are unaffected by {Icons.Inline(Icons.Retaliate)} this round."));
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
						return attackAbilityState.Performed && !attackAbilityState.UniqueTargetedFigures.TrueForAll(figure => figure.IsDead);
					}
				)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];
		public override bool Round => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62082916f, 0.6941986f)))
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state => [RetaliateAbility.Builder().WithRetaliateValue(1).WithRange(2).Build()])
				.Build())
		];

		public override bool Round => true;
	}
}