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

namespace JobFinder.Services
{
    class SapoEmprego
    {
        static /*public*/ int page_counter = 1;

        static public void PageIncremet() => page_counter += 1;
        static public void ResetPageCounter() => page_counter = 1;
        public static async Task<string> Search(string search_term, string city)
        {
            
            var handler = new HttpClientHandler
            {
                UseCookies = false // We'll set manual Cookie header
            };

            using var client = new HttpClient(handler);

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://emprego.sapo.pt/offers/search?local={city.ToLower()}&categoria=informatica-tecnologias&pesquisa={search_term.ToLower()}&pagina={page_counter}&ordem=relevancia");
            // Add all required headers
            request.Headers.Accept.ParseAdd("application/json, text/plain, */*");
            request.Headers.Add("accept-language", "en-US,en;q=0.6");
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("pragma", "no-cache");
            request.Headers.Add("priority", "u=1, i");
            request.Headers.Referrer = new Uri($"https://emprego.sapo.pt/offers?local={city.ToLower()}&categoria=informatica-tecnologias&pesquisa={search_term.ToLower()}&pagina={page_counter}&ordem=relevancia");
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

            // Send and read response
            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine(body);

            MessageBox.Show(body);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) SapoEmprego.PageIncremet();

            return response.StatusCode == System.Net.HttpStatusCode.OK ? body : "Error";
        }
    }
}
