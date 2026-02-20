using System.Linq;
using Godot;

public partial class CoinInfoItem : InfoItem<CoinInfoItem.Parameters>
{
	public class Parameters(Coin hexObject) : InfoItemParameters<Coin>(hexObject)
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/InfoView/CoinInfoItem.tscn";
	}

	[Export]
	private Label _coinCountLabel;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_coinCountLabel.SetText($"x {parameters.HexObject.Hex.GetHexObjectsOfType<Coin>().Count()}");
	}
}