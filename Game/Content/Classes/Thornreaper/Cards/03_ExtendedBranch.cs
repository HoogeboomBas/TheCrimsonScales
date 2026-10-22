using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ExtendedBranch : ThornreaperCardModel<ExtendedBranch.CardTop, ExtendedBranch.CardBottom>
{
	public override string Name => "Extended Branch";
	public override int Level => 1;
	public override int Initiative => 61;
	protected override int AtlasIndex => 8;

	public class CardTop : ThornreaperCardSide
	{
		string _hintText = $"{Icons.HintText(Icons.Attack)}2";

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackSquare(this, new Vector2(0.5f, 0.6f)))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero,AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
				]),
				new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest), this, new Vector2(0.5f, 0.4f)),
				new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East).Add(Direction.East), this, new Vector2(0.5f, 0.4f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => GetLightElementState() is ElementState.Strong or ElementState.Waning,
						async parameters =>
						{
							await AbilityCmd.GainXP(parameters.Performer, 1);
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light);
							parameters.AbilityState.SingleTargetAttackValue += 1;
						}
					)
				).WithGetTargetingHintText(state => 
					{			
						if (GetLightElementState() is ElementState.Strong or ElementState.Waning)
						{
							return $"Select a target for {Icons.HintText(Icons.Attack)}3";
						}
						else
						{
							return $"Select a target for {Icons.HintText(Icons.Attack)}2";
						}	
					})
				.Build())
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Subscribe(abilityState, this,
						canApplyParameters =>
							abilityState.Performer.AlliedWith(canApplyParameters.Performer) &&
							(canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>() && canApplyParameters.Hex.Creator == abilityState.Performer),
						applyParameters =>
						{
							if(applyParameters.Hex.HasHexObjectOfType<HazardousTerrain>())
							{
								applyParameters.SetAffectedByNegativeHex(false);
							}
						});

					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
						canApplyParameters => abilityState.Performer.AlliedWith(canApplyParameters.PotentialAbilityState?.Performer) && canApplyParameters.Hex.Creator == abilityState.Performer,
						applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							return GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(abilityState, this);
					ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];
		public override bool Loss => true;
		public override int XP => 1;
		public override bool Persistent => true;
	}
}