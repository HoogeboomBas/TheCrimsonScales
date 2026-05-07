using System;
using System.Collections.Generic;
using Godot;

public partial class RewardsPopup : Popup<RewardsPopup.Request>
{
	public class Request : PopupRequest
	{
		public List<SavedReward> Rewards { get; init; }
	}

	[Export]
	private PackedScene _rewardLabelScene;
	[Export]
	private Control _rewardLabelParent;
	[Export]
	private ChoiceButton _confirmButton;

	private readonly List<RichTextLabel> _labels = new List<RichTextLabel>();

	public override void _Ready()
	{
		base._Ready();

		_confirmButton.BetterButton.Pressed += OnConfirmPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		foreach(SavedReward reward in PopupRequest.Rewards)
		{
			RichTextLabel label = _rewardLabelScene.Instantiate<RichTextLabel>();
			_rewardLabelParent.AddChild(label);
			RichTextParameters textParameters = label.GetRichTextParameters();
			label.SetText(reward.GetLabelText(textParameters));
			_labels.Add(label);
		}
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(RichTextLabel label in _labels)
		{
			label.QueueFree();
		}

		_labels.Clear();
	}

	private void OnConfirmPressed()
	{
		Close();
	}
}