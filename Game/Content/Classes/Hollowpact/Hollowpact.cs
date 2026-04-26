using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;

public partial class Hollowpact : Character
{
	public const string VoidEnergy = "res://Content/Classes/Hollowpact/cs-void-energy.png";

	//[Export]
	//private voidEnergyIndicator _voidEnergyIndicator;

	private int _voidEnergyCount;

	public override async GDTask Spawn(SavedCharacter savedCharacter, int index)
	{
		await base.Spawn(savedCharacter, index);
		//_voidEnergyIndicator.Hide();
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
					await AbilityCmd.AddCondition(null, this, ModelDB.Condition<Muddle>());
				}

				if(_voidEnergyCount == 3)
				{
					await AbilityCmd.AddCondition(null, this, ModelDB.Condition<Wound>());
				}

				await GDTask.CompletedTask;
			}
		);
	}

	public void GainVoidEnergy()
	{
		if(_voidEnergyCount == 3)
		{
			return;
		}

		_voidEnergyCount++;
		if(_voidEnergyCount == 1)
		{
			//_voidEnergyIndicator.ShowAnimated();
		}

		//_voidEnergyIndicator.SetStackText(_voidEnergyCount.ToString());
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
			//_voidEnergyIndicator.HideAnimated();
		}
		else
		{
			//_voidEnergyIndicator.SetStackText(_voidEnergyCount.ToString());
		}
	}

	public bool HasXVoidEnergy(int x)
	{
		return _voidEnergyCount >= x;
	}
}
