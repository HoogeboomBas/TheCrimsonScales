using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public abstract class LuminaryCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : LuminaryCardSide, new()
	where TBottom : LuminaryCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/Luminary/Cards.jpg";
	protected override int ColumnCount => 8;
	protected override int RowCount => 4;
}

public abstract class LuminaryCardSide : AbilityCardSideModel
{
	protected AbilityCardAbility Scuttle(int distance, IReadOnlyCollection<Element> possibleElements)
	{
		return new AbilityCardAbility(MoveAbility.Builder()
			.WithDistance(distance)
			.WithMoveType(MoveType.Jump)
			.WithConditionalAbilityCheck(async state =>
			{
				await GDTask.CompletedTask;

				return state.ActionState.GetAbilityState<AttackAbility.State>(0).Performed;
			})
			.WithOnAbilityEndedPerformed(async state =>
			{
				if(possibleElements.Count == 1)
				{
					await AbilityCmd.InfuseElement(state, possibleElements.First());
				}
				else
				{
					await AbilityCmd.InfuseElement(state, possibleElements);
				}
			})
			.WithOnAbilityStarted(async state =>
			{
				ScenarioCheckEvents.MoveCanStopAtCheckEvent.Subscribe(state.Performer, this,
					parameters =>
						parameters.AbilityState == state && !state.ActionState.GetAbilityState<AttackAbility.State>(0).GetEmptyAOEHexes()
							.Contains(parameters.Hex),
					parameters =>
					{
						parameters.SetCannotStopAt();
					}
				);

				await GDTask.CompletedTask;
			})
			.WithOnAbilityEnded(async state =>
				{
					ScenarioCheckEvents.MoveCanStopAtCheckEvent.Unsubscribe(state.Performer, this);

					await GDTask.CompletedTask;
				}
			)
			.Build());
	}

	protected AbilityCardAbility PerformFreeGlow()
	{
		return new AbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				GlowActiveAbility.State abilityState = (GlowActiveAbility.State)((Character)state.Performer).Cards
					.SelectMany(card => card.ActiveActionStates.SelectMany(actionState => actionState.AbilityStates))
					.FirstOrDefault(abilityState => abilityState is GlowActiveAbility.State);
				if(abilityState != null)
				{
					await GlowAbility(state.Performer, abilityState.GlowAbilityModels, false);
					state.SetPerformed();
				}

				await GDTask.CompletedTask;
			})
			.Build());
	}

	public static async GDTask GlowAbility(Figure performer, GlowAbilityModel[] glowAbilities, bool consumeElements = true)
	{
		List<ScenarioEvents.GenericChoice.Subscription> subscriptions = [];
		foreach(GlowAbilityModel glowAbility in glowAbilities)
		{
			if(consumeElements)
			{
				List<CardElementConsumption> consumptions = [];
				foreach(Element element in glowAbility.Elements)
				{
					consumptions.Add(CardElementConsumption.Consume(element));
				}

				subscriptions.Add(ScenarioEvents.GenericChoice.Subscription.ConsumeElement(consumptions,
					applyFunction: async parameters =>
					{
						ActionState actionState = new ActionState(performer, [glowAbility.Ability(glowAbility.Elements)]);
						await actionState.Perform();
					},
					effectInfoViewParameters: new TextEffectInfoView.Parameters(glowAbility.HintText)
				));
			}
			else
			{
				subscriptions.Add(ScenarioEvents.GenericChoice.Subscription.New(
					applyFunction: async applyParameters =>
					{
						ActionState actionState = new ActionState(performer, [glowAbility.Ability([])]);
						await actionState.Perform();
					},
					effectButtonParameters: new IconEffectButton.Parameters(glowAbility.HintIcon),
					effectInfoViewParameters: new TextEffectInfoView.Parameters(glowAbility.HintText),
					effectType: EffectType.SelectableMandatory
				));
			}
		}

		await AbilityCmd.GenericChoice(performer, subscriptions, hintText: "Select a glow ability to perform");
	}
}