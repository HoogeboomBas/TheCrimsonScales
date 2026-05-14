using System;
using Fractural.Tasks;

public abstract class HollowpactLevelUpCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : HollowpactCardSide
	where TBottom : HollowpactCardSide
{
	protected override string TexturePath => "res://Content/Classes/Hollowpact/LevelUpCards.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 4;
}

public abstract class HollowpactCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : HollowpactCardSide
	where TBottom : HollowpactCardSide
{
	protected override string TexturePath => "res://Content/Classes/Hollowpact/Cards.jpg";
	protected override int ColumnCount => 8;
	protected override int RowCount => 2;
}

public abstract class HollowpactCardSide : AbilityCardSideModel
{
	public static DivinationAbility.DivinationBuilder VoidsightAbilityBuilder()
	{
		return Hollowpact.VoidsightAbilityBuilder();
	}

	public static CreateObstacleAbility.CreateObstacleBuilder CreateVoidPitObstacleAbilityBuilder()
	{
		return Hollowpact.CreateVoidPitObstacleAbilityBuilder();
	}

	public static OtherAbility.OtherBuilder GainVoidEnergyAbilityBuilder(int count = 1)
	{
		return OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await GainVoidEnergy(state, count);
				state.SetPerformed();
			});
	}

	// Void energy produce/consume helpers
	public static async GDTask GainVoidEnergy(AbilityState state, int count)
	{
		if(state.Performer is Hollowpact hollowpact)
		{
			hollowpact.GainVoidEnergy(count);
		}

		await GDTask.CompletedTask;
	}

	public static async GDTask GainVoidEnergy(AbilityState state)
	{
		await GainVoidEnergy(state, 1);
	}

	public static void LoseVoidEnergy(Figure figure, int count = 1)
	{
		if(figure is Hollowpact hollowpact)
		{
			hollowpact.LoseVoidEnergy(count);
		}
	}

	public static bool HasXVoidEnergy(Figure figure, int x)
	{
		if(figure is Hollowpact hollowpact)
		{
			return hollowpact.HasXVoidEnergy(x);
		}

		return false;
	}

	protected static async GDTask<bool> LoseVoidEnergyConditionalAbilityCheck(Figure figure, int count, EffectInfoViewParameters effectInfoViewParameters)
	{
		bool lostVoidEnergy = false;
		await AbilityCmd.GenericChoice(figure,
		[
			ScenarioEvents.GenericChoice.Subscription.New(
				_ => HasXVoidEnergy(figure, count),
				async _ =>
				{
					LoseVoidEnergy(figure, count);
					lostVoidEnergy = true;
					await GDTask.CompletedTask;
				}, EffectType.Selectable,
				effectButtonParameters: new TextEffectButton.Parameters($"{count}{Icons.HintText(Hollowpact.VoidEnergy)}"),
				effectInfoViewParameters: effectInfoViewParameters)
		]);
		return lostVoidEnergy;
	}

	protected static ScenarioEvent<T>.Subscription LoseVoidEnergySubscription<T>(int count, Func<T, GDTask> applyFunction,
		EffectInfoViewParameters effectInfoViewParameters)
		where T : ScenarioEvent.ParametersBaseWithAbilityState
	{
		return ScenarioEvent<T>.Subscription.New(
			parameters => HasXVoidEnergy(parameters.BaseAbilityState.Performer, count),
			async parameters =>
			{
				LoseVoidEnergy(parameters.BaseAbilityState.Performer, count);
				await applyFunction(parameters);
			}, EffectType.Selectable,
			effectButtonParameters: new TextEffectButton.Parameters($"{count}{Icons.HintText(Hollowpact.VoidEnergy)}"),
			effectInfoViewParameters: effectInfoViewParameters);
	}
}