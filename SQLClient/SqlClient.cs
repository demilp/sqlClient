using System;
using System.Configuration;
using System.IO;
using System.Text;
using Bypass;

namespace SQLClient
{
    public class SqlClient
    {
        public BypassClient socket;
        public SqlClient()
        {
            socket = new BypassClient(ConfigurationManager.AppSettings["server.ip"], int.Parse(ConfigurationManager.AppSettings["server.port"]), ConfigurationManager.AppSettings["server.delimitador"], "sql", "tool");
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
        public void Exit()
        {
            socket.Close();
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
