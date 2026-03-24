using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class ModelDB
{
	private static readonly Dictionary<ModelId, AbstractModel> ContentById = new Dictionary<ModelId, AbstractModel>(1024);

	public static ModelId GetId<T>()
		where T : AbstractModel
	{
		return GetId(typeof(T));
	}

	public static ModelId GetId(Type type)
	{
		return new ModelId()
		{
			Category = SlugHelper.GetSlug(type.BaseType?.Name),
			Entry = SlugHelper.GetSlug(type.FullName)
		};
	}

	public static T GetById<T>(ModelId id)
		where T : AbstractModel
	{
		if(ContentById.TryGetValue(id, out AbstractModel value))
		{
			return (T)value;
		}

		foreach(Type subtype in GetSubtypes<T>())
		{
			AbstractModel byId = DeserializeGetOrCreate(subtype);
			if(byId.Id == id)
			{
				return (T)byId;
			}
		}

		return default;
	}

	public static T GetById<T>(string id)
		where T : AbstractModel
	{
		return id == null ? null : GetById<T>(new ModelId(id));
	}

	public static T GetByType<T>(Type type)
		where T : AbstractModel
	{
		return GetById<T>(GetId(type));
	}

	public static bool Contains(Type type)
	{
		return ContentById.ContainsKey(GetId(type));
	}

	private static T GetOrCreate<T>() where T : AbstractModel
	{
		ModelId id = GetId<T>();
		if(!ContentById.ContainsKey(id))
		{
			ContentById[id] = Activator.CreateInstance<T>();
		}

		return (T)ContentById[id];
	}

	private static AbstractModel DeserializeGetOrCreate(Type type)
	{
		ModelId id = GetId(type);
		if(!ContentById.ContainsKey(id))
		{
			ContentById[id] = (AbstractModel)Activator.CreateInstance(type);
		}

		return ContentById[id];
	}

	public static T AbilityCard<T>() where T : AbilityCardModel
	{
		return GetOrCreate<T>();
	}

	public static T AbilityCardSide<T>() where T : AbilityCardSideModel
	{
		return GetOrCreate<T>();
	}

	public static T Class<T>() where T : ClassModel
	{
		return GetOrCreate<T>();
	}

	public static T Monster<T>() where T : MonsterModel
	{
		return GetOrCreate<T>();
	}

	public static T MonsterAbilityCard<T>() where T : MonsterAbilityCardModel
	{
		return GetOrCreate<T>();
	}

	public static T Condition<T>() where T : ConditionModel
	{
		return GetOrCreate<T>();
	}

	public static T Scenario<T>() where T : ScenarioModel
	{
		return GetOrCreate<T>();
	}

	public static T ScenarioChain<T>() where T : ScenarioChain
	{
		return GetOrCreate<T>();
	}

	public static T Item<T>() where T : ItemModel
	{
		return GetOrCreate<T>();
	}

	public static T AMDCard<T>() where T : AMDCardModel
	{
		return GetOrCreate<T>();
	}

	public static T Event<T>() where T : EventModel
	{
		return GetOrCreate<T>();
	}

	public static T EventChoice<T>() where T : EventChoiceModel
	{
		return GetOrCreate<T>();
	}

	public static T PersonalQuest<T>() where T : PersonalQuestModel
	{
		return GetOrCreate<T>();
	}

	public static T Enhancement<T>() where T : EnhancementModel
	{
		return GetOrCreate<T>();
	}

	public static T Perk<T>() where T : PerkModel
	{
		return GetOrCreate<T>();
	}

	public static T BattleGoal<T>() where T : BattleGoalModel
	{
		return GetOrCreate<T>();
	}

	public static IEnumerable<Type> GetSubtypes<T>()
	{
		return GetSubtypes(typeof(T));
	}

	public static IEnumerable<Type> GetSubtypes(Type type)
	{
		Assembly assembly = Assembly.GetAssembly(type);
		return (object)assembly == null
			? null
			: (assembly.GetTypes()).Where((Func<Type, bool>)(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(type)));
	}
}