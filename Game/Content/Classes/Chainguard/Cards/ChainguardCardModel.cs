public abstract class ChainguardLevelUpCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : ChainguardCardSide, new()
	where TBottom : ChainguardCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/Chainguard/LevelUpCards.png";
	protected override int ColumnCount => 5;
	protected override int RowCount => 4;
}

public abstract class ChainguardCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : ChainguardCardSide, new()
	where TBottom : ChainguardCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/Chainguard/Cards.png";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}

public abstract class ChainguardCardSide : AbilityCardSide
{
}