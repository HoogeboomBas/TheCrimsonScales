using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedAbilityCard
{
	[JsonProperty]
	public string ModelId { get; private set; }

	[JsonProperty]
	public Dictionary<int, SavedEnhancement> SavedTopEnhancements { get; private set; } = new Dictionary<int, SavedEnhancement>();

	[JsonProperty]
	public Dictionary<int, SavedEnhancement> SavedBottomEnhancements { get; private set; } = new Dictionary<int, SavedEnhancement>();

	public AbilityCardModel Model => ModelDB.GetById<AbilityCardModel>(ModelId);

	public SavedAbilityCard()
	{
	}

	public SavedAbilityCard(AbilityCardModel abilityCardModel)
	{
		ModelId = abilityCardModel.Id.ToString();
	}

	public void AddSavedEnhancement(bool top, int index, SavedEnhancement savedEnhancement)
	{
		if(top)
		{
			SavedTopEnhancements.Add(index, savedEnhancement);
		}
		else
		{
			SavedBottomEnhancements.Add(index, savedEnhancement);
		}
	}

	public Dictionary<int, SavedEnhancement> GetEnhancements(bool top)
	{
		return top ? SavedTopEnhancements : SavedBottomEnhancements;
	}
}