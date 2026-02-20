using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class DominatingIllusion : LuminaryCardModel<DominatingIllusion.CardTop, DominatingIllusion.CardBottom>
{
	public override string Name => "Dominating Illusion";
	public override int Level => 8;
	public override int Initiative => 51;
	protected override int AtlasIndex => 26;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GlowActiveAbility.Builder()
				.WithGlowAbility(new GlowAbilityModel([Element.Fire], GlowAbility1,
						$"Add +1{Icons.Inline(Icons.Attack)} to all your attacks this turn", Icons.Attack),
					new GlowAbilityModel([Element.Light], GlowAbility2,
						$"Perform {Icons.Inline(Icons.GetCondition(Conditions.Bless))}, self", Icons.GetCondition(Conditions.Bless)))
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;

		private Ability GlowAbility1(List<Element> elements)
		{
			return OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							await GDTask.CompletedTask;
						});

					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						canApplyParameters => true,
						async applyParameters =>
						{
							ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
							ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
					state.SetPerformed();
				})
				.WithOnAbilityStarted(async state =>
				{
					state.SetCustomValue(state.Performer, "Glow Ability", true);
					state.SetCustomValue(state.Performer, "Consumed Elements", elements);

					await GDTask.CompletedTask;
				})
				.Build();
		}

		private Ability GlowAbility2(List<Element> elements)
		{
			return ConditionAbility.Builder()
				.WithConditions(Conditions.Bless)
				.WithTarget(Target.Self)
				.WithOnAbilityStarted(async state =>
				{
					state.SetCustomValue(state.Performer, "Glow Ability", true);
					state.SetCustomValue(state.Performer, "Consumed Elements", elements);

					await GDTask.CompletedTask;
				})
				.Build();
		}
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.3882981f, 0.66303355f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.East), AOEHexType.Empty),
					]
				))
				.Build()),
			Scuttle(2, [Element.Light]),
		];
	}
}