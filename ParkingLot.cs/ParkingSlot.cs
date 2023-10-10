using System.Collections.Generic;
using ParkingLot.Enum;

namespace ParkingLot.ParkingSlot
{
    public class ParkingSlot
    {
        public int Number { get; set; }
        public Enum.SlotType Type { get; set; }
        public bool IsOccupied { get; set; }
        public Enum.VehicleType OccupiedBy { get; set; }
        public int Capacity { get; set; }

        public class ParkingSlots
        {
            public int ParkingSlotId { get; set; }
            public int SlotNumber { get; set; }
            public SlotType SlotType { get; set; }
            public bool IsOccupied { get; set; }
            public VehicleType? OccupiedBy { get; set; }
        }

        public class ParkedVehicle
        {
            public int ParkedVehicleId { get; set; }
            public int ParkingSlotId { get; internal set; }
            public VehicleType VehicleType { get; set; }
            public DateTime EntryTime { get; set; }
            public DateTime? ExitTime { get; set; }
        }

    }
}


