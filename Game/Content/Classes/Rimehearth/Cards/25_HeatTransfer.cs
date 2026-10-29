using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class HeatTransfer : RimehearthCardModel<HeatTransfer.CardTop, HeatTransfer.CardBottom>
{
	public override string Name => "Heat Transfer";
	public override int Level => 8;
	public override int Initiative => 64;
	protected override int AtlasIndex => 25;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					if(await AbilityCmd.RemoveCondition(state.Performer, Conditions.Chill, state))
					{
						state.SetPerformed();
					}
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return state.Performer.HasWound();
				})
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62163085f, 0.25706372f)))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.6162984f, 0.35583276f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse([Element.Fire, Element.Ice])];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.5239462f, 0.7011389f)))
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					bool infused = false;
					if(state.Performer.HasWound())
					{
						await AbilityCmd.InfuseElement(state, Element.Fire);
						infused = true;
					}

					if(state.Performer.HasCondition(Conditions.Chill))
					{
						await AbilityCmd.InfuseElement(state, Element.Ice);
						infused = true;
					}

					if(infused)
					{
						state.SetPerformed();
						await AbilityCmd.GainXP(state.Performer, 1);
					}
				})
				.Build())
		];
	}
}