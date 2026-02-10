using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SupremeAuthority : AmberAegisCardModel<SupremeAuthority.CardTop, SupremeAuthority.CardBottom>
{
	public override string Name => "Supreme Authority";
	public override int Level => 9;
	public override int Initiative => 03;
	protected override int AtlasIndex => 29;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							await MoveColonyToken(state, 3);
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.HexObjectDestroyedEvent.Subscribe(state, this,
						parameters => parameters.HexObject is ColonyToken,
						async parameters =>
						{
							Figure figure = await AbilityCmd.SelectFigure(state.Performer,
								list => list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 3)
									.Where(figure => figure.EnemiesWith(state.Performer))),
								hintText: () =>
									$"Select an enemy to gain {Icons.HintText(Icons.GetCondition(Conditions.Wound1))}, {Icons.HintText(Icons.GetCondition(Conditions.Poison1))}, and {Icons.HintText(Icons.GetCondition(Conditions.Muddle))}");
							if(figure == null)
							{
								return;
							}
							await AbilityCmd.AddConditions(state, figure, [Conditions.Wound1, Conditions.Poison1, Conditions.Muddle]);
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.HexObjectDestroyedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					ColonyToken colonyToken = await PlaceAnyColonyToken(state, list => list.AddRange(RangeHelper
						.GetHexesInRange(state.Performer.Hex, 1)
						.Where(hex => hex.IsEmpty() && !hex.HasHexObjectOfType<ColonyToken>())));
					state.SetCustomValue(this, "ColonyToken", colonyToken);
				})
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Ward, new ConditionDiamondPlus(this, new Vector2(0.25831854f, 0.8322751f)))
				.WithTarget(Target.TargetAll | Target.SelfOrAllies)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(RangeHelper.GetFiguresInRange(
						state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<ColonyToken>(this, "ColonyToken").Hex, 3));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}
}