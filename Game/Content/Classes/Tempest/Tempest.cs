using Fractural.Tasks;
using Godot;

public partial class Tempest : Character
{
	public const string SparkIconPath = "res://Content/Classes/Tempest/Spark.svg";
	public const string SparkTokenIconPath = "res://Content/Classes/Tempest/SparkToken.png";

	[Export]
	private SparkIndicator _sparkIndicator;

	public int SparkCount;

	public override async GDTask Spawn(SavedCharacter savedCharacter, int index)
	{
		await base.Spawn(savedCharacter, index);
		_sparkIndicator.Hide();
	}

	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		GainSparks(4 - GameController.Instance.SavedCampaign.Characters.Count);

		ScenarioEvents.FigureKilledEvent.Subscribe(this, _sparkIndicator,
			parameters => EnemiesWith(parameters.Figure),
			async parameters =>
			{
				TempestSpark spark = SceneLoader.InstantiateScene<TempestSpark>("res://Content/Classes/Tempest/TempestSpark.tscn");
				GameController.Instance.Map.AddChild(spark);
				await spark.Init(parameters.Figure.Hex);
			}
		);
	}

	public void GainSparks(int count = 1)
	{
		for(int i = 0; i < count; i++)
		{
			if(SparkCount == 5)
			{
				break;
			}

			SparkCount++;
			if(SparkCount == 1)
			{
				_sparkIndicator.ShowAnimated();
			}
		}

		_sparkIndicator.SetStackText(SparkCount.ToString());
	}

	public void LoseSparks(int count = 1)
	{
		for(int i = 0; i < count; i++)
		{
			if(SparkCount == 0)
			{
				break;
			}

			SparkCount--;
		}

		if(SparkCount == 0)
		{
			_sparkIndicator.HideAnimated();
		}
		else
		{
			_sparkIndicator.SetStackText(SparkCount.ToString());
		}
	}

	public bool HasXSparks(int x)
	{
		return SparkCount >= x;
	}
}