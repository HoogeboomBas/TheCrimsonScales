using System.Collections.Generic;
using Newtonsoft.Json;

public class SaveManager
{
	public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
	{
		Formatting = Formatting.Indented,
		TypeNameHandling = TypeNameHandling.Auto,
		NullValueHandling = NullValueHandling.Ignore,
		ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
		ContractResolver = SaveFileContractResolver.Instance,
		ObjectCreationHandling = ObjectCreationHandling.Replace
	};

	private readonly List<object> _saveBlockers = new List<object>();

	public List<SaveFile<CampaignSaveData>> CampaignSaveFiles { get; } = new List<SaveFile<CampaignSaveData>>();
	public int CurrentCampaignIndex { get; private set; } = -1;

	public SaveFile<DeviceSaveData> DeviceSaveFile { get; private set; }

	public bool CanSave => _saveBlockers.Count == 0;
	public SaveFile<CampaignSaveData> CampaignSaveFile => CurrentCampaignIndex < 0 ? null : CampaignSaveFiles[CurrentCampaignIndex];

	public SaveManager()
	{
		DeviceSaveFile = new SaveFile<DeviceSaveData>("user://DeviceSaveFile.save");

		for(int i = 0; i < 3; i++)
		{
			CampaignSaveFiles.Add(new SaveFile<CampaignSaveData>($"user://Campaign-{i + 1}.save"));
		}
	}

	public void SetCampaignIndex(int campaignIndex)
	{
		CurrentCampaignIndex = campaignIndex;
	}

	public void SaveCampaignAndDevice()
	{
		if(!CanSave)
		{
			return;
		}

		SaveGame();
		SaveDevice();
	}

	public void SaveAll()
	{
		if(!CanSave)
		{
			return;
		}

		foreach(SaveFile<CampaignSaveData> saveFile in CampaignSaveFiles)
		{
			saveFile.Save();
		}

		SaveDevice();
	}

	public void SaveGame()
	{
		if(!CanSave)
		{
			return;
		}

		CampaignSaveFile?.Save();
	}

	public void SaveDevice()
	{
		if(!CanSave)
		{
			return;
		}

		DeviceSaveFile.Save();
	}

	public void BlockSaving(object blocker)
	{
		_saveBlockers.Add(blocker);
	}

	public void UnblockSaving(object blocker)
	{
		_saveBlockers.Remove(blocker);
	}
}