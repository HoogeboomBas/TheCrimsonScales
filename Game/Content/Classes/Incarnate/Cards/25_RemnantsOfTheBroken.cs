using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class RemnantsOfTheBroken : IncarnateCardModel<RemnantsOfTheBroken.CardTop, RemnantsOfTheBroken.CardBottom>
{
	public override string Name => "Remnants of the Broken";
	public override int Level => 7;
	public override int Initiative => 67;
	protected override int AtlasIndex => 25;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(0)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(RangeHelper.GetFiguresInRange(state.GetCustomValue<Figure>(this, "Figure").Hex, 1));
				})
				//TODO: Have a better way to do this than relying on the visual Shield Check Event
				.WithConditionalAbilityCheck(async state =>
				{
					Figure figure = await AbilityCmd.SelectFigure(state, figures =>
					{
						figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1)
							.Where(figure => state.Performer.EnemiesWith(figure)));
					}, hintText: () => "Designate one adjacent enemy");

					if(figure == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Figure", figure);
					state.AdjustAbilityDamage(ScenarioCheckEvents.ShieldCheckEvent
						.Fire(new ScenarioCheckEvents.ShieldCheck.Parameters(state.GetCustomValue<Figure>(this, "Figure"))).Shield);
					return true;
				})
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6, new AttackDiamond(this, new Vector2(0.30809918f, 0.3795014f)))
				.WithCustomGetTargets((state, figures) =>
				{
					figures.Add(state.ActionState.GetAbilityState<SufferDamageAbility.State>(0).GetCustomValue<Figure>(this, "Figure"));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.41467288f, 0.7218837f)))
				.WithDuringMovementSubscription(
					InSpiritSubscription<ScenarioEvents.DuringMovement.Parameters>(IncarnateSpirit.Ritualist,
						async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(2);
							parameters.AbilityState.AdjustMoveType(MoveType.Jump);

							await GDTask.CompletedTask;
						}))
				.Build()),
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Conqueror, IncarnateSpirit.Reaver];
	}
}