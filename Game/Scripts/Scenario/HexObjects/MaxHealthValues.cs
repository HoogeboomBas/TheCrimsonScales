public class MaxHealthValues
{
	public int[] Values { get; }

	public static MaxHealthValues Low { get; } = new MaxHealthValues([6, 7, 8, 9, 10, 11, 12, 13, 14]);
	public static MaxHealthValues Medium { get; } = new MaxHealthValues([8, 9, 11, 12, 14, 15, 17, 18, 20]);
	public static MaxHealthValues MediumHigh { get; } = new MaxHealthValues([9, 10, 12, 14, 16, 17, 19, 21, 23]);
	public static MaxHealthValues High { get; } = new MaxHealthValues([10, 12, 14, 16, 18, 20, 22, 24, 26]);

	public MaxHealthValues(int[] values)
	{
		Values = values;
	}
}