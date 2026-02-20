using Fractural.Tasks;

public abstract class RuinmawCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : RuinmawCardSide
	where TBottom : RuinmawCardSide
{
	protected override string TexturePath => "res://Content/Classes/Ruinmaw/Cards.jpg";
	protected override int ColumnCount => 8;
	protected override int RowCount => 4;
}

public abstract class RuinmawCardSide : AbilityCardSideModel
{
	protected virtual bool Sate => false;

	public override async GDTask OnActionPerformed(Figure figure)
	{
		await base.OnActionPerformed(figure);

		if(Sate)
		{
			await SateRuinmaw(figure);
		}
	}

	protected async GDTask SateRuinmaw(Figure figure)
	{
		if(figure is Ruinmaw ruinmaw)
		{
			await ruinmaw.Sate();
		}

		await GDTask.CompletedTask;
	}

	protected async GDTask SateRuinmaw(AbilityState state)
	{
		await SateRuinmaw(state.Performer);
	}

	protected static bool IsSated(Figure figure)
	{
		return figure is Ruinmaw ruinmaw && ruinmaw.Sated;
	}
}