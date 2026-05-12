using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class WhistlingWinds : SpiritCallerCardModel<WhistlingWinds.CardTop, WhistlingWinds.CardBottom>
{
	public override string Name => "Whistling Winds";
	public override int Level => 1;
	public override int Initiative => 82;
	protected override int AtlasIndex => 28 - 9;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.6195456f, 0.15117243f)))
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPush(2);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Push)}2")
					),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Curse);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Curse))}")
					)
				])
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
				.Build()),
		];
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62149024f, 0.64566964f)))
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities([
					MoveAbility.Builder()
						.WithDistance(1)
						.Build()
				])
				.WithCustomGetTargets((state, list) =>
				{
					list.AddRange(Spirit.GetAllSpirits());
				})
				.WithTarget(Target.Any | Target.TargetAll)
				.WithCanTargetNonFigures()
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
	}
}