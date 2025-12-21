using System;

public abstract class AbstractModel<T> : AbstractModel
	where T : AbstractModel<T>
{
	public T ImmutableInstance { get; private set; }

	public T ToMutable()
	{
		AssertImmutable();
		T mutable = (T)MutableClone();
		mutable.ImmutableInstance = (T)this;
		return mutable;
	}
}

public abstract class AbstractModel : IComparable<AbstractModel>
{
	private bool _idSet;
	private ModelId _id;

	public bool IsMutable { get; private set; }

	public ModelId Id
	{
		get
		{
			if(_idSet)
			{
				return _id;
			}

			_idSet = true;
			_id = ModelDB.GetId(GetType());
			return _id;
		}
	}

	protected AbstractModel()
	{
		if(ModelDB.Contains(GetType()))
		{
			throw new Exception("Trying to create an instance of a model through a constructor, use ModelDB instead.");
		}
	}

	public virtual int CompareTo(AbstractModel other)
	{
		return Id.CompareTo(other.Id);
	}

	public void AssertMutable(string message = "Immutable model used in an incorrect place.")
	{
		if(!IsMutable)
		{
			throw new Exception(message);
		}
	}

	public void AssertImmutable(string message = "Mutable model used in an incorrect place.")
	{
		if(IsMutable)
		{
			throw new Exception(message);
		}
	}

	protected AbstractModel MutableClone()
	{
		//AbstractModel abstractModel = (AbstractModel)MemberwiseClone();
		AbstractModel abstractModel = this.DeepClone();
		abstractModel.IsMutable = true;
		abstractModel.DeepCloneFields();
		return abstractModel;
	}

	protected virtual void DeepCloneFields()
	{
	}
}