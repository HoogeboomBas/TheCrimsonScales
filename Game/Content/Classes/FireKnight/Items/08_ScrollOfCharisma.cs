using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ScrollOfCharisma : FireKnightItem
{
	public override string Name => "Scroll of Charisma";
	public override int ItemNumber => 8;
	protected override int AtlasIndex => 10 - 8;

	protected override List<ItemUseSlot> GetUseSlots() =>
	[
		new ItemUseSlot(new Vector2(0.3234997f, 0.81101197f), async item => await AbilityCmd.InfuseElement(Element.Fire)),
		new ItemUseSlot(new Vector2(0.6015022f, 0.81101197f))
	];

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeAttackAfterTargetConfirmed(
			state => state.Performer == Owner && RangeHelper.GetFiguresInRange(Owner.Hex, 1, false).Any(figure => Owner.AlliedWith(figure)),
			async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetAdjustAttackValue(1);

					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.AbilityState == state,
						async applyParameters =>
						{
							ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);

							if(applyParameters.AMDCard.IsNull)
							{
								applyParameters.SetCard(new BasicAMDCard(applyParameters.AMDCard, 0));
							}

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}