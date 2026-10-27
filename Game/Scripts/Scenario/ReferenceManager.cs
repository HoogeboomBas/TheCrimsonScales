using System.Collections.Generic;
using Godot;

public class ReferenceManager
{
	private readonly List<IReferenced> _references = new List<IReferenced>();

	public void Register(IReferenced referenced)
	{
		_references.Add(referenced);
		referenced.ReferenceId = _references.Count - 1;
	}

	public T Get<T>(int referenceId)
		where T : IReferenced
	{
		if(_references[referenceId] is not T castType)
		{
			Log.Error("Stored a wrong reference to a "+ typeof(T) + "! Might be a desync issue.");
			return default(T);
		}

		return castType;
	}
}