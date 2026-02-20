using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ShiningDiversion : LuminaryCardModel<ShiningDiversion.CardTop, ShiningDiversion.CardBottom>
{
	public override string Name => "Shining Diversion";
	public override int Level => 3;
	public override int Initiative => 29;
	protected override int AtlasIndex => 17;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GlowActiveAbility.Builder()
				.WithGlowAbility(new GlowAbilityModel([Element.Light], GlowAbility,
					$"Perform granted {Icons.Inline(Icons.Shield)} ability", Icons.Shield))
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;

		private Ability GlowAbility(List<Element> elements)
		{
			return GrantAbility.Builder()
				.WithGetAbilities(state =>
				[
					ShieldAbility.Builder()
						.WithShieldValue(1)
						.WithOnAbilityEndedPerformed(async shieldState =>
						{
							await AbilityCmd.InfuseElement(shieldState, Element.Ice);
						})
						.Build(),
				])
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red, "Loot", Icons.Loot),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Red, "Loot", Icons.Loot),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red, "Loot", Icons.Loot),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithOnAbilityStarted(async state =>
				{
					state.SetCustomValue(state.Performer, "Glow Ability", true);
					state.SetCustomValue(state.Performer, "Consumed Elements", elements);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
				{
					foreach(Hex hex in state.ActionState.GetAbilityState<GrantAbility.State>(0).GetCustomMarkedHexes("Loot"))
					{
						await AbilityCmd.LootHex(state.Performer, hex);
					}
				})
				.Build();
		}
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6215359f, 0.66763306f)))
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Muddle)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithRange(1)
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6215359f, 0.8736304f)))
				.Build()),
		];
	}
}