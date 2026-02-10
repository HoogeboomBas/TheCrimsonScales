using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class EruptingMandibles : AmberAegisCardModel<EruptingMandibles.CardTop, EruptingMandibles.CardBottom>
{
	public override string Name => "Erupting Mandibles";
	public override int Level => 8;
	public override int Initiative => 54;
	protected override int AtlasIndex => 27;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					int damage = ((Character)state.Performer).Cards.Count(card =>
						card.CardState is CardState.Persistent && card.Top.Model.CustomTag == "Cultivate") + 1;
					foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 1)
						        .Where(figure => figure.EnemiesWith(state.Performer) || figure.AlliedWith(state.Performer)))
					{
						await AbilityCmd.SufferDamage(state, figure, damage);
						state.SetPerformed();
					}

					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth,
					effectInfoText:
					$"All adjacent allies and enemies suffer {Icons.Inline(Icons.Damage)}X+1, where X is the number of active CULTIVATES"))
				.Build())
		];

		public override IEnumerable<Element> Elements => [Element.Fire];
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.62114084f, 0.66008836f)))
				.Build()),
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(2)
				.WithConditionalAbilityCheck(state =>
					AbilityCmd.AskConsumeElement(state.Performer, Element.Fire, effectInfoText: $"{Icons.Inline(Icons.Retaliate)}2"))
				.WithOnAbilityEndedPerformed(async state =>
				{
					state.ActionState.SetOverrideRound();
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override IEnumerable<Element> Elements => [Element.Earth];
	}
}