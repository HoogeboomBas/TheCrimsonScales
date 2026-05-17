using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class NetherBlades : HollowpactCardModel<NetherBlades.CardTop, NetherBlades.CardBottom>
{
	public override string Name => "Nether Blades";
	public override int Level => 1;
	public override int Initiative => 55;
	protected override int AtlasIndex => 1;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(VoidsightAbilityBuilder().Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.50749993f, 0.2722222f)))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
				]),
					new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), this,
						new Vector2(0.77593327f, 0.16805553f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.SouthEast), this,
						new Vector2(0.77583325f, 0.41338894f)))
				.Build()),
			
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await GainVoidEnergy(state, state.ActionState.GetAbilityState<AttackAbility.State>(1).UniqueTargetedFigures.Count);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					return await AbilityCmd.HasPerformedAbility(state, 1);
				})
				.Build()),
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62152207f, 0.6665666f)))
				.WithConditionalAbilityCheck(async state =>
				{
					bool usedVoidEnergy = await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 1, 
						new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Teleport)}4 instead"));

					state.SetCustomValue(this, "UsedVoidEnergy", usedVoidEnergy);
					return !usedVoidEnergy;
				})
				.Build()),
			
			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(4)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					
					return state.ActionState.GetAbilityState<MoveAbility.State>(0).GetCustomValue<bool>(this, "UsedVoidEnergy");
				})
				.Build()),
		];
		
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
	}
}