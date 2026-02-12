using System.Collections.Generic;
using Fractural.Tasks;

public class PreparedRations : ChieftainCardModel<PreparedRations.CardTop, PreparedRations.CardBottom>
{
	public override string Name => "Prepared Rations";
	public override int Level => 1;
	public override int Initiative => 91;
	protected override int AtlasIndex => 10;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 4,
					Move = 3,
					Traits = 
					[
						new RetaliateTrait(1),
						new MountTrait(
							async (owner, mount) => 
							{
								ScenarioEvents.RoundEndedEvent.Subscribe(owner, this,
									parameters => true,
									async parameters => 
									{
										ActionState actionState = new ActionState(owner, 
											[HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()]);
										await actionState.Perform();
									}
								);
								await GDTask.CompletedTask;
							},
							async (owner, mount) => 
							{ 
								ScenarioEvents.RoundEndedEvent.Unsubscribe(owner, this);

								await GDTask.CompletedTask;
							}
						)
					]
				})
				.WithName("Pack Mule")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/pack_mule_AI.png")
				.Build()
			),
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).Build()),
			new AbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()),
		];
	}
}