using System;
using Fractural.Tasks;
using Godot;

public partial class Ruinmaw : Character, IHasEmpower
{
	public bool Sated = false;
	private bool _satedAppliedThisTurn = false;
	private SatedIndicator _satedIndicator;
	public int RemainingEmpowerCount { get; set; } = 12;
	public Type EmpowerType { get; set; } = typeof(RuinmawEmpowerAMDCard);
	public Action<Ruinmaw> SateEvent;

	public void Sate()
	{
		_satedIndicator = ResourceLoader.Load<PackedScene>("res://Content/Classes/Ruinmaw/SatedIndicator.tscn").Instantiate<SatedIndicator>();
		
		AddChild(_satedIndicator);
		_satedIndicator.Modulate = Colors.White;
		_satedIndicator.SelfModulate = Colors.White;
		_satedIndicator.Init();
		if(TakingTurn)
		{
			_satedAppliedThisTurn = true;
		}
		if(!Sated)
		{
			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, Sated,
				canApplyParameters => canApplyParameters.Figure == this,
				async applyParameters =>
				{
					if(_satedAppliedThisTurn)
					{
						_satedAppliedThisTurn = false;
					}
					else
					{
						_satedIndicator?.Destroy();

						ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(this, Sated);
						ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(this, Sated);
					}
					await GDTask.CompletedTask;
				});
			ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this, Sated,
				parameters => parameters.Figure == this,
				parameters =>
				{
					parameters.Add(new FigureInfoTextExtraEffect.Parameters($"{Icons.Inline("res://Content/Classes/Ruinmaw/RuinmawSated.png")}"));
				}
			);
		}
		Sated = true;
	}
}