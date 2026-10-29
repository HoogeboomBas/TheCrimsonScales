using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public partial class Incarnate : Character, IHasEmpower, IHasEnfeeble
{
	public const string RitualistIconPath = "res://Content/Classes/Incarnate/Ritualist.svg";
	public const string ConquerorIconPath = "res://Content/Classes/Incarnate/Conqueror.svg";
	public const string ReaverIconPath = "res://Content/Classes/Incarnate/Reaver.svg";
	public const string ThreeSpiritIconPath = "res://Content/Classes/Incarnate/ThreeSpirit.png";

	public static readonly Dictionary<IncarnateSpirit, string> SpiritIconPaths = new Dictionary<IncarnateSpirit, string>
	{
		{ IncarnateSpirit.Ritualist, RitualistIconPath },
		{ IncarnateSpirit.Conqueror, ConquerorIconPath },
		{ IncarnateSpirit.Reaver, ReaverIconPath },
	};

	public static EmpowerIncarnate Empower { get; } = ModelDB.Condition<EmpowerIncarnate>();
	public static EnfeebleIncarnate Enfeeble { get; } = ModelDB.Condition<EnfeebleIncarnate>();

	[Export]
	private Sprite2D _ritualistIndicator;
	[Export]
	private Sprite2D _conquerorIndicator;
	[Export]
	private Sprite2D _reaverIndicator;

	private bool _satedAppliedThisTurn;

	public IncarnateSpirit? Spirit { get; private set; }
	public int RemainingEmpowerCount { get; set; } = 10;
	public int RemainingEnfeebleCount { get; set; } = 10;

	public override async GDTask Spawn(SavedCharacter savedCharacter, int index)
	{
		await base.Spawn(savedCharacter, index);

		object subscriber = new object();
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this, subscriber,
			parameters => parameters.Figure == this,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(_ =>
					$"{Icons.Inline($"res://Content/Classes/Incarnate/{Spirit}.svg")}"));
			}
		);
	}

	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		await ChooseSpirit([IncarnateSpirit.Ritualist, IncarnateSpirit.Conqueror, IncarnateSpirit.Reaver]);
	}

	public async GDTask ChooseSpirit(IEnumerable<IncarnateSpirit> choices)
	{
		List<IncarnateSpirit> incarnateSpirits = (await ScenarioEvents.ChangeIncarnateSpiritEvent.CreatePrompt(
			new ScenarioEvents.ChangeIncarnateSpirit.Parameters(this, choices.ToList()))).SpiritChoices;

		if(incarnateSpirits.Count == 1)
		{
			await SwitchSpirit(incarnateSpirits.First());
			return;
		}

		List<ScenarioEvents.GenericChoice.Subscription> subscriptions = [];
		foreach(IncarnateSpirit spirit in incarnateSpirits)
		{
			subscriptions.Add(ScenarioEvents.GenericChoice.Subscription.New(
				applyFunction: async _ =>
				{
					await SwitchSpirit(spirit);
				},
				effectType: EffectType.SelectableMandatory,
				effectButtonParameters: new IconEffectButton.Parameters(SpiritIconPaths[spirit]),
				effectInfoViewParameters: new TextEffectInfoView.Parameters(
					$"{Icons.Inline(SpiritIconPaths[spirit])}")
			));
		}

		await AbilityCmd.GenericChoice(this, subscriptions, hintText: "Select a spirit");
	}

	private async GDTask SwitchSpirit(IncarnateSpirit spirit)
	{
		if(Spirit == spirit)
		{
			return;
		}

		Spirit = spirit;
		_ritualistIndicator.Hide();
		_reaverIndicator.Hide();
		_conquerorIndicator.Hide();

		switch(Spirit)
		{
			case IncarnateSpirit.Ritualist:
				_ritualistIndicator.Show();
				break;
			case IncarnateSpirit.Conqueror:
				_conquerorIndicator.Show();
				break;
			case IncarnateSpirit.Reaver:
				_reaverIndicator.Show();
				break;
		}

		await ScenarioEvents.IncarnateSpiritChangedEvent.CreatePrompt(
			new ScenarioEvents.IncarnateSpiritChanged.Parameters(this));
	}

	public AMDCardModel CreateEmpower()
	{
		return ModelDB.AMDCard<IncarnateEmpowerAMDCard>();
	}

	public AMDCardModel CreateEnfeeble()
	{
		return ModelDB.AMDCard<IncarnateEnfeebleAMDCard>();
	}
}