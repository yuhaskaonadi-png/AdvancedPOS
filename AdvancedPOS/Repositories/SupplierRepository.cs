using System;
using System.Data;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;

namespace AdvancedPOS.Repositories
{
    public class SupplierRepository
    {
        // සියලුම සැපයුම්කරුවන් ලබා ගැනීම
        public DataTable GetAllSuppliers()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT * FROM Suppliers";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // අලුත් සැපයුම්කරුවෙක් ඇතුළත් කිරීම
        public bool AddSupplier(Supplier supplier)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "INSERT INTO Suppliers (SupplierName, ContactPerson, Phone, Address) VALUES (@name, @contact, @phone, @address)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", supplier.SupplierName);
                cmd.Parameters.AddWithValue("@contact", (object)supplier.ContactPerson ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", supplier.Phone);
                cmd.Parameters.AddWithValue("@address", (object)supplier.Address ?? DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // සැපයුම්කරුවෙක් යාවත්කාලීන කිරීම
        public bool UpdateSupplier(Supplier supplier)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "UPDATE Suppliers SET SupplierName=@name, ContactPerson=@contact, Phone=@phone, Address=@address WHERE SupplierID=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", supplier.SupplierID);
                cmd.Parameters.AddWithValue("@name", supplier.SupplierName);
                cmd.Parameters.AddWithValue("@contact", (object)supplier.ContactPerson ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", supplier.Phone);
                cmd.Parameters.AddWithValue("@address", (object)supplier.Address ?? DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // සැපයුම්කරුවෙක් ඉවත් කිරීම
        public bool DeleteSupplier(int id)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "DELETE FROM Suppliers WHERE SupplierID=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}