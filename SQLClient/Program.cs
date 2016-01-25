using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLClient
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlClient sql = new SqlClient();
            bool exit = false;
            string line = "";
            do
            {
                line = Console.ReadLine();
                line = line.ToLower();
                if (line == "clear")
                {
                    Console.Clear();
                }
                else if(line == "exit")
                {
                    exit = true;
                }
            } while (!exit);
            sql.Exit();
        }
    }
}
