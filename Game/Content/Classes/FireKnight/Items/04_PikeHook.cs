using Fractural.Tasks;

public class FireKnightPikeHook : FireKnightItem
{
	public override string Name => "Pike Hook";
	public override int ItemNumber => 4;
	protected override int AtlasIndex => 10 - 4;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Melee && state.IsSingleTarget,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetAdjustPierce(2);
					state.SingleTargetAdjustPull(1);
					if(state.SingleTargetRange == 1)
					{
						state.SingleTargetAdjustRange(1);
					}

					await GDTask.CompletedTask;
				});
			}
		);
	}
}