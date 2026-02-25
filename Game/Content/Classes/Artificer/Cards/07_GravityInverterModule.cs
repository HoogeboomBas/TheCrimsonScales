using System.Collections.Generic;
using Godot;

public class GravityInverterModule : ArtificerCardModel<GravityInverterModule.CardTop, GravityInverterModule.CardBottom>
{
	public override string Name => "Gravity Inverter Module";
	public override int Level => 1;
	public override int Initiative => 20;
	protected override int AtlasIndex => 7;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.4474074f, 0.22804232f)))
				.WithPush(1, new PushSquare(this, new Vector2(0.66888887f, 0.22804232f)))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					AttackAbility.State attackState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					foreach(Figure figure in attackState.UniqueTargetedFigures)
					{
						await AbilityCmd.AddCharacterToken(state, figure,
							$"This figure no longer has {Icons.Inline(Icons.Flying)} until the end of the round.");
					}

					ScenarioCheckEvents.FlyingCheckEvent.Subscribe(state, this,
						parameters => attackState.UniqueTargetedFigures.Contains(parameters.Figure),
						parameters =>
						{
							parameters.SetFlying(false);
						}, 100);
				})
				.WithOnDeactivate(async state =>
				{
					AttackAbility.State attackState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					ScenarioCheckEvents.FlyingCheckEvent.Unsubscribe(state, this);
					foreach(Figure figure in attackState.UniqueTargetedFigures)
					{
						await AbilityCmd.RemoveCharacterToken(state, figure);
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithMandatory(true)
				.Build())
		];

		public override int XP => 1;
		public override bool Round => true;
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.51703703f, 0.7608465f)))
				.WithMoveType(MoveType.Jump)
				.Build())
		];
	}
}