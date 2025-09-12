public abstract class ChainguardCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : ChainguardCardSide, new()
	where TBottom : ChainguardCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/Chainguard/Cards.jpg";
	protected override int ColumnCount => 6;
	protected override int RowCount => 5;
}

public abstract class ChainguardCardSide : AbilityCardSide
{
}