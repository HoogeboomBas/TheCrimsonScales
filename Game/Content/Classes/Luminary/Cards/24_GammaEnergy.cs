using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class GammaEnergy : LuminaryCardModel<GammaEnergy.CardTop, GammaEnergy.CardBottom>
{
	public override string Name => "Gamma Energy";
	public override int Level => 7;
	public override int Initiative => 65;
	protected override int AtlasIndex => 24;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GlowActiveAbility.Builder()
				.WithGlowAbility(new GlowAbilityModel([Element.Fire], GlowAbility,
					$"Perform {Icons.Inline(Icons.Damage)}2 ability", Icons.Damage))
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;

		private Ability GlowAbility(List<Element> elements)
		{
			return SufferDamageAbility.Builder()
				.WithDamage(2)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					]
				))
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
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					int consumedElements = 0;
					for(int i = 0; i < 6; i++)
					{
						if(await AbilityCmd.TryConsumeElement((Element)i, state.Performer))
						{
							consumedElements++;
							state.SetPerformed();
						}
					}

					state.SetCustomValue(this, "ConsumedElements", consumedElements);

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(0)
				.WithTargets(0)
				.WithRange(3)
				.WithOnAbilityStarted(async state =>
				{
					int consumedElements = state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<int>(this, "ConsumedElements");
					state.AbilityAdjustAttackValue(consumedElements);
					state.AdjustTargets(consumedElements);

					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}