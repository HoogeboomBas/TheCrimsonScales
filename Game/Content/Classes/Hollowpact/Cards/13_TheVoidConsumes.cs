using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class TheVoidConsumes : HollowpactCardModel<TheVoidConsumes.CardTop, TheVoidConsumes.CardBottom>
{
	public override string Name => "The Void Consumes";
	public override int Level => 1;
	public override int Initiative => 35;
	protected override int AtlasIndex => 13;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6)
				.WithAdvantage()
				.WithPush(1)
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					//TODO: Consume 3
					return false;
				})
				.Build()),
			
			new AbilityCardAbility(CreateObstacleAbility.Builder()
				.WithCustomAsset("res://Content/Classes/Hollowpact/VoidPit.tscn")
				.Build())
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Hex hex = await AbilityCmd.SelectHex(state, list =>
					{
						list.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, range: 3)
							.Where(hex =>hex.GetHexObjectsOfType<Obstacle>().Any(hexObject => !hexObject.CannotBeDestroyed)));
					});

					if(hex != null)
					{
						await hex.HexObjects.First(hexObject => hexObject is Obstacle).Destroy();
						//TODO: Produce 1
						
						state.SetPerformed();
						state.SetCustomValue(this, "DestroyedObstacleHex", hex);
					}
				})
				.Build()),
			
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditions(Conditions.Immobilize)
				.WithConditionalAbilityCheck(async state => await AbilityCmd.HasPerformedAbility(state, 0))
				.WithCustomGetTargets((state, list) =>
				{
					Hex hex = state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<Hex>(this, "DestroyedObstacleHex");
					list.AddRange(RangeHelper.GetFiguresInRange(hex, 1));
				})
				.Build())
		];
	}
}