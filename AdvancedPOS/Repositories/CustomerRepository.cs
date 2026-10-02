using System;
using System.Data;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;

namespace AdvancedPOS.Repositories
{
    public class CustomerRepository
    {
        // ==========================================
        // 1. සියලුම Customers ලබා ගැනීම
        // ==========================================
        public DataTable GetAllCustomers()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"SELECT CustomerID, CustomerName, Phone, Email, 
                                        Address, LoyaltyPoints, RegisteredDate
                                 FROM Customers
                                 ORDER BY CustomerName ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // ==========================================
        // 2. Search (නම හෝ Phone අනුව)
        // ==========================================
        public DataTable SearchCustomers(string keyword)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"SELECT CustomerID, CustomerName, Phone, Email, 
                                        Address, LoyaltyPoints, RegisteredDate
                                 FROM Customers
                                 WHERE CustomerName LIKE @kw OR Phone LIKE @kw
                                 ORDER BY CustomerName ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // ==========================================
        // 3. Customer එකක් Phone එකෙන් හොයාගැනීම (POS එකට)
        // ==========================================
        public Customer GetCustomerByPhone(string phone)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"SELECT TOP 1 CustomerID, CustomerName, Phone, Email, 
                                         Address, LoyaltyPoints, RegisteredDate
                                  FROM Customers
                                  WHERE Phone = @phone";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@phone", phone);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Customer
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "",
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : "",
                                LoyaltyPoints = Convert.ToInt32(reader["LoyaltyPoints"]),
                                RegisteredDate = Convert.ToDateTime(reader["RegisteredDate"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        // ==========================================
        // 4. Customer එකක් Add කිරීම
        // ==========================================
        public bool AddCustomer(Customer customer)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"INSERT INTO Customers (CustomerName, Phone, Email, Address, LoyaltyPoints, RegisteredDate)
                                 VALUES (@name, @phone, @email, @address, 0, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@phone", (object)customer.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", (object)customer.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@address", (object)customer.Address ?? DBNull.Value);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ==========================================
        // 5. Customer එකක් Update කිරීම
        // ==========================================
        public bool UpdateCustomer(Customer customer)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"UPDATE Customers 
                                 SET CustomerName = @name, Phone = @phone, 
                                     Email = @email, Address = @address
                                 WHERE CustomerID = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@phone", (object)customer.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", (object)customer.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@address", (object)customer.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", customer.CustomerID);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ==========================================
        // 6. Customer එකක් Delete කිරීම
        // ==========================================
        public bool DeleteCustomer(int customerId)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "DELETE FROM Customers WHERE CustomerID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", customerId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ==========================================
        // 7. Loyalty Points එකතු කිරීම (Sale එකකින් පස්සේ)
        // ==========================================
        public bool AddLoyaltyPoints(int customerId, int points)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"UPDATE Customers 
                                 SET LoyaltyPoints = LoyaltyPoints + @points
                                 WHERE CustomerID = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@points", points);
                    cmd.Parameters.AddWithValue("@id", customerId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
