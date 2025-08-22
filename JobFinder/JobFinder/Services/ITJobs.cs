using HtmlAgilityPack;
using JobFinder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JobFinder.Services
{
    class ITJobs
    {

        // This will get the current WORKING directory (i.e. \bin\Debug)
        static string workingDirectory = Environment.CurrentDirectory;

        // This will get the current PROJECT directory
        static string PROJECT_Directory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;

        static Random rdn = new Random();

        List<Job> job_list = new();

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

        static public string special_offer = "//div[@class='block borderless promoted']";

        public bool Optimized_Search(string searchTerm , string city)
        {
            string processed_searchTerm = searchTerm.Trim().Replace(' ', '+');
            city = city.Trim();
            string processed_city = char.ToUpper(city[0]) + city.Substring(1).ToLower();

            string url = city == ""
                ? $"https://www.itjobs.pt/emprego?q={searchTerm}"
                : $"https://www.itjobs.pt/emprego?q={searchTerm}&location={Locality[processed_city]}";

            HtmlWeb web = new HtmlWeb();

            HtmlAgilityPack.HtmlDocument document = web.Load(url);

            Optimized_Page_Scrapper(ref document, url, element: current_job_XPath);

            return true;
        }

        public bool Optimized_Page_Scrapper(ref HtmlAgilityPack.HtmlDocument document, string url, string element)
        {
            int page_index;
            bool final_page = false;

            HtmlNode number1_offer = document.DocumentNode.SelectSingleNode(special_offer);

            if(number1_offer != null)
            {
                Job current_job = new Job();

                var titleNode = number1_offer!.SelectSingleNode(".//a[@class='title']");
                current_job.Title = titleNode != null ? HtmlEntity.DeEntitize(titleNode.InnerText) : "";

                var companyNode = number1_offer.SelectSingleNode(".//div[@class='list-name']").SelectSingleNode(".//a");
                current_job.Company = companyNode != null ? HtmlEntity.DeEntitize(companyNode.InnerText) : "";

                var locationNode = number1_offer.SelectSingleNode(".//div[@class='list-details']");
                if (locationNode != null)
                {
                    string locationText = HtmlEntity.DeEntitize(locationNode.InnerText);
                    locationText = locationText.Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");
                    locationText = System.Text.RegularExpressions.Regex.Replace(locationText, @"\s+", " ").Trim();

                    List<string> foundCities = new();
                    foreach (var city in Locality.Keys)
                    {
                        if (locationText.Contains(city, StringComparison.OrdinalIgnoreCase))
                        {
                            foundCities.Add(city);
                        }
                    }

                    locationText = locationText.Replace("Full-time", "");

                    current_job.Location = foundCities.Count > 0 ? string.Join(", ", foundCities) : locationText;
                }

                current_job.Url = titleNode != null && titleNode.Attributes["href"] != null
                    ? HtmlEntity.DeEntitize(titleNode.Attributes["href"].Value)
                    : "";

                current_job.Salary = null; //there isnt a salary description on the page
            }

            //HtmlAgilityPack.HtmlNodeCollection element_list;
            //while (final_page)
            //{
            //    element_list = document.DocumentNode.SelectNodes(current_job_XPath);


            //    foreach(HtmlNode html_element in element_list)
            //    {

            //    }

            //}


            return false;
        }
    }
}
