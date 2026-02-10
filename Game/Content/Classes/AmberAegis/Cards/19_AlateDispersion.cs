using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AlateDispersion : AmberAegisCardModel<AlateDispersion.CardTop, AlateDispersion.CardBottom>
{
	public override string Name => "Alate Dispersion";
	public override int Level => 4;
	public override int Initiative => 58;
	protected override int AtlasIndex => 19;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PlaceColonyTokenAbility<RockspineTermiteColony>()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => IsAdjacentToColonyToken<RockspineTermiteColony>(parameters.Performer) &&
						              parameters.Performer.AlliedWith(state.Performer, true),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustPush(2);
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Muddle);
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override string CustomTag => "Cultivate";
		public override IEnumerable<Element> Elements => [Element.Earth];
		public override bool Persistent => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder()
						.WithDistance(4, new MoveCircle(this, new Vector2(0.524285f, 0.7410988f)))
						.WithMoveType(MoveType.Jump)
						.Build()
				])
				.WithOnAbilityStarted(async state =>
				{
					Hex hex = await AbilityCmd.SelectHex(state,
						list => list.AddRange(GameController.Instance.Map.Hexes.Values.Where(hex => hex.HasHexObjectOfType<ColonyToken>())),
						true,
						$"Designate a {Icons.HintText(ColonyToken.AnyColony)}");
					//TODO: Change to selecting overlay tile
					if(hex == null)
					{
						state.SetBlocked();
						return;
					}

					state.SetCustomValue(this, "DesignatedColony", hex.GetHexObjectOfType<ColonyToken>());
				})
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(RangeHelper.GetFiguresInRange(state.GetCustomValue<ColonyToken>(this, "DesignatedColony").Hex, 1));
				})
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					ColonyToken colonyToken = state.ActionState.GetAbilityState<GrantAbility.State>(0)
						.GetCustomValue<ColonyToken>(this, "DesignatedColony");
					await colonyToken.Destroy();
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override int XP => 1;
	}
}