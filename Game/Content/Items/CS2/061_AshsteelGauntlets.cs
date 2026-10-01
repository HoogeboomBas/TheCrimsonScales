using System.Linq;
using Fractural.Tasks;

public class AshsteelGauntlets : CS2Item
{
	public override string Name => "Ashsteel Gauntlets";
	public override int ItemNumber => 61;
	public override int ShopCount => 1;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;
	public override bool Round => true;
	public override int MinusOneCount => 2;

	protected override int AtlasIndex => 38;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		ConditionModel conditionModel = Conditions.Disarm;

		AbilityCmd.AddConditionImmunity(Conditions.Disarm, Owner, this);

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await GetActionState(user,
					[
						GrantAbility.Builder()
							.WithAbilities(ShieldAbility.Builder().WithShieldValue(1).Build())
							.WithTarget(Target.Self)
							.Build()
					]).Perform();
				});
			}
		);
	}
}