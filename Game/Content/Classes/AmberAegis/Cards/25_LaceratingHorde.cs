using System.Collections.Generic;
using System.Linq;
using Godot;

public class LaceratingHorde : AmberAegisCardModel<LaceratingHorde.CardTop, LaceratingHorde.CardBottom>
{
	public override string Name => "Lacerating Horde";
	public override int Level => 7;
	public override int Initiative => 22;
	protected override int AtlasIndex => 25;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					List<Hex> hexes = await AbilityCmd.SelectHexes(state,
						list => list.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 3)
							.Where(hex => hex.HasHexObjectOfType<ColonyToken>())), 0,
						7, false, hintText: $"Select any number of {Icons.HintText(ColonyToken.AnyColony)} to destroy");
					//TODO: Change to selecting the overlay tiles themselves
					List<ColonyToken> colonyTokens = hexes.Select(hex => hex.GetHexObjectOfType<ColonyToken>()).ToList();
					foreach(ColonyToken colonyToken in colonyTokens)
					{
						await colonyToken.Destroy();
						state.SetPerformed();
					}

					for(int i = 0; i < colonyTokens.Count; i++)
					{
						await AttackAbility.Builder()
							.WithDamage(4, new AttackDiamond(this, new Vector2(0.45407405f, 0.33703703f)))
							.WithRange(4)
							.WithConditions(Conditions.Poison1)
							.Build().Perform(state.ActionState);
					}
				})
				.Build())
		];

		public override bool Loss => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(2)
				.WithRange(3)
				.WithAbilityStartedSubscriptions(
				[
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							((RetaliateAbility.State)parameters.AbilityState).AdjustRetaliateValue(1);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Retaliate)}")),
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Earth,
						applyFunction: async parameters =>
						{
							((RetaliateAbility.State)parameters.AbilityState).AdjustRange(2);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Range)}")),
				])
				.Build())
		];

		public override bool Round => true;
	}
}