public abstract class HollowpactLevelUpCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : HollowpactCardSide
	where TBottom : HollowpactCardSide
{
	protected override string TexturePath => "res://Content/Classes/Hollowpact/LevelUpCards.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 4;
}

public abstract class HollowpactCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : HollowpactCardSide
	where TBottom : HollowpactCardSide
{
	protected override string TexturePath => "res://Content/Classes/Hollowpact/Cards.png";
	protected override int ColumnCount => 8;
	protected override int RowCount => 2;
}

public abstract class HollowpactCardSide : AbilityCardSideModel
{
}