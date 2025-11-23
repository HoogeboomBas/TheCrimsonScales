using Godot;

public class DynamicTarget<TArg> : DynamicType<Target, TArg>
{
	public DynamicTarget(GetValueDelegate getValueFunc) : base(getValueFunc) {}
	public DynamicTarget(Target? value) : base(value) {}

	public static implicit operator DynamicTarget<TArg>(Target value) => new(value);
	public static implicit operator DynamicTarget<TArg>(GetValueDelegate getValueFunc) => new(getValueFunc);
}

public class DynamicAOEPattern<TArg> : DynamicType<AOEPattern, TArg>
{
	public DynamicAOEPattern(GetValueDelegate getValueFunc) : base(getValueFunc) {}
	public DynamicAOEPattern(AOEPattern? value) : base(value) {}

	public static implicit operator DynamicAOEPattern<TArg>(AOEPattern value) => new(value);
	public static implicit operator DynamicAOEPattern<TArg>(GetValueDelegate getValueFunc) => new(getValueFunc);
}

public class DynamicRangeType<TArg> : DynamicType<RangeType, TArg>
{
	public DynamicRangeType(GetValueDelegate getValueFunc) : base(getValueFunc) {}
	public DynamicRangeType(RangeType? value) : base(value) {}

	public static implicit operator DynamicRangeType<TArg>(RangeType value) => new(value);
	public static implicit operator DynamicRangeType<TArg>(GetValueDelegate getValueFunc) => new(getValueFunc);
}

public class DynamicInt<TArg> : DynamicType<int, TArg>
{
	public DynamicInt(GetValueDelegate getValueFunc) : base(getValueFunc) {}
	public DynamicInt(int? value) : base(value) {}

	public static implicit operator DynamicInt<TArg>(int value) => new(value);
	public static implicit operator DynamicInt<TArg>(GetValueDelegate getValueFunc) => new(getValueFunc);
}

public abstract class DynamicType<T, TArg> where T : struct
{
	public T? Value { get; }
	public GetValueDelegate GetValueFunc { get; }

	public delegate T GetValueDelegate(TArg arg);

	private DynamicType(T? value, GetValueDelegate getValueFunc)
	{
		Value = value;
		GetValueFunc = getValueFunc;
	}

	public DynamicType(GetValueDelegate getValueFunc) : this(null, getValueFunc)
	{
	}

	public DynamicType(T? value) : this(value, null)
	{
	}

	public T GetValue(TArg arg)
	{
		if(GetValueFunc != null)
		{
			return GetValueFunc(arg);
		}

		if(!Value.HasValue)
		{
			Log.Error("Both Value and GetValue are null for this dynamic value.");
			return default;
		}

		return Value.Value;
	}
}