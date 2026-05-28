using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

public class SavedPersonalQuests
{
	private static readonly PersonalQuestModel[] StartingPersonalQuestDeck =
	[
		ModelDB.PersonalQuest<ProtectAndServe>(),
		ModelDB.PersonalQuest<WeaponsSpecialist>(),
		ModelDB.PersonalQuest<Experimentation>(),
		ModelDB.PersonalQuest<ThrillSeeker>(),
		ModelDB.PersonalQuest<TrapSetter>(),
		ModelDB.PersonalQuest<BanditBanisher>(),
		ModelDB.PersonalQuest<CreaturesInTheNight>(),
		ModelDB.PersonalQuest<ExperiencedLeader>(),
		ModelDB.PersonalQuest<AdrenalineSpike>(),
		ModelDB.PersonalQuest<MutualSupport>(),
		ModelDB.PersonalQuest<ThyBeBlessed>(),
		ModelDB.PersonalQuest<SpiritualGainsPersonalQuest>(),
		// ModelDB.PersonalQuest<ThePathOfAgony>(), // Requires Hollowpact
		// ModelDB.PersonalQuest<TheDyingOfLight>(), // Requires Hollowpact
		ModelDB.PersonalQuest<NaturalSelection>(),
		ModelDB.PersonalQuest<PredatorAndPrey>(),
		ModelDB.PersonalQuest<AnAdderDivides>(),
		ModelDB.PersonalQuest<FieldResearch>(),
		ModelDB.PersonalQuest<ConjurersHand>(),
		ModelDB.PersonalQuest<NoRestForTheWicked>(),
		ModelDB.PersonalQuest<HealthFirst>(),
		ModelDB.PersonalQuest<LimitlessSearching>(),
	];

	[JsonProperty]
	public List<string> PersonalQuestDeckIds { get; private set; }

	public SavedPersonalQuests()
	{
		PersonalQuestDeckIds = StartingPersonalQuestDeck.Select(personalQuestModel => personalQuestModel.Id.ToString()).ToList();

		ShuffleDeck();
	}

	public PersonalQuestModel PeekPersonalQuest(int indexFromTop)
	{
		if(PersonalQuestDeckIds.Count <= indexFromTop)
		{
			return null;
		}

		int index = PersonalQuestDeckIds.Count - 1 - indexFromTop;
		PersonalQuestModel personalQuestModel = ModelDB.GetById<PersonalQuestModel>(PersonalQuestDeckIds[index]);
		return personalQuestModel;
	}

	public void DrawPersonalQuest(PersonalQuestModel personalQuestModel, bool shuffle = true)
	{
		PersonalQuestDeckIds.Remove(personalQuestModel?.Id.ToString());

		if(shuffle)
		{
			ShuffleDeck();
		}
	}

	public void AddPersonalQuest(PersonalQuestModel personalQuestModel, bool shuffle = true)
	{
		PersonalQuestDeckIds.Add(personalQuestModel.Id.ToString());

		if(shuffle)
		{
			ShuffleDeck();
		}
	}

	public void FilterOutClassPersonalQuests(ClassModel classModel)
	{
		for(int i = PersonalQuestDeckIds.Count - 1; i >= 0; i--)
		{
			string personalQuestDeckId = PersonalQuestDeckIds[i];
			PersonalQuestModel personalQuestModel = ModelDB.GetById<PersonalQuestModel>(personalQuestDeckId);

			if(personalQuestModel.ClassToUnlock == classModel)
			{
				PersonalQuestDeckIds.RemoveAt(i);
			}
		}
	}

	private void ShuffleDeck()
	{
		RandomNumberGenerator tempRNG = new RandomNumberGenerator();
		tempRNG.Randomize();
		PersonalQuestDeckIds.Shuffle(tempRNG);
	}
}