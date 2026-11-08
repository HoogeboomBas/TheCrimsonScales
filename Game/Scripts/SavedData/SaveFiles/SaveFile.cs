using System;
using System.IO;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FileAccess = Godot.FileAccess;

public abstract class SaveFile<TSaveData>
	where TSaveData : SaveData, new()
{
	private readonly string _normalPath;
	private readonly string _backupPath;

	public TSaveData SaveData { get; private set; }

	protected abstract Migration[] Migrations { get; }

	private int MigrationVersion => Migrations.Length;

	public SaveFile(string path)
	{
		_normalPath = path + ".save";
		_backupPath = path + "-backup.save";

		TSaveData saveData = null;
		if(!TryLoad(_normalPath, ref saveData, out bool corruptedNormal))
		{
			if(!TryLoad(_backupPath, ref saveData, out bool corruptedBackup))
			{
				if(corruptedNormal)
				{
					AppController.Instance.DelayedCall(() =>
					{
						string fileName = Path.GetFileNameWithoutExtension(_normalPath);
						AppController.Instance.PopupManager.RequestPopup(
							new TextPopup.Request("Save file issue",
								$"The save file `{fileName}` was corrupted! It has been replaced with a new save file.\nWe're very sorry for the inconvenience."));
						Save();
					});
				}

				NewSaveData();
				saveData = SaveData;
			}
		}

		SaveData = saveData;
	}


	public void Save()
	{
		if(SaveData == null)
		{
			return;
		}

		SaveData.AppVersion = GetVersion();
		SaveData.LastSaved = DateTime.Now;

		if(TrySave(_normalPath))
		{
			// Only save a backup save file if the save succeeded
			TrySave(_backupPath);
		}
	}

	public void NewSaveData()
	{
		SaveData = new TSaveData()
		{
			MigrationVersion = MigrationVersion
		};
	}

	protected virtual void Migrate(JObject saveData)
	{
	}

	protected static string GetVersion()
	{
		return ProjectSettings.GetSetting("application/config/version").AsString();
	}

	private string Migrate(string json)
	{
		JObject saveData = JObject.Parse(json);
		int migrationVersion = saveData.GetValue("MigrationVersion")?.ToObject<int>() ?? 0;

		for(int i = migrationVersion; i < Migrations.Length; i++)
		{
			Log.Write($"Migrating save file from version {i} to {i + 1}.");
			Migrations[i].Migrate(saveData);
		}

		saveData["MigrationVersion"] = MigrationVersion;

		Migrate(saveData);

		json = JsonConvert.SerializeObject(saveData, SaveManager.JsonSerializerSettings);

		return json;
	}

	private bool TrySave(string path)
	{
		try
		{
			using FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);

			string json = JsonConvert.SerializeObject(SaveData, SaveManager.JsonSerializerSettings);
			file.StoreLine(json);

			return true;
		}
		catch(Exception e)
		{
			Log.Warning(e);

			return false;
		}
	}

	private bool TryLoad(string path, ref TSaveData saveData, out bool corrupted)
	{
		corrupted = false;

		if(FileAccess.FileExists(path))
		{
			try
			{
				string json = FileAccess.GetFileAsString(path);
				json = Migrate(json);
				saveData = JsonConvert.DeserializeObject<TSaveData>(json, SaveManager.JsonSerializerSettings);

				return true;
			}
			catch(Exception e)
			{
				Log.Warning(e);

				corrupted = true;
				return false;
			}
		}

		return false;
	}
}