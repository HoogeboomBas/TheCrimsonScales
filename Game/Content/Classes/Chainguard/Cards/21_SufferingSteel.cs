using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SufferingSteel : ChainguardLevelUpCardModel<SufferingSteel.CardTop, SufferingSteel.CardBottom>
{
	public override string Name => "Suffering Steel";
	public override int Level => 6;
	public override int Initiative => 09;
	protected override int AtlasIndex => 15 - 8;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(4, new RetaliateDiamondPlus(this, new Vector2(0.61210763f, 0.20048997f)))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						canApply: canApplyParameters => canApplyParameters.FromAttack &&
						                                canApplyParameters.Figure == state.Performer &&
						                                canApplyParameters.PotentialAbilityState.Performer.HasCondition(Chainguard.Shackle),
						async applyParameters =>
						{
							applyParameters.SetDamagePrevented();

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Round => true;
		public override bool Loss => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62026906f, 0.7225138f)))
				.Build()),

			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(2, new RetaliateDiamondPlus(this, new Vector2(0.6131678f, 0.83107513f)))
				.Build()),
		];

		public override bool Round => true;
	}
}