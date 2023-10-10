using ParkingLot.Enum;
using Xunit;

namespace ParkingLot.Tests
{
    public class ParkingServiceTests : IDisposable
    {
        private readonly ParkingDbContext dbContext;
        private readonly ParkingService parkingService;

        public ParkingServiceTests()
        {
            var connectionString = "your_test_connection_string";
            dbContext = new ParkingDbContext(connectionString);
            dbContext.CreateTables();
            dbContext.InsertSampleData();

            parkingService = new ParkingService(connectionString);
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        [Fact]
        public void ParkVehicle_Should_ParkVehicleSuccessfully()
        {
            var vehicleType = VehicleType.Hatchback;
            var result = parkingService.ParkVehicle(vehicleType);
            Assert.True(result > 0);
        }

        [Fact]
        public void LeaveSlot_Should_LeaveVehicleFromSlot()
        {
            var slotNumber = 1;

            parkingService.LeaveSlot(slotNumber);

            var slot = parkingService.GetSlot(slotNumber);
            Assert.False(slot.IsOccupied);
        }

        [Fact]
        public void FindAvailableSlot_Should_FindAvailableSlot()
        {
            var vehicleType = VehicleType.Hatchback;

            var slot = parkingService.FindAvailableSlot(vehicleType);

            Assert.NotNull(slot);
            Assert.False(slot.IsOccupied);
        }

        [Fact]
        public void GetSlot_Should_ReturnSlotInformation()
        {
            var slotNumber = 1;

            var slot = parkingService.GetSlot(slotNumber);

            Assert.NotNull(slot);
            Assert.Equal(slotNumber, slot.Number);
        }
    }
}
