using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class GalvanicCoil : ArtificerCardModel<GalvanicCoil.CardTop, GalvanicCoil.CardBottom>
{
	public override string Name => "Galvanic Coil";
	public override int Level => 6;
	public override int Initiative => 56;
	protected override int AtlasIndex => 21;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && HasXScrapTokens(state.Performer, 1),
						async parameters =>
						{
							LoseScrapTokens(parameters.Figure);
							Hex hex = await AbilityCmd.SelectHex(state,
								hexes => hexes.AddRange(GameController.Instance.Map.Hexes.Values.Where(hex => hex.HasHexObjectOfType<Trap>())),
								hintText:
								$"Select a trap to perform the attack ability from");
							if(hex == null)
							{
								return;
							}

							//TODO: Set PerformHex (requires something else)
							await new ActionState(parameters.Figure,
								[AttackAbility.Builder().WithDamage(3).WithRange(3).WithRangeType(RangeType.Melee).WithPierce(3).Build()]).Perform();
						}, EffectType.Selectable,
						effectButtonParameters: new TextEffectButton.Parameters($"1{Icons.HintText(Artificer.ScrapToken)}"),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform {Icons.Inline(Icons.Attack)}3, {Icons.Inline(Icons.Targets)}1 enemy within 3 hexes, {Icons.Inline(Icons.Pierce)}3 from any trap"));
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.WithOnAbilityStarted(async state =>
				{
					await AbilityCmd.GenericChoice(state.Performer,
					[
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async _ =>
							{
								state.SetCustomValue(this, "ChoseMove", true);
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new TextEffectButton.Parameters($"{Icons.HintText(Icons.Move)}"),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								$"Perform {Icons.Inline(Icons.Move)}4"),
							effectType: EffectType.SelectableMandatory
						),
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async _ =>
							{
								state.SetBlocked();
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new TextEffectButton.Parameters($"{Icons.HintText(Icons.Damage)}"),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								$"Create one {Icons.Inline(Icons.Damage)}4 trap in an adjacent empty hex"),
							effectType: EffectType.SelectableMandatory
						)
					], hintText: "Select an ability to perform:");
				})
				.Build()),
			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(4)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return !state.ActionState.GetAbilityState<MoveAbility.State>(0).GetCustomValue<bool>(this, "ChoseMove");
				})
				.Build())
		];
	}
}