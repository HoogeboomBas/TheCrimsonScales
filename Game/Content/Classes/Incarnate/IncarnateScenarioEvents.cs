using System.Collections.Generic;

public partial class ScenarioEvents
{
	public class ChangeIncarnateSpirit : ScenarioEvent<ChangeIncarnateSpirit.Parameters>
	{
		public class Parameters(Incarnate incarnate, List<IncarnateSpirit> spiritChoices) : ParametersBase
		{
			public Incarnate Incarnate { get; } = incarnate;
			public List<IncarnateSpirit> SpiritChoices { get; } = spiritChoices;

			public void AddSpiritChoices(List<IncarnateSpirit> spiritChoices)
			{
				foreach(IncarnateSpirit spirit in spiritChoices)
				{
					SpiritChoices.AddIfNew(spirit);
				}
			}
		}
	}

	private readonly ChangeIncarnateSpirit _changeIncarnateSpirit = new ChangeIncarnateSpirit();
	public static ChangeIncarnateSpirit ChangeIncarnateSpiritEvent => GameController.Instance.ScenarioEvents._changeIncarnateSpirit;

	public class IncarnateSpiritChanged : ScenarioEvent<IncarnateSpiritChanged.Parameters>
	{
		public class Parameters(Incarnate incarnate) : ParametersBase
		{
			public Incarnate Incarnate { get; } = incarnate;
			public IncarnateSpirit Spirit { get; } = incarnate.Spirit!.Value;
		}
	}

	private readonly IncarnateSpiritChanged _incarnateSpiritChanged = new IncarnateSpiritChanged();
	public static IncarnateSpiritChanged IncarnateSpiritChangedEvent => GameController.Instance.ScenarioEvents._incarnateSpiritChanged;
}