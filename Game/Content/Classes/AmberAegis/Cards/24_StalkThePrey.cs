using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class StalkThePrey : AmberAegisCardModel<StalkThePrey.CardTop, StalkThePrey.CardBottom>
{
	public override string Name => "Stalk the Prey";
	public override int Level => 7;
	public override int Initiative => 79;
	protected override int AtlasIndex => 24;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PlaceColonyTokenAbility<DeathshroudSpiderColony>()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters => IsAdjacentToColonyToken<DeathshroudSpiderColony>(parameters.Hex) &&
						              parameters.Figure.EnemiesWith(state.Performer),
						async parameters =>
						{
							await AbilityCmd.SufferDamage(state, parameters.Figure, 1);
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override string CustomTag => "Cultivate";
		public override IEnumerable<Element> Elements => [Element.Fire, Element.Earth];
		public override bool Persistent => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Venator Tarantula")
				.WithTexturePath("res://Content/Classes/AmberAegis/venator_tarantula.png")
				.WithHealth(2, new SummonHealthSquare(this, new Vector2(0.44666666f, 0.67619044f)))
				.WithMove(3, new SummonMoveSquare(this, new Vector2(0.67777777f, 0.67619044f)))
				.WithAttack(1)
				.WithTraits(new ApplyConditionTrait(Conditions.Poison1), new ApplyConditionTrait(Conditions.Immobilize))
				.WithCount(2)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					List<Summon> summons = state.ActionState.GetAbilityState<SummonAbility.State>(0).Summons;

					ScenarioEvents.JustBeforeSufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure is Summon summon && summons.Contains(summon) && parameters.WouldSufferDamage,
						async parameters =>
						{
							parameters.SetPrevented();
							await AbilityCmd.SufferDamage(parameters.PotentialAbilityState, state.Performer, parameters.Damage);
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters("res://Content/Classes/AmberAegis/venator_tarantula.png"),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Have {state.Performer.DebugName} suffer the {Icons.Inline(Icons.Damage)} instead"));
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.JustBeforeSufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}