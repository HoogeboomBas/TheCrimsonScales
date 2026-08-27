using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class RighteousAtonement : HierophantLevelUpCardModel<RighteousAtonement.CardTop, RighteousAtonement.CardBottom>
{
	public override string Name => "Righteous Atonement";
	public override int Level => 8;
	public override int Initiative => 20;
	protected override int AtlasIndex => 15 - 12;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					AbilityCmd.SubscribeDuringCharacterTurn(ScenarioEvents.GetSubscriberPair(state, this), EffectType.Selectable,
						character => character.AlliedWith(state.Performer),
						async character =>
						{
							await ApplyParameters(character, state);
						},
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Damage),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Suffer up to {Icons.Inline(Icons.Damage)}5 for to increase the value of your next single target attack by that much")
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					AbilityCmd.UnsubscribeDuringCharacterTurn(ScenarioEvents.GetSubscriberPair(state, this));
					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override bool Round => true;

		private async GDTask ApplyParameters(Figure figure, AbilityState state)
		{
			int damageToSuffer = 0;

			List<ScenarioEvents.GenericChoice.Subscription> choices = Enumerable.Range(1, 5)
				.Select(i =>
					ScenarioEvents.GenericChoice.Subscription.New(
						canApplyParameters => true,
						async applyParameters =>
						{
							damageToSuffer = i;
							await GDTask.CompletedTask;
						},
						effectButtonParameters: new TextEffectButton.Parameters($"{Icons.HintText(Icons.Damage)}{i}"),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Suffer {Icons.Inline(Icons.Damage)}{i}"),
						effectType: EffectType.SelectableMandatory
					)
				).ToList();

			await AbilityCmd.GenericChoice(
				figure,
				choices,
				hintText: $"Choose an amount of {Icons.Inline(Icons.Damage)} to suffer"
			);
			ScenarioEvents.AfterSufferDamageEvent.Subscribe(state, this,
				afterSufferParameters => afterSufferParameters.PotentialAbilityState == state,
				async afterSufferParameters =>
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						abilityStartedParameters => abilityStartedParameters.Performer == figure &&
						                            abilityStartedParameters.AbilityState is AttackAbility.State attackAbilityState &&
						                            attackAbilityState.IsSingleTarget,
						async abilityStartedParameters =>
						{
							((AttackAbility.State)abilityStartedParameters.AbilityState).AbilityAdjustAttackValue(
								afterSufferParameters.DamageSuffered);
							await state.ActionState.RequestDiscardOrLose();
						});
					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						turnEndedParameters => turnEndedParameters.Figure == figure,
						async turnEndedParameters =>
						{
							await state.ActionState.RequestDiscardOrLose();
						});
					await GDTask.CompletedTask;
				});
			await AbilityCmd.SufferDamage(state, figure, damageToSuffer);
			ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(state, this);
		}
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						parameters => true,
						async parameters =>
						{
							List<Character> characters = [];
							foreach(Character ally in GameController.Instance.Map.Figures.Where(figure =>
								        figure is Character && figure.AlliedWith(state.Performer)))
							{
								AbilityCard abilityCard = await AbilityCmd.SelectAbilityCard(ally, list =>
								{
									foreach(AbilityCard roundCard in ally.RoundCards)
									{
										if(roundCard.CardState == CardState.Lost)
										{
											list.Add(roundCard);
										}
									}
								}, CardState.Lost, hintText: "Select a lost card to recover.");

								if(abilityCard != null)
								{
									await AbilityCmd.ReturnToHand(abilityCard);
									characters.Add(ally);
								}
							}

							if(characters.Count == 1)
							{
								await GivePrayerCard(state, characters[0]);
							}
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override int XP => 2;
		public override bool Round => true;
		public override bool Loss => true;
		public override bool Unrecoverable => true;
	}
}