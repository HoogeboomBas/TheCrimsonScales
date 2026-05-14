using System.Collections.Generic;
using System.Linq;
using Godot;

public class TendrilsOfNight : HollowpactCardModel<TendrilsOfNight.CardTop, TendrilsOfNight.CardBottom>
{
	public override string Name => "Tendrils of Night";
	public override int Level => 8;
	public override int Initiative => 44;
	protected override int AtlasIndex => 0;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(2)
				.WithTarget(Target.Any | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 2)
						.Where(figure => figure.Hex.HasHexObjectOfType<Coin>())
						.Distinct());
				})
				.WithOnAbilityEndedPerformed(async state =>
				{
					await GainVoidEnergy(state, state.UniqueTargetedFigures.Count);
				})
				.Build()),

			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(2)
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Invisible)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark,
					effectInfoText: $"{Icons.Inline(Icons.GetCondition(Conditions.Invisible))} self"))
				.WithOnAbilityEndedPerformed(GainXP)
				.Build()),
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Hex hex = await AbilityCmd.SelectHex(state, list =>
					{
						list.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 6).Where(hex => hex.HasHexObjectOfType<Obstacle>()));
					}, hintText: "Designate a hex within range 6 containing an obstacle.");

					if(hex != null)
					{
						await hex.GetHexObjectOfType<Obstacle>().Destroy();

						state.SetCustomValue(this, "DesignatedHex", hex);
						state.SetPerformed();
					}
				})
				.Build()),

			new AbilityCardAbility(TeleportAbility.Builder()
				.WithCustomGetHexes((state, hexes) =>
				{
					hexes.Add(state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<Hex>(this, "DesignatedHex"));
				})
				.WithConditionalAbilityCheck(async state =>
				{
					return await AbilityCmd.HasPerformedAbility(state, 0) && await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 1, 
						new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Teleport)} to the designated hex, then perform {Icons.Inline(Icons.Attack)}3, {Icons.Inline(Icons.GetCondition(Conditions.Poison1))}."));
				})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithConditions(Conditions.Poison1)
				.WithConditionalAbilityCheck(async state =>
				{
					return await AbilityCmd.HasPerformedAbility(state, 0) && await AbilityCmd.HasPerformedAbility(state, 1);
				})
				.WithDuringAttackSubscription(LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(1,
					async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Wound1);
					},
					new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Wound1))}")))
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
	}
}