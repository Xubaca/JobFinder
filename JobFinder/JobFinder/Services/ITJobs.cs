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

        public string JSON_Name = "";

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

        static public string current_job_XPath = "//div[@class='block borderless']";

        static public string special_offer = "//div[@class='block borderless promoted']";

        public bool Optimized_Search(string searchTerm , string city)
        {
            string processed_searchTerm = searchTerm.Trim().Replace(' ', '+');
            city = city.Trim();
            string processed_city = char.ToUpper(city[0]) + city.Substring(1).ToLower();

            string url = city == ""
                ? $"https://www.itjobs.pt/emprego?q={searchTerm}"
                : $"https://www.itjobs.pt/emprego?q={searchTerm}&location={Locality[processed_city]}";

            JSON_Name = processed_city != "" ? processed_searchTerm + '_' + processed_city + ".json" : processed_searchTerm + ".json";

            HtmlWeb client = new HtmlWeb();

            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

            HtmlAgilityPack.HtmlDocument document = client.Load(url);

            Optimized_Page_Scrapper(ref document, url, element: current_job_XPath);

            return true;
        }

        public bool Optimized_Page_Scrapper(ref HtmlAgilityPack.HtmlDocument document, string url, string element)
        {
            int page_index=1;

            HtmlNode number1_offer = document.DocumentNode.SelectSingleNode(special_offer);

            //its made with a different structure from everything else and it only appears once , so its easier to just out of hand scrape it
            #region special_offer
            if (number1_offer != null)
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

                job_list.Add(current_job);
            }
            #endregion special_offer

            HtmlNodeCollection element_list;
            element_list = document.DocumentNode.SelectNodes(current_job_XPath);

            while (true)
            {

                foreach(var html_element in element_list)
                {
                    Job current_job = new Job();

                    var titleNode = html_element!.SelectSingleNode(".//a[@class='title']");
                    current_job.Title = titleNode != null ? HtmlEntity.DeEntitize(titleNode.InnerText) : "";

                    var companyNode = html_element.SelectSingleNode(".//div[@class='list-name']").SelectSingleNode(".//a");
                    current_job.Company = companyNode != null ? HtmlEntity.DeEntitize(companyNode.InnerText) : "";

                    var locationNode = html_element.SelectSingleNode(".//div[@class='list-details']");
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

                    job_list.Add(current_job);
                }

                element_list = Optimized_Page_Turner( url, page_index);

                if(element_list == null)
                {
                    string complete_path = PROJECT_Directory + @"\JobFinder\DB\" + "IT_" + JSON_Name;
                    //TODO: Save the json , i should organize via {search_term}_{city}.json EXEMPLE: ".NET_Lisboa.json", i should also replace all spaces with underscores
                    using (StreamWriter swriter = new StreamWriter(complete_path))
                    {
                        //System.Text.Json.Serialization.Metadata.JsonTypeInfo js = new System.Text.Json.Serialization.Metadata.JsonTypeInfo() { };
                        swriter.Write(System.Text.Json.JsonSerializer.Serialize(value: job_list));
                    }
                    break;
                }
                page_index++;
            }

            return false;
        }

        public HtmlNodeCollection Optimized_Page_Turner( string url, int page_index)
        {
            HtmlWeb client = new();

            string full_url = url + $"&page={page_index+1}";
            HtmlAgilityPack.HtmlDocument document = client.Load(full_url);

            var jobs = document.DocumentNode.SelectNodes(current_job_XPath);

            return jobs;
        }
    }
}
