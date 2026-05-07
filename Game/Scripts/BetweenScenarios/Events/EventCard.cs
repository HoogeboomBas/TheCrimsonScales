using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Fractural.Tasks;
using Godot;

public partial class EventCard : Control
{
	[Export]
	public Control FrontContainer { get; private set; }

	[Export]
	public ShapedEventText FrontEventText { get; private set; }

	[Export]
	public Control BackContainer { get; private set; }

	[Export]
	public ShapedEventText BackEventText { get; private set; }

	[Export]
	public Label NumberLabel;

	[Export]
	private PackedScene _rewardLabelScene;

	[Export]
	private Control _rewardLabelParent;

	private readonly List<RichTextLabel> _rewardLabels = new List<RichTextLabel>();

	private bool _skipText;

	public void SetupFront(EventModel eventModel, bool showText, bool showFront = true)
	{
		if(showFront)
		{
			ShowFront();
		}

		FrontEventText.SetText(TextHelper.Prettify(eventModel.Text), showText);
		NumberLabel.SetText(eventModel.Number.ToString());
	}

	public void SetupBack(SavedEventState savedEventState, bool showText = true, bool showBack = true)
	{
		if(showBack)
		{
			ShowBack();
		}

		BackEventText.SetText(TextHelper.Prettify(savedEventState.Choice.GetStoryText(savedEventState)), showText);

		foreach(RichTextLabel rewardLabel in _rewardLabels)
		{
			rewardLabel.QueueFree();
		}

		_rewardLabels.Clear();

		List<SavedReward> eventRewards = savedEventState.Choice.GetRewards(savedEventState);
		if(eventRewards.Count == 0)
		{
			eventRewards.Add(new NoEffectReward());
		}

		foreach(SavedReward eventReward in eventRewards)
		{
			RichTextLabel rewardLabel = _rewardLabelScene.Instantiate<RichTextLabel>();
			_rewardLabelParent.AddChild(rewardLabel);
			RichTextParameters textParameters = rewardLabel.GetRichTextParameters();
			rewardLabel.SetText(eventReward.GetLabelText(textParameters));
			rewardLabel.SetVisibleCharacters(0);
			_rewardLabels.Add(rewardLabel);
		}

		this.DelayedCall(() =>
		{
			RichTextLabel lastLabel = BackEventText.RichTextLabels.Last();
			_rewardLabelParent.SetPosition(new Vector2(0f, lastLabel.Position.Y + lastLabel.Size.Y + 10f));
		});
	}

	public void ShowFront()
	{
		FrontContainer.SetVisible(true);
		BackContainer.SetVisible(true);
		FrontContainer.SetModulate(Colors.White);
		BackContainer.SetModulate(Colors.Transparent);
	}

	public void ShowBack()
	{
		FrontContainer.SetVisible(true);
		BackContainer.SetVisible(true);
		FrontContainer.SetModulate(Colors.Transparent);
		BackContainer.SetModulate(Colors.White);
	}

	public async GDTask AnimateText(ShapedEventText shapedEventText, CancellationToken cancellationToken)
	{
		_skipText = false;

		const float charactersPerSecond = 50f;
		float charactersToDisplay = 0f;
		bool waitedFrame = false;

		List<RichTextLabel> allLabels = new List<RichTextLabel>();
		allLabels.AddRange(shapedEventText.RichTextLabels);
		allLabels.AddRange(_rewardLabels);
		foreach(RichTextLabel label in allLabels)
		{
			int labelLength = label.GetParsedText().Length;
			while(true)
			{
				if(_skipText)
				{
					charactersToDisplay += Mathf.Inf;
				}

				if(waitedFrame)
				{
					charactersToDisplay += charactersPerSecond * (float)GetProcessDeltaTime();
					waitedFrame = false;
				}

				label.SetVisibleCharacters(Mathf.Min(Mathf.FloorToInt(charactersToDisplay), labelLength));

				if(charactersToDisplay > labelLength)
				{
					charactersToDisplay -= labelLength;
					break;
				}

				await GDTask.Yield(cancellationToken);
				waitedFrame = true;
			}
		}
	}

	public void SkipText()
	{
		_skipText = true;
	}
}