using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Homeostasis : RimehearthCardModel<Homeostasis.CardTop, Homeostasis.CardBottom>
{
	public override string Name => "Homeostasis";
	public override int Level => 2;
	public override int Initiative => 12;
	protected override int AtlasIndex => 14;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					if(state.GetCustomValue<bool>(this, "GenerateFire"))
					{
						await AbilityCmd.InfuseElement(state, Element.Fire);
						state.SetPerformed();
					}

					if(state.GetCustomValue<bool>(this, "GenerateIce"))
					{
						await AbilityCmd.InfuseElement(state, Element.Ice);
						state.SetPerformed();
					}
				})
				.WithAbilityStartedSubscriptions(
					[
						ScenarioEvents.AbilityStarted.Subscription.New(
							parameters => parameters.Performer.HasCondition(Conditions.Chill),
							async parameters =>
							{
								await AbilityCmd.RemoveChillStack(parameters.Performer);
								parameters.AbilityState.SetCustomValue(this, "GenerateFire", true);

								await GDTask.CompletedTask;
							}, EffectType.Selectable,
							effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Chill)),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								$"Remove one {Icons.Inline(Icons.GetCondition(Conditions.Chill))} token from self to {Icons.Inline(Icons.GetElement(Element.Fire))}")
						),
						ScenarioEvents.AbilityStarted.Subscription.New(
							parameters => parameters.Performer.HasWound(),
							async parameters =>
							{
								await AbilityCmd.RemoveWound(parameters.Performer, parameters.AbilityState);

								parameters.AbilityState.SetCustomValue(this, "GenerateIce", true);
							}, EffectType.Selectable,
							effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Wound1)),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								$"Remove {Icons.Inline(Icons.GetCondition(Conditions.Wound1))} from self to {Icons.Inline(Icons.GetElement(Element.Ice))}")
						)
					]
				)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(4, new HealDiamondPlus(this, new Vector2(0.49367955f, 0.36519697f)))
				.WithTarget(Target.Self)
				.Build())
		];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.4619607f, 0.76595104f)))
				.WithConditions([Conditions.Wound1, Conditions.Chill])
				.Build())
		];
	}
}