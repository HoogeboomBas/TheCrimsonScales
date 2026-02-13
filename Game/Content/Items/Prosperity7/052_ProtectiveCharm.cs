using Fractural.Tasks;

public class ProtectiveCharm : Prosperity7Item
{
	public override string Name => "Protective Charm";
	public override int ItemNumber => 52;
	public override int ShopCount => 2;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;

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
				(AbilityCmd.CheckImmunity(parameters.ConditionModel, Conditions.Wound1) ||
				 AbilityCmd.CheckImmunity(parameters.ConditionModel, Conditions.Poison1)),
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
				parameters.AddImmunity(Conditions.Wound1);
				parameters.AddImmunity(Conditions.Poison1);
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