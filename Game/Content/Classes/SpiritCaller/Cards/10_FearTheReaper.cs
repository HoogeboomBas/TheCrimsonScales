using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class FearTheReaper : SpiritCallerCardModel<FearTheReaper.CardTop, FearTheReaper.CardBottom>
{
	public override string Name => "Fear the Reaper";
	public override int Level => 1;
	public override int Initiative => 91;
	protected override int AtlasIndex => 28 - 10;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Deatheater")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/deatheater.png")
				.WithHealth(2)
				.WithMove(3)
				.WithAttack(1)
				.WithRange(3)
				.Build()
			),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					int usedRoundNumber = -1;
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						parameters =>
							state.Performer.EnemiesWith(parameters.Figure) &&
							parameters.PotentialKiller == state.Performer &&
							usedRoundNumber != GameController.Instance.ScenarioPhaseManager.RoundIndex &&
							RangeHelper.Distance(parameters.Figure.Hex, state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit.Hex) <= 3,
						async parameters =>
						{
							usedRoundNumber = GameController.Instance.ScenarioPhaseManager.RoundIndex;
							Spirit spirit = state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit;
							await Spirit.RemoveDamageCounters(spirit, 1);
						}
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

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62159026f, 0.6668694f)))
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Curse)
				.WithCustomGetPerformHex(state => state.GetCustomValue<Hex>(this, "Hex"))
				.WithConditionalAbilityCheck(async state =>
				{
					Figure spirit = await Spirit.SelectSpirit(state);

					if(spirit == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Hex", spirit.Hex);
					return true;
				})
				.Build())
		];
	}
}