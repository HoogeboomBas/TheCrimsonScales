using Godot;

public class DynamicInt<TArg> : DynamicType<int, TArg>
{
	public DynamicInt(GetValueDelegate getValueFunc) : base(getValueFunc) {}
	public DynamicInt(int? value) : base(value) {}

	public static implicit operator DynamicInt<TArg>(int value) => new(value);
	public static implicit operator DynamicInt<TArg>(GetValueDelegate getValueFunc) => new(getValueFunc);
}

public class DynamicInt : DynamicType<int>
{
	public DynamicInt(GetValueDelegate getValueFunc) : base(getValueFunc) {}
	public DynamicInt(int? value) : base(value) {}

	public static implicit operator DynamicInt(int value) => new(value);
	public static implicit operator DynamicInt(GetValueDelegate getValueFunc) => new(getValueFunc);
}

public class DynamicTarget : DynamicType<Target>
{
	public DynamicTarget(GetValueDelegate getValueFunc) : base(getValueFunc) {}
	public DynamicTarget(Target? value) : base(value) {}

	public static implicit operator DynamicTarget(Target value) => new(value);
	public static implicit operator DynamicTarget(GetValueDelegate getValueFunc) => new(getValueFunc);
}

public class DynamicRangeType : DynamicType<RangeType>
{
	public DynamicRangeType(GetValueDelegate getValueFunc) : base(getValueFunc) {}
	public DynamicRangeType(RangeType? value) : base(value) {}

	public static implicit operator DynamicRangeType(RangeType value) => new(value);
	public static implicit operator DynamicRangeType(GetValueDelegate getValueFunc) => new(getValueFunc);
}

public class DynamicAOEPattern : DynamicClassType<AOEPattern>
{
	public DynamicAOEPattern(GetValueDelegate getValueFunc) : base(getValueFunc) {}
	public DynamicAOEPattern(AOEPattern value) : base(value) {}

	public static implicit operator DynamicAOEPattern(AOEPattern value) => new(value);
	public static implicit operator DynamicAOEPattern(GetValueDelegate getValueFunc) => new(getValueFunc);
}

public abstract class DynamicType<T> where T : struct
{
	private T? Value { get; }
	private GetValueDelegate GetValueFunc { get; }

	public delegate T? GetValueDelegate();

	private DynamicType(T? value, GetValueDelegate getValueFunc)
	{
		Value = value;
		GetValueFunc = getValueFunc;
	}

	protected DynamicType(GetValueDelegate getValueFunc) : this(null, getValueFunc)
	{
	}

	protected DynamicType(T? value) : this(value, null)
	{
	}

	public T GetValue()
	{
		T? dynamicValue = GetValueFunc?.Invoke();
		if(dynamicValue.HasValue)
		{
			return dynamicValue.Value;
		}

		if(Value.HasValue)
		{
			return Value.Value;
		}

		Log.Error("Both Value and GetValue are null for this dynamic value.");
		return default;
	}
}

public abstract class DynamicType<T, TArg> where T : struct
{
	private T? Value { get; }
	private GetValueDelegate GetValueFunc { get; }

	public delegate T? GetValueDelegate(TArg arg);

	private DynamicType(T? value, GetValueDelegate getValueFunc)
	{
		Value = value;
		GetValueFunc = getValueFunc;
	}

	protected DynamicType(GetValueDelegate getValueFunc) : this(null, getValueFunc)
	{
	}

	protected DynamicType(T? value) : this(value, null)
	{
	}

	public T GetValue(TArg arg)
	{
		T? dynamicValue = GetValueFunc?.Invoke(arg);
		if(dynamicValue.HasValue)
		{
			return dynamicValue.Value;
		}

		if(Value.HasValue)
		{
			return Value.Value;
		}

		Log.Error("Both Value and GetValue are null for this dynamic value.");
		return default;
	}
}

public abstract class DynamicClassType<T> where T : class
{
	private T Value { get; }
	private GetValueDelegate GetValueFunc { get; }

	public delegate T GetValueDelegate();

	private DynamicClassType(T value, GetValueDelegate getValueFunc)
	{
		Value = value;
		GetValueFunc = getValueFunc;
	}

	protected DynamicClassType(GetValueDelegate getValueFunc) : this(null, getValueFunc)
	{
	}

	protected DynamicClassType(T value) : this(value, null)
	{
	}

	public T GetValue()
	{
		T dynamicValue = GetValueFunc?.Invoke();
		if(dynamicValue != null)
		{
			return dynamicValue;
		}

		if(Value != null)
		{
			return Value;
		}

		Log.Error("Both Value and GetValue are null for this dynamic value.");
		return null;
	}
}

public abstract class DynamicClassType<T, TArg> where T : class
{
	private T Value { get; }
	private GetValueDelegate GetValueFunc { get; }

	public delegate T GetValueDelegate(TArg arg);

	private DynamicClassType(T value, GetValueDelegate getValueFunc)
	{
		Value = value;
		GetValueFunc = getValueFunc;
	}

	protected DynamicClassType(GetValueDelegate getValueFunc) : this(null, getValueFunc)
	{
	}

	protected DynamicClassType(T value) : this(value, null)
	{
	}

	public T GetValue(TArg arg)
	{
		T dynamicValue = GetValueFunc?.Invoke(arg);
		if(dynamicValue != null)
		{
			return dynamicValue;
		}

		if(Value != null)
		{
			return Value;
		}

		Log.Error("Both Value and GetValue are null for this dynamic value.");
		return null;
	}
}