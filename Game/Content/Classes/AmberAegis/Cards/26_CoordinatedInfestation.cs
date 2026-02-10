using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class CoordinatedInfestation : AmberAegisCardModel<CoordinatedInfestation.CardTop, CoordinatedInfestation.CardBottom>
{
	public override string Name => "Coordinated Infestation";
	public override int Level => 8;
	public override int Initiative => 45;
	protected override int AtlasIndex => 26;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(3)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red)
					]
				))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Wound1);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Wound1)))))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound1)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<AttackAbility.State>(0).GetRedAOEHexes()
						.SelectMany(hex => hex.GetHexObjectsOfType<Figure>()));
				})
				.WithMandatory(true)
				.Build())
		];

		public override IEnumerable<Element> Elements => [Element.Earth];
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.62114084f, 0.66008836f)))
				.Build()),
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(2)
				.WithConditionalAbilityCheck(state =>
					AbilityCmd.AskConsumeElement(state.Performer, Element.Earth, effectInfoText: $"{Icons.Inline(Icons.Shield)}2"))
				.WithOnAbilityEndedPerformed(async state =>
				{
					state.ActionState.SetOverrideRound();
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override IEnumerable<Element> Elements => [Element.Fire];
	}
}