using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedEnhancement
{
	[JsonProperty]
	public string ModelId { get; private set; }

	public EnhancementModel Model => ModelDB.GetById<EnhancementModel>(ModelId);

	public SavedEnhancement()
	{
	}

	public SavedEnhancement(EnhancementModel model)
	{
		ModelId = model.Id.ToString();
	}
}