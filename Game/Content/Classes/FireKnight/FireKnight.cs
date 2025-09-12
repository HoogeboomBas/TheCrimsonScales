using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;

public partial class FireKnight : Character
{
	private FireKnightModel _fireKnightModel;

	public List<ItemModel> FireKnightItems { get; } = new List<ItemModel>();

	public Ladder Ladder { get; private set; }

	public bool PlacedLadder => Ladder.Hex != null;

	public override void Spawn(SavedCharacter savedCharacter, int index)
	{
		base.Spawn(savedCharacter, index);

		_fireKnightModel = (FireKnightModel)savedCharacter.ClassModel;

		// Copy over all items from the character
		foreach(ItemModel itemModel in _fireKnightModel.AllItems)
		{
			ItemModel item = itemModel.ToMutable();
			item.Init(this);
			item.SetOwner(null);
			FireKnightItems.Add(item);
		}
	}

	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		PackedScene scene = ResourceLoader.Load<PackedScene>("res://Content/Classes/FireKnight/Ladder.tscn");
		Ladder = scene.Instantiate<Ladder>();
		GameController.Instance.Map.AddChild(Ladder);
		await Ladder.Init(null, hexCanBeNull: true);

		Ladder.SetScale(Vector2.Zero);

		IconEffectButton.Parameters placeEffectButtonParameters = new IconEffectButton.Parameters("res://Content/Classes/FireKnight/cs-ladder.png");
		TextEffectInfoView.Parameters placeEffectInfoViewParameters = new TextEffectInfoView.Parameters(
			"Place the Ladder in an adjacent unoccupied hex adjacent to a wall, on top of a trap, obstacle, or hazardous terrain overlay tile.");
		SubscribeLadderAction(
			character =>
				character == this &&
				!PlacedLadder &&
				character.TakingTurn &&
				GetValidLadderPlacementHexes().Any(),
			PlaceLadder, placeEffectButtonParameters, placeEffectInfoViewParameters);

		IconEffectButton.Parameters removeEffectButtonParameters = new IconEffectButton.Parameters("res://Content/Classes/FireKnight/cs-ladder.png");
		TextEffectInfoView.Parameters removeEffectInfoViewParameters = new TextEffectInfoView.Parameters(
			"Remove the Ladder from an adjacent unoccupied hex.");
		SubscribeLadderAction(
			character =>
				character == this &&
				PlacedLadder &&
				character.TakingTurn &&
				Ladder.Hex.IsUnoccupied() &&
				RangeHelper.Distance(Hex, Ladder.Hex) == 1,
			RemoveLadder, removeEffectButtonParameters, removeEffectInfoViewParameters);
	}

	public void GiveItem(ItemModel itemModel, Character character)
	{
		ItemModel item = FireKnightItems.FirstOrDefault(item => item.ImmutableInstance == itemModel);
		if(item == null)
		{
			Log.Error($"Tried giving an item {itemModel.Name} that is no longer available.");
			return;
		}

		FireKnightItems.Remove(item);

		character.AddItem(item);
	}

	protected void SubscribeLadderAction(Func<Character, bool> canApply, Func<GDTask> apply, IconEffectButton.Parameters effectButtonParameters, TextEffectInfoView.Parameters effectInfoViewParameters)
	{
		object subscriber = new object();

		ScenarioEvents.CardSideSelectionEvent.Subscribe(this, subscriber,
			canApplyParameters => canApply(canApplyParameters.Character),
			async applyParameters =>
			{
				await apply();
			},
			EffectType.Selectable,
			order: 0,
			canApplyMultipleTimesInEffectCollection: true,
			effectButtonParameters: effectButtonParameters,
			effectInfoViewParameters: effectInfoViewParameters);

		ScenarioEvents.AfterCardsPlayedEvent.Subscribe(this, subscriber,
			canApplyParameters => canApply(canApplyParameters.Character),
			async applyParameters =>
			{
				await apply();
			},
			EffectType.Selectable,
			order: 0,
			canApplyMultipleTimesInEffectCollection: true,
			effectButtonParameters: effectButtonParameters,
			effectInfoViewParameters: effectInfoViewParameters);

		ScenarioEvents.LongRestCardSelectionEvent.Subscribe(this, subscriber,
			canApplyParameters => canApply(canApplyParameters.Character),
			async applyParameters =>
			{
				await apply();
			},
			EffectType.Selectable,
			order: 0,
			canApplyMultipleTimesInEffectCollection: true,
			effectButtonParameters: effectButtonParameters,
			effectInfoViewParameters: effectInfoViewParameters);

		ScenarioEvents.DuringMovementEvent.Subscribe(this, subscriber,
			canApplyParameters => canApplyParameters.Performer is Character character && canApply(character),
			async applyParameters =>
			{
				await apply();
			},
			EffectType.Selectable,
			order: 0,
			canApplyMultipleTimesInEffectCollection: true,
			effectButtonParameters: effectButtonParameters,
			effectInfoViewParameters: effectInfoViewParameters);
	}

	private async GDTask PlaceLadder()
	{
		Hex hex = await AbilityCmd.SelectHex(this, list =>
		{
			//list.AddRange(RangeHelper.GetHexesInRange(Hex, 1, false));
			list.AddRange(GetValidLadderPlacementHexes());
		}, mandatory: true, hintText: "Select a hex to place the Ladder in");

		Ladder.SetOriginHexAndRotation(hex);
		Ladder.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardable();
	}

	private async GDTask RemoveLadder()
	{
		Ladder.RemoveFromMap();
		Ladder.TweenScale(0f, 0.15f).SetEasing(Easing.InBack).PlayFastForwardable();

		await GDTask.CompletedTask;
	}

	private IEnumerable<Hex> GetValidLadderPlacementHexes()
	{
		foreach(Hex hex in RangeHelper.GetHexesInRange(Hex, 1, false))
		{
			if(!hex.IsUnoccupied())
			{
				continue;
			}

			if(hex.Neighbours.Count < 6 || hex.HasHexObjectOfType<Obstacle>() || hex.HasHexObjectOfType<HazardousTerrain>() || hex.HasHexObjectOfType<Trap>())
			{
				yield return hex;
			}
		}
	}
}