using Fractural.Tasks;

public class MedicalKit : CS3Item
{
	public override string Name => "Medical Kit";
	public override int ItemNumber => 73;
	public override int ShopCount => 1;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Always;

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

		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this, _subscriber,
			parameters =>
				!parameters.ForgoneAction &&
				parameters.AbilityCardSide.AbilityCardSideType is AbilityCardSideType.BasicTop,
			async parameters =>
			{
				parameters.ForgoAction();

				ActionState actionState = new ActionState(parameters.Performer, [HealAbility.Builder().WithHealValue(2).WithRange(1).Build()]);
				await actionState.Perform();
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.Heal),
			effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Heal)}2, {Icons.Inline(Icons.Range)}1")
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.AbilityCardSideStartedEvent.Unsubscribe(this, _subscriber);
	}
}