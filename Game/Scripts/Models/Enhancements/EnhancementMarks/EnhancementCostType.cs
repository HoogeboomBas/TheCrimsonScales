using System;

[Flags]
public enum EnhancementCostType
{
	AutoDetect = 0,
	Normal = 1,
	MultiTarget = 2,
	Persistent = 4,
	LossNoPersistent = 8,
}