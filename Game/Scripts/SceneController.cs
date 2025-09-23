public abstract partial class SceneController<T> : SingletonNode<T>, ISceneController
	where T : SceneController<T>
{
	public virtual bool AdditionalLoadingCompleted => true;
}