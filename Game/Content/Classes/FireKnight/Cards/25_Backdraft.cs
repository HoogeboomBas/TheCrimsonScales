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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
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
				))
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

		protected override IEnumerable<Element> Elements => [Element.Fire, Element.Dark];
		protected override int XP => 1;
		protected override bool Loss => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2)
				.WithRange(1)
				.WithOnAbilityStarted(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters =>
							parameters.PotentialAbilityState == state &&
							parameters.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							await AbilityCmd.SufferDamage(null, parameters.Figure, 2);
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

		protected override IEnumerable<Element> Elements => [Element.Fire];
	}
}