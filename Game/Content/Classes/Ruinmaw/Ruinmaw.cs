using System;
using Fractural.Tasks;
using Godot;

public partial class Ruinmaw : Character, IHasEmpower
{
	public const string SatedIconPath = "res://Content/Classes/Ruinmaw/SatedIcon.png";
	public const string SatedUpIconPath = "res://Content/Classes/Ruinmaw/SatedUpIcon.png";
	public static EmpowerRuinmaw Empower { get; } = ModelDB.Condition<EmpowerRuinmaw>();

	[Export]
	private SatedIndicator _satedIndicator;

	private bool _satedAppliedThisTurn;

	public bool Sated { get; private set; }
	public int RemainingEmpowerCount { get; set; } = 12;

	public event Func<Ruinmaw, GDTask> SateEvent;

	public override async GDTask Spawn(SavedCharacter savedCharacter, int index)
	{
		await base.Spawn(savedCharacter, index);

		_satedIndicator.Hide();
	}

	public async GDTask Sate()
	{
		if(TakingTurn)
		{
			_satedAppliedThisTurn = true;
		}

		if(Sated)
		{
			return;
		}

		_satedIndicator.ShowAnimated();
		object subscriber = new object();
		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, subscriber,
			canApplyParameters => canApplyParameters.Figure == this,
			async applyParameters =>
			{
				if(_satedAppliedThisTurn)
				{
					_satedAppliedThisTurn = false;
				}
				else
				{
					_satedIndicator.HideAnimated();

					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(this, subscriber);
					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(this, subscriber);
				}

				await GDTask.CompletedTask;
			});
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this, subscriber,
			parameters => parameters.Figure == this,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
					$"{Icons.Inline("res://Content/Classes/Ruinmaw/RuinmawSated.png")}"));
			}
		);

		Sated = true;
		if(SateEvent != null)
		{
			await SateEvent(this);
		}

		await GDTask.CompletedTask;
	}

	public AMDCardModel CreateEmpower()
	{
		return ModelDB.AMDCard<RuinmawEmpowerAMDCard>();
	}
}