using Godot;

public partial class AudioController : Node
{
	[Export]
	private PackedScene _audioStreamPlayerScene;
	[Export]
	private AudioStreamPlayer _bgmPlayer;
	[Export]
	private AudioStreamPlayer _bgsPlayer;

	private int _sfxBusIndex;
	private int _bgmBusIndex;
	private int _bgsBusIndex;

	private string _bgmPath;
	private string _bgsPath;

	public override void _Ready()
	{
		base._Ready();

		_sfxBusIndex = AudioServer.GetBusIndex("SFX");
		_bgmBusIndex = AudioServer.GetBusIndex("BGM");
		_bgsBusIndex = AudioServer.GetBusIndex("BGS");

		AppController.Instance.SaveFile.SaveData.Options.SFXVolume.ValueChangedEvent += OnSFXVolumeChanged;
		AppController.Instance.SaveFile.SaveData.Options.BGMVolume.ValueChangedEvent += OnBGMVolumeChanged;
		AppController.Instance.SaveFile.SaveData.Options.BGSVolume.ValueChangedEvent += OnBGSVolumeChanged;

		OnSFXVolumeChanged(AppController.Instance.SaveFile.SaveData.Options.SFXVolume.Value);
		OnBGMVolumeChanged(AppController.Instance.SaveFile.SaveData.Options.BGMVolume.Value);
		OnBGSVolumeChanged(AppController.Instance.SaveFile.SaveData.Options.BGSVolume.Value);
	}

	public void Play(string path, float minPitch = 0.9f, float maxPitch = 1.1f, float volumeDb = 0f, float delay = 0f)
	{
		AudioStream audioStream = LoadAudioStream(path);

		if(audioStream == null)
		{
			return;
		}

		AudioStreamPlayer audioStreamPlayer = _audioStreamPlayerScene.Instantiate<AudioStreamPlayer>();
		AddChild(audioStreamPlayer);
		audioStreamPlayer.SetStream(audioStream);
		audioStreamPlayer.SetPitchScale((float)GD.RandRange(minPitch, maxPitch));
		audioStreamPlayer.SetVolumeDb(volumeDb);
		audioStreamPlayer.DelayedCall(() => audioStreamPlayer.Play(), delay);
		//audioStreamPlayer.Play();
		audioStreamPlayer.QueueFree((float)audioStream.GetLength() + 2f + delay);
	}

	public void PlayFastForwardable(string path, float minPitch = 0.9f, float maxPitch = 1.1f, float volumeDb = 0f, float delay = 0f)
	{
		if(GameController.FastForward)
		{
			return;
		}

		Play(path, minPitch, maxPitch, volumeDb, delay);
	}

	public void SetBGM(string path, float volumeDb = -4f)
	{
		if(path == _bgmPath)
		{
			return;
		}

		_bgmPath = path;

		AudioStream audioStream = LoadAudioStream(_bgmPath);
		_bgmPlayer.SetStream(audioStream);
		if(audioStream == null)
		{
			_bgmPlayer.Stop();
		}
		else
		{
			_bgmPlayer.SetVolumeDb(volumeDb);
			_bgmPlayer.Play();
		}
	}

	public void SetBGS(string path)
	{
		if(path == _bgsPath)
		{
			return;
		}

		_bgsPath = path;

		AudioStream audioStream = LoadAudioStream(_bgsPath);
		_bgsPlayer.SetStream(audioStream);
		if(audioStream == null)
		{
			_bgsPlayer.Stop();
		}
		else
		{
			_bgsPlayer.Play();
		}
	}

	private AudioStream LoadAudioStream(string path)
	{
		if(path == null)
		{
			return null;
		}

		AudioStream audioStream = ResourceLoader.Exists(path) ? ResourceLoader.Load<AudioStream>(path) : null;

		if(audioStream == null)
		{
			Log.Warning($"Audio stream path is incorrect: {path}");
		}

		return audioStream;
	}

	private void OnSFXVolumeChanged(int volume)
	{
		float volumeDb = Mathf.LinearToDb(volume * 0.01f);
		AudioServer.SetBusVolumeDb(_sfxBusIndex, volumeDb);
	}

	private void OnBGMVolumeChanged(int volume)
	{
		float volumeDb = Mathf.LinearToDb(volume * 0.01f);
		AudioServer.SetBusVolumeDb(_bgmBusIndex, volumeDb);
	}

	private void OnBGSVolumeChanged(int volume)
	{
		float volumeDb = Mathf.LinearToDb(volume * 0.01f);
		AudioServer.SetBusVolumeDb(_bgsBusIndex, volumeDb);
	}
}