using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class AwnusRetribution : IncarnateCardModel<AwnusRetribution.CardTop, AwnusRetribution.CardBottom>
{
	public override string Name => "Awnu's Retribution";
	public override int Level => 6;
	public override int Initiative => 19;
	protected override int AtlasIndex => 23;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Hex hex = await AbilityCmd.SelectHex(state, hexes =>
						{
							hexes.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 2)
								.Where(hex => hex.GetFigures().Any(figure => state.Performer.EnemiesWith(figure))));
						}, hintText: $"Designate one hex within {Icons.HintText(Icons.Range)}2 occupied by an enemy");

					if(hex == null)
					{
						return;
					}

					state.SetCustomValue(this, "Hex", hex);

					Figure figure = hex.GetFigures().First();

					if(InSpirit(state.Performer, IncarnateSpirit.Ritualist))
					{
						await AbilityCmd.AddConditions(state, figure, [Incarnate.Enfeeble, Incarnate.Enfeeble, Incarnate.Enfeeble]);
					}
					else
					{
						await AbilityCmd.AddCondition(state, figure, Incarnate.Enfeeble);
					}

					await AbilityCmd.SufferDamage(state, figure, 3);

					state.SetPerformed();
				})
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(
						RangeHelper.GetFiguresInRange(state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<Hex>(this, "Hex"), 1,
							false));
				})
				.WithConditionalAbilityCheck(async state =>
					await AbilityCmd.HasPerformedAbility(state, 0) && InSpirit(state.Performer, IncarnateSpirit.Reaver))
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						parameters => parameters.AMDCard.Model is EnfeebleAMDCard && state.Performer.EnemiesWith(parameters.Performer),
						async parameters =>
						{
							await AbilityCmd.SufferDamage(state, parameters.Performer, 1);
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Conqueror];
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];
		public override bool Persistent => true;
	}
}