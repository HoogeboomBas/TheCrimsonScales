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

		AppController.Instance.DeviceOptions.SFXVolume.ValueChangedEvent += OnSFXVolumeChanged;
		AppController.Instance.DeviceOptions.BGMVolume.ValueChangedEvent += OnBGMVolumeChanged;
		AppController.Instance.DeviceOptions.BGSVolume.ValueChangedEvent += OnBGSVolumeChanged;

		OnSFXVolumeChanged(AppController.Instance.DeviceOptions.SFXVolume.Value);
		OnBGMVolumeChanged(AppController.Instance.DeviceOptions.BGMVolume.Value);
		OnBGSVolumeChanged(AppController.Instance.DeviceOptions.BGSVolume.Value);
	}

	public AudioStreamPlayer Play(string path, float minPitch = 0.9f, float maxPitch = 1.1f, float volumeDb = 0f, float delay = 0f,
		bool freeAutomatically = true)
	{
		AudioStreamPlayer audioStreamPlayer = CreateAudioStreamPlayer(path);

		if(audioStreamPlayer == null)
		{
			return null;
		}

		audioStreamPlayer.SetPitchScale((float)GD.RandRange(minPitch, maxPitch));
		audioStreamPlayer.SetVolumeDb(volumeDb);
		audioStreamPlayer.DelayedCall(() => audioStreamPlayer.Play(), delay);
		if(freeAutomatically)
		{
			audioStreamPlayer.QueueFree((float)audioStreamPlayer.Stream.GetLength() + 2f + delay);
		}

		return audioStreamPlayer;
	}

	public AudioStreamPlayer PlayFastForwardable(string path, float minPitch = 0.9f, float maxPitch = 1.1f, float volumeDb = 0f, float delay = 0f,
		bool freeAutomatically = true)
	{
		if(GameController.FastForward)
		{
			return null;
		}

		return Play(path, minPitch, maxPitch, volumeDb, delay, freeAutomatically);
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

	private AudioStreamPlayer CreateAudioStreamPlayer(string path)
	{
		AudioStream audioStream = LoadAudioStream(path);

		if(audioStream == null)
		{
			return null;
		}

		AudioStreamPlayer audioStreamPlayer = _audioStreamPlayerScene.Instantiate<AudioStreamPlayer>();
		AddChild(audioStreamPlayer);
		audioStreamPlayer.SetStream(audioStream);
		return audioStreamPlayer;
	}
}