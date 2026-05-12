public abstract class SpiritCallerCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : SpiritCallerCardSide
	where TBottom : SpiritCallerCardSide
{
	protected override string TexturePath => "res://Content/Classes/SpiritCaller/Cards.jpg";
	protected override int ColumnCount => 6;
	protected override int RowCount => 5;
}

public abstract class SpiritCallerCardSide : AbilityCardSideModel
{
}