using System.Collections.Generic;
using Fractural.Tasks;

public interface IUseSlotsAbilityState
{
	List<UseSlot> Slots { get; set; }
	int UseSlotIndex { get; set; }

	void SetSlots(List<UseSlot> slots);
	GDTask AdvanceUseSlot();
	GDTask MoveBackUseSlot();
}