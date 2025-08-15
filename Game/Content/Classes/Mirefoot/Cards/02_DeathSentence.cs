﻿using System.Collections.Generic;
using Fractural.Tasks;

public class DeathSentence : MirefootCardModel<DeathSentence.CardTop, DeathSentence.CardBottom>
{
	public override string Name => "Death Sentence";
	public override int Level => 1;
	public override int Initiative => 08;
	protected override int AtlasIndex => 2;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(2, conditions: [Conditions.Poison3, Conditions.Stun]))
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(5,
				onAbilityStarted: async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.AbilityState == state &&
							canApplyParameters.Hex.HasHexObjectOfType<DifficultTerrain>(),
						async applyParameters =>
						{
							DifficultTerrain difficultTerrain = applyParameters.Hex.GetHexObjectOfType<DifficultTerrain>();
							await AbilityCmd.DestroyDifficultTerrain(difficultTerrain);
						});

					await GDTask.CompletedTask;
				},
				onAbilityEnded: async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}))
		];
	}
}