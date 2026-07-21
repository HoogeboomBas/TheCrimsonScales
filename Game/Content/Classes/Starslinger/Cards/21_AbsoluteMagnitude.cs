using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AbsoluteMagnitude : StarslingerCardModel<AbsoluteMagnitude.CardTop, AbsoluteMagnitude.CardBottom>
{
	public override string Name => "AbsoluteMagnitude";
	public override int Level => 6;
	public override int Initiative => 20;
	protected override int AtlasIndex => 21;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.44791222f, 0.18584071f)))
				.WithPush(3, new PushSquare(this, new Vector2(0.66871876f, 0.18584071f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					Figure swapped = await AbilityCmd.SelectFigure(state, list =>
					{
						list.AddRange(attackAbilityState.UniqueTargetedFigures.Where(figure =>
							!figure.IsDead &&
							AbilityCmd.CanSwap(state.Performer, figure)));
					}, mandatory: false, hintText: () => "Choose an enemy to swap hexes with");
					if(swapped == null)
					{
						return;
					}

					if(await AbilityCmd.TrySwap(state, state.Performer, swapped))
					{
						await AbilityCmd.GainXP(state.Performer, 1);
						state.SetPerformed();
					}
				})
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(3)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithRange(5)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					foreach(SingleTargetState singleTargetState in state.ActionState.GetAbilityState<PushAbility.State>(0).SingleTargetStates)
					{
						await AbilityCmd.SufferDamage(state, singleTargetState.Target, singleTargetState.PushHexes.Count);
					}

					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<PushAbility.State>(0).SingleTargetStates
						.Any(singleTargetState => singleTargetState.PushHexes.Count > 0);
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}