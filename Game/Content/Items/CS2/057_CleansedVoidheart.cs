using System.Linq;
using Fractural.Tasks;

public class CleansedVoidheart : CS2Item
{
	public override string Name => "Cleansed Voidheart";
	public override int ItemNumber => 57;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Always;
	public override bool IsSolo => true;

	protected override int AtlasIndex => 31;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		ConditionModel muddle = Conditions.Muddle;
		ScenarioEvents.InflictConditionEvent.Subscribe(this, _subscriber,
			parameters =>
				Owner != null &&
				parameters.Target == Owner &&
				parameters.ConditionModel?.ImmunityCompareBaseConditions != null &&
				muddle.ImmunityCompareBaseConditions != null &&
				parameters.ConditionModel.ImmunityCompareBaseConditions
					.Any(c1 => muddle.ImmunityCompareBaseConditions.Contains(c1)),
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
				parameters.AddImmunity(muddle);
			}
		);

		ScenarioEvents.RoundEndedEvent.Subscribe(this, _subscriber,
			_ => Owner.HasWound(),
			async _ =>
			{
				await AbilityCmd.AddCondition(null, Owner, Conditions.Regenerate);
			});
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.InflictConditionEvent.Unsubscribe(this, _subscriber);
		ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.RoundEndedEvent.Unsubscribe(this, _subscriber);
	}
}