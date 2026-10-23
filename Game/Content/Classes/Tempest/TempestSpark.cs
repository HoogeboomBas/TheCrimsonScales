using System.Collections.Generic;
using Fractural.Tasks;

public partial class TempestSpark : LootableObject
{
	private bool IsFirstSpark => Hex.GetHexObjectOfType<TempestSpark>() == this;

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
			parameters => parameters.Figure is Tempest tempest && tempest.SparkCount < 5 && parameters.Hex == Hex,
			async parameters =>
			{
				await Loot(parameters.Figure);
			});
	}

	public override async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
	{
		await base.Destroy(immediately, forceDestroy);

		ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(this);
	}

	public override bool CanLoot(Figure lootObtainer)
	{
		return lootObtainer is Tempest tempest && tempest.SparkCount < 5;
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		if(IsFirstSpark)
		{
			parametersList.Add(new TempestSparkInfoItem.Parameters(this));
		}
	}

	public override async GDTask Loot(Figure lootObtainer)
	{
		await base.Loot(lootObtainer);

		if(lootObtainer is Tempest tempest)
		{
			tempest.GainSparks();
		}
	}
}