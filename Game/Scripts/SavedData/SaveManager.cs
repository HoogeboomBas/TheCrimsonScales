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

	public List<CampaignSaveFile> CampaignSaveFiles { get; } = new List<CampaignSaveFile>();
	public int CurrentCampaignIndex { get; private set; } = -1;

	public DeviceSaveFile DeviceSaveFile { get; }

	public bool CanSave => _saveBlockers.Count == 0;
	public CampaignSaveFile CampaignSaveFile => CurrentCampaignIndex < 0 ? null : CampaignSaveFiles[CurrentCampaignIndex];

	public SaveManager()
	{
		DeviceSaveFile = new DeviceSaveFile("user://DeviceSaveFile");

		for(int i = 0; i < 3; i++)
		{
			CampaignSaveFiles.Add(new CampaignSaveFile($"user://Campaign-{i + 1}"));
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

		foreach(CampaignSaveFile saveFile in CampaignSaveFiles)
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