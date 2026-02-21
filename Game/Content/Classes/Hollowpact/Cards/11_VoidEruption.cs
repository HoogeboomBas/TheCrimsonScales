using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class VoidEruption : HollowpactCardModel<VoidEruption.CardTop, VoidEruption.CardBottom>
{
	public override string Name => "Void Eruption";
	public override int Level => 1;
	public override int Initiative => 31;
	protected override int AtlasIndex => 11;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithTargets(3)
				.WithPush(1)
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Hex hex in state.TargetedHexes.Where(hex => hex.IsUnoccupied()))
					{
						await AbilityCmd.CreateObstacle(hex, "res://Content/Classes/Hollowpact/VoidPit.tscn");
					}
					
					//TODO: Produce 2
					await GDTask.CompletedTask;
				})
				.Build()),
			
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Invisible)
				.WithTarget(Target.Self)
				.Build())
		];
		
		public override int XP => 1;
		public override bool Loss => true;
		
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			
			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(3)
				.WithConditionalAbilityCheck(async state =>
				{
					//TODO: Consume
					await GDTask.CompletedTask;

					return false;
				})
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Muddle);
				})
				.Build()),
		];
	}
}