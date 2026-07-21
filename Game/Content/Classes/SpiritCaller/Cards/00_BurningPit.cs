using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BurningPit : SpiritCallerCardModel<BurningPit.CardTop, BurningPit.CardBottom>
{
	public override string Name => "Burning Pit";
	public override int Level => 1;
	public override int Initiative => 45;
	protected override int AtlasIndex => 28 - 0;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Blazing Fire")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/blazing_fire.png")
				.WithHealth(2)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters =>
							parameters.Figure.Hex == state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit.Hex &&
							state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit.EnemiesWith(parameters.Figure),
						async parameters =>
						{
							Spirit spirit = state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit;

							await spirit.Destroy();

							await AbilityCmd.SufferDamage(state, parameters.Figure,
								GameController.Instance.ElementManager.GetState(Element.Fire) > ElementState.Inert ? 4 : 3);

							await state.ActionState.RequestDiscardOrLose();
						}, order: 1
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithMandatory(true)
				.WithSkipConfirmation()
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62150884f, 0.6548199f)))
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(1);
							parameters.AbilityState.AdjustMoveType(MoveType.Jump);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}")
					)
				)
				.Build()),

			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithCustomGetTargets((state, list) =>
				{
					foreach(Figure spirit in Spirit.GetAllSpirits())
					{
						foreach(Figure otherFigure in spirit.Hex.GetHexObjectsOfType<Figure>())
						{
							if(otherFigure != spirit)
							{
								list.Add(otherFigure);
							}
						}
					}
				})
				.WithTarget(Target.Enemies | Target.TargetAll)
				.Build())
		];
	}
}