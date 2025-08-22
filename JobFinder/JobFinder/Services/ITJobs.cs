using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobFinder.Services
{
    class ITJobs
    {

        // This will get the current WORKING directory (i.e. \bin\Debug)
        static string workingDirectory = Environment.CurrentDirectory;

        // This will get the current PROJECT directory
        static string PROJECT_Directory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;

        static Random rdn = new Random();

        //var reverseLocality = Locality.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        Dictionary<string,int> Locality = new Dictionary<string, int>
        {
            { "Lisboa", 14 },
            { "Porto", 18 },
            { "Braga", 4 },
            { "Aveiro", 1 },
            { "Coimbra", 8 },
            { "Setúbal", 17 },
            { "Internacional", 29 },
            { "Viana do Castelo", 22 },
            { "Castelo Branco", 6 },
            { "Viseu", 16 },
            { "Leiria", 13 },
            { "Bragança", 5 },
            { "Guarda", 11 },
            { "Santarém", 20 },
            { "Faro", 9 },
            { "Évora", 10 },
            { "Portalegre", 12 },
            { "Beja", 3 },
            { "Vila Real", 21 },
            { "Madeira", 15 }
        };

        static public string current_job_XPath = "//div[@class='job-item media']";

        public bool Optimized_Search(string searchTerm , string city)
        {
            string processed_searchTerm = searchTerm.Trim().Replace(' ', '+');
            city = city.Trim();
            string processed_city = char.ToUpper(city[0]) + city.Substring(1);

            string url = city == ""
                ? $"https://www.itjobs.pt/emprego?q={searchTerm}"
                : $"https://www.itjobs.pt/emprego?q={searchTerm}&location={Locality[processed_city]}";



            return true;
        }
    }
}
