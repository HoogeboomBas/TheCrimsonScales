public static class Enhancements
{
	public static EnhancementModel[] NegativeConditions { get; } =
	[
		ModelDB.Enhancement<MuddleEnhancement>(),
		ModelDB.Enhancement<PoisonEnhancement>(),
		ModelDB.Enhancement<WoundEnhancement>(),
		ModelDB.Enhancement<ImmobilizeEnhancement>(),
		ModelDB.Enhancement<CurseEnhancement>()
	];

	public static EnhancementModel[] PositiveConditions { get; } =
	[
		ModelDB.Enhancement<RegenerateEnhancement>(),
		ModelDB.Enhancement<WardEnhancement>(),
		ModelDB.Enhancement<BlessEnhancement>(),
		ModelDB.Enhancement<StrengthenEnhancement>(),
	];

	public static EnhancementModel[] Elements { get; } =
	[
		ModelDB.Enhancement<FireElementEnhancement>(),
		ModelDB.Enhancement<IceElementEnhancement>(),
		ModelDB.Enhancement<AirElementEnhancement>(),
		ModelDB.Enhancement<EarthElementEnhancement>(),
		ModelDB.Enhancement<LightElementEnhancement>(),
		ModelDB.Enhancement<DarkElementEnhancement>(),
		ModelDB.Enhancement<WildElementEnhancement>(),
	];
}