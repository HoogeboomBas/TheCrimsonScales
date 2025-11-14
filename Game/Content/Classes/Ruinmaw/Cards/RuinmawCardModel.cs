using System;
using System.Linq;
using Fractural.Tasks;
using Godot;

public abstract class RuinmawCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : RuinmawCardSide, new()
	where TBottom : RuinmawCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/Ruinmaw/Cards.jpg";
	protected override int ColumnCount => 8;
	protected override int RowCount => 4;
}

public abstract class RuinmawCardSide : AbilityCardSide
{
	protected virtual bool Sate => false;
	protected override Action<Figure> ActionPerformed => async (performer) =>
	{
		if(Sate)
		{
			await SateRuinmaw(performer);
		}
	};

	protected async GDTask SateRuinmaw(Figure figure)
	{
		if(figure is Ruinmaw ruinmaw)
		{
			ruinmaw.Sate();
			ruinmaw.SateEvent?.Invoke(ruinmaw);
		}

		await GDTask.CompletedTask;
	}
	
	protected static bool IsSated(Figure figure)
    {
		return figure is Ruinmaw ruinmaw && ruinmaw.Sated;
    }
}