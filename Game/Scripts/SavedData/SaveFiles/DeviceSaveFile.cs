public class DeviceSaveFile : SaveFile<DeviceSaveData>
{
	protected override Migration[] Migrations { get; } =
	[
		new DummyMigration(), // Don't remove this
		new DummyMigration(), // Don't remove this
		new DummyMigration(), // Don't remove this
		new DummyMigration(), // Don't remove this
	];

	public DeviceSaveFile(string path)
		: base(path)
	{
	}
}