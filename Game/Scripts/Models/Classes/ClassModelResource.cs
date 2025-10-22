using Godot;

[GlobalClass, Tool]
public partial class ClassModelResource : Resource
{
	[Export, ModelId(typeof(ClassModel))]
	public string ModelIdString { get; set; } = ModelId.None.ToString();

	public ClassModel Model => ModelDB.GetById<ClassModel>(new ModelId(ModelIdString));
}