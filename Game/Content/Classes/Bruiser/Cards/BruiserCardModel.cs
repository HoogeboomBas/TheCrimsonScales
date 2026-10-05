public abstract class BruiserLevelUpCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : BruiserCardSide
	where TBottom : BruiserCardSide
{
	protected override string TexturePath => "res://Content/Classes/Bruiser/LevelUpCards.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 4;
}

public abstract class BruiserCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : BruiserCardSide
	where TBottom : BruiserCardSide
{
	protected override string TexturePath => "res://Content/Classes/Bruiser/Cards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}

public abstract class BruiserCardSide : AbilityCardSideModel<Character>
{
}
