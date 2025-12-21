using System;
using Fractural.Tasks;

public class AdjacentEnemiesSufferSanctuaryCritAMDCard : SanctuaryCritAMDCardModel
{
	protected override int AtlasIndex => 6;

	public override Func<AttackAbility.State, GDTask> GetExtraEffects(AttackAbility.State attackAbilityState) =>
		async state =>
		{
			foreach(Figure figure in RangeHelper.GetFiguresInRange(attackAbilityState.Target.Hex, 1))
			{
				if(attackAbilityState.Performer.EnemiesWith(figure) && figure != attackAbilityState.Target)
				{
					await AbilityCmd.SufferDamage(null, figure, 1);
				}
			}
		};
}