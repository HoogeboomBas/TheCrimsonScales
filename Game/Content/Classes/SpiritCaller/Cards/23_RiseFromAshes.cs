using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class RiseFromAshes : SpiritCallerCardModel<RiseFromAshes.CardTop, RiseFromAshes.CardBottom>
{
	public override string Name => "Rise from Ashes";
	public override int Level => 7;
	public override int Initiative => 55;
	protected override int AtlasIndex => 28 - 23;

	public class SpectralPhoenixTrait : FigureTrait
	{
		public override async GDTask Activate(Figure figure)
		{
			await base.Activate(figure);

			ScenarioEvents.BeforeFigureKilledEvent.Subscribe(figure, this,
				parameters => parameters.Figure == figure,
				async parameters =>
				{
					ActionState actionState = new ActionState(figure,
						[
							ConditionAbility.Builder()
								.WithConditions(Conditions.Wound1)
								.WithTarget(Target.Any | Target.TargetAll)
								.WithRange(1)
								.WithMandatory(true)
								.Build()
						]
					);

					await actionState.Perform();
				}
			);
		}

		public override async GDTask Deactivate(Figure figure)
		{
			await base.Deactivate(figure);

			ScenarioEvents.BeforeFigureKilledEvent.Unsubscribe(figure, this);
		}
	}

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Glowing Egg")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/spectral_phoenix.png")
				.WithHealth(3)
				.WithSetDontRequestDiscardOrLoseAfterSpiritKilled()
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Spirit spirit = state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit;

					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						parameters => parameters.Figure == spirit,
						async parameters =>
						{
							ActionState actionState = new ActionState(state.ActionState, state.Performer,
								[
									SpawnAbility.Builder()
										.WithName("Spectral Phoenix")
										.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/spectral_phoenix.png")
										.WithHealth(1)
										.WithMove(4)
										.WithAttack(3)
										.WithTraits(new SpectralPhoenixTrait())
										.WithGetValidHexes((_, list) =>
										{
											list.Add(spirit.Hex);
										})
										.Build()
								]
							);

							await actionState.Perform();

							if(!actionState.GetHasPerformed())
							{
								await actionState.RequestDiscardOrLose();
							}
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

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
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5, new AttackDiamond(this, new Vector2(0.28229654f, 0.73834634f)))
				.WithCustomGetTargets((state, list) =>
				{
					list.AddRange(RangeHelper.GetFiguresInRange(state.GetCustomValue<Figure>(this, "Spirit"), 1));
				})
				.WithConditionalAbilityCheck(async state =>
				{
					Figure spirit = await Spirit.SelectSpirit(state);

					if(spirit == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Spirit", spirit);
					return true;
				})
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.KillOrExhaust(state,
						state.ActionState.GetAbilityState<AttackAbility.State>(0).GetCustomValue<Figure>(this, "Spirit"));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}
}