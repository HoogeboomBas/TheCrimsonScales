using Fractural.Tasks;

public class AllSufferDamageScenarioEffect : ScenarioEffect
{
	private readonly int _damage;

	public AllSufferDamageScenarioEffect(int damage)
	{
		_damage = damage;
	}

	public override string GetDescription(RichTextParameters textParameters) =>
		$"All characters suffer {Icons.Inline(Icons.Damage, textParameters)}{_damage}.";

	public override async GDTask Activate(Character character)
	{
		await AbilityCmd.SufferDamage(character, 1, null);
	}
}