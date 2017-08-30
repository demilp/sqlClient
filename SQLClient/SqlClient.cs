using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using Bypass;
using ExportToExcel;

namespace SQLClient
{
    public class SqlClient
    {
        public BypassClient socket;
        public SqlClient()
        {
            LoadReportSettings();
            socket = new BypassClient(ConfigurationManager.AppSettings["server.ip"], int.Parse(ConfigurationManager.AppSettings["server.port"]), ConfigurationManager.AppSettings["server.delimitador"], "sql"+ ConfigurationManager.AppSettings["bypassId"], "tool");
            socket.CommandReceivedEvent += OnData;
            DB.Instance.Initialize(ConfigurationManager.AppSettings["db.connectionString"]);
        }


        public void OnData(object sender, CommandEventArgs data)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("Received: " + data.comando);
            string[] p = data.comando.Split('|');
            if(p.Length < 2)
            {
                return;
            }
            string senderId = p[0];
            string queryId = p[1];
            if (p.Length > 3)
            {
                if (p[2].ToLower()=="report")
                {
                    int settingIndex = -1;
                    if (int.TryParse(p[3], out settingIndex) && reportSettings.Count >= settingIndex)
                    {
                        Report(reportSettings[settingIndex-1].query, reportSettings[settingIndex - 1].outputPath);
                    }
                    else
                    {
                        Console.WriteLine(p[3]+" is not a valid setting index");
                    }
                }
                else
                {
                    Report(p[2], p[3]);
                }
            }
            else
            {
                string query = "";
                for (int i = 2; i < p.Length; i++)
                {
                    query += p[i];
                }
                query = query.Trim();
                //Descapeo la comilla
                //Console.WriteLine(query.IndexOf("\""));
                //query.Replace("\"", "\"");
                p = query.Split(' ');
                if (p[0].ToLower() == "select")
                {
                    bool s = true;
                    string response = DB.Instance.ExecuteSelect(query, out s);
                    if (s)
                    {
                        socket.SendData(GetJSon(response, queryId, "ok"), "", senderId);
                    }
                    else
                    {
                        socket.SendData(GetJSon(response, queryId, "error"), "", senderId);
                    }
                
                }
                else
                {
                    bool s = true;
                    string r = DB.Instance.ExecuteQuery(query, out s);
                    /*if (r == "" || r == "0")
                    {
                        r = "success";
                    }
                    else
                    {
                        Console.WriteLine(r);
                    }*/
                    string j;
                    if (s)
                    {
                        j = GetJSon(r, queryId, "ok");
                    }
                    else
                    {
                        j = GetJSon(r, queryId, "error");
                    }
                
                    socket.SendData(j, "", senderId);
                    /*if (r != "-1")
                    {
                        socket.SendData("{\"type\":\"send\",\"ids\":[\"" + senderId + "\"],\"data\":\'{\"result\":\"b2s=\",\"queryId\":\"" + queryId + "\"}\'}");
                    }
                    else
                    {
                        socket.SendData("{\"type\":\"send\",\"ids\":[\"" + senderId + "\"],\"data\":\'{\"result\":\"ZXJyb3I=\",\"queryId\":\"" + queryId + "\"}\'}");
                    }*/
                }
            }
        }
        public void Exit()
        {
            socket.Close();
        }
        public struct Setting
        {
            public string query;
            public string outputPath;
        }

        List<Setting> reportSettings; 
        void LoadReportSettings()
        {
            reportSettings = new List<Setting>();
            bool f = true;
            int c = 0;
            while (f)
            {
                c++;
                if (ConfigurationManager.AppSettings[c + "_query"] != null)
                {
                    Setting s = new Setting();
                    s.query = ConfigurationManager.AppSettings[c + "_query"];
                    s.outputPath = ConfigurationManager.AppSettings[c + "_outputPath"];
                    reportSettings.Add(s);
                }
                else
                {
                    f = false;
                }

            }
        }

        void Report(string query, string filePath)
        {
            DataSet ds = DB.Instance.EQ2(query);
            CreateExcelFile.CreateExcelDocument(ds, Path.ChangeExtension(filePath, "xlsx"));


            DbDataReader dr = DB.Instance.EQ(query);
            StringBuilder sb = new StringBuilder();
            bool first = true;
            while (dr.Read())
            {
                if (first)
                {
                    string s = "";
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        s += dr.GetName(i);
                        if (i < dr.FieldCount - 1)
                        {
                            s += ";";
                        }
                    }
                    sb.AppendLine(s);
                }
                first = false;
                IDataRecord record = (IDataRecord)dr;
                for (int i = 0; i < record.FieldCount; i++)
                {
                    sb.Append(record[i] + ";");
                }
                sb.AppendLine();
            }
            File.WriteAllText(Path.ChangeExtension(filePath, "csv"), sb.ToString(0, sb.Length));
            if (dr != null)
                dr.Close();
        }

        public string GetJSon(string data, string queryId, string result)
        {
            
            Bypass.SimpleJSON.JSONClass j = new Bypass.SimpleJSON.JSONClass();

            j["result"] = result;
            j["queryId"] = queryId;
            j["data"] = data;
            return j.ToString();
            
        }
    }
}
