using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class EncasedInThorns : ThornreaperCardModel<EncasedInThorns.CardTop, EncasedInThorns.CardBottom>
{
	public override string Name => "Impetuous Inquisition";
	public override int Level => 1;
	public override int Initiative => 28;
	protected override int AtlasIndex => 13 - 2;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(2, new RetaliateSquare(this, new Vector2(0.2f, 0.3f)))
				.WithRange(2)
				.Build())
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Subscribe(state, this,
						canApply: canApplyParameters =>
						{
							return
								canApplyParameters.SufferDamageParameters.FromAttack &&
								state.Performer.EnemiesWith(canApplyParameters.SufferDamageParameters.PotentialAbilityState.Performer) &&
								state.Performer.AlliedWith(canApplyParameters.SufferDamageParameters.Figure) &&
								canApplyParameters.DamageSuffered >= 3;
						},
						apply: async applyParameters =>
						{
							await AbilityCmd.SufferDamage(state, applyParameters.PotentialAbilityState.Performer, 2);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
					[
						RetaliateAbility.Builder().WithRetaliateValue(1).Build()
					]
				)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
				.Build())
		];

		public override int XP => 1;
		public override bool Round => true;
		public override bool Loss => true;
	}
}