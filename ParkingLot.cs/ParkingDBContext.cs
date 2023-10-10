using System.Data.SqlClient;

public class ParkingDbContext : IDisposable
{
    private readonly string connectionString;
    private SqlConnection connection;

    public ParkingDbContext(string connectionString)
    {
        this.connectionString = connectionString;
        this.connection = new SqlConnection(connectionString);
        this.connection.Open();
    }

    public void CreateTables()
    {
        ExecuteSqlCommand(@"
            CREATE TABLE IF NOT EXISTS ParkingSlots
            (
                ParkingSlotId INT PRIMARY KEY,
                SlotNumber INT NOT NULL,
                SlotType INT NOT NULL,
                IsOccupied BIT NOT NULL,
                OccupiedBy INT
            );

            CREATE TABLE IF NOT EXISTS ParkedVehicles
            (
                ParkedVehicleId INT PRIMARY KEY,
                ParkingSlotId INT NOT NULL,
                VehicleType INT NOT NULL,
                EntryTime DATETIME NOT NULL,
                ExitTime DATETIME
            );
        ");
    }

    public void InsertSampleData()
    {
        ExecuteSqlCommand(@"
            INSERT INTO ParkingSlots (ParkingSlotId, SlotNumber, SlotType, IsOccupied, OccupiedBy)
            VALUES
                (1, 101, 1, 0, NULL),
                (2, 102, 1, 0, NULL),
                (3, 201, 2, 0, NULL);

            INSERT INTO ParkedVehicles (ParkedVehicleId, ParkingSlotId, VehicleType, EntryTime, ExitTime)
            VALUES
                (1, 1, 1, GETDATE(), NULL),
                (2, 3, 2, GETDATE(), NULL);
        ");
    }

    private void ExecuteSqlCommand(string sql)
    {
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public void Dispose()
    {
        if (connection != null)
        {
            connection.Close();
            connection.Dispose();
        }
    }


}