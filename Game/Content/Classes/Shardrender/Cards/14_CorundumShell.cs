using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class CorundumShell : ShardrenderCardModel<CorundumShell.CardTop, CorundumShell.CardBottom>
{
	public override string Name => "Corundum Shell";
	public override int Level => 2;
	public override int Initiative => 28;
	protected override int AtlasIndex => 14;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Ward, new ConditionDiamond(this, new Vector2(0.48581886f, 0.23379503f)))
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(CrystallizeAbility.Builder()
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.2910257f, 0.36191875f)),
					new UseSlot(new Vector2(0.49823594f, 0.36191875f)),
					new UseSlot(new Vector2(0.7062223f, 0.36191875f))
				])
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62153083f, 0.6489074f)))
				.Build()),
			new AbilityCardAbility(MoveCharacterTokenBackAbility(1).Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.FromAttack && parameters.Figure == state.Performer && parameters.WouldSufferDamage,
						async parameters =>
						{
							parameters.AdjustShield(2);

							await state.ActionState.RequestDiscardOrLose();
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Shield),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Discard Corundum Shell to gain {Icons.Inline(Icons.Shield)}2"));

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Persistent => true;
	}
}