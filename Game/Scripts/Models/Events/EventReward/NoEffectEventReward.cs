using Godot;

public class NoEffectEventReward() : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => "No effect.";
}