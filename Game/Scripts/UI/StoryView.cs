using System;
using System.Collections.Generic;
using System.Threading;
using Fractural.Tasks;
using Godot;
using GTweensGodot.Extensions;

public partial class StoryView : Control
{
	[Export]
	private Label _title;
	[Export]
	private Label _subtitle;

	[Export]
	private RichTextLabel _text;

	[Export]
	private BetterButton _skipTextButton;
	[Export]
	private ChoiceButton _backButton;
	[Export]
	private ChoiceButton _continueButton;

	private readonly List<string> _texts = new List<string>();
	private int _textIndex;

	private CancellationToken _cancellationToken;
	private bool _skipText;
	private bool _transitioningText;

	public bool Opened { get; private set; }

	public override void _Ready()
	{
		base._Ready();

		_skipTextButton.Pressed += OnSkipTextPressed;
		_backButton.BetterButton.Pressed += OnBackPressed;
		_continueButton.BetterButton.Pressed += OnContinuePressed;

		SetVisible(false);
	}

	public async GDTask OpenAsync(string title, string subtitle, string text, float fadeInDuration = 0.5f,
		CancellationToken cancellationToken = default)
	{
		Open(title, subtitle, text, fadeInDuration, cancellationToken);

		await GDTask.WaitWhile(() => Opened, cancellationToken: cancellationToken);

		await GDTask.Delay(0.5f, cancellationToken: cancellationToken);
	}

	public void Open(string title, string subtitle, string text, float fadeInDuration = 0.5f, CancellationToken cancellationToken = default)
	{
		_cancellationToken = cancellationToken;

		_title.SetText(title);
		_subtitle.SetText(subtitle ?? string.Empty);

		_texts.Clear();

		string paragraphMarker = "\n";
		string[] paragraphs = text.Split([paragraphMarker], StringSplitOptions.RemoveEmptyEntries);
		_texts.AddRange(paragraphs);

		SetIndex(0, 0.5f);

		SetVisible(true);

		this.SetModulateAlpha(0f);
		this.TweenModulateAlpha(1f, fadeInDuration).Play();

		_text.SetVisibleCharacters(0);

		Opened = true;
	}

	private void Close()
	{
		_backButton.SetActive(false);
		_continueButton.SetActive(false);

		this.TweenModulateAlpha(0f, 0.5f).OnComplete(Hide).Play();

		Opened = false;
	}

	private void SetIndex(int index, float initialDelay = 0f)
	{
		_textIndex = index;

		AnimateText(TextHelper.Prettify(_texts[index]), initialDelay).Forget();

		UpdateButtons();
	}

	private async GDTaskVoid AnimateText(string text, float initialDelay)
	{
		await GDTask.Yield(_cancellationToken);
		await GDTask.Yield(_cancellationToken);

		await GDTask.Delay(initialDelay, cancellationToken: _cancellationToken);

		_text.SetText(text);
		_text.SetVisibleCharacters(0);

		_skipText = false;
		_transitioningText = false;
		_skipTextButton.Show();

		const float charactersPerSecond = 50f;
		float charactersToDisplay = 0f;
		bool waitedFrame = false;

		int labelLength = _text.GetParsedText().Length;
		while(!_transitioningText && Opened)
		{
			if(_skipText)
			{
				charactersToDisplay += Mathf.Inf;
			}

			if(waitedFrame)
			{
				charactersToDisplay += charactersPerSecond * (float)GetProcessDeltaTime();
				waitedFrame = false;
			}

			_text.SetVisibleCharacters(Mathf.Min(Mathf.FloorToInt(charactersToDisplay), labelLength));

			if(charactersToDisplay > labelLength)
			{
				_skipTextButton.Hide();
				charactersToDisplay -= labelLength;
				break;
			}

			await GDTask.Yield(_cancellationToken);
			waitedFrame = true;
		}
	}

	private void SkipText()
	{
		_skipText = true;
	}

	private void UpdateButtons()
	{
		_backButton.SetActive(_textIndex > 0);
		//_continueButton.SetActive(_textIndex < _texts.Count - 1);
	}

	private void OnSkipTextPressed()
	{
		SkipText();
		_skipTextButton.Hide();
	}

	private void OnBackPressed()
	{
		_transitioningText = true;

		SetIndex(_textIndex - 1);
	}

	private void OnContinuePressed()
	{
		if(_textIndex >= _texts.Count - 1)
		{
			Close();

			return;
		}

		_transitioningText = true;

		SetIndex(_textIndex + 1);
	}
}