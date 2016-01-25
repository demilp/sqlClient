using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Bypass.SimpleJSON;

public class DB
{
    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    //PUBLIC_MEMBERS
    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------
    //PUBLIC_CLASS
    //------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------
    //PUBLIC_VARS
    //------------------------------------------------------------------------------------------

    public static DB Instance
    {
        get
        {
            if (_instance == null) _instance = new DB();
            return _instance;
        }
    }

    //------------------------------------------------------------------------------------------
    //PUBLIC_METHODS
    //------------------------------------------------------------------------------------------

    public void Initialize( string pPath)
    {
        if (_initialized) return;
        _conn = new AccessConection(pPath);
        _initialized = true;
    }
    public void Close()
    {
        if (!_initialized) return;

        _conn.ForceClose();
    }
    public string ExecuteQuery(string pQuery, out bool r)
    {
        r = true;
        JSONArray a = new JSONArray();
        JSONClass j = new JSONClass();
        if (!_initialized)
        {
            j["result"] = "not_initialized";
            a.Add(j);
            //j["data"] = a;
            r = false;
            return a.ToString();
        }
        bool s = _conn.ExecuteQuery(pQuery);
        j["result"] = s?"ok":"error";
        a.Add(j);
        return a.ToString();
    }
    public string ExecuteSelect(string pQuery, out bool s)
    {
        s = true;
        //JSONClass j = new JSONClass();
        JSONArray a = new JSONArray();
        //j["data"] = a;
        //if (!_initialized) return "{\"result\":" + "[\"not_initialized\"]" + "}";
        if (!_initialized)
        {
            JSONClass r = new JSONClass();
            r["result"] = "not_initialized";
            s = false;
            a.Add(r);
            //j["data"] = a;
            return a.ToString();
        }

        //string data = "";
        try
        {
            DataTable table = _conn.ExecuteSelect(pQuery);

            if (table != null)
            {
                //data += "[";

                //int r = 0;
                List<string> columnNames = new List<string>();
                foreach (DataColumn column in table.Columns)
                {
                    columnNames.Add(column.ColumnName);
                }
                foreach (DataRow row in table.Rows)
                {
                    //data += "{";

                    int i = 0;
                    JSONClass _r = new JSONClass();
                    foreach (object value in row.ItemArray)
                    {
                        _r[columnNames[i]] = value.ToString();
                        //data += "\"" + columnNames[i] + "\":\"" + value + "\",";
                        i++;
                    }
                    a.Add(_r);
                    //if (i > 0)
                        //data = data.Substring(0, data.LastIndexOf(","));

                    //data += "},";
                    //r++;
                }

                /*if (r > 0)
                    data = data.Substring(0, data.LastIndexOf(","));

                data += "]";
                data = "\"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) + "\"";
                data += "}";*/
            }
            else
            {
                JSONClass _r = new JSONClass();
                s = false;
                _r["result"] = "table is null";
                a.Add(_r);
                /*data += "table is null";
                data = "\"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) + "\"";
                data += "}";*/
            }
        }
        catch(Exception e)
        {
            JSONClass _r = new JSONClass();
            _r["result"] = e.Message;
            s = false;
            a.Add(_r);
            //data +=e.Message;
            //data = "\"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) + "\"";
            //data += "}";
        }
        return a.ToString();
        //return "{\"result\":" + data;
    }
    public DataTable ExecuteSelect2(string query)
    {
        if (!_initialized) return null;
        return _conn.ExecuteSelect(query);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    //PRIVATE_MEMBERS
    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------
    //PRIVATE_VARS
    //------------------------------------------------------------------------------------------

    private static DB _instance;
    private AccessConection _conn;
    private bool _initialized;

    //------------------------------------------------------------------------------------------
    //CONSTRUCTORS_DESTRUCTORS
    //------------------------------------------------------------------------------------------

    private DB()
    {
        _initialized = false;
    }

    private string GetNowStr()
    {
        DateTime now = DateTime.Now;
        return ("#" + now.ToString("yyyy/MM/dd HH:mm:ss") + "#");
    }

    private string GetNowDateStr()
    {
        DateTime now = DateTime.Now;
        return ("(" + now.ToString("yyyy-MM-dd") + ")");
    }
}
