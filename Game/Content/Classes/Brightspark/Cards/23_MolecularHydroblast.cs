using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class MolecularHydroblast : BrightsparkCardModel<MolecularHydroblast.CardTop, MolecularHydroblast.CardBottom>
{
	public override string Name => "Molecular Hydroblast";
	public override int Level => 6;
	public override int Initiative => 29;
	protected override int AtlasIndex => 23;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5, new AttackDiamond(this, new Vector2(0.44888887f, 0.23915341f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.65925926f, 0.23809522f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Immobilize);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}")
					)
				)
				.Build())
		];
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.5237037f, 0.7026455f)))
				.WithMoveType(MoveType.Jump)
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(
						[CardElementConsumption.Consume(Element.Fire), CardElementConsumption.Consume(Element.Ice)],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(-1);
							parameters.AbilityState.SetCustomValue(this, "ElementsConsumed", true);
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"-1{Icons.Inline(Icons.Move)}, all enemies moved through gain {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}")))
				.WithAbilityPerformedSubscription(
					ScenarioEvents.AbilityPerformed.Subscription.New(parameters =>
							parameters.AbilityState.GetCustomValue<bool>(this, "ElementsConsumed"),
						async parameters =>
						{
							MoveAbility.State moveAbilityState = (MoveAbility.State)parameters.AbilityState;
							foreach(Figure figure in moveAbilityState.Hexes
								        .SelectMany(hex => hex.GetHexObjectsOfType<Figure>()).Distinct()
								        .Where(f => moveAbilityState.Performer.EnemiesWith(f)))
							{
								await AbilityCmd.AddCondition(moveAbilityState, figure, Conditions.Immobilize);
							}
						})
				)
				.Build())
		];
	}
}