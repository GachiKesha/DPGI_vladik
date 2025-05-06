using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace DPGI_vladik
{
    public class AdoAssistant
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager
            .ConnectionStrings["connectionString"].ConnectionString;

        private SqlDataAdapter adapter;
        private DataTable dt;

        public DataTable TableLoad()
        {
            dt = new DataTable();

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("SELECT * FROM [dbo].[Books]", connection);

            adapter = new SqlDataAdapter(command);
            _ = new SqlCommandBuilder(adapter);

            try
            {
                connection.Open();
                adapter.Fill(dt);
                connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading table: " + e.Message);
            }

            return dt;
        }

        public void SaveChanges()
        {
            if (dt == null)
            {
                MessageBox.Show("Please load the table first.");
                return;
            }

            try
            {
                adapter.Update(dt);
                MessageBox.Show("Changes saved successfully.");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error saving changes: " + e.Message);
            }
        }
        public void Paste(string[] values, DataRowView row)
        {
            if (dt == null) return;
            int count = Math.Min(values.Length, dt.Columns.Count);
            for (int i = 0; i < count; i++)
                row[i] = values[i];
        }
        public void Paste(string[] values)
        {
            if (dt == null) return;
            DataRow newRow = dt.NewRow();
            int count = Math.Min(values.Length, dt.Columns.Count);
            for (int i = 0; i < count; i++)
                newRow[i] = values[i];

            dt.Rows.Add(newRow);
        }
    }
}
