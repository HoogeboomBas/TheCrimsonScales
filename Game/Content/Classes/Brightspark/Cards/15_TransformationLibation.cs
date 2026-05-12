using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class TransformationLibation : BrightsparkCardModel<TransformationLibation.CardTop, TransformationLibation.CardBottom>
{
	public override string Name => "Transformation Libation";
	public override int Level => 2;
	public override int Initiative => 27;
	protected override int AtlasIndex => 15;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Monster monster = state.GetCustomValue<Monster>(this, "Monster");

					await AbilityCmd.AddCharacterToken(state, monster, textParameters =>
						$"This monster is an ally to you, suffers {Icons.Inline(Icons.Damage)}1 at the start of each of its turns, and cannot be healed");

					ScenarioCheckEvents.FigureRelationshipCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == monster || parameters.OtherFigure == monster,
						parameters =>
						{
							if(parameters.Figure == state.Performer || parameters.OtherFigure == state.Performer)
							{
								parameters.SetAlliedWith();
								return;
							}

							if(parameters.Figure == monster)
							{
								parameters.SetFigureRelationship(state.Performer.GetRelationship(parameters.OtherFigure));
							}
							else
							{
								parameters.SetFigureRelationship(parameters.Figure.GetRelationship(state.Performer));
							}
						}
					);

					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						parameters => parameters.Figure == monster,
						async parameters =>
						{
							await AbilityCmd.SufferDamage(monster, 1, monster);
						}
					);
					ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(state, this,
						parameters => parameters.PotentialTarget == monster && parameters.PotentialAbilityState is HealAbility.State,
						parameters =>
						{
							parameters.SetCannotBeTargeted();
						}
					);
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						parameters => parameters.Figure == monster,
						async parameters =>
						{
							await state.ActionState.RequestDiscardOrLose();
						}
					);
				})
				.WithOnDeactivate(async state =>
				{
					Monster monster = state.GetCustomValue<Monster>(this, "Monster");

					await AbilityCmd.RemoveCharacterToken(state, monster);

					ScenarioCheckEvents.FigureRelationshipCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state =>
				{
					Monster monster = (Monster)await AbilityCmd.SelectFigure(state, list =>
						list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1).Where(figure =>
							figure.EnemiesWith(state.Performer) && figure.Health <= 4 && figure is Monster monster &&
							monster.MonsterType is MonsterType.Normal)));

					if(monster == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Monster", monster);
					return true;
				})
				.WithSkipConfirmation()
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Air)];
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ControlAbility.Builder()
				.WithGetAbilities(state =>
					[
						MoveAbility.Builder()
							.WithDistance(2, new MoveCircle(this, new Vector2(0.6213409f, 0.7961962f)))
							.Build()
					]
				)
				.WithRange(3, new RangeSquare(this, new Vector2(0.7985185f, 0.73534966f)))
				.Build()),
		];
	}
}