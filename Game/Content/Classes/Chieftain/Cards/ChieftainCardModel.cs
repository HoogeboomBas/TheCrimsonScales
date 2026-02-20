public abstract class ChieftainCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : ChieftainCardSide
	where TBottom : ChieftainCardSide
{
	protected override string TexturePath => "res://Content/Classes/Chieftain/Cards.jpg";
	protected override int ColumnCount => 6;
	protected override int RowCount => 5;
}

public abstract class ChieftainCardSide : AbilityCardSideModel
{
}