using System.Collections.Generic;
using System.Linq;
using Godot;

public class AmassedFormation : ShardrenderCardModel<AmassedFormation.CardTop, AmassedFormation.CardBottom>
{
	public override string Name => "Amassed Formation";
	public override int Level => 5;
	public override int Initiative => 16;
	protected override int AtlasIndex => 20;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					AbilityCard abilityCard = await AbilityCmd.SelectAbilityCard((Character)state.Performer, CardState.Lost,
						canSelectFunc: abilityCard =>
							abilityCard.Top.Model.Abilities.Any(abilityCardAbility => abilityCardAbility.Ability is CrystallizeAbility) ||
							abilityCard.Bottom.Model.Abilities.Any(abilityCardAbility => abilityCardAbility.Ability is CrystallizeAbility),
						hintText: $"Select a card with a {Icons.HintText(CrystallizeIconPath)} action to play");
					if(abilityCard != null)
					{
						if(abilityCard.Top.Model.Abilities.Any(abilityCardAbility => abilityCardAbility.Ability is CrystallizeAbility))
						{
							await abilityCard.Top.Perform(state.Performer);
						}
						else
						{
							await abilityCard.Bottom.Perform(state.Performer);
						}

						state.SetPerformed();
					}
				})
				.Build()),
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(3)
				.Build())
		];

		public override int XP => 2;
		public override bool Round => true;
		public override bool Loss => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62163085f, 0.77063715f)))
				.Build()),
			new AbilityCardAbility(CrystallizeAbility.Builder()
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29180175f, 0.87423825f)),
					new UseSlot(new Vector2(0.49823594f, 0.87313026f)),
					new UseSlot(new Vector2(0.7062223f, 0.87257624f))
				])
				.WithDiscardOtherCrystallize(false)
				.Build())
		];

		public override bool Persistent => true;
	}
}