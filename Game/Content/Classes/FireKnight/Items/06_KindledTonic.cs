public class FireKnightKindledTonic : FireKnightItem
{
	public override string Name => "Kindled Tonic";
	public override int ItemNumber => 6;
	protected override int AtlasIndex => 10 - 6;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.RemoveOneNegativeCondition(null, user);

					object subscriber = new object();
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(user, subscriber,
						parameters => parameters.Figure == user,
						async parameters =>
						{
							ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(user, subscriber);

							await AbilityCmd.AddCondition(null, user, Conditions.Strengthen, user);

							if(await AbilityCmd.AskConsumeElement(user, Element.Fire,
								   effectInfoText: $"{Icons.Inline(Icons.GetCondition(Conditions.Bless))}"))
							{
								await AbilityCmd.AddCondition(null, user, Conditions.Bless, user);
							}
						}
					);
				});
			}
		);
	}
}