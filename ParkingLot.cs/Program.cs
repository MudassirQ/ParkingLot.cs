using ParkingLot;
using ParkingLot.Enum;

public class ParkingLots
{
    static void Main(string[] args)
    {
        string connectionString = "Data Source=.;Initial Catalog=ParkingLot;Integrated Security=True;MultipleActiveResultSets=True;";
        ParkingService parkingService = new ParkingService(connectionString);

        while (true)
        {
            Console.WriteLine("Parking Lot Management System");
            Console.WriteLine("1. Park Vehicle");
            Console.WriteLine("2. Leave Slot");
            Console.WriteLine("3. Exit");

            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ParkVehicle(parkingService);
                    break;
                case "2":
                    LeaveSlot(parkingService);
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void ParkVehicle(ParkingService parkingService)
    {
        try
        {
            Console.WriteLine("Enter the vehicle type (1 for Hatchback, 2 for SedanCompactSUV, 3 for SUVorLarge): ");
            if (Enum.TryParse(Console.ReadLine(), out VehicleType vehicleType))
            {
                int result = parkingService.ParkVehicle(vehicleType);
                if (result > 0)
                {
                    Console.WriteLine($"Vehicle parked successfully. Parking slot ID: {result}");
                }
                else
                {
                    Console.WriteLine("Error parking the vehicle.");
                }
            }
            else
            {
                Console.WriteLine("Invalid vehicle type.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static void LeaveSlot(ParkingService parkingService)
    {
        Console.Write("Enter the slot number to leave: ");
        if (int.TryParse(Console.ReadLine(), out int slotNumber))
        {
            try
            {
                parkingService.LeaveSlot(slotNumber);
                Console.WriteLine("Vehicle left the slot successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Invalid slot number.");
        }
    }
}
