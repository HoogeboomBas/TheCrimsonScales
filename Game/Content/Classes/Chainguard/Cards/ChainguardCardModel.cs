public abstract class ChainguardLevelUpCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : ChainguardCardSide
	where TBottom : ChainguardCardSide
{
	protected override string TexturePath => "res://Content/Classes/Chainguard/LevelUpCards.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 4;
}

public abstract class ChainguardCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : ChainguardCardSide
	where TBottom : ChainguardCardSide
{
	protected override string TexturePath => "res://Content/Classes/Chainguard/Cards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}

public abstract class ChainguardCardSide : AbilityCardSideModel
{
}