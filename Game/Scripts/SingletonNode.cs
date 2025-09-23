using System.Threading;
using Godot;

public abstract partial class SingletonNode<T> : Node
	where T : SingletonNode<T>
{
	public static T Instance { get; private set; }

	private readonly CancellationTokenSource _destroyCancellationTokenSource;

	public CancellationToken DestroyCancellationToken => _destroyCancellationTokenSource.Token;

	protected SingletonNode()
	{
		if(Instance != null)
		{
			Log.Error($"Instance of Singleton {GetType()} already exists.");
			return;
		}

		Instance = (T)this;
		Log.Write($"Initialized Singleton of type {GetType()}.");

		_destroyCancellationTokenSource = new CancellationTokenSource();
	}

	public override void _ExitTree()
	{
		if(Instance == this)
		{
			Instance = null;
			OnDestroy();
		}
	}

	public override void _Notification(int what)
	{
		if(what == NotificationWMCloseRequest)
		{
			OnDestroy();
		}
	}

	protected virtual void OnDestroy()
	{
		_destroyCancellationTokenSource.Cancel();
		Log.Write($"Removed Singleton of type {GetType()}.");
	}
}