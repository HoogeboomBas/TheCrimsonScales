using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class NutrientOverdose : BrightsparkCardModel<NutrientOverdose.CardTop, NutrientOverdose.CardBottom>
{
	public override string Name => "Nutrient Overdose";
	public override int Level => 2;
	public override int Initiative => 17;
	protected override int AtlasIndex => 14;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => parameters.Performer is Character character && character.RoundCardData.Any(roundCardData =>
							roundCardData.AbilityCard.Model != AbilityCardModel && (roundCardData.CanPlayBasicBottom || roundCardData.CanPlayBottom)),
						async parameters =>
						{
							foreach(CardPlayCardData cardData in ((Character)parameters.Performer).RoundCardData)
							{
								cardData.CanPlayBottom = false;
								cardData.CanPlayBasicBottom = false;
							}

							parameters.AbilityState.AbilityAdjustAttackValue(3);

							await GDTask.CompletedTask;
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Forgo your bottom action to add +3{Icons.Inline(Icons.Attack)}")))
				.Build())
		];
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6208408f, 0.7248677f)))
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.New(
						parameters => parameters.Performer is Character character && character.RoundCardData.Any(roundCardData =>
							roundCardData.AbilityCard.Model != AbilityCardModel && (roundCardData.CanPlayBasicTop || roundCardData.CanPlayTop)),
						async parameters =>
						{
							foreach(CardPlayCardData cardData in ((Character)parameters.Performer).RoundCardData)
							{
								cardData.CanPlayTop = false;
								cardData.CanPlayBasicTop = false;
							}

							parameters.AbilityState.AdjustMoveValue(3);

							await GDTask.CompletedTask;
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Move),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Forgo your bottom action to add +3{Icons.Inline(Icons.Move)}")))
				.Build())
		];
	}
}