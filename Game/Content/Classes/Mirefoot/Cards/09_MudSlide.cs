using System.Collections.Generic;
using Fractural.Tasks;

public class Mudslide : MirefootCardModel<Mudslide.CardTop, Mudslide.CardBottom>
{
	public override string Name => "Mudslide";
	public override int Level => 1;
	public override int Initiative => 18;
	protected override int AtlasIndex => 9;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithTargets(2)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.Hex.HasHexObjectOfType<DifficultTerrain>(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Muddle);
							await GDTask.CompletedTask;
						})
				)
				.Build())
		];

		protected override int XP => 1;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async abilityState =>
				{
					Hex hex = abilityState.Performer.Hex;

					List<Hex> selectedHexes = new List<Hex>();
					if(hex.IsFeatureless())
					{
						selectedHexes.AddRange(
							await AbilityCmd.SelectHexes(abilityState, list => list.Add(hex), 0, 1, true, "Place difficult terrain in occupied hex"));
					}

					List<Hex> possibleHexes = new List<Hex>();
					for(int i = 0; i < 6; i++)
					{
						Hex possibleHex = GameController.Instance.Map.GetHex(hex.Coords.Add((Direction)i));
						if(possibleHex != null && possibleHex.IsFeatureless())
						{
							possibleHexes.Add(possibleHex);
						}
					}

					selectedHexes.AddRange(
						await AbilityCmd.SelectHexes(abilityState, list => list.AddRange(possibleHexes), 0, 2, false,
							"Place difficult terrain in up to two adjacent hexes"));

					foreach(Hex selectedHex in selectedHexes)
					{
						await CreateDifficultTerrain(selectedHex);
						abilityState.SetPerformed();
					}
				})
				.Build())
		];
	}
}