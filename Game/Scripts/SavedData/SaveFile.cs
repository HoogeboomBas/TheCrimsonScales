using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

public class SaveFile
{
	public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
	{
		Formatting = Formatting.Indented,
		TypeNameHandling = TypeNameHandling.Auto,
		NullValueHandling = NullValueHandling.Ignore,
		ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
		ContractResolver = PromptManager.PromptContractResolver.Instance,
		ObjectCreationHandling = ObjectCreationHandling.Replace
	};

	private readonly string _path;

	private readonly List<object> _saveBlockers = new List<object>();

	public SaveData SaveData { get; }
	public bool RemovedSavedScenario { get; }

	public SaveFile(string path)
	{
		_path = path;

		if(FileAccess.FileExists(path))
		{
			string json = FileAccess.GetFileAsString(_path);
			try
			{
				json = Migrator.Migrate(json, GetVersion(), out bool removedSavedScenario);
				SaveData = JsonConvert.DeserializeObject<SaveData>(json, JsonSerializerSettings);
				RemovedSavedScenario = removedSavedScenario;
			}
			catch(Exception e)
			{
				Log.Error(e);
				SaveData = null;
				return;
			}
		}

		if(SaveData == null)
		{
			SaveData = new SaveData()
			{
				PlayerId = Guid.NewGuid(),
				SavedCampaign = null,
				MigrationVersion = Migrator.MigrationVersion
			};
		}
	}

	public void Save()
	{
		if(SaveData == null)
		{
			return;
		}

		if(_saveBlockers.Count > 0)
		{
			return;
		}

		SaveData.AppVersion = GetVersion();

		using FileAccess saveFile = FileAccess.Open(_path, FileAccess.ModeFlags.Write);

		string json = JsonConvert.SerializeObject(SaveData, JsonSerializerSettings);
		saveFile.StoreLine(json);
	}

	public void BlockSaving(object blocker)
	{
		_saveBlockers.Add(blocker);
	}

	public void UnblockSaving(object blocker)
	{
		_saveBlockers.Remove(blocker);
	}

	private string GetVersion()
	{
		return ProjectSettings.GetSetting("application/config/version").AsString();
	}
}