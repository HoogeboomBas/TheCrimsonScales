public class CuriousPendant : CS1Item
{
	public override string Name => "Curious Pendant";
	public override int ItemNumber => 20;
	public override int ShopCount => 2;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override bool Unrecoverable => true;

	protected override int AtlasIndex => 34;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					AbilityCard abilityCard = await AbilityCmd.SelectAbilityCard(character, list =>
					{
						foreach(AbilityCard roundCard in character.RoundCards)
						{
							if(roundCard.CardState == CardState.Lost)
							{
								list.Add(roundCard);
							}
						}
					}, CardState.Lost, hintText: "Select a lost card to recover.");

					if(abilityCard != null)
					{
						await AbilityCmd.ReturnToHand(abilityCard);
					}
				});
			}
		);
	}
}