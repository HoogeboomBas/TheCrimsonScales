using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class GlisteningFacets : ShardrenderCardModel<GlisteningFacets.CardTop, GlisteningFacets.CardBottom>
{
	public override string Name => "Glistening Facets";
	public override int Level => 4;
	public override int Initiative => 58;
	protected override int AtlasIndex => 18;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.39511883f, 0.23933518f)))
				.WithTargets(2)
				.WithRange(2)
				.WithPull(1)
				.WithDuringAttackSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringAttack.Parameters>(async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Muddle);

						await AbilityCmd.GainXP(parameters.Performer, 1);
					}, new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Muddle)))))
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.5239462f, 0.65177757f)))
				.WithMoveType(MoveType.Jump)
				.WithAbilityPerformedSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.AbilityPerformed.Parameters>(async parameters =>
					{
						Figure figure = await AbilityCmd.SelectFigure(parameters.AbilityState, figures =>
						{
							figures.AddRange(((MoveAbility.State)parameters.AbilityState).Hexes.SelectMany(hex => hex.GetFigures())
								.Where(figure => parameters.Performer.EnemiesWith(figure)).Distinct());
						});
						if(figure != null)
						{
							await AbilityCmd.AddCondition(parameters.AbilityState, figure, Conditions.Immobilize);
						}

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"One enemy moved through gains {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}")))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					List<Hex> hexes = await AbilityCmd.SelectHexes(state, hexes =>
						{
							hexes.AddRange(state.ActionState.GetAbilityState<MoveAbility.State>(0).Hexes
								.Where(hex => hex.HexObjects.Any(hexObject =>
									hexObject is LootableObject lootableObject && lootableObject.CanLoot(state.Performer))));
						}, 0, 2, true, hintText: $"{Icons.HintText(Icons.Loot)} up to two hexes entered");

					foreach(Hex hex in hexes)
					{
						await AbilityCmd.LootHex(state.Performer, hex);
						state.SetPerformed();
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}
}