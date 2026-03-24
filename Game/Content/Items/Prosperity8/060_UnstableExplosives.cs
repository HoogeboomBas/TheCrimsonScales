using System.Linq;
using Fractural.Tasks;
using Godot;

public class UnstableExplosives : Prosperity8Item
{
	public override string Name => "Unstable Explosives";
	public override int ItemNumber => 60;
	public override int ShopCount => 2;
	public override int Cost => 45;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 6;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeAbilityStarted<AttackAbility.State>(
			canApply: state =>
				state.Performer == Owner &&
				state.IsSingleTarget &&
				state.AbilityRangeType == RangeType.Range,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilitySetAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red)
						]
					));

					ScenarioEvents.AbilityEndedEvent.Subscribe(state, this,
						parameters => parameters.AbilityState == state,
						async parameters =>
						{
							foreach(Figure figure in state.GetRedAOEHexes().SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
								        .Where(figure => state.Performer.AlliedWith(figure)))
							{
								await AbilityCmd.SufferDamage(state, figure, 3);
							}

							ScenarioEvents.AbilityEndedEvent.Unsubscribe(state, this);
						});


					await GDTask.CompletedTask;
				});
			}
		);
	}
}