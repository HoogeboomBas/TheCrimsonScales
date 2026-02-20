using Fractural.Tasks;

public class FireKnightEmberCladding : FireKnightItem
{
	public override string Name => "Ember Cladding";
	public override int ItemNumber => 1;
	protected override int AtlasIndex => 10 - 1;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this, _subscriber,
			parameters =>
				Owner.EnemiesWith(parameters.Figure) &&
				RangeHelper.Distance(parameters.Hex, Owner.Hex) == 1,
			async parameters =>
			{
				await Use(async user =>
				{
					await AbilityCmd.SufferDamage(parameters.PotentialAbilityState, parameters.Figure, 2);

					await AbilityCmd.InfuseElement(null, Element.Fire, user);
				});

				await GDTask.CompletedTask;
			},
			EffectType.Selectable,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(this, _subscriber);
	}
}