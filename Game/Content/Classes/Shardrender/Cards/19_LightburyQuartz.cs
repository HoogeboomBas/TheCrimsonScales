using System.Collections.Generic;
using Godot;

public class LightburyQuartz : ShardrenderCardModel<LightburyQuartz.CardTop, LightburyQuartz.CardBottom>
{
	public override string Name => "Lightbury Quartz";
	public override int Level => 4;
	public override int Initiative => 14;
	protected override int AtlasIndex => 19;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AbilityCmd.AllOpposingAttacksGainDisadvantageActiveAbility(false)),
			new AbilityCardAbility(CrystallizeAbility.Builder()
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.16452652f, 0.37396124f)),
						new UseSlot(new Vector2(0.37168446f, 0.37396124f)),
						new UseSlot(new Vector2(0.578171f, 0.37396124f)),
						new UseSlot(new Vector2(0.78882915f, 0.37396124f))
					]
				)
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6214308f, 0.7212836f)))
				.Build()),
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1, new ShieldCircle(this, new Vector2(0.6198307f, 0.81988245f)))
				.Build())
		];

		public override bool Round => true;
	}
}