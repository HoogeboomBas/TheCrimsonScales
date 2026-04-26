using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;

public partial class Door : OverlayTile, IEventSubscriber
{
	[Export]
	private PackedScene _corridorScene;
	[Export]
	private bool _startsLocked;
	[Export]
	private Sprite2D _lock;

	private readonly List<Room> _roomsToOpen = new List<Room>();

	public bool Locked { get; private set; }
	public bool Opened { get; private set; }

	public override void _Ready()
	{
		base._Ready();

		_lock.SetVisible(_startsLocked);
		_lock.SetGlobalRotation(0f);
	}

	public void AddRoom(Room room)
	{
		_roomsToOpen.Add(room);
	}

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		Show();

		await base.Init(originHex, rotationIndex, hexCanBeNull);

		Locked = _startsLocked;

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
			parameters => parameters.Hex == Hex,
			async parameters =>
			{
				await Open(parameters.Figure);
			},
			effectType: EffectType.MandatoryBeforeOptionals
		);
	}

	public async GDTask Unlock()
	{
		Locked = false;
		await _lock.TweenScale(0f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardableAsync();

		await GDTask.CompletedTask;
	}

	public async GDTask Open(Figure potentialOpener)
	{
		Opened = true;

		await ScenarioEvents.DoorOpenedEvent.CreatePrompt(
			new ScenarioEvents.DoorOpened.Parameters(this, potentialOpener));

		ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(this);

		foreach(Room room in _roomsToOpen)
		{
			await room.Reveal(this, potentialOpener, false);
		}

		GameController.Instance.Map.UpdateWallLines();

		HexObject corridor = _corridorScene.Instantiate<HexObject>();
		GameController.Instance.Map.AddChild(corridor);
		await corridor.Init(Hex);

		await Destroy();
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new GenericInfoItem.Parameters(this, "Door",
			Locked
				? "This door is locked. It will open once specific conditions are met."
				: "A character can move on top of a door to open it."));
	}
}