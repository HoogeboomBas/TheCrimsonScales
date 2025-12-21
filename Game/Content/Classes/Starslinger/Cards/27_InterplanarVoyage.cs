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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
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

					await AbilityCmd.TrySwap(state, state.Performer, swapped);
				})
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Room performerRoom = null;
					foreach(Room room in GameController.Instance.Map.Rooms)
					{
						if(room.MapTiles.Contains(state.Performer.Hex.MapTile))
						{
							performerRoom = room;
							break;
						}
					}

					if(performerRoom == null)
					{
						return;
					}

					Figure swapped = await AbilityCmd.SelectFigure(state, list =>
					{
						list.AddRange(performerRoom.Hexes.SelectMany(hex => hex.GetChildrenOfType<Figure>()).Where(figure =>
							figure != state.Performer &&
							AbilityCmd.CanSwap(state.Performer, figure)));
					}, mandatory: false, hintText: () => "Choose a figure to swap hexes with");
					if(swapped == null)
					{
						return;
					}

					await AbilityCmd.TrySwap(state, state.Performer, swapped);
				})
				.Build())
		];
	}
}