using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class ChainguardAMDCard07 : ChainguardAMDCardModel
{
	protected override int AtlasIndex => 14;

	public override int? GetValue(AttackAbility.State state) => 1;

	public override Func<AttackAbility.State, GDTask> GetExtraEffects(AttackAbility.State attackAbilityState) =>
		async state =>
		{
			await AbilityCmd.CreateTraps(damage: 2, range: 2, performer: state.Performer);
		};
}