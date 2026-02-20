using Fractural.Tasks;
using Godot;

public abstract class StarslingerCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : StarslingerCardSide
	where TBottom : StarslingerCardSide
{
	protected override string TexturePath => "res://Content/Classes/Starslinger/Cards.jpg";
	protected override int ColumnCount => 6;
	protected override int RowCount => 5;
}

public abstract class StarslingerCardSide : AbilityCardSideModel
{
}