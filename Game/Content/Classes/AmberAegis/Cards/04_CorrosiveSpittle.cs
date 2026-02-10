using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class CorrosiveSpittle : AmberAegisCardModel<CorrosiveSpittle.CardTop, CorrosiveSpittle.CardBottom>
{
	public override string Name => "Corrosive Spittle";
	public override int Level => 1;
	public override int Initiative => 45;
	protected override int AtlasIndex => 4;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.46376285f, 0.18548882f)))
				.WithTargets(2)
				.WithConditions(Conditions.Wound1)
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async duringAttackParameters =>
						{
							AttackAbility.State state = duringAttackParameters.AbilityState;
							int shieldValue = 0;
							ScenarioEvents.JustBeforeSufferDamageEvent.Subscribe(state, this,
								parameters => parameters.PotentialAbilityState == state && parameters.SufferDamageParameters.FromAttack,
								async parameters =>
								{
									shieldValue = parameters.SufferDamageParameters.Shield + parameters.SufferDamageParameters.UnpierceableShield;
									await GDTask.CompletedTask;
								});
							ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, this,
								parameters => parameters.AbilityState == state,
								async parameters =>
								{
									await AbilityCmd.SufferDamage(state, state.Target,
										2 + shieldValue);
								});
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Each target suffer {Icons.Inline(Icons.Damage)}X+2, where X is the {Icons.Inline(Icons.Shield)} value of that enemy")),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Earth,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Targets)}"))
				])
				.WithOnAbilityEnded(async state =>
				{
					ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, this);
					ScenarioEvents.JustBeforeSufferDamageEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 1;
		public override bool Loss => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Poison1)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
				]))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Poison1)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<ConditionAbility.State>(0).GetRedAOEHexes()
						.SelectMany(hex => hex.GetHexObjectsOfType<Figure>()));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithMandatory(true)
				.Build()),
		];

		public override int XP => 1;
	}
}