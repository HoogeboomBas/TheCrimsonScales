using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedAbilityCard
{
	[JsonProperty]
	public string ModelId { get; private set; }

	public AbilityCardModel Model => ModelDB.GetById<AbilityCardModel>(ModelId);

	public SavedAbilityCard()
	{
	}

	public SavedAbilityCard(AbilityCardModel abilityCardModel)
	{
		ModelId = abilityCardModel.Id.ToString();
	}
}