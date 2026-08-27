public class FireKnightFireproofHelm : FireKnightItem
{
	public override string Name => "Fireproof Helm";
	public override int ItemNumber => 5;
	protected override int AtlasIndex => 10 - 5;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.AddCondition(null, user, Conditions.Ward, user);

					if(await AbilityCmd.AskConsumeElement(user, Element.Fire,
						   effectInfoText: $"{Icons.Inline(Icons.GetCondition(Conditions.Safeguard))}"))
					{
						await AbilityCmd.AddCondition(null, user, Conditions.Safeguard, user);
					}
				});
			}
		);
	}
}