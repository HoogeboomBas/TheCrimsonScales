using Fractural.Tasks;
using GTweens.Easings;

public abstract partial class LootableObject : HexObject
{
	public virtual bool CanLoot(Figure lootObtainer)
	{
		return true;
	}

	public virtual async GDTask Loot(Figure lootObtainer)
	{
		ZIndex = 100;

		this.TweenGlobalJump(lootObtainer.Hex.GlobalPosition, 0.5f * Map.HexSize, 0.3f).PlayFastForwardable();
		this.TweenScale(0f, 0.35f).SetEasing(Easing.InBack).PlayFastForwardable();

		await ScenarioEvents.LootableObjectLootedEvent.CreatePrompt(new ScenarioEvents.LootableObjectLooted.Parameters(lootObtainer, this));

		await GDTask.DelayFastForwardable(0.3f);

		await Destroy();
	}
}