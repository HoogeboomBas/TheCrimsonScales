using Godot;

public static class VibrationController
{
	public enum VibrationType
	{
		Short,
		Medium,
		Long
	}

	public static void Vibrate(VibrationType type = VibrationType.Short)
	{
		if(!AppController.Instance.DeviceOptions.VibrationsEnabled.Value)
		{
			return;
		}

		int duration = type switch
		{
			VibrationType.Short => 25,
			VibrationType.Medium => 100,
			VibrationType.Long => 500,
			_ => 0
		};

		Input.VibrateHandheld(duration);
	}
}