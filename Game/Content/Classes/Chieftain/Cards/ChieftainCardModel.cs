public abstract class ChieftainCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : ChieftainCardSide, new()
	where TBottom : ChieftainCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/Chieftain/Cards.jpg";
	protected override int ColumnCount => 6;
	protected override int RowCount => 5;
}

public abstract class ChieftainCardSide : AbilityCardSide
{
}