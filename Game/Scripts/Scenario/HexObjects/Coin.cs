using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweensGodot.Extensions;

public partial class Coin : LootableObject, IEventSubscriber
{
	private bool IsFirstCoin => Hex.GetHexObjectOfType<Coin>() == this;

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		ScenarioEvents.HexObjectDestroyedEvent.Subscribe(this,
			parameters => parameters.HexObject is Coin coin && coin.Hex == Hex,
			async parameters =>
			{
				if(IsFirstCoin)
				{
					ReorganizeCoins();
				}

				await GDTask.CompletedTask;
			}
		);

		ReorganizeCoins();
	}

	public override async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
	{
		await base.Destroy(immediately, forceDestroy);

		ScenarioEvents.HexObjectDestroyedEvent.Unsubscribe(this);
	}

	public override async GDTask Loot(Figure lootObtainer)
	{
		AppController.Instance.AudioController.PlayFastForwardable(SFX.CoinPickup, delay: 0.1f);

		await base.Loot(lootObtainer);

		await ScenarioEvents.CoinLootedEvent.CreatePrompt(new ScenarioEvents.CoinLooted.Parameters(lootObtainer, this));

		lootObtainer.AddCoin();
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		if(IsFirstCoin)
		{
			parametersList.Add(new CoinInfoItem.Parameters(this));
		}
	}

	private void ReorganizeCoins()
	{
		List<Coin> coins = Hex.GetHexObjectsOfType<Coin>().ToList();

		if(coins.Count == 1)
		{
			this.TweenGlobalPosition(Hex.GlobalPosition, 0.2f);
		}
		else
		{
			for(int i = 0; i < coins.Count; i++)
			{
				HexObject coin = coins[i];
				float t = (float)i / coins.Count;
				Vector2 position = Vector2.Right;
				position = position.Rotated(Mathf.DegToRad(t * -360f));
				coin.TweenGlobalPosition(Hex.GlobalPosition + position * 30f, 0.2f).PlayFastForwardable();
			}
		}
	}
}