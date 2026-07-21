using System.Collections.Generic;
using Godot;
using System.Linq;
using Fractural.Tasks;

public class InterplanarVoyage : StarslingerCardModel<InterplanarVoyage.CardTop, InterplanarVoyage.CardBottom>
{
	public override string Name => "Interplanar Voyage";
	public override int Level => 9;
	public override int Initiative => 24;
	protected override int AtlasIndex => 27;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.3611026f, 0.13470992f)))
				.WithPierce(2)
				.WithAOEPattern(new AOEPattern(
				[
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Yellow),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Yellow),
					new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Yellow),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Yellow),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthWest), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.SouthEast), AOEHexType.Red)
				]))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithCustomGetTargets((abilityState, list) =>
				{
					AttackAbility.State attackAbilityState = abilityState.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach(Hex yellowHex in attackAbilityState.GetYellowAOEHexes())
					{
						foreach(Figure figure in yellowHex.GetHexObjectsOfType<Figure>())
						{
							list.Add(figure);
						}
					}
				})
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					Figure swapped = await AbilityCmd.SelectFigure(state, list =>
					{
						GD.Print(attackAbilityState.UniqueTargetedFigures.Count);
						list.AddRange(attackAbilityState.UniqueTargetedFigures.Where(figure =>
							!figure.IsDead &&
							AbilityCmd.CanSwap(state.Performer, figure)));
					}, mandatory: false, hintText: () => "Choose an enemy to swap hexes with");
					if(swapped == null)
					{
						return;
					}

					if(await AbilityCmd.TrySwap(state, state.Performer, swapped))
					{
						state.SetPerformed();
					}
				})
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Room performerRoom = state.Performer.Hex.Room;

					if(performerRoom == null)
					{
						return;
					}

					Figure swapped = await AbilityCmd.SelectFigure(state, list =>
					{
						list.AddRange(performerRoom.Figures.Where(figure =>
							figure != state.Performer &&
							AbilityCmd.CanSwap(state.Performer, figure)));
					}, mandatory: false, hintText: () => "Choose a figure to swap hexes with");
					if(swapped == null)
					{
						return;
					}

					if(await AbilityCmd.TrySwap(state, state.Performer, swapped))
					{
						state.SetPerformed();
					}
				})
				.Build())
		];
	}
}