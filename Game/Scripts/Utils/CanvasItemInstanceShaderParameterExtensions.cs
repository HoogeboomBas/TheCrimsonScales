using Godot;
using GTweens.Extensions;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public static class CanvasItemInstanceShaderParameterExtensions
{
	public static GTween TweenInstanceShaderPropertyInt(this CanvasItem target, StringName property, int to, float duration)
	{
		return GTweenExtensions.Tween(
			() => target.GetInstanceShaderParameter(property).AsInt32(),
			current => target.SetInstanceShaderParameter(property, current),
			to,
			duration,
			GodotObjectExtensions.GetGodotObjectValidationFunction(target)
		);
	}

	public static GTween TweenInstanceShaderPropertyFloat(this CanvasItem target, StringName property, float to, float duration)
	{
		return GTweenExtensions.Tween(
			() => target.GetInstanceShaderParameter(property).AsSingle(),
			current => target.SetInstanceShaderParameter(property, current),
			to,
			duration,
			GodotObjectExtensions.GetGodotObjectValidationFunction(target)
		);
	}

	public static GTween TweenInstanceShaderPropertyVector2(this CanvasItem target, StringName property, Vector2 to, float duration)
	{
		return GTweenGodotExtensions.Tween(
			() => target.GetInstanceShaderParameter(property).AsVector2(),
			current => target.SetInstanceShaderParameter(property, current),
			to,
			duration,
			GodotObjectExtensions.GetGodotObjectValidationFunction(target)
		);
	}

	public static GTween TweenInstanceShaderPropertyVector2I(this CanvasItem target, StringName property, Vector2I to, float duration)
	{
		return GTweenGodotExtensions.Tween(
			() => target.GetInstanceShaderParameter(property).AsVector2I(),
			current => target.SetInstanceShaderParameter(property, current),
			to,
			duration,
			GodotObjectExtensions.GetGodotObjectValidationFunction(target)
		);
	}

	public static GTween TweenInstanceShaderPropertyColor(this CanvasItem target, StringName property, Color to, float duration)
	{
		return GTweenGodotExtensions.Tween(
			() => target.GetInstanceShaderParameter(property).AsColor(),
			current => target.SetInstanceShaderParameter(property, current),
			to,
			duration,
			GodotObjectExtensions.GetGodotObjectValidationFunction(target)
		);
	}

	public static GTween TweenInstanceShaderPropertyColorRgb(this CanvasItem target, StringName property, Color to, float duration)
	{
		return GTweenGodotExtensions.Tween(
			() => target.GetInstanceShaderParameter(property).AsColor(),
			current => target.SetInstanceShaderParameter(
				property,
				new Color(current.R, current.G, current.B, target.GetInstanceShaderParameter(property).AsColor().A)
			),
			to,
			duration,
			GodotObjectExtensions.GetGodotObjectValidationFunction(target)
		);
	}

	public static GTween TweenInstanceShaderPropertyColorAlpha(this CanvasItem target, StringName property, float to, float duration)
	{
		return GTweenExtensions.Tween(
			() => target.GetInstanceShaderParameter(property).AsColor().A,
			current =>
			{
				Color previous = target.GetInstanceShaderParameter(property).AsColor();
				target.SetInstanceShaderParameter(property, new Color(previous.R, previous.G, previous.B, current));
			},
			to,
			duration,
			GodotObjectExtensions.GetGodotObjectValidationFunction(target)
		);
	}
}