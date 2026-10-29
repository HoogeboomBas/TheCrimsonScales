using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ProtectiveGale : TempestCardModel<ProtectiveGale.CardTop, ProtectiveGale.CardBottom>
{
	public override string Name => "Protective Gale";
	public override int Level => 9;
	public override int Initiative => 02;
	protected override int AtlasIndex => 28;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer && parameters.Performer.HasCondition(Conditions.Dodge),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Immobilize);

							await GDTask.CompletedTask;
						});

					await AbilityCmd.GainXP(state.Performer, 2);

					state.ActionState.SetOverridePersistent();
					state.ActionState.SetOverrideLoss();
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6, new AttackDiamond(this, new Vector2(0.49590778f, 0.29806095f)))
				.WithPush(2)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(applyFunction: async parameters =>
					{
						Hex hex = await AbilityCmd.SelectHex(parameters.AbilityState,
							hexes => hexes.AddRange(RangeHelper.GetHexesInRange(parameters.AbilityState.Target.Hex, 1, false)),
							hintText: "Select a hex to push away from");

						if(hex != null)
						{
							parameters.AbilityState.SingleTargetSetForcedMovementOriginHex(hex);
						}
					}))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse([Element.Air, Element.Light])];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(6, new MoveCircle(this, new Vector2(0.52307016f, 0.61066455f)))
				.WithMoveType(MoveType.Jump)
				.WithAbilityPerformedSubscription(
					LoseSparksConsumeElementSubscription<ScenarioEvents.AbilityPerformed.Parameters>(2, Element.Light, async parameters =>
					{
						foreach(Figure figure in ((MoveAbility.State)parameters.AbilityState).Hexes.SelectMany(hex =>
							        hex.GetFigures().Where(figure => parameters.Performer.EnemiesWith(figure)))
						        .Distinct())
						{
							await AbilityCmd.AddCondition(parameters.AbilityState, figure, Conditions.Immobilize);
						}
					}, new TextEffectInfoView.Parameters($"All enemies moved through gain {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}")))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					for(int i = 0; i < state.GetCustomValue<int>(this, "X"); i++)
					{
						List<Figure> targetedFigures = [];
						Figure figure = await AbilityCmd.SelectFigure(state, figures =>
							{
								figures.AddRange(GameController.Instance.Map.Figures.Where(figure => state.Performer.AlliedWith(figure, true))
									.Except(targetedFigures));
							}, autoSkipIfNone: true,
							hintText: () =>
								$"Select an ally or self to {Icons.HintText(Icons.RecoverCard)} up to three discarded cards and gain {Icons.Inline(Icons.GetCondition(Conditions.Dodge))}");
						if(figure == null)
						{
							break;
						}

						targetedFigures.Add(figure);
						if(figure is Character character)
						{
							List<AbilityCard> abilityCards = await AbilityCmd.SelectAbilityCards(character, CardState.Discarded, 0, 3,
								hintText: $"{Icons.HintText(Icons.RecoverCard)} up to three discarded cards");

							foreach(AbilityCard abilityCard in abilityCards)
							{
								await AbilityCmd.ReturnToHand(abilityCard);
							}
						}

						await AbilityCmd.AddCondition(state, figure, Conditions.Dodge);
					}

					await AbilityCmd.GainXP(state.Performer, 1);
					state.ActionState.SetOverrideLoss();
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					bool lostSparks = false;
					List<ScenarioEvents.GenericChoice.Subscription> subscriptions = [];
					for(int i = 1; i <= SparkCount(state.Performer); i++)
					{
						int count = i;
						subscriptions.Add(ScenarioEvents.GenericChoice.Subscription.New(
							_ => HasXSparks(state.Performer, count),
							async _ =>
							{
								LoseSparks(state.Performer, count);
								state.SetCustomValue(this, "X", count);
								lostSparks = true;
								await GDTask.CompletedTask;
							},
							EffectType.Selectable,
							effectButtonParameters: new TextEffectButton.Parameters($"{count}{Icons.HintText(Tempest.SparkIconPath)}"),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Affect {count} allies"),
							canApplyMultipleTimesDuringSubscription: false));
					}
					await AbilityCmd.GenericChoice(state.Performer, subscriptions, hintText: $"Spend {Icons.HintText(Tempest.SparkIconPath)}?");

					return lostSparks;
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Air)];
	}
}