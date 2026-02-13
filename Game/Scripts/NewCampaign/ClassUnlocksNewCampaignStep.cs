using System.Threading;
using Godot;

public partial class ClassUnlocksNewCampaignStep : NewCampaignStep
{
	[Export]
	private UnlockCharacterView _unlockCharacterView;

	private CancellationTokenSource _cancellationTokenSource;
	private ClassModel[] _classModels;
	private int _classIndex;

	public override bool ConfirmButtonActive => false;

	public override void Activate()
	{
		base.Activate();

		_cancellationTokenSource = new CancellationTokenSource();
		_classModels = SavedCampaign.GetStartingClasses(NewCampaignController.Instance.StartingGroup);
		_classIndex = 0;

		_unlockCharacterView.ClosedEvent += OnUnlockViewClosed;
		_unlockCharacterView.SkipButtonPressedEvent += OnSkipButtonPressed;

		_unlockCharacterView.Open(_classModels[_classIndex], _cancellationTokenSource.Token);
	}

	public override void Deactivate()
	{
		base.Deactivate();

		_cancellationTokenSource.Cancel();

		_unlockCharacterView.ClosedEvent -= OnUnlockViewClosed;
		_unlockCharacterView.SkipButtonPressedEvent -= OnSkipButtonPressed;
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		_cancellationTokenSource?.Cancel();
	}

	private void OnUnlockViewClosed()
	{
		_classIndex++;

		if(_classIndex >= _classModels.Length)
		{
			NewCampaignController.Instance.NextStep();
			return;
		}

		_unlockCharacterView.Open(_classModels[_classIndex], _cancellationTokenSource.Token);
	}

	private void OnSkipButtonPressed()
	{
		_cancellationTokenSource?.Cancel();
		_cancellationTokenSource = new CancellationTokenSource();
		_unlockCharacterView.Skip();
	}
}