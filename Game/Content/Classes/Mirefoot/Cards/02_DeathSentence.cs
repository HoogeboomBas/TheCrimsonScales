using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class DeathSentence : MirefootCardModel<DeathSentence.CardTop, DeathSentence.CardBottom>
{
	public override string Name => "Death Sentence";
	public override int Level => 1;
	public override int Initiative => 08;
	protected override int AtlasIndex => 2;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.4508403f, 0.27815282f)))
				.WithConditions([Conditions.Poison3, Conditions.Stun])
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.62066f, 0.719864f)))
				.WithOnAbilityStarted(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.PotentialAbilityState == state &&
							canApplyParameters.Hex.HasHexObjectOfType<DifficultTerrain>(),
						async applyParameters =>
						{
							DifficultTerrain difficultTerrain = applyParameters.Hex.GetHexObjectOfType<DifficultTerrain>();
							await AbilityCmd.DestroyDifficultTerrain(difficultTerrain);
						});

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];
	}
}