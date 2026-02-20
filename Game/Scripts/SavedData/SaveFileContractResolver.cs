using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class SaveFileContractResolver : DefaultContractResolver
{
	public static readonly SaveFileContractResolver Instance = new SaveFileContractResolver();

	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
	{
		JsonProperty property = base.CreateProperty(member, memberSerialization);

		// Prompt Answer
		if(property.DeclaringType!.IsAssignableTo(typeof(PromptAnswer)))
		{
			property.ShouldSerialize = property.PropertyName switch
			{
				nameof(PromptAnswer.Skipped) => instance =>
				{
					PromptAnswer promptAnswer = (PromptAnswer)instance;
					return promptAnswer.Skipped;
				},
				nameof(PromptAnswer.ImmediateCompletion) => instance =>
				{
					PromptAnswer promptAnswer = (PromptAnswer)instance;
					return promptAnswer.ImmediateCompletion;
				},
				nameof(PromptAnswer.SelectedEffectIndex) => instance =>
				{
					PromptAnswer promptAnswer = (PromptAnswer)instance;
					return promptAnswer.SelectedEffectIndex >= 0;
				},
				nameof(PromptAnswer.SyncedAction) => instance =>
				{
					PromptAnswer promptAnswer = (PromptAnswer)instance;
					return promptAnswer.SyncedAction != null;
				},
				_ => instance =>
				{
					PromptAnswer promptAnswer = (PromptAnswer)instance;
					return !promptAnswer.Skipped && promptAnswer.SelectedEffectIndex < 0;
				}
			};
		}

		return property;
	}
}