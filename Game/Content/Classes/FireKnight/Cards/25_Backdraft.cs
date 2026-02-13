using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Backdraft : FireKnightLevelUpCardModel<Backdraft.CardTop, Backdraft.CardBottom>
{
	public override string Name => "Backdraft";
	public override int Level => 8;
	public override int Initiative => 38;
	protected override int AtlasIndex => 3;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithRange(2)
				.WithPush(1)
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red),
						]
					),
					new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), this, new Vector2(0.8676434f, 0.15360905f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.West), this, new Vector2(0.61196f, 0.21662252f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.East), this, new Vector2(0.8676434f, 0.2808193f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(2);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Attack)}")
					)
				)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Muddle)
				.WithTarget(Target.TargetAll | Target.SelfOrAllies | Target.Enemies)
				.WithCustomGetTargets((state, list) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					list.AddRange(attackAbilityState.GetRedAOEHexes().SelectMany(hex => hex.GetHexObjectsOfType<Figure>()));
				})
				.WithMandatory(true)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements =>
			[CardElementInfusion.Infuse(Element.Fire), CardElementInfusion.Infuse(Element.Dark)];

		public override int XP => 1;
		public override bool Loss => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6176377f, 0.6209524f)))
				.Build()),
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2, new PushSquare(this, new Vector2(0.5074909f, 0.7079646f)))
				.WithRange(1)
				.WithOnAbilityStarted(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters =>
							parameters.PotentialAbilityState == state &&
							parameters.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							await AbilityCmd.SufferDamage(state, parameters.Figure, 2);
							await AbilityCmd.AddCondition(state, parameters.Figure, Conditions.Wound1);
							await AbilityCmd.GainXP(state.Performer, 1);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
					{
						ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
	}
}