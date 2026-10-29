using Fractural.Tasks;
using Godot;

public abstract class RimehearthCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : RimehearthCardSide
	where TBottom : RimehearthCardSide
{
	protected override string TexturePath => "res://Content/Classes/Rimehearth/Cards.jpg";
	protected override int ColumnCount => 6;
	protected override int RowCount => 5;
}

public abstract class RimehearthCardSide : AbilityCardSideModel
{
}