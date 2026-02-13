// using System.Collections.Generic;
// using System.Linq;
//
// public class City52 : CityEventModel<City52.ChoiceA, City52.ChoiceB>
// {
// 	public override int Number => 52;
//
// 	public override string Text =>
// 		"""
// 		As you exit the Sleeping Lion on your way home, you notice waves of people running hurriedly past you as the pungent smell of smoke burns your nostrils. Ah, the smell of opportunity! You follow the crowd into a bustling square to find smoke billowing from a shop window. The fire is quickly getting out of control as the Fire Brigade arrives, who work hastily in an effort to slow the spread of the flames.
//
// 		One member of the Brigade, a familiar Valrath, stands separate from the rest of the crew, trying to keep the flames at bay through sorcery. You watch in continued amazement as your former compatriot is single-handedly doing more to bring the fire under control than the rest of the Brigade combined, but the screams from inside the shop quickly snap you back to reality. You feel compelled to aid your friend in order to prevent this disaster from getting worse.
// 		""";
//
// 	public class ChoiceA : EventChoiceModel
// 	{
// 		private const string ConditionsMetKey = "ConditionsMet";
//
// 		private static readonly ClassModel[] ClassModels =
// 		[
// 			ModelDB.Class<FireKnightModel>(),
// 			ModelDB.Class<LuminaryModel>(),
// 			ModelDB.Class<StarslingerModel>()
// 		];
//
// 		public override string ChoiceText => "Help mitigate the fire's spread.";
//
// 		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
// 		{
// 			base.InitState(state, savedCampaign);
//
// 			bool conditionsMet = savedCampaign.Characters.Any(character => ClassModels.Contains(character.ClassModel));
// 			state.SetCustomValue(ConditionsMetKey, conditionsMet);
// 		}
//
// 		public override string GetStoryText(SavedEventState state)
// 		{
// 			if(state.GetCustomValue<bool>(ConditionsMetKey))
// 			{
// 				return
// 					"""
// 					Without a second thought, you move through the frantic crowd to help evacuate citizens from the burning shop and keep the area clear for the Fire Brigade to operate.
//
// 					Your mastery of fire, along with the expertise of your former companion, swiftly bring the flames under control.
//
// 					The Fire Knight thanks you for your assistance and invites you to the Brigade's quarters to reminisce over your past adventures.
// 					""";
// 			}
// 			else
// 			{
// 				return
// 					"""
// 					Without a second thought, you move through the frantic crowd to help evacuate citizens from the burning shop and keep the area clear for the Fire Brigade to operate.
//
// 					The Fire Knight thanks you for your assistance and invites you to the Brigade's quarters to reminisce over your past adventures.
// 					""";
// 			}
// 		}
//
// 		public override List<EventReward> GetRewards(SavedEventState state)
// 		{
// 			if(state.GetCustomValue<bool>(ConditionsMetKey))
// 			{
// 				return
// 				[
// 					new AllStartScenarioWithConditionEventReward(Conditions.Strengthen),
// 					new AddCityEventEventReward(ModelDB.Event<City58>())
// 				];
// 			}
// 			else
// 			{
// 				return
// 				[
// 					new AddCityEventEventReward(ModelDB.Event<City58>())
// 				];
// 			}
// 		}
// 	}
//
// 	public class ChoiceB : EventChoiceModel
// 	{
// 		private const string ConditionsMetKey = "ConditionsMet";
//
// 		private static readonly ClassModel[] ClassModels =
// 		[
// 			ModelDB.Class<FireKnightModel>(),
// 			ModelDB.Class<HierophantModel>()
// 		];
//
// 		public override string ChoiceText => "Help tend to the injured citizens.";
//
// 		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
// 		{
// 			base.InitState(state, savedCampaign);
//
// 			bool conditionsMet = savedCampaign.Characters.Any(character => ClassModels.Contains(character.ClassModel));
// 			state.SetCustomValue(ConditionsMetKey, conditionsMet);
// 		}
//
// 		public override string GetStoryText(SavedEventState state)
// 		{
// 			if(state.GetCustomValue<bool>(ConditionsMetKey))
// 			{
// 				return
// 					"""
// 					You move quickly to hold pressure on an injured man's wound as the rest of the Fire Brigade sets up their equipment.
//
// 					Your medicinal skills enable the Brigade to focus on extinguishing the blaze while you tend to the injured.
//
// 					The Fire Knight thanks you for your assistance and invites you to the Brigade's quarters to reminisce over your past adventures.
// 					""";
// 			}
// 			else
// 			{
// 				return
// 					"""
// 					You move quickly to hold pressure on an injured man's wound as the rest of the Fire Brigade sets up their equipment.
//
// 					The Fire Knight thanks you for your assistance and invites you to the Brigade's quarters to reminisce over your past adventures.
// 					""";
// 			}
// 		}
//
// 		public override List<EventReward> GetRewards(SavedEventState state)
// 		{
// 			if(state.GetCustomValue<bool>(ConditionsMetKey))
// 			{
// 				return
// 				[
// 					new AllStartScenarioWithConditionEventReward(Conditions.Bless),
// 					new AddCityEventEventReward(ModelDB.Event<City58>())
// 				];
// 			}
// 			else
// 			{
// 				return
// 				[
// 					new AddCityEventEventReward(ModelDB.Event<City58>())
// 				];
// 			}
// 		}
// 	}
// }

