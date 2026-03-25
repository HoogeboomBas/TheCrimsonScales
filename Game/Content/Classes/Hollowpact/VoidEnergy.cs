using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class VoidEnergy : ClassResource
{
	private Dictionary<Figure, int> _voidEnergyCount = [];
	private const int MaxEnergyCount = 3;
	private int _count = 0;

	// End of round subscribe if too much - muddle, wound

	public async void Gain(Figure figure, int quantity)
	{
		_voidEnergyCount[figure] = _voidEnergyCount.ContainsKey(figure) ? Math.Min(MaxEnergyCount, _voidEnergyCount[figure] + quantity) : quantity;
	}

	public override bool CheckAvailability(Figure figure, int quantity)
	{
		return _voidEnergyCount.ContainsKey(figure) && _voidEnergyCount[figure] >= quantity);
	}

	public override bool TryConsume(Figure figure, int quantity)
	{
		if(CheckAvailability(figure, quantity))
		{
			_voidEnergyCount[figure] -= quantity;
		}
	}

	public override string GetIcon()
	{
		return Icons.getclass(hollowhuy);
	}

	public override string GetText(int quantity)
	{
		return $"Spend {quantity} Void Energy";
	}
}