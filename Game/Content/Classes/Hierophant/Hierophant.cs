using System.Collections.Generic;
using Fractural.Tasks;

public partial class Hierophant : Character
{
	private HierophantModel _hierophantModel;

	public List<AbilityCard> PrayerCards { get; } = new List<AbilityCard>();

	public override async GDTask Spawn(SavedCharacter savedCharacter, int index)
	{
		await base.Spawn(savedCharacter, index);

		_hierophantModel = (HierophantModel)savedCharacter.ClassModel;

		// Copy over all prayer cards from the character
		foreach(AbilityCardModel prayerCard in _hierophantModel.AllPrayerCards)
		{
			AbilityCard abilityCard = new AbilityCard(new SavedAbilityCard(prayerCard), this);
			PrayerCards.Add(abilityCard);
		}
	}
}