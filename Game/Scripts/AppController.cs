using Godot;

public partial class AppController : SingletonNode<AppController>
{
	[Export]
	public InputController InputController { get; private set; }

	[Export]
	public SceneLoader SceneLoader { get; private set; }

	[Export]
	public PopupManager PopupManager { get; private set; }

	[Export]
	public AudioController AudioController { get; private set; }

	[Export]
	public CardSelectionCardPreview CardSelectionCardPreview { get; private set; }

	[Export]
	public ItemPreview ItemPreview { get; private set; }

	public SaveFile SaveFile { get; private set; }

	public SavedOptions Options => SaveFile.SaveData.Options;

	public override void _EnterTree()
	{
		SaveFile = new SaveFile("user://SaveFile.save");
	}

	public override void _Ready()
	{
		base._Ready();

		if(SaveFile.RemovedSavedScenario)
		{
			PopupManager.RequestPopup(new TextPopup.Request("New Version",
				"A new version of The Crimson Scales was installed. This unfortunately meant that the progress on the last scenario was incompatible with the new version." +
				"\nPlease always make sure to finish up a scenario before installing a new version of the application!"));
		}
	}
}