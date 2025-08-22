using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;
using System;
using System.Threading.Tasks;
using OpenQA.Selenium.Internal;
using JobFinder.Model;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace JobFinder.Services
{
    class SapoEmprego
    {
        // This will get the current WORKING directory (i.e. \bin\Debug)
        static string workingDirectory = Environment.CurrentDirectory;

        // This will get the current PROJECT directory
        static string PROJECT_Directory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;

        static Random rdn = new Random();

        public List<Job> job_list = new List<Job>();

        static /*public*/ int page_counter = 1;

        public string JSON_Name = "";

        static public void PageIncremet() => page_counter += 1;

        //Job oferring json DTO(data transfer object)
        #region JobOffer_DTO
        public class SapoEmpregoRoot
        {
            [JsonPropertyName("offers")]
            public List<offers> Offers { get; set; }
        }

        public class offers
        {
            [JsonPropertyName("offer_name")]
            public string? OfferName { get; set; }

            [JsonPropertyName("company_name")]
            public string? CompanyName { get; set; }

            [JsonPropertyName("location")]
            public string? Location { get; set; }

            [JsonPropertyName("link")]
            public string? Link { get; set; }

            [JsonPropertyName("remote_work")]
            public bool? RemoteWork { get; set; }

            [JsonPropertyName("job_description")]
            public string? JobDescription { get; set; }

            // Para casos em que "company" é um objeto, pode adicionar:
            [JsonPropertyName("company")]
            public CompanyInfo? Company { get; set; }

            // Para casos em que "external_details" existe:
            [JsonPropertyName("external_details")]
            public ExternalDetails? ExternalDetails { get; set; }
        }

        public class CompanyInfo
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }
            [JsonPropertyName("name")]
            public string? Name { get; set; }
            // Adicione outros campos se necessário
        }

        public class ExternalDetails
        {
            [JsonPropertyName("link")]
            public string? Link { get; set; }
            [JsonPropertyName("title")]
            public string? Title { get; set; }
            [JsonPropertyName("description")]
            public string? Description { get; set; }
        }
        #endregion JobOffer_DTO
        static public void ResetPageCounter() => page_counter = 1;

        public void Search(string search_term, string city="")
        {
            string processed_searchTerm = search_term.Trim().Replace(' ', '_');
            string processed_city = city.Trim().Replace(' ', '_');
            processed_city = char.ToUpper(processed_city[0]) + processed_city.Substring(1).ToLower();
            JSON_Name = city != "" ? processed_searchTerm + '_' + processed_city + ".json" : processed_searchTerm + ".json";


            var handler = new HttpClientHandler
            {
                UseCookies = false // We'll set manual Cookie header
            };
            
            //$"https://emprego.sapo.pt/offers?local={city.ToLower()}&categoria=informatica-tecnologias&pesquisa={search_term.ToLower()}&pagina={page_counter}&ordem=relevancia";
            string url = city == ""
                ? $"https://emprego.sapo.pt/offers?&categoria=informatica-tecnologias&pesquisa={search_term.ToLower()}&ordem=relevancia"
                : $"https://emprego.sapo.pt/offers?local={city}&categoria=informatica-tecnologias&pesquisa={search_term.ToLower()}&ordem=relevancia";

            using var client = new HttpClient(handler);
            #region headers
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://emprego.sapo.pt/offers/search?local={city.ToLower()}&categoria=informatica-tecnologias&pesquisa={search_term.ToLower()}&pagina={page_counter}&ordem=relevancia");
            // Add all required headers
            request.Headers.Accept.ParseAdd("application/json, text/plain, */*");
            request.Headers.Add("accept-language", "en-US,en;q=0.6");
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("pragma", "no-cache");
            request.Headers.Add("priority", "u=1, i");
            request.Headers.Referrer = new Uri(url);
            request.Headers.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Brave\";v=\"138\"");
            request.Headers.Add("sec-ch-ua-mobile", "?0");
            request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
            request.Headers.Add("sec-fetch-dest", "empty");
            request.Headers.Add("sec-fetch-mode", "cors");
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("sec-gpc", "1");
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
            request.Headers.Add("x-csrf-token", "waBxhdkCGq4lraQKYZfHSUSnd4Weya54wikrDjV0");
            request.Headers.Add("x-requested-with", "XMLHttpRequest");
            request.Headers.Add("x-xsrf-token", "eyJpdiI6Ikpzc0ZqTFdTdEJYTlk1cHQyYy9SZ1E9PSIsInZhbHVlIjoiWWVvWTdKaWNUbTZVRldQVkFIRE5sTXFWM2RrYUZuRkpneGxUUXF3RWdwdFhLdjR3U2lWdUdsWDhJQ1dUYUIwa2NvV2IvQmowSUNKbm4wZ2VncEFVZFc1UGMxYlV4cll3NWlzK3F4YWZ5UGhGblYxUkRyTnllSnY4SGliYk9lbC8iLCJtYWMiOiIyMGJmZWZiYWU1YWZlMDY2ODExMWIwMTJjYjAyMDY4MDlmMWMwN2U1NTNiMWJmOThlODI0MjNmZGI1NGViNzU5IiwidGFnIjoiIn0=");

            // Add Cookie header manually (since auto cookie handler is off)
            request.Headers.Add("Cookie",
                "se_ro=eyJpdiI6ImEwWEtObVZmSDZZSTRES0NzRXJGZUE9PSIsInZhbHVlIjoiNWdweE41eVA4R2x3VHdHNDZiK05GUUFmdnV1ZC9OYjNYMEtoUnhQRDVuOHBIbmRVaTVLdUk1YVVTaHlQc0FEQVdrWFI3TnpVZ2NlR2N3eVNYYXhsQUIySDExTVk1RVlLR2NCVUVSZmJQRmN6cTlxeFpPaUs0UmZ0Unp0ZW9iZWxpc0s0WFFLR1BLL2ppRGJtVDdCWVFTK3dZV0xtUGJxZlBaTHkzcFpqZ0p4Z1VYVHhuS1h2cS9MSW0reDVabDhQUFEzbXVTNE42QmhYVTNuSUtYYUlkODVMbE5jSUExOVdDdDNIQy9RMnI1K1FoV2V6dU8xcE5TOC84WVMvT3NsQUxVZmZvNGpWbCt3VmR1QkpWVDFzUWFMd2tMOFN1dzBaUTNXU1dkMjl6S21YRTlvL294ZDZwYWZzalpRcUlBaXMiLCJtYWMiOiJjM2JjMzljOGZjODM2ZWRkNjlkOWRjMGRjYmJlZmJkMWFlZjIyZDFiNDE4ZmIyYTgzMmVmNzQ2NzYwNDBlNzgzIiwidGFnIjoiIn0%3D; XSRF-TOKEN=eyJpdiI6Ikpzc0ZqTFdTdEJYTlk1cHQyYy9SZ1E9PSIsInZhbHVlIjoiWWVvWTdKaWNUbTZVRldQVkFIRE5sTXFWM2RrYUZuRkpneGxUUXF3RWdwdFhLdjR3U2lWdUdsWDhJQ1dUYUIwa2NvV2IvQmowSUNKbm4wZ2VncEFVZFc1UGMxYlV4cll3NWlzK3F4YWZ5UGhGblYxUkRyTnllSnY4SGliYk9lbC8iLCJtYWMiOiIyMGJmZWZiYWU1YWZlMDY2ODExMWIwMTJjYjAyMDY4MDlmMWMwN2U1NTNiMWJmOThlODI0MjNmZGI1NGViNzU5IiwidGFnIjoiIn0%3D; sapo_emprego_session=eyJpdiI6IjgvbVRkT25CWDB3VEo4WGU2bTM3SGc9PSIsInZhbHVlIjoiMzF5ZDNZLy9YNi9EZUREVlQvZEdMSVJkUnhKTUJmSGJUQWRqMDZCcnlvL2R3T1BsZkdQR1A3TmhmRE92YTM0a0Vmb2w5QlM5UnZDVytwV2JqM0JQMEpDRkpnQUgybnd1dURvRUlzZ0lXUlo4dEorQnAwY1RweStBUGNpNkpPRnAiLCJtYWMiOiJjYzQ1ODE5MDY4YThhZjM2YjljZTdjNjFiNTI5ZTA2MTI1MzkyZDA4NmExNDA1MTFlZmQyYjRkMmRlYzIzNDZhIiwidGFnIjoiIn0%3D " +
                "XSRF-TOKEN=eyJpdiI6Ikpzc0ZqTFdTdEJYTlk1cHQyYy9SZ1E9PSIsInZhbHVlIjoiWWVvWTdKaWNUbTZVRldQVkFIRE5sTXFWM2RrYUZuRkpneGxUUXF3RWdwdFhLdjR3U2lWdUdsWDhJQ1dUYUIwa2NvV2IvQmowSUNKbm4wZ2VncEFVZFc1UGMxYlV4cll3NWlzK3F4YWZ5UGhGblYxUkRyTnllSnY4SGliYk9lbC8iLCJtYWMiOiIyMGJmZWZiYWU1YWZlMDY2ODExMWIwMTJjYjAyMDY4MDlmMWMwN2U1NTNiMWJmOThlODI0MjNmZGI1NGViNzU5IiwidGFnIjoiIn0; " +
                "sapo_emprego_session=waBxhdkCGq4lraQKYZfHSUSnd4Weya54wikrDjV0"
            );
            #endregion

            // Send and read response
            HttpResponseMessage? response = client.Send(request);
            string body;
            Task<string?> read_body = Task.Factory.StartNew<string?>(() => response.Content.ReadAsStringAsync().Result);
            read_body.Wait();
            body = read_body.Status == TaskStatus.RanToCompletion ? read_body.Result! : "";

            //Console.WriteLine($"Status: {response.StatusCode}");
            //Console.WriteLine(body);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };


            SapoEmpregoRoot? root = JsonSerializer.Deserialize<SapoEmpregoRoot>(body, options);
            List<offers> result = root?.Offers ?? new List<offers>();
            // Agora 'result' contém a lista de ofertas, com campos opcionais tratados

            JsonSave(JsonSerializer.Serialize(result.Where(x => x.Company == null)));

            Task.WaitAll();
            //MessageBox.Show(body);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) SapoEmprego.PageIncremet();

            //return response.StatusCode == System.Net.HttpStatusCode.OK ? body : "Error";
        }

        //public bool Optimized_Page_Scrapper(ref HttpClient? client , ref HttpRequestMessage? request, string url)
        //{
        //    //first Request
        //    HttpResponseMessage? response = client.Send(request);
        //    do
        //    {
        //        string body;
        //        Task<string?> read_body = Task.Factory.StartNew<string?>(() => response.Content.ReadAsStringAsync().Result);
        //        read_body.Wait();
        //    } 
        //    while (response.StatusCode == System.Net.HttpStatusCode.OK);

        //    return true;
        //}

        public bool JsonSave(string body)
        {
            string complete_path = PROJECT_Directory + @"\JobFinder\DB\" + "SE_" + JSON_Name;
            //TODO: Save the json , i should organize via {search_term}_{city}.json EXEMPLE: ".NET_Lisboa.json", i should also replace all spaces with underscores
            using (StreamWriter swriter = new StreamWriter(complete_path))
            {
                //System.Text.Json.Serialization.Metadata.JsonTypeInfo js = new System.Text.Json.Serialization.Metadata.JsonTypeInfo() { };
                swriter.Write(body/*System.Text.Json.JsonSerializer.Serialize(value: job_list)*/);
            }

            return true;
        }
    }
}
