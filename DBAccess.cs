using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows;

namespace DPGI_vladik
{
    public class AdoAssistant
    {
        readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        DataTable dt = null;
        public string ISBN { get; set; }
        public string Name { get; set; }
        public string Authors { get; set; }
        public string Publisher { get; set; }
        public int Year { get; set; }

        public DataTable TableLoad()
        {
            dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand(); 
                SqlDataAdapter adapter = new SqlDataAdapter(command);  
                command.CommandText = "SELECT * FROM [dbo].[Books]";
                try
                {
                    adapter.Fill(dt);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return dt;
        }
        public void TableDelete(string ISBN)
        {
            using (SqlConnection сonnection = new SqlConnection(connectionString))
            {
                string strSQL = string.Format("DELETE FROM [dbo].[Books] WHERE ISBN='{0}'", ISBN);
                SqlCommand command = new SqlCommand(strSQL, сonnection);
                сonnection.Open();
                command.ExecuteNonQuery();
                сonnection.Close();
            }
        }
        public void TableUpdate(string ISBN, string Name, string Authors, string Publisher, int Year)
        {
            using (SqlConnection сonnection = new SqlConnection(connectionString))
            {
                string strSQL = string.Format("UPDATE [dbo].[Books] SET Name='{1}',Authors='{2}',Publisher='{3}', Year='{4}' WHERE ISBN='{0}'", ISBN, Name, Authors, Publisher, Year);
                SqlCommand command = new SqlCommand(strSQL, сonnection);
                сonnection.Open();
                command.ExecuteNonQuery();
                сonnection.Close();
            }
        }

        public void TableInsert(string ISBN, string Name, string Authors, string Publisher, int Year)
        {
            if (ISBN == null || Name == null || Authors == null || Publisher == null)
            {
                MessageBox.Show("Enter full data to insert");
                return;
            }
            using (SqlConnection сonnection = new SqlConnection(connectionString))
            {
                string strSQL = string.Format("INSERT INTO [dbo].[Books](ISBN,Name,Authors,Publisher,Year) Values('{0}','{1}','{2}','{3}','{4}')", ISBN, Name, Authors, Publisher, Year);
                SqlCommand command = new SqlCommand(strSQL, сonnection);
                сonnection.Open();
                command.ExecuteNonQuery();
                сonnection.Close();
            }
        }
        public DataTable FindBookByYear(int year)
        {
            DataTable t = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                command.CommandText = string.Format("SELECT * FROM [dbo].[Books] WHERE Year ='{0}'", year);
                try
                {
                    adapter.Fill(t);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                return t;                
            }
        }
        public DataTable FindBookByISBN(string ISBN)
        {
            DataTable t = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                command.CommandText = string.Format("SELECT * FROM [dbo].[Books] WHERE ISBN ='{0}'", ISBN);
                try
                {
                    adapter.Fill(t);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                return t;
            }
        }
    }
}
