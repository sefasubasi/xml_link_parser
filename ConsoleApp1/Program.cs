using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using static System.Net.WebRequestMethods;
using System.Xml;

namespace ReadingDataFromCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<Contact>();
            SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=device_links;Integrated Security=True");
            con.Open();
            string com = "select * from links";
            SqlCommand all_data = new SqlCommand(com, con);
            SqlDataReader reader = all_data.ExecuteReader();

            while (reader.Read())
            {
                var contact = new Contact() { Device = reader.GetString(0), XmlLink = reader.GetString(1) };
                list.Add(contact);
            }
            con.Close();


            foreach(var x in list)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(x.XmlLink);

                string pattern = @"https?://[\w\.\-]+(/[\w\-\./?%&=]*)?";
                XmlNodeList urlNodes = doc.SelectNodes("//url");
                foreach (XmlNode urlNode in urlNodes)
                {
                    string url = urlNode.InnerText;
                    Match match = Regex.Match(url, pattern);
                    if (match.Success)
                    {
                        Console.Write(x.Device + "->");
                        Console.WriteLine("Found URL: " + match.Value);
                    }
                    else
                    {
                        Console.WriteLine("Invalid URL format: " + url);
                    }
                }
            }

           



        }

        public class Contact
        {
            public string Device { get; set; }
            public string XmlLink { get; set; }

        }


    }
}
