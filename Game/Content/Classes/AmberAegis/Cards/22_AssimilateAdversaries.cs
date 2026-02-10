using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class AssimilateAdversaries : AmberAegisCardModel<AssimilateAdversaries.CardTop, AssimilateAdversaries.CardBottom>
{
	public override string Name => "Assimilate Adversaries";
	public override int Level => 6;
	public override int Initiative => 50;
	protected override int AtlasIndex => 22;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Bane)
				.WithOnAbilityStarted(async state =>
				{
					List<Hex> hexes = await AbilityCmd.SelectHexes(state,
						list => list.AddRange(GameController.Instance.Map.Hexes.Values.Where(hex => hex.HasHexObjectOfType<ColonyToken>())), 0,
						2, false, hintText: $"Select two {Icons.HintText(ColonyToken.AnyColony)} to destroy");
					//TODO: Change to selecting the overlay tiles themselves
					List<ColonyToken> colonyTokens = hexes.Select(hex => hex.GetHexObjectOfType<ColonyToken>()).ToList();
					foreach(ColonyToken colonyToken in colonyTokens)
					{
						await colonyToken.Destroy();
					}

					if(colonyTokens.Count < 2)
					{
						state.SetBlocked();
						return;
					}

					state.SetCustomValue(this, "DestroyedColonies", colonyTokens);
				})
				.WithCustomGetTargets((state, figures) =>
				{
					foreach(ColonyToken colonyToken in state.GetCustomValue<List<ColonyToken>>(this, "DestroyedColonies"))
					{
						figures.AddRange(RangeHelper.GetFiguresInRange(colonyToken.Hex, 1).Where(figure =>
							figure is Monster monster && monster.MonsterType is MonsterType.Normal or MonsterType.Elite));
					}
				})
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure target = state.ActionState.GetAbilityState<ConditionAbility.State>(0).Target;
					await AbilityCmd.AddCharacterToken(state, target,
						$"When this figure dies, place one {Icons.Inline(ColonyToken.AnyColony)} of your choice in or adjacent to the hex they occupied.");
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						parameters => parameters.Figure == target,
						async parameters =>
						{
							await PlaceAnyColonyToken(state, list => list.AddRange(RangeHelper.GetHexesInRange(parameters.Figure.Hex, 1)
								.Where(hex => hex.IsEmpty() && !hex.HasHexObjectOfType<ColonyToken>())));
						});
				})
				.WithOnDeactivate(async state =>
				{
					await AbilityCmd.RemoveCharacterToken(state, state.ActionState.GetAbilityState<ConditionAbility.State>(0).Target);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);
				})
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						parameters => parameters.PotentialKiller == state.Performer && parameters.Figure.EnemiesWith(state.Performer),
						async parameters =>
						{
							ActionState actionState = new ActionState(state.Performer,
								[HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).Build()]);
							await actionState.Perform();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}