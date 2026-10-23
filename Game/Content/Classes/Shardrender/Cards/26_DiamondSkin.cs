using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class DiamondSkin : ShardrenderCardModel<DiamondSkin.CardTop, DiamondSkin.CardBottom>
{
	public override string Name => "Diamond Skin";
	public override int Level => 8;
	public override int Initiative => 19;
	protected override int AtlasIndex => 26;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.Build()),
			new AbilityCardAbility(CrystallizeAbility.Builder()
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29180175f, 0.29750696f)),
					new UseSlot(new Vector2(0.49901202f, 0.29750696f)),
					new UseSlot(new Vector2(0.7069984f, 0.29750696f)),
					new UseSlot(new Vector2(0.18876071f, 0.4253307f)),
					new UseSlot(new Vector2(0.396471f, 0.4253307f)),
					new UseSlot(new Vector2(0.6039813f, 0.4253307f))
				])
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RemoveConditionEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && parameters.ConditionModel == Conditions.Ward,
						async parameters =>
						{
							parameters.SetPrevented();

							await GDTask.CompletedTask;
						});
					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Ward);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RemoveConditionEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}