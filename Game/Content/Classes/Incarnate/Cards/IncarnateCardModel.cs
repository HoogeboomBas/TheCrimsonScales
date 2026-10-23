using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public abstract class IncarnateCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : IncarnateCardSide
	where TBottom : IncarnateCardSide
{
	protected override string TexturePath => "res://Content/Classes/Incarnate/Cards.jpg";
	protected override int ColumnCount => 8;
	protected override int RowCount => 4;
}

public abstract class IncarnateCardSide : AbilityCardSideModel
{
	protected virtual IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [];

	public override async GDTask OnActionPerformed(Figure figure)
	{
		await base.OnActionPerformed(figure);

		if(SwitchSpiritChoices.Any())
		{
			await ChooseSpirit(figure, SwitchSpiritChoices);
		}
	}

	public static bool InSpirit(Figure figure, IncarnateSpirit spirit)
	{
		if(figure is Incarnate incarnate)
		{
			return incarnate.Spirit == spirit;
		}

		return false;
	}

	protected static async GDTask<bool> InSpirit(AbilityState state, IncarnateSpirit spirit)
	{
		await GDTask.CompletedTask;
		if(state.Performer is Incarnate incarnate)
		{
			return incarnate.Spirit == spirit;
		}

		return false;
	}

	public static async GDTask ChooseSpirit(Figure figure, IEnumerable<IncarnateSpirit> spiritChoices)
	{
		if(figure is Incarnate incarnate)
		{
			await incarnate.ChooseSpirit(spiritChoices);
		}

		await GDTask.CompletedTask;
	}

	protected static ScenarioEvent<T>.Subscription InSpiritSubscription<T>(IncarnateSpirit spirit,
		ScenarioEvent<T>.ApplyFunction applyFunction = null)
		where T : ScenarioEvent.ParametersBaseWithAbilityState
	{
		return ScenarioEvent<T>.Subscription.New(parameters => InSpirit(parameters.BaseAbilityState.Performer, spirit), applyFunction,
			EffectType.MandatoryBeforeOptionals, 0,
			false);
	}
}