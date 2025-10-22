using System;
using Godot;

public static class Platform
{
	public enum Type
	{
		Windows,
		Android,
		iOS,
		macOS,
		Linux,
	}

	private static Type? _platformType;

	public static Type PlatformType
	{
		get
		{
			if(!_platformType.HasValue)
			{
				switch(OS.GetName())
				{
					case "Android":
						_platformType = Type.Android;
						break;
					case "iOS":
						_platformType = Type.iOS;
						break;
					case "Windows":
						_platformType = Type.Windows;
						break;
					case "macOS":
						_platformType = Type.macOS;
						break;
					case "Linux":
						_platformType = Type.Linux;
						break;
					default:
						throw new Exception("Unknown platform type");
				}
			}

			return _platformType.Value;
		}
	}

	public static bool DeskTop => PlatformType is Type.Windows or Type.macOS or Type.Linux;
}