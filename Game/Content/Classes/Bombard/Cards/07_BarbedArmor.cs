using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BarbedArmor : BombardCardModel<BarbedArmor.CardTop, BarbedArmor.CardBottom>
{
	public override string Name => "Barbed Armor";
	public override int Level => 1;
	public override int Initiative => 13;
	protected override int AtlasIndex => 7;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1, new RetaliateDiamondPlus(this, new Vector2(0.6193324f, 0.22017238f)))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterRetaliateEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.RetaliatingFigure == state.Performer &&
							canApplyParameters.DamageSuffered > 0,
						async parameters =>
						{
							await AbilityCmd.AddCondition(state, parameters.AbilityState.Performer, Conditions.Wound1);
						});
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AfterRetaliateEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(2, new ShieldDiamondPlus(this, new Vector2(0.6174878f, 0.7124995f)))
				.WithRequiredRangeType(RangeType.Melee)
				.Build())
		];

		public override bool Round => true;
	}
}