using Fractural.Tasks;
using Godot;

public class BootsOfSpeed : Prosperity2Item
{
	public override string Name => "Boots of Speed";
	public override int ItemNumber => 15;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 0;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeInitiativesSorted(
			parameters => true,
			async parameters =>
			{
				await Use(async user =>
				{
					await AbilityCmd.GenericChoice(user,
						[GetSubscription(-10), GetSubscription(10)], hintText: "Decrease or increase your initiative?");
				});
			}
		);
	}

	private ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription GetSubscription(int adjustmentAmount)
	{
		return ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
			subscriptionParameters => true,
			async subscriptionParameters =>
			{
				ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(this, _subscriber,
					initiativeCheckParameters => initiativeCheckParameters.Figure == Owner,
					initiativeCheckParameters => initiativeCheckParameters.AdjustInitiative(adjustmentAmount));

				ScenarioEvents.RoundEndedEvent.Subscribe(Owner, _subscriber,
					parameters => true,
					async parameters =>
					{
						ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(this, _subscriber);
						ScenarioEvents.RoundEndedEvent.Unsubscribe(Owner, _subscriber);

						await GDTask.CompletedTask;
					});

				Owner.UpdateInitiative();

				await GDTask.CompletedTask;
			},
			effectType: EffectType.SelectableMandatory,
			effectButtonParameters: new TextEffectButton.Parameters($"{(adjustmentAmount > 0 ? "+" : "")}{adjustmentAmount}"),
			effectInfoViewParameters: new TextEffectInfoView.Parameters(
				$"{(adjustmentAmount > 0 ? "Increase" : "Decrease")} your initiative by {Mathf.Abs(adjustmentAmount)}")
		);
	}
}