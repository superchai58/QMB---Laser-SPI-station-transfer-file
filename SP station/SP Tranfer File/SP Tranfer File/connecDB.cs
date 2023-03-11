using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

public class connecDB
{
    private bool useDB = true;
    private string connectionString = null;
    public connecDB()
    {
        connectionString = "Data Source=10.97.2.11;Initial Catalog=SMT; Persist Security Info=True; User ID=sa;Password=pqmb#7sa;";
        SqlConnection conn = new SqlConnection(connectionString);
        conn = new SqlConnection(connectionString);
        try
        {
            conn.Open();
            //MessageBox.Show("Connection Open ! ");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Can not open connection ! ");
        }
    }

    public DataTable Query(SqlCommand commandDb)
    {


        if (useDB)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            commandDb.Connection = conn;
            SqlDataAdapter adapter = new SqlDataAdapter(commandDb);
            DataTable dTable = new DataTable();
            DataSet dSet = new DataSet();

            try
            {
                adapter.Fill(dSet, "dataTable");
                return dSet.Tables["dataTable"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return dTable;
            }

        }
        else
        {
            return new DataTable();
        }

    }

}
