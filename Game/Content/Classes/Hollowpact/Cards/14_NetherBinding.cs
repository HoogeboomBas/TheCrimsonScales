using System.Collections.Generic;
using System.Linq;
using Godot;

public class NetherBinding : HollowpactLevelUpCardModel<NetherBinding.CardTop, NetherBinding.CardBottom>
{
	public override string Name => "Nether Binding";
	public override int Level => 2;
	public override int Initiative => 64;
	protected override int AtlasIndex => 0;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(3, new PushCircle(this, new Vector2(0.46492216f, 0.20446666f)))
				.WithRange(1)
				.WithConditions(Conditions.Immobilize)
				.Build()),

			new AbilityCardAbility(CreateVoidPitObstacleAbilityBuilder()
				.WithConditionalAbilityCheck(async state => await AbilityCmd.HasPerformedAbility(state, 0))
				.WithCustomSelectHexes((state, hexes) =>
				{
					hexes.AddRange(state.ActionState.GetAbilityState<PushAbility.State>(0).Target.Hex.Neighbours.Where(hex => hex.IsEmpty()));
				})
				.WithOnAbilityEndedPerformed(async state => 
				{
					await GainVoidEnergy(state); 
					await GainXP(state);
				})
				.Build()),
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(5)
				.WithFilterHexes((state, hex) => hex.Neighbours.Any(hex => hex.GetFigures().Any(figure => figure.AlliedWith(state.Performer))))
				.WithConditionalAbilityCheck(async state =>
				{
					return await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 1, 
						new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Teleport)}5, end adjacent to an ally."));
				})
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(4, new HealDiamondPlus(this, new Vector2(0.4088779f, 0.7680555f)))
				.WithTarget(Target.Allies)
				.WithRange(1)
				.WithConditions(Conditions.Wound1)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements =>
			[CardElementInfusion.Infuse(Element.Earth), CardElementInfusion.Infuse(Element.Dark)];
	}
}