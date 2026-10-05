/// <summary>
/// Base class for scene-level controllers that participate in scene loading.
/// </summary>
public abstract partial class SceneController<T> : SingletonNode<T>, ISceneController
	where T : SceneController<T>
{
	public virtual bool AdditionalLoadingCompleted => true;
}