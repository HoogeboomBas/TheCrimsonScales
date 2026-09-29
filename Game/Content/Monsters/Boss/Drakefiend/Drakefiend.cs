using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public class Drakefiend : MonsterModel, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
	[
		new MonsterStats()
		{
			Health = 11 * CharacterCount,
			Attack = 3,
			Traits =
			[
				ConditionImmunityTrait.WoundImmunityTrait(), ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Immobilize),
				new ConditionImmunityTrait(Conditions.Stun), new ForcedMovementImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 12 * CharacterCount,
			Attack = 4,
			Traits =
			[
				ConditionImmunityTrait.WoundImmunityTrait(), ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Immobilize),
				new ConditionImmunityTrait(Conditions.Stun), new ForcedMovementImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 15 * CharacterCount,
			Attack = 4,
			Traits =
			[
				ConditionImmunityTrait.WoundImmunityTrait(), ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Immobilize),
				new ConditionImmunityTrait(Conditions.Stun), new ForcedMovementImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 16 * CharacterCount,
			Attack = 5,
			Traits =
			[
				ConditionImmunityTrait.WoundImmunityTrait(), ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Immobilize),
				new ConditionImmunityTrait(Conditions.Stun), new ForcedMovementImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 20 * CharacterCount,
			Attack = 5,
			Traits =
			[
				ConditionImmunityTrait.WoundImmunityTrait(), ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Immobilize),
				new ConditionImmunityTrait(Conditions.Stun), new ForcedMovementImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 22 * CharacterCount,
			Attack = 6,
			Traits =
			[
				ConditionImmunityTrait.WoundImmunityTrait(), ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Immobilize),
				new ConditionImmunityTrait(Conditions.Stun), new ForcedMovementImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 27 * CharacterCount,
			Attack = 6,
			Traits =
			[
				ConditionImmunityTrait.WoundImmunityTrait(), ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Immobilize),
				new ConditionImmunityTrait(Conditions.Stun), new ForcedMovementImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 29 * CharacterCount,
			Attack = 7,
			Traits =
			[
				ConditionImmunityTrait.WoundImmunityTrait(), ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Immobilize),
				new ConditionImmunityTrait(Conditions.Stun), new ForcedMovementImmunityTrait()
			]
		},
	];

	public override string Name => "Drakefiend";

	public override string AssetPath => "res://Content/Monsters/Boss/Drakefiend";

	public override string PortraitTexturePath => $"{AssetPath}/Portrait.tres";

	public override int MaxStandeeCount => 1;

	private static readonly AOEPattern AOEPattern = new AOEPattern(
		[
			new AOEHex(Vector2I.Zero, AOEHexType.Gray),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East).Add(Direction.East), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.East), AOEHexType.Red),
		]
	);

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	public string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 All adjacent figures with {Icons.InlineCondition(Conditions.Wound1, richTextParameters)} suffer {Icons.Inline(Icons.Damage, richTextParameters)}1.
		 {Icons.Inline(Icons.Attack, richTextParameters)}{monster.Stats.Attack - 1}, {Icons.InlineCondition(Conditions.Wound1, richTextParameters)}{Icons.InlineAOEPattern(AOEPattern, richTextParameters)}
		 """;

	public string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 Remove all Hot Coals overlay tiles from the N1a map tile.
		 Fly in a straight line to the next boulder in alphabetical order, creating a Hot Coals overlay tile in every featureless hex along its path.
		 Any figure occupying one of these hexes immediately suffers {Icons.Inline(Icons.Damage, richTextParameters)}{(2 + GameController.Instance.SavedScenario.ScenarioLevel) / 2}.
		 Summon 1 normal Spitting Drake at half health (rounded down).
		 """;

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(SufferDamageAbility.Builder()
			.WithDamage(1)
			.WithTarget(Target.Any | Target.TargetAll)
			.WithCustomGetTargets((state, figures) =>
			{
				figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer, 1, false).Where(figure => figure.HasWound()));
			})
			.Build()),
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, -1, conditions: [Conditions.Wound1], aoePattern: AOEPattern))
	];

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				foreach(HotCoals hotCoals in GameController.Instance.Map.Rooms[2].Hexes
					        .SelectMany(hex => hex.GetHexObjectsOfType<HotCoals>()))
				{
					await hotCoals.Destroy();
					state.SetPerformed();
				}
			})
			.Build()),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				ScenarioCheckEvents.FlyingCheckEvent.Subscribe(state, this,
					parameters => parameters.Figure == state.Performer,
					parameters =>
					{
						parameters.SetFlying(true);
					});

				List<Marker> markers = [];
				for(int i = 0; i < 6; i++)
				{
					markers.Add(GameController.Instance.Map.GetMarker((Marker.Type)i));
				}

				List<Hex> markerHexes = markers.Select(marker => marker.Hex).ToList();

				Marker currentMarker;

				if(!markerHexes.Contains(state.Performer.Hex) || markerHexes.Last() == state.Performer.Hex)
				{
					currentMarker = GameController.Instance.Map.GetMarker(Marker.Type.a);
				}
				else
				{
					int markerIndex = markerHexes.IndexOf(state.Performer.Hex);
					currentMarker = GameController.Instance.Map.GetMarker(markers[markerIndex].MarkerType + 1);
				}

				Hex nextHex = state.Performer.Hex;
				do
				{
					if(nextHex.IsFeatureless())
					{
						await AbilityCmd.CreateOverlayTile<HotCoals>(nextHex,
							SceneLoader.LoadPackedScene("res://Content/OverlayTiles/HazardousTerrain/HotCoals1H.tscn"));
						foreach(Figure figure in nextHex.GetFigures().Where(figure => figure != state.Performer))
						{
							await AbilityCmd.SufferDamage(state, figure, (2 + GameController.Instance.SavedScenario.ScenarioLevel) / 2);
						}
					}

					Direction direction = ((int)currentMarker.MarkerType % 3) switch
					{
						0 => Direction.SouthEast,
						1 => Direction.West,
						_ => Direction.NorthEast
					};

					nextHex = GameController.Instance.Map.GetHex(nextHex.Coords.Add(direction));
					await AbilityCmd.ExitHex(null, state.Performer, null);

					await state.Performer.TweenGlobalPosition(nextHex.GlobalPosition, 0.3f).SetEasing(Easing.OutSine).PlayFastForwardableAsync();
					await GDTask.DelayFastForwardable(0.03f);

					await AbilityCmd.EnterHex(null, state.Performer, null, nextHex, true, true);
					state.SetPerformed();
				} while(nextHex != currentMarker.Hex);

				ScenarioCheckEvents.FlyingCheckEvent.Unsubscribe(state, this);
			})
			.Build()),
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<SpittingDrake>())
			.WithMonsterType(MonsterType.Normal)
			.WithOnAbilityStarted(async state =>
			{
				state.SetForcedHitPoints(() => state.SummonedMonster.Health / 2);

				await GDTask.CompletedTask;
			})
			.Build())
	];
}