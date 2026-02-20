using System.Collections.Generic;
using Fractural.Tasks;

public class LightTheWay : LuminaryCardModel<LightTheWay.CardTop, LightTheWay.CardBottom>
{
	public override string Name => "Light the Way";
	public override int Level => 9;
	public override int Initiative => 93;
	protected override int AtlasIndex => 29;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GlowActiveAbility.Builder()
				.WithGlowAbility(new GlowAbilityModel([Element.Ice, Element.Dark], GlowAbility,
					"Summon Gleaming Squid", "Summon"))
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;

		private Ability GlowAbility(List<Element> elements)
		{
			return SummonAbility.Builder()
				.WithName("Gleaming Squid")
				.WithTexturePath("res://Content/Classes/Luminary/GleamingSquid.png")
				.WithHealth(3)
				.WithMove(3)
				.WithAttack(2)
				.WithTraits(
					new PierceTrait(2),
					new InfuseElementAfterAttackTrait(Element.Light)
				)
				.WithOnAbilityStarted(async state =>
				{
					state.SetCustomValue(state.Performer, "Glow Ability", true);
					state.SetCustomValue(state.Performer, "Consumed Elements", elements);

					await GDTask.CompletedTask;
				})
				.Build();
		}
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && parameters.Figure.TurnMovedHexes.Count >= 4,
						async parameters =>
						{
							await AbilityCmd.InfuseElement(state, [Element.Fire, Element.Light]);
						}, EffectType.Selectable,
						effectButtonParameters: new TextEffectButton.Parameters(
							$"{Icons.Inline(Icons.GetElement(Element.Fire))} or {Icons.Inline(Icons.GetElement(Element.Light))}"),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"{Icons.Inline(Icons.GetElement(Element.Fire))} or {Icons.Inline(Icons.GetElement(Element.Light))}"));

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.InfuseWild()];
		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}