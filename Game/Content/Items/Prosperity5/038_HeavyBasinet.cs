using Fractural.Tasks;

public class HeavyBasinet : Prosperity5Item
{
	public override string Name => "Heavy Basinet";
	public override int ItemNumber => 38;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;
	public override int MinusOneCount => 2;

	protected override int AtlasIndex => 4;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		ScenarioEvents.InflictConditionEvent.Subscribe(this, _subscriber,
			parameters =>
				parameters.Target == Owner &&
				(AbilityCmd.CheckImmunity(parameters.ConditionModel, Conditions.Stun) ||
				 AbilityCmd.CheckImmunity(parameters.ConditionModel, Conditions.Muddle)),
			async parameters =>
			{
				parameters.SetPrevented(true);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Subscribe(this, _subscriber,
			parameters => parameters.Figure == Owner,
			parameters =>
			{
				parameters.AddImmunity(Conditions.Stun);
				parameters.AddImmunity(Conditions.Muddle);
			}
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.InflictConditionEvent.Unsubscribe(this, _subscriber);
		ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Unsubscribe(this, _subscriber);
	}
}