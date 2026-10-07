using System.Collections.Generic;
using Fractural.Tasks;

public partial class Thornreaper : Character
{
	private ThornreaperModel _ThornreaperModel;

	public List<AbilityCard> PrayerCards { get; } = new List<AbilityCard>();

	public override async GDTask Spawn(SavedCharacter savedCharacter, int index)
	{
		await base.Spawn(savedCharacter, index);

		_ThornreaperModel = (ThornreaperModel)savedCharacter.ClassModel;
	}
}