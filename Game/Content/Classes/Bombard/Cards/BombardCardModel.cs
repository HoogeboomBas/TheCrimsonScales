public abstract class BombardCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : BombardCardSide
	where TBottom : BombardCardSide
{
	protected override string TexturePath => "res://Content/Classes/Bombard/Cards.jpg";
	protected override int ColumnCount => 6;
	protected override int RowCount => 5;
}

public abstract class BombardCardSide : AbilityCardSideModel
{
	public const string ProjectileIconPath = "res://Content/Classes/Bombard/ProjectileIcon.svg";
}