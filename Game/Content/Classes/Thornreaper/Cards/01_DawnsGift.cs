using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class DawnsGift : ThornreaperCardModel<DawnsGift.CardTop, DawnsGift.CardBottom>
{
	public override string Name => "Dawn's Gift";
	public override int Level => 1;
	public override int Initiative => 56;
	protected override int AtlasIndex => 29 - 1;

	public class CardTop : ThornreaperCardSide
	{
		public Hex SelectedHex;
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
					{
						SelectedHex = await AbilityCmd.SelectHex(state, 
							list => 
							{
								foreach (Hex possibleHex in RangeHelper.GetHexesInRange(state.Performer.Hex, 1, true))
								{
									if (possibleHex != null && possibleHex.IsFeatureless())
									{
										list.Add(possibleHex);
									}
								}
							}, true, "Create one 1-hex hazardous terrain in one adjacent featureless hex");

						await CreateHazardousTerrain(SelectedHex);
						state.SetPerformed();
					}
				)
				.Build()),

			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.WithCustomLootFromLocation(state => state.ActionState.GetAbilityState<OtherAbility>(0).)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];
		public override bool Round => true;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.36594453f, 0.668933f)))
				.WithRangeType(RangeType.Range)
				.WithCustomGetTargets((state, list) =>
					{
						foreach(Figure figure in GameController.Instance.Map.Figures)
						{
							foreach(Figure potentialAlly in RangeHelper.GetFiguresInRange(figure.Hex, 1))
							{
								if(state.Performer.AlliedWith(potentialAlly))
								{
									list.Add(figure);
									break;
								}
							}
						}
					}
				)
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
				[
					ShieldAbility.Builder()
						.WithShieldValue(1)
						.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
						.WithOnAbilityEndedPerformed(async state =>
						{
							await GDTask.CompletedTask;

							state.ActionState.SetOverrideRound();
						})
						.Build()
				])
				.WithCustomGetTargets((state, list) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach(Figure target in attackAbilityState.UniqueTargetedFigures)
					{
						list.AddRange(RangeHelper.GetFiguresInRange(target.Hex, 1));
					}
				})
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

						return attackAbilityState.Performed;
					}
				)
				.Build())
		];
	}
}