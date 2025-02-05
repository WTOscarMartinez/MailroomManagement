using System.Data;
using Google.Protobuf;
using MailroomManagement;
using MySql.Data;
using MySql.Data.MySqlClient;
class DataTier{
    public string connStr = "server=;user=;database=;port=8080;password=";

    // perform login check using Stored Procedure "LoginCount" in Database based on given staff username and Password
    public bool LoginCheck(StaffLogin staffLogin){
        MySqlConnection conn = new MySqlConnection(connStr);
        try
        {  
            conn.Open();
            string procedure = "LoginCount";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure; // set the commandType as storedProcedure
            cmd.Parameters.AddWithValue("@inputUserID", staffLogin.StaffUsername);
            cmd.Parameters.AddWithValue("@inputUserPassword", staffLogin.StaffPassword);
            cmd.Parameters.Add("@userCount", MySqlDbType.Int32).Direction =  ParameterDirection.Output;
            MySqlDataReader rdr = cmd.ExecuteReader();
           
            int returnCount = (int) cmd.Parameters["@userCount"].Value;
            rdr.Close();
            conn.Close();

            if (returnCount ==1){
                return true;
            }
            else{
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
            return false;
        }
       
    }

    public Resident CheckForResident(string searchName)
    {
        //Initialize as null to return a null if no result is found
        Resident resident = null!; 
        try
        {  
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                // Use a parameterized query to prevent SQL injection
                string query = "SELECT * FROM Residents WHERE full_name LIKE @searchTerm";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Add the parameter with wildcard characters for LIKE
                    cmd.Parameters.AddWithValue("@searchTerm", $"%{searchName}%");

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            resident = new Resident
                            {
                                id = rdr.GetInt32("id"),
                                full_name = rdr.GetString("full_name"),
                                email = rdr.GetString("email"),
                                unit_number = rdr.GetInt32("unit_number")
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        //Return the resident object
        return resident!; 
    }

    public void AddPackage(Package package){
        MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
            
                string sql = "INSERT INTO Package (owner_name, postal_agency, delivery_date, _status) VALUES (@owner_name, @postal_agency, @delivery_date, @_status)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                // assign values to the SQL command
                cmd.Parameters.AddWithValue("@owner_name", package.OwnerName);
                cmd.Parameters.AddWithValue("@postal_agency", package.PostalAgency);
                cmd.Parameters.AddWithValue("@delivery_date", package.DeliveryDate.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@_status", package.status );
            
                // ExecuteNonQuery to insert, update, and delete data.
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
    }

    //Retrieve package table
    public DataTable CheckPackage(){
        MySqlConnection conn = new MySqlConnection(connStr);
        try
        {  
            conn.Open();
            string table = "Package";
            MySqlCommand cmd = new MySqlCommand(table, conn);
            cmd.CommandType = CommandType.TableDirect;

            MySqlDataReader rdr = cmd.ExecuteReader();

            DataTable tablePackage = new DataTable();
            tablePackage.Load(rdr);
            rdr.Close();
            conn.Close();
            return tablePackage;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
            return null!;
        }
    }

    public void UpdatePackageStatus(int id){
            MySqlConnection conn = new MySqlConnection(connStr);
                try
                {
                    conn.Open();
                
                    string sql = $"UPDATE Package SET _status = 'Picked Up' WHERE id = {id};";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                
                    // ExecuteNonQuery to insert, update, and delete data.
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                conn.Close();
                Console.WriteLine("Done.");
        }

    // perform package history check based on resident name
    public DataTable CheckEnrollment(StaffLogin staffLogin){
        MySqlConnection conn = new MySqlConnection(connStr);
        Console.WriteLine("Please input a semester in TermYear format, e.g: Fall2022, Spring2021");
        string semester = Console.ReadLine()!;
        try
        {  
            conn.Open();
            string procedure = "CheckEnrollment";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inputStudentID", staffLogin.StaffUsername);
            cmd.Parameters["@inputStudentID"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("@inputSemester", semester);
            cmd.Parameters["@inputSemester"].Direction = ParameterDirection.Input;

            MySqlDataReader rdr = cmd.ExecuteReader();

            DataTable tableEnrollment = new DataTable();
            tableEnrollment.Load(rdr);
            rdr.Close();
            conn.Close();
            return tableEnrollment;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
            
        }

        return null!;
    }
    
    public Resident CheckRecords(string name, int unit)
    {
        //Initialize as null to return a null if no result is found
        Resident resident = null!; 
        try
        {  
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                // Use a parameterized query to prevent SQL injection
                string query = "SELECT * FROM Residents WHERE LOWER(full_name) = LOWER(@name) AND unit_number = @unit;";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Add the parameter with for name, and unit.
                    cmd.Parameters.AddWithValue("@name", $"{name}");
                    cmd.Parameters.AddWithValue("@unit", $"{unit}");

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            resident = new Resident
                            {
                                id = rdr.GetInt32("id"),
                                full_name = rdr.GetString("full_name"),
                                email = rdr.GetString("email"),
                                unit_number = rdr.GetInt32("unit_number")
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        //Return the resident object
        return resident!; 
    }

}
