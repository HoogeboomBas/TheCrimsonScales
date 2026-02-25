using System.Collections.Generic;
using System;
using System.Linq;
using Fractural.Tasks;

public abstract class ArtificerCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : ArtificerCardSide
	where TBottom : ArtificerCardSide
{
	protected override string TexturePath => "res://Content/Classes/Artificer/Cards.jpg";
	protected override int ColumnCount => 6;
	protected override int RowCount => 5;
}

public abstract class ArtificerCardSide : AbilityCardSideModel
{
	protected AbilityCardAbility TimedTrack(List<UseSlot> useSlots)
	{
		return new AbilityCardAbility(UseSlotAbility.Builder()
			.WithOnActivate(async state =>
			{
				ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
					parameters => parameters.Figure == state.Performer && !parameters.Figure.TurnPerformedActionStates.Contains(state.ActionState),
					async _ =>
					{
						await state.AdvanceUseSlot();
					});
				await GDTask.CompletedTask;
			})
			.WithOnDeactivate(async state =>
			{
				ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
				await GDTask.CompletedTask;
			})
			.WithUseSlots(useSlots)
			.WithConditionalAbilityCheck(async state =>
			{
				await GDTask.CompletedTask;
				return state.ActionState.AbilityStates.Any(abilityState => abilityState.Performed);
			})
			.WithMandatory(true)
			.Build());
	}

	public static async GDTask GainScrapToken(AbilityState state)
	{
		if(state.Performer is Artificer artificer)
		{
			artificer.GainScrapToken();
		}

		await GDTask.CompletedTask;
	}

	public static async GDTask GainScrapToken(Character character)
	{
		if(character is Artificer artificer)
		{
			artificer.GainScrapToken();
		}

		await GDTask.CompletedTask;
	}

	public static void LoseScrapTokens(Figure figure, int count = 1)
	{
		if(figure is Artificer artificer)
		{
			artificer.LoseScrapTokens(count);
		}
	}

	public static bool HasXScrapTokens(Figure figure, int x)
	{
		if(figure is Artificer artificer)
		{
			return artificer.HasXScrapTokens(x);
		}

		return false;
	}

	protected async GDTask<bool> TryLoseScrapTokens(Figure figure, int count)
	{
		await GDTask.CompletedTask;
		if(HasXScrapTokens(figure, count))
		{
			LoseScrapTokens(figure, count);
			return true;
		}

		return false;
	}

	protected async GDTask<bool> LoseScrapTokensConditionalAbilityCheck(Figure figure, int count, EffectInfoViewParameters effectInfoViewParameters)
	{
		bool lostScrapTokens = false;
		await AbilityCmd.GenericChoice(figure,
		[
			ScenarioEvents.GenericChoice.Subscription.New(
				_ => true,
				async _ =>
				{
					LoseScrapTokens(figure, count);
					lostScrapTokens = true;
					await GDTask.CompletedTask;
				}, EffectType.Selectable,
				effectButtonParameters: new TextEffectButton.Parameters($"{count}{Icons.HintText(Artificer.ScrapToken)}"),
				effectInfoViewParameters: effectInfoViewParameters)
		]);
		return lostScrapTokens;
	}

	protected ScenarioEvent<T>.Subscription LoseScrapTokenSubscription<T>(int count, Func<T, GDTask> applyFunction,
		EffectInfoViewParameters effectInfoViewParameters)
		where T : ScenarioEvent.ParametersBaseWithAbilityState
	{
		return ScenarioEvent<T>.Subscription.New(
			parameters => HasXScrapTokens(parameters.BaseAbilityState.Performer, count),
			async parameters =>
			{
				LoseScrapTokens(parameters.BaseAbilityState.Performer, count);
				await applyFunction(parameters);
			}, EffectType.Selectable,
			effectButtonParameters: new TextEffectButton.Parameters($"{count}{Icons.HintText(Artificer.ScrapToken)}"),
			effectInfoViewParameters: effectInfoViewParameters);
	}

	protected AbilityCardAbility MoveCharacterTokenBackwardAbility()
	{
		return new AbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				AbilityCard abilityCard = await AbilityCmd.SelectAbilityCard(state.Performer as Character, CardState.Persistent, false, card =>
				{
					foreach(ActionState activeActionState in card.ActiveActionStates)
					{
						foreach(AbilityState abilityState in activeActionState.AbilityStates)
						{
							if(abilityState is UseSlotAbility.State useSlotAbilityState)
							{
								if(useSlotAbilityState.UseSlotIndex > 0)
								{
									return true;
								}
							}
						}
					}

					return false;
				}, hintText: "Select a card to move the character token back on");

				if(abilityCard != null)
				{
					foreach(ActionState activeActionState in abilityCard.ActiveActionStates)
					{
						foreach(AbilityState abilityState in activeActionState.AbilityStates)
						{
							if(abilityState is UseSlotAbility.State useSlotAbilityState)
							{
								if(useSlotAbilityState.UseSlotIndex > 0)
								{
									await useSlotAbilityState.MoveBackUseSlot();
									state.SetPerformed();
								}
							}
						}
					}
				}
			})
			.Build());
	}
}