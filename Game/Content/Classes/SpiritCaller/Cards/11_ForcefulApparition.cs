using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ForcefulApparition : SpiritCallerCardModel<ForcefulApparition.CardTop, ForcefulApparition.CardBottom>
{
	public override string Name => "Forceful Apparition";
	public override int Level => 1;
	public override int Initiative => 16;
	protected override int AtlasIndex => 28 - 11;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Leeching Phantasm")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/leeching_phantasm.png")
				.WithHealth(2)
				.WithMove(2)
				.WithAttack(2)
				.WithTraits(new ControlTargetTrait(
					MoveAbility.Builder()
						.WithDistance(2)
						.WithOnAbilityEndedPerformed(async state =>
						{
							await AbilityCmd.AddCondition(null, state.Performer, Conditions.Stun);
						})
						.Build(),
					textParameters =>
						$"{Icons.Inline(Icons.Move, textParameters)}2, if this is performed, this figure gains {Icons.InlineCondition(Conditions.Stun, textParameters)}"))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62149024f, 0.6477171f)))
				.Build()),

			new AbilityCardAbility(ControlAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder()
						.WithDistance(1)
						.WithOnAbilityStarted(async state =>
						{
							Figure spiritToMove = state.Performer.Hex.GetFigures(true).FirstOrDefault(figure => Spirit.CountsAsSpirit(figure));

							if(spiritToMove == null || spiritToMove == state.Performer)
							{
								return;
							}

							ScenarioEvents.MoveTogetherEvent.Subscribe(state, this,
								parameters =>
									parameters.AbilityState == state &&
									parameters.Performer == state.Performer,
								async parameters =>
								{
									parameters.AddOtherFigure(spiritToMove);
									parameters.SetTriggerHexEffects(false);

									await GDTask.CompletedTask;
								}
							);

							await GDTask.CompletedTask;
						})
						.WithOnAbilityEnded(async state =>
						{
							ScenarioEvents.MoveTogetherEvent.Unsubscribe(state, this);

							await GDTask.CompletedTask;
						})
						.Build()
				])
				.WithCustomGetTargets((state, list) =>
				{
					foreach(Figure spirit in Spirit.GetAllSpirits())
					{
						foreach(Figure otherFigure in spirit.Hex.GetFigures())
						{
							if(otherFigure != spirit)
							{
								list.Add(otherFigure);
							}
						}
					}
				})
				.Build()),
		];
	}
}