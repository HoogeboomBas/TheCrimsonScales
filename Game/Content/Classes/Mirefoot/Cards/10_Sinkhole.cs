using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Sinkhole : MirefootCardModel<Sinkhole.CardTop, Sinkhole.CardBottom>
{
	public override string Name => "Sinkhole";
	public override int Level => 1;
	public override int Initiative => 29;
	protected override int AtlasIndex => 10;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithRange(3)
				.WithConditions(Conditions.Immobilize)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add((Direction)0), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add((Direction)1), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add((Direction)2), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add((Direction)3), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add((Direction)4), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add((Direction)5), AOEHexType.Red),
				]))
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async abilityState =>
				{
					AttackAbility.State attackAbilityState = abilityState.ActionState.GetAbilityState<AttackAbility.State>(0);
					if(!attackAbilityState.Performed)
					{
						return;
					}

					List<Hex> possibleHexes = new List<Hex>();
					foreach(Hex aoeHex in attackAbilityState.GetRedAOEHexes())
					{
						if(aoeHex.IsFeatureless())
						{
							possibleHexes.Add(aoeHex);
						}
					}

					List<Hex> selectedHexes =
						await AbilityCmd.SelectHexes(abilityState, list => list.AddRange(possibleHexes), 0, possibleHexes.Count, true,
							"Place difficult terrain?");

					foreach(Hex selectedHex in selectedHexes)
					{
						await CreateDifficultTerrain(selectedHex);
						abilityState.SetPerformed();
					}
				})
				.Build())
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Subscribe(abilityState, this,
						canApplyParameters =>
							abilityState.Performer.AlliedWith(canApplyParameters.Performer) &&
							(canApplyParameters.Hex.HasHexObjectOfType<DifficultTerrain>()),
						applyParameters =>
						{
							if(applyParameters.Hex.HasHexObjectOfType<DifficultTerrain>())
							{
								applyParameters.SetMoveCost(1);
							}
						});

					return GDTask.CompletedTask;
				})
				.WithOnDeactivate(abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(abilityState, this);

					return GDTask.CompletedTask;
				})
				.Build())
		];

		protected override int XP => 2;
		protected override bool Round => true;
	}
}