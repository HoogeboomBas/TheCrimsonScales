using System.Collections.Generic;
using System.Linq;
using Godot;

public class SeverReality : HollowpactCardModel<SeverReality.CardTop, SeverReality.CardBottom>
{
	public override string Name => "Sever Reality";
	public override int Level => 5;
	public override int Initiative => 78;
	protected override int AtlasIndex => 0;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Hex hex = await AbilityCmd.SelectHex(state, list =>
					{
						list.AddRange(state.Performer.Hex.Neighbours.Where(hex => hex.HasHexObjectOfType<Obstacle>()));
					}, hintText: "Designate an adjacent hex containing an obstacle to destroy.");

					if(hex != null)
					{
						await hex.GetHexObjectOfType<Obstacle>().Destroy();
						await AbilityCmd.GainXP(state.Performer, 1);
						await GainVoidEnergy(state);

						state.SetCustomValue(this, "DesignatedHex", hex);
						state.SetPerformed();
					}
				})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithConditions(Conditions.Wound1)
				.WithConditionalAbilityCheck(async state =>
				{
					return await AbilityCmd.HasPerformedAbility(state, 0);
				})
				.WithCustomGetTargets((state, figures) =>
				{
					Hex hex = state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<Hex>(this, "DesignatedHex");

					figures.AddRange(hex.Neighbours.SelectMany(hex => hex.GetFigures()));
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(VoidsightAbility.Builder().Build()),

			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(3, new TeleportCircle(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithConditionalAbilityCheck(async state =>
				{
					return await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 1, 
						new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Teleport)}3"));
				})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithConditions(Conditions.Curse)
				.Build())
		];
	}
}