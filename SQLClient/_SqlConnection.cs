using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Security.Cryptography;

public class _SqlConnection : AccessConnection
{
    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    //PUBLIC_MEMBERS
    //-----------------------------------------------------------------------------------------------------------------------------------------------------


    //------------------------------------------------------------------------------------------
    //CONSTRUCTORS_DESTRUCTORS
    //------------------------------------------------------------------------------------------

    public _SqlConnection(string connectionString)//(string path, string pass)
    {
        //this._path = path;
        //this._pass = pass;

        try
        {
            _isConnected = false;
            _connection = new SqlConnection();//new OdbcConnection();
            //_connection.ConnectionString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=" + _path + ";Mode= Share Deny None;Uid=Admin;Pwd=" + _pass;
            _connection.ConnectionString = connectionString;//SQLClient.Properties.Settings.Default.connectionString;//connectionString;
            _connection.Open();
        }
        catch (Exception ex)
        {
            Utils.LogMessage(ex.Message);
        }
    }

    //------------------------------------------------------------------------------------------
    //PUBLIC_METHODS
    //------------------------------------------------------------------------------------------

    public override bool ExistDB()
    {
        try
        {
            _connection.Open();
            _connection.Close();            
            return true;
        }
        catch (OleDbException ex)
        {
            Utils.LogMessage("AccessConnection.ExistDB() error=" + ex.Message);
            return false;
        }
    }

    /*public string ExecuteQuery(string query, out int r)
    {
        r = 0;
        try
        {
            if ( _connection.State != ConnectionState.Open )
                _connection.Open();

            OdbcCommand insertCommand = new OdbcCommand(query, _connection);
            insertCommand.ExecuteNonQuery();

            insertCommand.CommandText = "Select @@Identity";
            insertCommand.Prepare();

            return insertCommand.ExecuteScalar().ToString();
        }
        catch (Exception ex)
        {
            r = -1;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Utils.LogMessage("AccessConnection.ExecuteQuery() query=" + query + "error=" + ex.Message);
            return ex.Message;
        }
    }*/

    public override bool ExecuteQuery(string query)
    {
        try
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            SqlCommand insertCommand = new SqlCommand(query, _connection);
            insertCommand.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Utils.LogMessage("AccessConnection.ExecuteQuery() query=" + query + "error=" + ex.Message);
            return false;
        }
    }


    public override DataTable ExecuteSelect(string query )
    {
        DataTable dt = null;

        try
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            SqlDataAdapter da = new SqlDataAdapter(query, _connection);

            DataSet ds = new DataSet();
            da.Fill(ds);

            dt = ds.Tables[0];
        }
        catch (Exception ex)
        {
            Utils.LogMessage("AccessConnection.ExecuteSelect() query=" + query + "error=" + ex.Message);
            throw new Exception(ex.Message);
        }

        return dt;
    }
    public override DbDataReader EQ(string query)
    {
        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        SqlCommand command =
            new SqlCommand(query, _connection);

        SqlDataReader reader = command.ExecuteReader();

        return reader;
    }

    public override DataSet EQ2(string query)
    {
        try
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            SqlDataAdapter da = new SqlDataAdapter(query, _connection);

            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
    public override void ForceClose()
    {
        if (_isConnected)
            _connection.Close();

        _connection.Dispose();        
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    //PRIVATE_MEMBERS
    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------
    //PRIVATE_VARS
    //------------------------------------------------------------------------------------------

    private string _path;
    private string _pass;
    private SqlConnection _connection;
    private bool _isConnected;

    //------------------------------------------------------------------------------------------
    //PRIVATE_METHODS
    //------------------------------------------------------------------------------------------

    private static string md5(string val)
    {
        MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.ASCII.GetBytes(val);
        data = x.ComputeHash(data);
        string ret = "";
        for (int i = 0; i < data.Length; i++)
            ret += data[i].ToString("x2").ToLower();

        return ret;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
}
