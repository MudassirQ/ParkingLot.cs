using System.Data.SqlClient;
using ParkingLot.Enum;

namespace ParkingLot
{
    public class ParkingService
    {
        private readonly string connectionString;

        public ParkingService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private List<ParkingSlot.ParkingSlot> parkingSlots;

        public int ParkVehicle(VehicleType vehicleType)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    ParkingSlot.ParkingSlot slot = FindAvailableSlot(vehicleType);
                    if (!CheckIfSlotAvailable(slot))
                    {
                        throw new ApplicationException("Parking is full for selected vehicle type");
                    };
                    if (slot != null)
                    {
                        string insertQuery = "INSERT INTO ParkedVehicles (ParkingSlotId, VehicleType, EntryTime) " +
                                             "VALUES (@ParkingSlotId, @VehicleType, @EntryTime); " +
                                             "SELECT SCOPE_IDENTITY();";

                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@ParkingSlotId", slot.Number);
                        insertCommand.Parameters.AddWithValue("@VehicleType", MapToVehicleType(vehicleType));
                        insertCommand.Parameters.AddWithValue("@EntryTime", DateTime.Now);

                        object result = insertCommand.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            int newRecordId = Convert.ToInt32(result);
                            return newRecordId;
                        }
                    }

                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        private Boolean CheckIfSlotAvailable(ParkingSlot.ParkingSlot slot)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "select count(*) from ParkedVehicles where ParkingSlotId=@SlotNumber and IsOccupied=0";

                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@SlotNumber", slot.Number);

                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int parkedVehicleCount = reader.GetInt32(0);

                        ParkingSlot.ParkingSlot objslot = GetSlot(slot.Number);
                        if (objslot.Capacity >= parkedVehicleCount)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        throw new ApplicationException("No parked vehicle found for the specified slot number.");
                    }
                }
            }
        }

        public void LeaveSlot(int slotNumber)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string selectQuery = "SELECT ParkedVehicleId FROM ParkedVehicles WHERE ParkedVehicleId = @SlotNumber and IsOccupied=1";

                    SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                    selectCommand.Parameters.AddWithValue("@SlotNumber", slotNumber);

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int parkedVehicleId = reader.GetInt32(0);

                            string updateQuery = "UPDATE ParkedVehicles SET IsOccupied = 0 WHERE ParkedVehicleId = @ParkedVehicleId";

                            SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                            updateCommand.Parameters.AddWithValue("@ParkedVehicleId", parkedVehicleId);

                            updateCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            throw new ApplicationException("No parked vehicle found for the specified slot number.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public ParkingSlot.ParkingSlot GetSlot(int slotNumber)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT SlotNumber, SlotType, IsOccupied, OccupiedBy,Capacity FROM ParkingSlots " +
                                   "WHERE SlotNumber = @SlotNumber";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SlotNumber", slotNumber);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ParkingSlot.ParkingSlot slot = new ParkingSlot.ParkingSlot
                            {
                                Number = reader.GetInt32(0),
                                Type = (SlotType)reader.GetInt32(1),
                                Capacity = reader.GetInt32(4)
                            };

                            return slot;
                        }
                        else
                        {
                            throw new ApplicationException("No parking slot found for the specified slot number.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error getting the parking slot.", ex);
            }
        }

        public ParkingSlot.ParkingSlot FindAvailableSlot(VehicleType vehicleType)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT TOP 1 * FROM ParkingSlots " +
                                   "WHERE IsOccupied = 0 AND SlotType = @SlotType " +
                                   "ORDER BY SlotNumber";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SlotType", GetSlotTypeForVehicle(vehicleType));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ParkingSlot.ParkingSlot slot = new ParkingSlot.ParkingSlot
                            {
                                Number = Convert.ToInt32(reader["SlotNumber"]),
                                Type = GetSlotTypeFromDatabaseValue(Convert.ToInt32(reader["SlotType"])),
                                IsOccupied = Convert.ToBoolean(reader["IsOccupied"]),
                                OccupiedBy = GetVehicleTypeFromDatabaseValue(reader["OccupiedBy"] == DBNull.Value ? 0 : Convert.ToInt32(reader["OccupiedBy"]))
                            };

                            return slot;
                        }
                        else
                        {
                            throw new ApplicationException("No available parking slots found for the specified vehicle type.");
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public SlotType GetSlotTypeFromDatabaseValue(int databaseValue)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT SlotType FROM SlotTypeMapping " +
                                   "WHERE DatabaseValue = @DatabaseValue";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DatabaseValue", databaseValue);

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        if (Enum.SlotType.TryParse(result.ToString(), out SlotType slotType))
                        {
                            return slotType;
                        }
                    }
                    return SlotType.Small;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error getting the slot type from the database value.", ex);
            }
        }

        public VehicleType GetVehicleTypeFromDatabaseValue(int databaseValue)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT VehicleType FROM VehicleTypeMapping " +
                                   "WHERE DatabaseValue = @DatabaseValue";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DatabaseValue", databaseValue);

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        if (Enum.SlotType.TryParse(result.ToString(), out VehicleType vehicleType))
                        {
                            return vehicleType;
                        }
                    }

                    return VehicleType.Hatchback;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error getting the vehicle type from the database value.", ex);
            }
        }
        public static int MapToVehicleType(VehicleType databaseValue)
        {
            switch (databaseValue)
            {
                case VehicleType.Hatchback:
                    return 1;
                case VehicleType.SedanCompactSUV:
                    return 2;
                case VehicleType.SUVorLarge:
                    return 3;
                default:
                    return 1;
            }
        }
        public int GetSlotTypeForVehicle(VehicleType vehicleType)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT SlotType FROM VehicleToSlotMapping " +
                                   "WHERE VehicleType = @VehicleType";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@VehicleType", MapToVehicleType(vehicleType));

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (int)result;
                    }

                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error getting the slot type for the vehicle.", ex);
            }
        }
    }
}
