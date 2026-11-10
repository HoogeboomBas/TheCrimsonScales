using Fractural.Tasks;
using Godot;

public partial class Hollowpact : Character
{
	public const string VoidEnergy = "res://Content/Classes/Hollowpact/cs-void-energy.png";
	public const string VoidPit = "res://Content/Classes/Hollowpact/cs-void-pit.png";
	public const string Voidsight = "res://Content/Classes/Hollowpact/cs-voidsight.png";

	[Export]
	private VoidEnergyIndicator _voidEnergyIndicator;

	private int _voidEnergyCount;

	public override async GDTask Spawn(SavedCharacter savedCharacter, int index)
	{
		await base.Spawn(savedCharacter, index);
		_voidEnergyIndicator.Hide();
	}

	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _voidEnergyCount,
			parameters => parameters.Figure == this,
			async parameters =>
			{
				if(_voidEnergyCount >= 2)
				{
					await AbilityCmd.AddCondition(null, this, ModelDB.Condition<Muddle>(), this);
				}

				if(_voidEnergyCount == 3)
				{
					await AbilityCmd.AddCondition(null, this, ModelDB.Condition<Wound>(), this);
				}

				await GDTask.CompletedTask;
			}
		);

		GameController.Instance.EndEvent += (scenarioResult, savedScenarioProgress) => _voidEnergyIndicator.QueueFree();
	}

	public static DivinationAbility.DivinationBuilder VoidsightAbilityBuilder()
	{
		return DivinationAbility.Builder().WithCardsToPeek(1).WithMaxCardsToPlaceAtBottom(1).WithMandatory(true).WithTarget(Target.Self);
	}

	public static CreateObstacleAbility.CreateObstacleBuilder CreateVoidPitObstacleAbilityBuilder()
	{
		return CreateObstacleAbility.Builder()
			.WithCustomAsset("res://Content/Classes/Hollowpact/VoidPit.tscn")
			.WithCustomName("Void Pit");
	}

	public void GainVoidEnergy(int count = 1)
	{
		for(int i = 0; i < count; i++)
		{
			if(_voidEnergyCount == 3)
			{
				break;
			}

			_voidEnergyCount++;
			if(_voidEnergyCount == 1)
			{
				_voidEnergyIndicator.ShowAnimated();
			}
		}

		_voidEnergyIndicator.SetStackText(_voidEnergyCount.ToString());
	}

	public void LoseVoidEnergy(int count = 1)
	{
		for(int i = 0; i < count; i++)
		{
			if(_voidEnergyCount == 0)
			{
				break;
			}

			_voidEnergyCount--;
		}

		if(_voidEnergyCount == 0)
		{
			_voidEnergyIndicator.HideAnimated();
		}
		else
		{
			_voidEnergyIndicator.SetStackText(_voidEnergyCount.ToString());
		}
	}

	public bool HasXVoidEnergy(int x)
	{
		return _voidEnergyCount >= x;
	}
}