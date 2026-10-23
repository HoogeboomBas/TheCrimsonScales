using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Arcblast : TempestCardModel<Arcblast.CardTop, Arcblast.CardBottom>
{
	public override string Name => "Arcblast";
	public override int Level => 2;
	public override int Initiative => 12;
	protected override int AtlasIndex => 13;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithPierce(1, new PierceSquare(this, new Vector2(0.5106531f, 0.13160996f)))
				.WithPush(1, new PushSquare(this, new Vector2(0.7294284f, 0.13140194f)))
				.WithDuringAttackSubscription(
					LoseSparksConsumeElementSubscription<ScenarioEvents.DuringAttack.Parameters>(1, Element.Light, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red)
					]
				), new AOEHexMark(Vector2I.Zero.Add(Direction.NorthWest), this, new Vector2(0.40102744f, 0.3110574f)))
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							await AbilityCmd.GenericChoice(parameters.Authority,
							[
								ScenarioEvents.GenericChoice.Subscription.New(
									applyFunction: async _ =>
									{
										parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);

										await GDTask.CompletedTask;
									}, effectType: EffectType.SelectableMandatory,
									effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Wound1)),
									effectInfoViewParameters: new TextEffectInfoView.Parameters(
										$"Add {Icons.Inline(Icons.GetCondition(Conditions.Wound1))} to the attack")),
								ScenarioEvents.GenericChoice.Subscription.New(
									applyFunction: async _ =>
									{
										parameters.AbilityState.SingleTargetAddCondition(Conditions.Immobilize);

										await GDTask.CompletedTask;
									}, effectType: EffectType.SelectableMandatory,
									effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Immobilize)),
									effectInfoViewParameters: new TextEffectInfoView.Parameters(
										$"Add {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))} to the attack")),
								ScenarioEvents.GenericChoice.Subscription.New(
									_ => HasXSparks(state.Performer, 2),
									async _ =>
									{
										LoseSparks(state.Performer, 2);
										parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);
										parameters.AbilityState.SingleTargetAddCondition(Conditions.Immobilize);

										await GDTask.CompletedTask;
									}, EffectType.SelectableMandatory,
									effectButtonParameters: new IconEffectButton.Parameters($"2{Icons.HintText(Tempest.SparkIconPath)}"),
									effectInfoViewParameters: new TextEffectInfoView.Parameters(
										$"Add {Icons.Inline(Icons.GetCondition(Conditions.Wound1))} and {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))} to the attack")
								)
							], hintText: "Select a condition to add to the attack");

							ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, this,
								canApplyParameters => canApplyParameters.AbilityState.SingleTargetState == parameters.AbilityState.SingleTargetState,
								async applyParameters =>
								{
									foreach(Figure figure in RangeHelper.GetFiguresInRange(applyParameters.AbilityState.Target, 1).Where(figure =>
										        state.Performer.EnemiesWith(figure) && figure != applyParameters.AbilityState.Target))
									{
										await AbilityCmd.SufferDamage(state, figure, 1);
									}

									ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, this);
								});
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62163085f, 0.90858734f)))
				.Build())
		];

		public override int XP => 1;
		public override bool Round => true;
	}
}