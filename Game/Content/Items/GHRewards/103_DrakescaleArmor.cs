using Fractural.Tasks;

public class DrakescaleArmor : GHRewardsItem
{
	public override string Name => "Drakescale Armor";
	public override int ItemNumber => 103;
	public override int ShopCount => 1;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 8;

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