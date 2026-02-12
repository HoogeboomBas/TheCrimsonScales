using Fractural.Tasks;
using Godot;

public abstract class StarslingerCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : StarslingerCardSide, new()
	where TBottom : StarslingerCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/Starslinger/Cards.jpg";
	protected override int ColumnCount => 6;
	protected override int RowCount => 5;
}

public abstract class StarslingerCardSide : AbilityCardSide
{
	
}