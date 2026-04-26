using System;
using Fractural.Tasks;

public class AdjacentEnemiesSufferSanctuaryCritAMDCard : SanctuaryCritAMDCardModel
{
	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, AMDCardType.Crit,
			extraText: $"All enemies adjacent to the target suffer {Icons.Inline(Icons.Damage, richTextParameters)}1",
			rolling: true);

	protected override int AtlasIndex => 6;

	public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
		async (state, _) =>
		{
			foreach(Figure adjacentFigure in RangeHelper.GetFiguresInRange(state.Target.Hex, 1))
			{
				if(state.Performer.EnemiesWith(adjacentFigure) && adjacentFigure != state.Target)
				{
					await AbilityCmd.SufferDamage(state, adjacentFigure, 1);
				}
			}
		};
}