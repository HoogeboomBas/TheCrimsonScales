using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class BombardPerks
{
	public abstract class BombardPerk : PerkModel
	{
	}

	public class RemoveTwoMinusOnes : BombardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>(),
			ModelDB.AMDCard<MinusOneAMDCard>()
		];
	}

	public class ReplaceOneMinusOneWithOneShieldOneRolling : BombardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BombardAMDCards.PlusZeroShieldOneRolling>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusZeroPlusThreeIfProjectile : BombardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BombardAMDCards.PlusZeroPlusThreeIfProjectile>()
		];
	}

	public class ReplaceTwoPlusZeroWithTwoPierceThreeRolling : BombardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BombardAMDCards.PlusZeroPierceThreeRolling>(),
			ModelDB.AMDCard<BombardAMDCards.PlusZeroPierceThreeRolling>(),
		];
	}

	public class ReplacePlusZeroWithOnePlusOneWound : BombardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BombardAMDCards.PlusOneWound>()
		];
	}

	public class ReplacePlusZeroWithOnePlusZeroStun : BombardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BombardAMDCards.PlusZeroStun>()
		];
	}

	public class ReplaceOnePlusOneWithTwoPlusOneRetaliateOne : BombardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BombardAMDCards.PlusOneRetaliateOne>(),
			ModelDB.AMDCard<BombardAMDCards.PlusOneRetaliateOne>()
		];
	}

	public class ReplaceOnePlusOneWithOnePlusZeroStrengthenSelf : BombardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BombardAMDCards.PlusZeroStrengthenSelf>()
		];
	}

	public class AddOnePlusTwoImmobilize : BombardPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BombardAMDCards.PlusTwoImmobilize>()
		];
	}

	public class AddTwoPlusZeroHealOneSelfRolling : BombardPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BombardAMDCards.PlusZeroHealOneSelfRolling>(),
			ModelDB.AMDCard<BombardAMDCards.PlusZeroHealOneSelfRolling>()
		];
	}

	public class IgnoreScenarioEffectsAddPlusOnePullSelf : BombardPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BombardAMDCards.PlusOnePullSelfTowardTarget>()
		];

		public override bool IgnoreScenarioEffects => true;
	}

	public class IgnoreItemMinusOneEffectsAddPlusOnePullSelf : BombardPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BombardAMDCards.PlusOnePullSelfTowardTarget>()
		];

		public override bool IgnoreItemMinusOneEffects => true;
	}

	public class EmergencyEmplacement : BombardPerk
	{
		protected override string Title => "Emergency Emplacement";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you short rest, you may gain {Icons.Inline(Icons.GetCondition(Conditions.Immobilize), richTextParameters, true)} to immediately resolve any of your active {Icons.Inline(BombardCardSide.ProjectileIconPath, richTextParameters)} abilities.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.ShortRestStartedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async _ =>
				{
					await AbilityCmd.AddCondition(null, character, Conditions.Immobilize, character);
					foreach(AbilityState abilityState in character.Cards.SelectMany(card => card.ActiveActionStates).SelectMany(actionState =>
						        actionState.AbilityStates.Where(abilityState => abilityState is ProjectileAbility.State)))
					{
						ProjectileAbility.State projectileState = (ProjectileAbility.State)abilityState;
						await projectileState.PerformAbilities();
					}
				});
		}
	}
}