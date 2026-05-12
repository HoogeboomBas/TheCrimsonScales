using System.Collections.Generic;
using Godot;

public class DimmedDefense : SpiritCallerCardModel<DimmedDefense.CardTop, DimmedDefense.CardBottom>
{
	public override string Name => "Dimmed Defense";
	public override int Level => 1;
	public override int Initiative => 12;
	protected override int AtlasIndex => 28 - 2;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Wall of Shadows")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/wall_of_shadows.png")
				.WithHealth(2)
				.WithTraits(new ShieldAuraTrait(1, 1, false))
				.Build()
			)
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62167174f, 0.6397274f)))
				.Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Air))
				.Build()),

			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1)
				.WithRange(2)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark))
				.Build()),
		];

		public override bool Round => true;
	}
}