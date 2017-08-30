using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Diagnostics;

public abstract class AccessConnection
{
    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    //PUBLIC_MEMBERS
    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------
    //PUBLIC_VARS
    //------------------------------------------------------------------------------------------

    public string Path
    {
        get { return _path; }
    }

    public string Pass
    {
        get { return _pass; }
    }

    //------------------------------------------------------------------------------------------
    //CONSTRUCTORS_DESTRUCTORS
    //------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------
    //PUBLIC_METHODS
    //------------------------------------------------------------------------------------------

    public abstract bool ExistDB();
    /*public string ExecuteQuery(string query, out int r)
    {
        r = 0;
        try
        {
            if ( _conection.State != ConnectionState.Open )
                _conection.Open();

            OdbcCommand insertCommand = new OdbcCommand(query, _conection);
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

    public abstract bool ExecuteQuery(string query);


    public abstract DbDataReader EQ(string query);
    public abstract DataSet EQ2(string query);

    public abstract DataTable ExecuteSelect(string query);


    public abstract void ForceClose();


    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    //PRIVATE_MEMBERS
    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------
    //PRIVATE_VARS
    //------------------------------------------------------------------------------------------

    private string _path;
    private string _pass;
    private SqlConnection _conection;
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
