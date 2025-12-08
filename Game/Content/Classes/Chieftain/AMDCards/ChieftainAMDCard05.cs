public class ChieftainAMDCard05 : ChieftainAMDCardModel
{
	protected override int AtlasIndex => 5;

	public override int? GetValue(AttackAbility.State state) =>
		state.Performer is Character performer ? performer.Summons.Count : 
		state.Performer is Summon summon ? summon.CharacterOwner.Summons.Count : 
		((Character)state.Authority).Summons.Count;
}