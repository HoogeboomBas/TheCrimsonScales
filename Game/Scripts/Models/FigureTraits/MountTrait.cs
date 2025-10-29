using Fractural.Tasks;
using System;

public class MountTrait(Func<Figure, Figure, GDTask> onMounted = null, Func<Figure, Figure, GDTask> onDismounted = null) : FigureTrait
{
	private bool _mounted = false;

	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		Figure characterOwner = ((Summon)figure).CharacterOwner;

		// Allow entering the same hex to mount
		ScenarioCheckEvents.CanEnterHexWithFigureCheckEvent.Subscribe(figure, this,
			parameters => parameters.OtherFigure == figure && parameters.Figure == characterOwner,
			parameters =>
			{
				parameters.SetCanEnter();
			}
		);

		// Control the mount
		ScenarioCheckEvents.IsSummonControlledCheckEvent.Subscribe(figure, this,
			parameters => parameters.Summon == figure && _mounted,
			parameters =>
			{
				parameters.SetIsControlled();
			}
		);

		// Follow the mount when it moves or being forcefully moved
		ScenarioEvents.MoveTogetherCheckEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure && _mounted,
			parameters =>
			{
				parameters.SetOtherFigure(characterOwner);

				return GDTask.CompletedTask;
			}
		);
		
		// Mounted summon goes just before the character
		ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure && _mounted,
			parameters => parameters.SetSortingInitiative(characterOwner.Initiative.SortingInitiative - 1)
		);

		// Trigger mounted effect
		ScenarioEvents.FigureEnteredHexEvent.Subscribe(figure, this,
			parameters => parameters.Figure == characterOwner,
			async parameters =>
			{
				if(!_mounted && parameters.Hex == figure.Hex)
				{
					_mounted = true;
					figure.UpdateInitiative();

					if(onMounted != null)
					{
						await onMounted(characterOwner, figure);
					}
				}
				else if(_mounted && parameters.Hex != figure.Hex)
				{
					_mounted = false;
					figure.UpdateInitiative();

					if(onDismounted != null)
					{
						await onDismounted(characterOwner, figure);
					}
				}
			}
		);

		// Returning mounted status for other effects and abilities
		ScenarioCheckEvents.IsMountedCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == characterOwner,
			async parameters =>
			{	
				if(_mounted) 
				{
					parameters.SetIsMounted();
					parameters.SetMount(figure);
				}

				await GDTask.CompletedTask;
			}
		);

		// Mounted summon can open doors
		ScenarioCheckEvents.CanOpenDoorsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			async parameters =>
			{	
				if(_mounted) 
				{
					parameters.SetCanOpenDoors();
				}

				await GDTask.CompletedTask;
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		_mounted = false;

		Figure characterOwner = ((Summon)figure).CharacterOwner;

		ScenarioCheckEvents.CanEnterHexWithFigureCheckEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.IsSummonControlledCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.MoveTogetherCheckEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.IsMountedCheckEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.CanOpenDoorsCheckEvent.Unsubscribe(figure, this);

		onDismounted?.Invoke(characterOwner, figure);
	}
}