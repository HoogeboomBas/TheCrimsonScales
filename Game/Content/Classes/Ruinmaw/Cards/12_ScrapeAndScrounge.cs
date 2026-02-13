using System.Collections.Generic;
using Fractural.Tasks;

public class ScrapeAndScrounge : RuinmawCardModel<ScrapeAndScrounge.CardTop, ScrapeAndScrounge.CardBottom>
{
	public override string Name => "Scrape and Scrounge";
	public override int Level => 1;
	public override int Initiative => 24;
	protected override int AtlasIndex => 12;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.WithConditions(Ruinmaw.Empower)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					AbilityCard selectedAbilityCard =
						await AbilityCmd.SelectAbilityCard((Character)state.Performer, list =>
						{
							foreach(AbilityCard card in ((Character)state.Performer).Cards)
							{
								if(card.CardState == CardState.Discarded && card.Model.Level == 1)
								{
									list.Add(card);
								}
							}
						}, CardState.Discarded, hintText: $"Select a level 1 discarded card to recover");

					if(selectedAbilityCard != null)
					{
						await AbilityCmd.ReturnToHand(selectedAbilityCard);
						state.SetPerformed();
					}
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					if(IsSated(state.Performer))
					{
						await AbilityCmd.GainXP(state.Performer, 1);
					}

					return IsSated(state.Performer);
				})
				.Build()),
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					foreach(Hex hex in state.ActionState.GetAbilityState<MoveAbility.State>(0).Hexes)
					{
						await AbilityCmd.LootHex(state.Performer, hex);
						state.SetPerformed();
					}

					await GDTask.CompletedTask;
				})
				.Build()),
		];

		protected override bool Sate => true;
		public override int XP => 2;
		public override bool Loss => true;
	}
}