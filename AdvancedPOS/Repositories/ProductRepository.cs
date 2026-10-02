using AdvancedPOS.Helpers;
using AdvancedPOS.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AdvancedPOS.Repositories
{
    public class ProductRepository
    {
        public DataTable GetAllProducts()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // SELECT * දැමූ විට සියලුම Columns නිවැරදිව ලැබේ (DiscountPercent ඇතුළුව)
                string query = "SELECT * FROM Products";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // අලුත් Product එකක් Database එකට Save කිරීම (Insert)
        public bool AddProduct(Product product)
        {
            bool isSuccess = false;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // Table එකේ Columns වලට හරියටම ගැලපෙන්න කේතය ලියලා තියෙන්නේ
                string query = @"INSERT INTO Products 
                                 (Barcode, ProductName, CategoryID, PurchasePrice, SellingPrice, StockQuantity, ReorderLevel, DiscountPercent) 
                                 VALUES (@barcode, @name, @catId, @pPrice, @sPrice, @qty, @reorder, @discount)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@barcode", product.Barcode);
                cmd.Parameters.AddWithValue("@name", product.ProductName);
                cmd.Parameters.AddWithValue("@catId", product.CategoryID);
                cmd.Parameters.AddWithValue("@pPrice", product.PurchasePrice);
                cmd.Parameters.AddWithValue("@sPrice", product.SellingPrice);
                cmd.Parameters.AddWithValue("@qty", product.StockQuantity);
                cmd.Parameters.AddWithValue("@reorder", product.ReorderLevel);
                cmd.Parameters.AddWithValue("@discount", product.DiscountPercent);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    isSuccess = true;
                }
            }
            return isSuccess;
        }

        // Products ඔක්කොම Database එකෙන් අරගෙන එන Method එක
        public System.Data.DataTable GetProducts()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // INNER JOIN එකක් මගින් Products සහ Categories tables එකතු කරලා CategoryName එක ගන්නවා
                string query = @"SELECT p.ProductID, p.Barcode, p.ProductName, 
                                c.CategoryName, p.PurchasePrice, p.SellingPrice, 
                                p.StockQuantity, p.ReorderLevel, p.DiscountPercent 
                         FROM Products p 
                         INNER JOIN Categories c ON p.CategoryID = c.CategoryID";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }

        // Product එකක් Update කිරීම
        public bool UpdateProduct(Product product)
        {
            bool isSuccess = false;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"UPDATE Products SET 
                         Barcode = @barcode, ProductName = @name, CategoryID = @catId, 
                         PurchasePrice = @pPrice, SellingPrice = @sPrice, 
                         StockQuantity = @qty, ReorderLevel = @reorder, DiscountPercent = @discount 
                         WHERE ProductID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@barcode", product.Barcode);
                cmd.Parameters.AddWithValue("@name", product.ProductName);
                cmd.Parameters.AddWithValue("@catId", product.CategoryID);
                cmd.Parameters.AddWithValue("@pPrice", product.PurchasePrice);
                cmd.Parameters.AddWithValue("@sPrice", product.SellingPrice);
                cmd.Parameters.AddWithValue("@qty", product.StockQuantity);
                cmd.Parameters.AddWithValue("@reorder", product.ReorderLevel);
                cmd.Parameters.AddWithValue("@discount", product.DiscountPercent);
                cmd.Parameters.AddWithValue("@id", product.ProductID);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0) isSuccess = true;
            }
            return isSuccess;
        }

        // Product එකක් Delete කිරීම
        public bool DeleteProduct(int id)
        {
            bool isSuccess = false;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "DELETE FROM Products WHERE ProductID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0) isSuccess = true;
            }
            return isSuccess;
        }

        // Barcode එකෙන් Product එක හොයන Method එක
        public Product GetProductByBarcode(string barcode)
        {
            Product product = null;
            using (System.Data.SqlClient.SqlConnection conn = Helpers.DatabaseConnection.GetConnection())
            {
                string query = "SELECT ProductID, ProductName, SellingPrice, StockQuantity, DiscountPercent FROM Products WHERE Barcode = @barcode";
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@barcode", barcode);

                conn.Open();
                System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    product = new Product
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        ProductName = reader["ProductName"].ToString(),
                        SellingPrice = Convert.ToDecimal(reader["SellingPrice"]),
                        StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                        DiscountPercent = reader["DiscountPercent"] != DBNull.Value ? Convert.ToDecimal(reader["DiscountPercent"]) : 0
                    };
                }
            }
            return product;
        }

        // Product ID එකෙන් Product එක සම්පූර්ණයෙන් හොයන Method එක (POS එකේ Qty Edit කරද්දී පාවිච්චි කරනවා)
        public Product GetProductById(int productId)
        {
            Product product = null;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT ProductID, ProductName, SellingPrice, StockQuantity, DiscountPercent FROM Products WHERE ProductID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", productId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    product = new Product
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        ProductName = reader["ProductName"].ToString(),
                        SellingPrice = Convert.ToDecimal(reader["SellingPrice"]),
                        StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                        DiscountPercent = reader["DiscountPercent"] != DBNull.Value ? Convert.ToDecimal(reader["DiscountPercent"]) : 0
                    };
                }
            }
            return product;
        }
        
    }
}