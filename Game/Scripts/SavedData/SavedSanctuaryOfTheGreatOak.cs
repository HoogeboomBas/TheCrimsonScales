using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedSanctuaryOfTheGreatOak
{
	public static AMDCardModel[] AllCritAMDCards { get; } =
	[
		ModelDB.AMDCard<BlessAllySanctuaryCritAMDCard>(),
		ModelDB.AMDCard<BlessAllySanctuaryCritAMDCard>(),
		ModelDB.AMDCard<HealSanctuaryCritAMDCard>(),
		ModelDB.AMDCard<HealSanctuaryCritAMDCard>(),
		ModelDB.AMDCard<WildElementSanctuaryCritAMDCard>(),
		ModelDB.AMDCard<WildElementSanctuaryCritAMDCard>(),
		ModelDB.AMDCard<AdjacentEnemiesSufferSanctuaryCritAMDCard>(),
		ModelDB.AMDCard<AdjacentEnemiesSufferSanctuaryCritAMDCard>(),
	];

	public static AMDCardModel[] AllRollingAMDCards { get; } =
	[
		ModelDB.AMDCard<PushSanctuaryRollingAMDCard>(),
		ModelDB.AMDCard<PushSanctuaryRollingAMDCard>(),
		ModelDB.AMDCard<HealSanctuaryRollingAMDCard>(),
		ModelDB.AMDCard<HealSanctuaryRollingAMDCard>(),
		ModelDB.AMDCard<WoundMuddleSanctuaryRollingAMDCard>(),
		ModelDB.AMDCard<WoundMuddleSanctuaryRollingAMDCard>(),
		ModelDB.AMDCard<PierceSanctuaryRollingAMDCard>(),
		ModelDB.AMDCard<PierceSanctuaryRollingAMDCard>(),
	];

	public static int[] DonationYellowNumbers =
	[
		5,
		10,
		15,
		20,
		25,
		30,
		40,
		50,
		60,
		70,
		80,
		90,
	];

	[JsonProperty]
	public int TotalDonationCount { get; private set; }

	[JsonProperty]
	public List<string> CritAMDCardIds { get; private set; }

	[JsonProperty]
	public List<string> RollingAMDCardIds { get; private set; }

	[JsonProperty]
	public string PartyAMDCardId { get; private set; }

	public SavedSanctuaryOfTheGreatOak()
	{
		TotalDonationCount = 0;

		CritAMDCardIds = AllCritAMDCards.Select(card => card.Id.ToString()).ToList();
		RollingAMDCardIds = AllRollingAMDCards.Select(card => card.Id.ToString()).ToList();
	}

	public bool CanDonate(SavedCharacter savedCharacter)
	{
		return
			savedCharacter.Gold >= 10 &&
			(savedCharacter.DonationAMDCardIds == null || savedCharacter.DonationAMDCardIds.Length == 0);
	}

	public void Donate(SavedCharacter savedCharacter, SavedCampaign savedCampaign)
	{
		if(!CanDonate(savedCharacter))
		{
			return;
		}

		savedCharacter.RemoveGold(10);

		TotalDonationCount++;
		if(DonationYellowNumbers.Contains(TotalDonationCount))
		{
			savedCampaign.AdjustProsperity(1);
		}

		string critAMDCardId = CritAMDCardIds.PickRandom(BetweenScenariosController.Instance.RNG);
		string rollingAMDCardId = RollingAMDCardIds.PickRandom(BetweenScenariosController.Instance.RNG);

		CritAMDCardIds.Remove(critAMDCardId);
		RollingAMDCardIds.Remove(rollingAMDCardId);

		List<string> donationAMDCardIds = new List<string>();
		donationAMDCardIds.Add(critAMDCardId);
		donationAMDCardIds.Add(rollingAMDCardId);
		if(PartyAMDCardId != null)
		{
			donationAMDCardIds.Add(PartyAMDCardId);
		}

		savedCharacter.SetDonationAMDCardIds(donationAMDCardIds.ToArray());
	}

	public void ReturnCards(SavedCharacter savedCharacter)
	{
		if(savedCharacter.DonationAMDCardIds != null)
		{
			foreach(string donationAMDCardId in savedCharacter.DonationAMDCardIds)
			{
				if(AllCritAMDCards.Any(card => card.Id.ToString() == donationAMDCardId))
				{
					CritAMDCardIds.Add(donationAMDCardId);
				}
				else if(AllRollingAMDCards.Any(card => card.Id.ToString() == donationAMDCardId))
				{
					RollingAMDCardIds.Add(donationAMDCardId);
				}
			}
		}

		savedCharacter.SetDonationAMDCardIds(null);
	}

	public void UnlockPartyAMD(AMDCardModel cardModel)
	{
		PartyAMDCardId = cardModel.Id.ToString();
	}
}