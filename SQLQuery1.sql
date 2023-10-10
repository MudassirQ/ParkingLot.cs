SELECT TOP 1 * FROM ParkingSlots WHERE  SlotType = 1 ORDER BY SlotNumber;

select count(*) from ParkedVehicles where ParkingSlotId=101 and IsOccupied=0;

SELECT SlotNumber, SlotType, IsOccupied, OccupiedBy,Capacity FROM ParkingSlots WHERE SlotNumber = 101, 