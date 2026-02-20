using System;
using Fractural.Tasks;

public class AdjacentEnemiesSufferSanctuaryCritAMDCard : SanctuaryCritAMDCardModel
{
	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, AMDCardType.Crit,
			extraText: $"All enemies adjacent to the target suffer {Icons.Inline(Icons.Damage, richTextParameters)}1",
			rolling: true);

	protected override int AtlasIndex => 6;

	public override Func<AttackAbility.State, GDTask> GetExtraEffects(AttackAbility.State attackAbilityState) =>
		async state =>
		{
			foreach(Figure figure in RangeHelper.GetFiguresInRange(attackAbilityState.Target.Hex, 1))
			{
				if(attackAbilityState.Performer.EnemiesWith(figure) && figure != attackAbilityState.Target)
				{
					await AbilityCmd.SufferDamage(attackAbilityState, figure, 1);
				}
			}
		};
}