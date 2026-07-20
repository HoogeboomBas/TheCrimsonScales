using Godot;

public static class SFX
{
	public enum StepType
	{
		Dirt = 0,
		Stone = 1,
		Water = 2,
		Wood = 3
	}

	public const string Click = "res://Audio/SFX/BUTTON_Clean_mono.wav";
	public const string Heal = "res://Audio/SFX/Heal.wav";
	public const string MoveFlying = "res://Audio/SFX/Elemental/ElementalWoosh1.wav";
	public const string CoinPickup = "res://Audio/SFX/CoinPickup.wav";
	public const string Shield = "res://Audio/SFX/IMPACT_Metal_Cling_Deep_Damped_mono.wav";
	public const string Buy = "res://Audio/SFX/ItemShop/COINS_Rattle_01_mono.wav";

	public static string GetSwordHit()
	{
		int randomIndex = GD.RandRange(1, 3);
		return $"res://Audio/SFX/Attacks/Sword Impact Hit {randomIndex}.wav";
	}

	public static string GetSwordBlocked()
	{
		int randomIndex = GD.RandRange(1, 3);
		return $"res://Audio/SFX/Attacks/Sword Blocked {randomIndex}.wav";
	}

	public static string GetBowAttack()
	{
		int randomIndex = GD.RandRange(1, 2);
		return $"res://Audio/SFX/Attacks/Bow Attack {randomIndex}.wav";
	}

	public static string GetBowHit()
	{
		int randomIndex = GD.RandRange(1, 3);
		return $"res://Audio/SFX/Attacks/Bow Impact Hit {randomIndex}.wav";
	}

	public static string GetBowBlocked()
	{
		int randomIndex = GD.RandRange(1, 3);
		return $"res://Audio/SFX/Attacks/Bow Blocked {randomIndex}.wav";
	}

	public static string GetStep(StepType stepType)
	{
		int randomIndex = GD.RandRange(1, 5);
		return $"res://Audio/SFX/Footsteps/{stepType.ToString()} Walk {randomIndex}.wav";
	}

	public static string GetJump(StepType stepType)
	{
		return $"res://Audio/SFX/Footsteps/{stepType.ToString()} Jump.wav";
	}

	public static string GetLand(StepType stepType)
	{
		return $"res://Audio/SFX/Footsteps/{stepType.ToString()} Land.wav";
	}

	public static string GetCardDraw()
	{
		int randomIndex = GD.RandRange(1, 2);
		return $"res://Audio/SFX/Cards/CardSlide{randomIndex}.wav";
	}

	public static string GetStep(Hex hex)
	{
		return GetStep(GetStepType(hex));
	}

	public static string GetJump(Hex hex)
	{
		return GetJump(GetStepType(hex));
	}

	public static string GetLand(Hex hex)
	{
		return GetLand(GetStepType(hex));
	}

	private static StepType GetStepType(Hex hex)
	{
		StepType? stepType = null;
		foreach(HexObject hexObject in hex.HexObjects)
		{
			stepType = hexObject.OverrideStepType;

			if(stepType.HasValue)
			{
				break;
			}
		}

		if(!stepType.HasValue)
		{
			stepType = hex.MapTile?.StepType;
		}

		if(!stepType.HasValue)
		{
			stepType = StepType.Stone;
		}

		return stepType.Value;
	}
}