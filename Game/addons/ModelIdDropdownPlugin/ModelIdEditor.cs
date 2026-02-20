#if TOOLS
using System;
using System.Collections.Generic;
using Godot;

public partial class ModelIdEditor : EditorProperty
{
	// The main control for editing the property.
	private readonly OptionButton _propertyControl = new OptionButton();
	// An internal value of the property.
	private string _currentValue = ModelId.None.ToString();
	// A guard against internal changes when the property is updated.
	private bool _updating = false;

	private readonly List<string> _options = new List<string>();

	public ModelIdEditor()
	{
	}

	public void SetType(Type type)
	{
		AddModelId(ModelId.None);
		foreach(Type subType in ModelDB.GetSubtypes(type))
		{
			if(subType.GetCustomAttributes(typeof(IgnoreModelIdAttribute), true).Length == 0)
			{
				AddModelId(ModelDB.GetId(subType));
			}
		}

		// Add the control as a direct child of EditorProperty node.
		AddChild(_propertyControl);
		// Make sure the control is able to retain the focus.
		AddFocusable(_propertyControl);
		// Setup the initial state and connect to the signal to track changes.
		RefreshControlText();

		_propertyControl.AllowReselect = true;
		_propertyControl.ItemSelected += OnItemSelected;
	}

	private void AddModelId(ModelId modelId)
	{
		_options.Add((modelId.ToString()));
		_propertyControl.AddItem(GetPrettyName(modelId.ToString()));
	}

	private void OnItemSelected(long index)
	{
		if(_updating)
		{
			return;
		}

		_currentValue = _options[(int)index];
		RefreshControlText();

		EmitChanged(GetEditedProperty(), _currentValue);
	}

	public override void _UpdateProperty()
	{
		// Read the current value from the property.
		string newValue = (string)GetEditedObject().Get(GetEditedProperty());
		if(newValue == _currentValue)
		{
			return;
		}

		// Update the control with the new value.
		_updating = true;
		_currentValue = newValue;
		RefreshControlText();
		_propertyControl.Selected = _options.IndexOf(_currentValue);
		_updating = false;
	}

	private void RefreshControlText()
	{
		_propertyControl.Text = GetPrettyName(_currentValue);
	}

	private static string GetPrettyName(string slug)
	{
		return SlugHelper.GetUnslug(new ModelId(slug).Entry);
	}
}
#endif