using System;
using System.Threading;
using Fractural.Tasks;
using Godot;

public partial class SceneLoader : Node
{
	[Export]
	private PackedScene _loadingScene;

	public SceneRequest CurrentSceneRequest { get; private set; }

	public bool IsTransitioning => CurrentSceneRequest != null && !CurrentSceneRequest.IsFinished;

	public bool RequestSceneChange(SceneRequest sceneRequest)
	{
		if(IsTransitioning)
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
		PackedScene packedScene = ResourceLoader.Load<PackedScene>(CurrentSceneRequest.ScenePath);

		Node newScene = packedScene.Instantiate();
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
}