using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class RollingFlames : FireKnightLevelUpCardModel<RollingFlames.CardTop, RollingFlames.CardBottom>
{
	public override string Name => "Rolling Flames";
	public override int Level => 7;
	public override int Initiative => 53;
	protected override int AtlasIndex => 5;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.4223046f, 0.18387413f)))
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
						]
					),
					new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast).Add(Direction.East), this,
						new Vector2(0.81984866f, 0.1615586f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East).Add(Direction.East), this,
						new Vector2(0.8716455f, 0.22535533f)))
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Figure target in state.UniqueTargetedFigures.Where(target => target.HasWound()))
					{
						await AbilityCmd.SufferDamage(state, target, 1);
					}

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					foreach(Figure target in state.ActionState.GetAbilityState<AttackAbility.State>(0).UniqueTargetedFigures
						        .Where(target => !target.IsDead))
					{
						await AbilityCmd.AddCondition(state, target, Conditions.Wound1);
					}

					await AbilityCmd.InfuseElement(state, Element.Fire);
					state.SetPerformed();
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Fire))
				.Build())
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound1)
				.WithTarget(Target.TargetAll | Target.SelfOrAllies | Target.Enemies)
				.WithCustomGetTargets((state, targets) =>
				{
					targets.AddRange(
						GameController.Instance.Map.Figures
							.Where(f => f.HasWound())
							.SelectMany(f => RangeHelper.GetFiguresInRange(f.Hex, 1, false))
							.Except(targets)
					);
				})
				.WithMandatory(true)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					foreach(Figure target in GameController.Instance.Map.Figures.Where(target => target.HasWound()))
					{
						await AbilityCmd.SufferDamage(state, target, state.GetCustomValue<bool>(this, "Fire Consumed") ? 2 : 1);
						state.SetPerformed();
					}

					await GDTask.CompletedTask;
				})
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "Fire Consumed", true);

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Fire);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+1{Icons.Inline(Icons.Damage)}, {Icons.Inline(Icons.GetElement(Element.Fire))}")
					)
				)
				.WithConditionalAbilityCheck(async state =>
				{
					ConfirmPrompt.Answer confirmAnswer =
						await PromptManager.Prompt(new ConfirmPrompt(null, () => "Perform damage ability?"), state.Authority);

					return confirmAnswer.Confirmed;
				})
				.Build())
		];
	}
}