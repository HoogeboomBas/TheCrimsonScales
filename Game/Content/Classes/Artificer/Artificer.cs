using Godot;

public partial class Artificer : Character
{
	public const string ScrapToken = "res://Content/Classes/Artificer/ScrapToken.svg";

	[Export]
	private ScrapTokenIndicator _scrapTokenIndicator;

	private int _scrapTokenCount;

	public override void Spawn(SavedCharacter savedCharacter, int index)
	{
		base.Spawn(savedCharacter, index);
		_scrapTokenIndicator.Hide();
	}

	public void GainScrapToken()
	{
		if(_scrapTokenCount == 5)
		{
			return;
		}

		_scrapTokenCount++;
		if(_scrapTokenCount == 1)
		{
			_scrapTokenIndicator.ShowAnimated();
		}

		_scrapTokenIndicator.SetStackText(_scrapTokenCount.ToString());
	}

	public void LoseScrapTokens(int count = 1)
	{
		for(int i = 0; i < count; i++)
		{
			if(_scrapTokenCount == 0)
			{
				break;
			}

			_scrapTokenCount--;
		}

		if(_scrapTokenCount == 0)
		{
			_scrapTokenIndicator.HideAnimated();
		}
		else
		{
			_scrapTokenIndicator.SetStackText(_scrapTokenCount.ToString());
		}
	}

	public bool HasXScrapTokens(int x)
	{
		return _scrapTokenCount >= x;
	}
}