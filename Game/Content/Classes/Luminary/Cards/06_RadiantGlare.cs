using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class RadiantGlare : LuminaryCardModel<RadiantGlare.CardTop, RadiantGlare.CardBottom>
{
	public override string Name => "Radiant Glare";
	public override int Level => 1;
	public override int Initiative => 36;
	protected override int AtlasIndex => 6;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GlowActiveAbility.Builder()
				.WithGlowAbility(new GlowAbilityModel([Element.Light], GlowAbility,
					$"Perform {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))} ability", Icons.GetCondition(Conditions.Immobilize)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
		public override int XP => 1;
		public override bool Persistent => true;

		private Ability GlowAbility(List<Element> elements)
		{
			return ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					]
				))
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
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62128437f, 0.72183055f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
	}
}