public abstract class HierophantPrayerCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : HierophantPrayerCardSide, new()
	where TBottom : HierophantPrayerCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/Hierophant/PrayerCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 2;

	public override int Level => 1;
	public override int Initiative => 50;
}

public abstract class HierophantPrayerCardSide : AbilityCardSide
{
}