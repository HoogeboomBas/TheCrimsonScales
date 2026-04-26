using Godot;

public partial class BetweenScenariosActionManager : Control
{
	[Export]
	private BetweenScenariosAction[] _actions;
	[Export]
	private BetweenScenariosAction _startActiveAction;

	public BetweenScenariosAction ActiveAction { get; private set; }

	public void Init()
	{
		foreach(BetweenScenariosAction action in _actions)
		{
			action.Button.BetterButton.Pressed += () => OnActionButtonPressed(action);
		}

		SetActive(_startActiveAction, false);
	}

	public void SetActive(BetweenScenariosAction action, bool checkInGloomhaven = true)
	{
		if(action == ActiveAction)
		{
			return;
		}

		if(action?.Transitioning == true || ActiveAction?.Transitioning == true)
		{
			return;
		}

		if(checkInGloomhaven && !BetweenScenariosController.Instance.InGloomhaven)
		{
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Not currently in Gloomhaven",
				"You have just completed a scenario that is linked to at least one other scenario. Would you like to return to Gloomhaven instead?",
				new TextButton.Parameters("Cancel",
					() =>
					{
					}
				),
				new TextButton.Parameters("Back to Gloomhaven",
					() =>
					{
						BetweenScenariosController.Instance.ReturnToGloomhaven();
					},
					TextButton.ColorType.Green,
					width: 400
				)
			));

			return;
		}

		BetweenScenariosAction previousActiveAction = ActiveAction;

		previousActiveAction?.Deactivate();

		ActiveAction = action;
		ActiveAction?.Activate(previousActiveAction);
	}

	private void OnActionButtonPressed(BetweenScenariosAction action)
	{
		SetActive(action);
	}
}