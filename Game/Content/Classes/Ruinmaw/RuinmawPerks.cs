using System.Collections.Generic;
using Fractural.Tasks;

public class RuinmawPerks
{
	public abstract class RuinmawPerk : PerkModel
	{
	}

	public class RemoveOneMinusTwo : RuinmawPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroRupture : RuinmawPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RuinmawAMDCards.PlusZeroRupture>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroWound : RuinmawPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RuinmawAMDCards.PlusZeroWound>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOnePlusThreeInsteadIfTargetHasRuptureOrWound : RuinmawPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RuinmawAMDCards.PlusOnePlusThreeInsteadIfTargetHasRuptureOrWound>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusZeroHealOneEmpowerSelfRolling : RuinmawPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RuinmawAMDCards.PlusZeroHealOneEmpowerSelfRolling>()
		];
	}

	public class AddOnePlusTwoIfThisAttackKillsTargetGainMoneyTokenDirectly : RuinmawPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RuinmawAMDCards.PlusTwoIfThisAttackKillsTargetGainMoneyTokenDirectly>()
		];
	}

	public class IgnoreScenarioEffectsRemoveOneMinusOne : RuinmawPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override bool IgnoreScenarioEffects => true;
	}

	public class FollowTheScent : RuinmawPerk
	{
		protected override string Title => "Follow the Scent";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever one of your abilities causes an enemy to gain {Icons.Inline(Icons.GetCondition(Conditions.Rupture), richTextParameters)}, immediately after the ability, perform {Icons.Inline(Icons.Move, richTextParameters)}2.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			List<AbilityState> currentAbilityStates = [];
			ScenarioEvents.InflictConditionEvent.Subscribe(this,
				parameters => parameters.PotentialConditionGiver == character && parameters.Target.EnemiesWith(character) &&
				              parameters.ConditionModel is Rupture && !currentAbilityStates.Contains(parameters.PotentialAbilityState),
				async parameters =>
				{
					currentAbilityStates.Add(parameters.PotentialAbilityState);
					ScenarioEvents.AbilityEndedEvent.Subscribe(this,
						abilityEndedParameters => abilityEndedParameters.AbilityState == parameters.PotentialAbilityState,
						async _ =>
						{
							await new ActionState(character, [MoveAbility.Builder().WithDistance(2).Build()]).Perform();
							currentAbilityStates.Remove(parameters.PotentialAbilityState);
							ScenarioEvents.AbilityEndedEvent.Unsubscribe(this);
						});
					await GDTask.CompletedTask;
				});
		}
	}

	public class ALullInHunger : RuinmawPerk
	{
		protected override string Title => "A Lull in Hunger";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Once each scenario, after you loot a money token, {Icons.Inline(Ruinmaw.SatedUpIconPath, richTextParameters)}.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.CoinLootedEvent.Subscribe(this,
				parameters => parameters.LootObtainer == character,
				async _ =>
				{
					await RuinmawCardSide.SateRuinmaw(character);
				}, EffectType.Selectable,
				effectButtonParameters: new IconEffectButton.Parameters(Ruinmaw.SatedUpIconPath),
				effectInfoViewParameters: new TextEffectInfoView.Parameters(Icons.Inline(Ruinmaw.SatedUpIconPath)));
		}
	}

	public class AdrenalineRush : RuinmawPerk
	{
		public override int PerkBoxCount => 2;
		protected override string Title => "Adrenaline Rush";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you become {Icons.Inline(Ruinmaw.SatedIconPath, richTextParameters)}, gain {Icons.Inline(Icons.GetCondition(Conditions.Ward))}.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			if(character is Ruinmaw ruinmawCharacter)
			{
				ruinmawCharacter.SateEvent += async ruinmaw => await AbilityCmd.AddCondition(null, ruinmaw, Conditions.Ward, ruinmaw);
			}
		}
	}
}