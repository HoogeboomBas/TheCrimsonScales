using System.Collections.Generic;

public class InnerReflection : HierophantCardModel<InnerReflection.CardTop, InnerReflection.CardBottom>
{
	public override string Name => "Inner Reflection";
	public override int Level => 1;
	public override int Initiative => 53;
	protected override int AtlasIndex => 13 - 5;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithRange(3)
				.WithPierce(3)
				.WithConditions(Conditions.Wound1)
				.Build())
		];

		protected override int XP => 1;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(2)
				.WithOnAbilityEnded(async state =>
				{
					List<Figure> targetedFigures = new List<Figure>();
					for(int i = 0; i < state.LootedCoinCount; i++)
					{
						Figure figure = await AbilityCmd.SelectFigure(state, list =>
						{
							foreach(Figure otherFigure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 2))
							{
								if(state.Performer.AlliedWith(otherFigure))
								{
									list.Add(otherFigure);
								}
							}
						}, autoSelectIfOne: false, hintText: "Select a character ally to receive a coin");

						if(figure != null)
						{
							targetedFigures.Add(figure);

							state.Performer.RemoveCoin();
							figure.AddCoin();

							await GivePrayerCard(state, figure);
						}
					}

					if(targetedFigures.Count == 1)
					{
						await GivePrayerCard(state, targetedFigures[0]);
					}
				})
				.Build())
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}
}