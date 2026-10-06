using System;
using System.Collections.Generic;
using System.Threading;
using Fractural.Tasks;
using Godot;

/// <summary>
/// Serializes scene transitions and carries the active typed request into the next scene.
/// </summary>
public partial class SceneLoader : Node
{
	private static readonly Dictionary<string, PackedScene> CachedScenes = new Dictionary<string, PackedScene>();

	[Export]
	private PackedScene _loadingScene;

	public SceneRequest CurrentSceneRequest { get; private set; }

	public bool IsTransitioning => CurrentSceneRequest != null && !CurrentSceneRequest.IsFinished;

	public bool RequestSceneChange(SceneRequest sceneRequest)
	{
		if(IsTransitioning || AppController.Instance == null || !AppController.Instance.SaveManager.CanSave)
		{
			return false;
		}

		CurrentSceneRequest = sceneRequest;

		ChangeScene().Forget();
		return true;
	}

	private async GDTaskVoid ChangeScene()
	{
		CancellationToken cancellationToken = AppController.Instance.DestroyCancellationToken;

		AppController.Instance.PopupManager.CloseAll();

		Node currentScene = GetTree().CurrentScene;

		// Add loading scene
		LoadingSceneController loadingSceneController = _loadingScene.Instantiate<LoadingSceneController>();
		GetTree().Root.AddChild(loadingSceneController);

		await loadingSceneController.FadeIn(cancellationToken);

		// Remove current scene
		currentScene.QueueFree();

		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen(), cancellationToken: cancellationToken);

		await GDTask.Yield(cancellationToken);
		await GDTask.Yield(cancellationToken);

		// Add new scene
		Node newScene = InstantiateScene<Node>(CurrentSceneRequest.ScenePath);
		GetTree().Root.AddChild(newScene);
		GetTree().CurrentScene = newScene;

		GC.Collect();

		await GDTask.Yield(cancellationToken);
		await GDTask.Yield(cancellationToken);
		await GDTask.WaitUntil(() => ((ISceneController)newScene).AdditionalLoadingCompleted, cancellationToken: cancellationToken);
		await GDTask.Yield(cancellationToken);
		await GDTask.Yield(cancellationToken);

		await loadingSceneController.FadeOut(cancellationToken);

		loadingSceneController.QueueFree();

		CurrentSceneRequest.Finish();
	}

	public static T InstantiateScene<T>(string path)
		where T : Node
	{
		return LoadPackedScene(path).Instantiate<T>();
	}

	public static PackedScene LoadPackedScene(string path)
	{
		if(!CachedScenes.TryGetValue(path, out PackedScene packedScene))
		{
			packedScene = ResourceLoader.Load<PackedScene>(path, null, ResourceLoader.CacheMode.IgnoreDeep);
			CachedScenes.Add(path, packedScene);
		}

		return packedScene;
	}
}