using System;
public interface IHasEmpower
{
	public int RemainingEmpowerCount { get; set; }
	public Type EmpowerType { get; set; }
}