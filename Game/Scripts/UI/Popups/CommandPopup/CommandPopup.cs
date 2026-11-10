using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class CommandPopup : Popup<CommandPopup.Request>
{
	public class Request : PopupRequest
	{
	}

	[Export]
	private RichTextLabel _output;
	[Export]
	private LineEdit _input;
	[Export]
	private RichTextLabel _help;

	public List<CommandPopupInput> PossibleInputs = [];

	public override void _Ready()
	{
		base._Ready();

		PossibleInputs.Add(new CommandPopupInputScenario());
		PossibleInputs.Add(new CommandPopupInputEvent());
		PossibleInputs.Add(new CommandPopupInputProsperity());
		PossibleInputs.Add(new CommandPopupInputReputation());
		PossibleInputs.Add(new CommandPopupInputItem());
		PossibleInputs.Add(new CommandPopupInputClass());
		PossibleInputs.Add(new CommandPopupInputXP());
		PossibleInputs.Add(new CommandPopupInputGold());
		PossibleInputs.Add(new CommandPopupInputPerk());
		PossibleInputs.Add(new CommandPopupInputAbilityCard());
		PossibleInputs.Add(new CommandPopupInputAbilityCardHelp());
		PossibleInputs.Add(new CommandPopupInputAbilityClassHelp());
		PossibleInputs.Add(new CommandPopupInputHelp());

		_help.AddText(
			"\nCommand List (all class names, item names, and ability card names use screaming snake case (e.g. BRIGHTSPARK, HEATER_SHIELD, BURNING_SPARKS)):\n");
		foreach(CommandPopupInput commandPopupInput in PossibleInputs)
		{
			_help.AddText(commandPopupInput.ArgumentDescriptions.First() + ": " + commandPopupInput.Description + "\n");
		}
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		_output.Text = "";
		_input.Text = "";
		_input.FocusMode = FocusModeEnum.All;

		_input.GrabFocus();
		_input.TextSubmitted += OnInputSubmitted;
	}

	protected override void OnClose()
	{
		base.OnClose();

		_input.ReleaseFocus();
		_input.TextSubmitted -= OnInputSubmitted;
	}

	private void OnInputSubmitted(string text)
	{
		List<string> arguments = [""];
		foreach(char character in text)
		{
			if(char.IsWhiteSpace(character))
			{
				arguments.Add("");
			}
			else
			{
				arguments[^1] += character;
			}
		}

		arguments.RemoveAll(argument => argument == "");
		if(arguments.Count == 0)
		{
			return;
		}

		HandleInput(arguments);
		_input.Clear();
		_input.ReleaseFocus();
		_input.CallDeferred(Control.MethodName.GrabFocus);
	}

	private void HandleInput(List<string> arguments)
	{
		CommandPopupInput input = PossibleInputs.FirstOrDefault(input => input.ArgumentDescriptions.Contains(arguments[0]));
		if(input == null)
		{
			SetOutputErrorText(arguments[0] + " is not a valid command.");
		}
		else
		{
			input.HandleInput(arguments, this);
		}
	}

	public void SetOutputText(string text)
	{
		_output.SetText(text);
	}

	public void SetOutputErrorText(string text)
	{
		_output.Clear();
		_output.PushColor(Colors.Red);
		_output.AppendText("[!] " + text);
		_output.Pop();
	}
}