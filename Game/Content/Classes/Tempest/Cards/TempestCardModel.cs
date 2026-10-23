using System;
using Fractural.Tasks;

public abstract class TempestCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : TempestCardSide
	where TBottom : TempestCardSide
{
	protected override string TexturePath => "res://Content/Classes/Tempest/Cards.jpg";
	protected override int ColumnCount => 6;
	protected override int RowCount => 5;
}

public abstract class TempestCardSide : AbilityCardSideModel
{
	public static OtherAbility.OtherBuilder GainSparkAbilityBuilder(int count = 1)
	{
		return OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await GainSparks(state, count);
				state.SetPerformed();
			});
	}

	public static async GDTask GainSparks(AbilityState state, int count)
	{
		if(state.Performer is Tempest tempest)
		{
			tempest.GainSparks(count);
		}

		await GDTask.CompletedTask;
	}

	public static async GDTask GainSpark(AbilityState state)
	{
		await GainSparks(state, 1);
	}

	public static void LoseSparks(Figure figure, int count = 1)
	{
		if(figure is Tempest tempest)
		{
			tempest.LoseSparks(count);
		}
	}

	public static bool HasXSparks(Figure figure, int x)
	{
		if(figure is Tempest tempest)
		{
			return tempest.HasXSparks(x);
		}

		return false;
	}

	public static int SparkCount(Figure figure)
	{
		if(figure is Tempest tempest)
		{
			return tempest.SparkCount;
		}

		return 0;
	}

	protected static async GDTask<bool> LoseSparksConditionalAbilityCheck(Figure figure, int count, EffectInfoViewParameters effectInfoViewParameters)
	{
		bool lostSparks = false;
		await AbilityCmd.GenericChoice(figure,
			[
				ScenarioEvents.GenericChoice.Subscription.New(
					_ => HasXSparks(figure, count),
					async _ =>
					{
						LoseSparks(figure, count);
						lostSparks = true;
						await GDTask.CompletedTask;
					},
					EffectType.Selectable,
					effectButtonParameters: new TextEffectButton.Parameters($"{count}{Icons.HintText(Tempest.SparkIconPath)}"),
					effectInfoViewParameters: effectInfoViewParameters,
					canApplyMultipleTimesDuringSubscription: false)
			], hintText: $"Spend {count}{Icons.HintText(Tempest.SparkIconPath)}?");
		return lostSparks;
	}

	protected static async GDTask<bool> LoseSparksConsumeElementConditionalAbilityCheck(Figure figure, int count, Element element,
		string effectInfoText)
	{
		if(HasXSparks(figure, count) && await AbilityCmd.AskConsumeElement(figure, Element.Air,
			   effectInfoText: effectInfoText ?? $"Consume {Icons.Inline(Icons.GetElement(element))}",
			   hintText: $"Spend {count}{Icons.HintText(Tempest.SparkIconPath)} and consume {Icons.HintText(Icons.GetElement(element))}?"))
		{
			LoseSparks(figure, count);
			return true;
		}

		return false;
	}

	protected static ScenarioEvent<T>.Subscription LoseSparksSubscription<T>(int count, Func<T, GDTask> applyFunction,
		EffectInfoViewParameters effectInfoViewParameters, Func<T, bool> canApplyFunction = null,
		bool canApplyMultipleTimesDuringSubscription = false)
		where T : ScenarioEvent.ParametersBaseWithAbilityState
	{
		return ScenarioEvent<T>.Subscription.New(
			parameters => (canApplyFunction == null || canApplyFunction.Invoke(parameters)) &&
			              HasXSparks(parameters.BaseAbilityState.Performer, count),
			async parameters =>
			{
				LoseSparks(parameters.BaseAbilityState.Performer, count);
				await applyFunction(parameters);
			},
			EffectType.Selectable,
			canApplyMultipleTimesDuringSubscription: canApplyMultipleTimesDuringSubscription,
			effectButtonParameters: new TextEffectButton.Parameters($"{count}{Icons.HintText(Tempest.SparkIconPath)}"),
			effectInfoViewParameters: effectInfoViewParameters);
	}

	protected static ScenarioEvent<T>.Subscription LoseSparksConsumeElementSubscription<T>(int count, Element element, Func<T, GDTask> applyFunction,
		EffectInfoViewParameters effectInfoViewParameters)
		where T : ScenarioEvent.ParametersBaseWithAbilityState
	{
		return ScenarioEvent<T>.Subscription.ConsumeElement(element,
			parameters => HasXSparks(parameters.BaseAbilityState.Performer, count),
			async parameters =>
			{
				LoseSparks(parameters.BaseAbilityState.Performer, count);
				await applyFunction.Invoke(parameters);
			},
			effectButtonParameters: new TextEffectButton.Parameters(
				$"{count}{Icons.HintText(Tempest.SparkIconPath)}{Icons.HintText(Icons.GetElement(element))}"),
			effectInfoViewParameters: effectInfoViewParameters);
	}
}