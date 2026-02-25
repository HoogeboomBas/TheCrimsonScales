using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ElementalCondenser : ArtificerCardModel<ElementalCondenser.CardTop, ElementalCondenser.CardBottom>
{
	public override string Name => "Elemental Condenser";
	public override int Level => 1;
	public override int Initiative => 10;
	protected override int AtlasIndex => 12;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.45037037f, 0.24021162f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.65925926f, 0.24024072f)))
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						parameters => !parameters.AbilityState.GetCustomValue<bool>(this, "ConsumedElement"),
						async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Wound1);
							parameters.AbilityState.SetCustomValue(this, "ConsumedElement", true);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Wound1)))),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Ice,
						parameters => !parameters.AbilityState.GetCustomValue<bool>(this, "ConsumedElement"),
						async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Immobilize);
							parameters.AbilityState.SetCustomValue(this, "ConsumedElement", true);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Immobilize)))),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Air,
						parameters => !parameters.AbilityState.GetCustomValue<bool>(this, "ConsumedElement"),
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPush(2);
							parameters.AbilityState.SetCustomValue(this, "ConsumedElement", true);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Push)}2")),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Earth,
						parameters => !parameters.AbilityState.GetCustomValue<bool>(this, "ConsumedElement"),
						async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Poison1);
							parameters.AbilityState.SetCustomValue(this, "ConsumedElement", true);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Poison1))))
				])
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6207408f, 0.72010577f)))
				.WithDuringMovementSubscriptions(
				[
					//TODO: Change to consume choice element
					ScenarioEvents.DuringMovement.Subscription.ConsumeElements([Element.Fire, Element.Ice],
						parameters => !parameters.AbilityState.GetCustomValue<bool>(this, "ConsumedElement"),
						async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(2);
							parameters.AbilityState.SetCustomValue(this, "ConsumedElement", true);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Move)}")),
					ScenarioEvents.DuringMovement.Subscription.ConsumeElements([Element.Air, Element.Earth],
						parameters => !parameters.AbilityState.GetCustomValue<bool>(this, "ConsumedElement"),
						async parameters =>
						{
							parameters.AbilityState.AddJump();
							parameters.AbilityState.SetCustomValue(this, "ConsumedElement", true);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters(Icons.Inline(Icons.Jump)))
				])
				.Build())
		];
	}
}