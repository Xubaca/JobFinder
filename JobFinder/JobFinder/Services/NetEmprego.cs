using JobFinder.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.DevTools.V136.IndexedDB;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using HtmlAgilityPack;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;


namespace JobFinder.Services
{
    class NetEmprego
    {
        // This will get the current WORKING directory (i.e. \bin\Debug)
        static string workingDirectory = Environment.CurrentDirectory;

        // This will get the current PROJECT directory
        static string PROJECT_Directory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;

        static Random rdn = new Random();

        //XPath to job postings divs
        static public string current_job_XPath = "//div[@class='job-item media']";
        static public string last_page_marker = "//*[@id='btn-voltar']";

        public string JSON_Name = "";
        public enum HTML_Action
        {
            Click = 1,
            Form = 2,
            Radio = 3,
            ComboBox = 4
        }

        public List<Job> job_list = new List<Job>();

        static Dictionary<string, int> regiao = new Dictionary<string, int>()
        {
            { "Todas as Zonas",0 },
            { "Açores",25 },
            { "Aveiro",4 },
            { "Beja",15 },
            { "Braga",3 },
            { "Bragança",5 },
            { "Castelo Branco",10 },
            { "Coimbra",9 },
            { "Evora",14 },
            { "Faro",17 },
            { "Guarda",7 },
            { "Leiria",11 },
            { "Lisboa",1 },
            { "Madeira",26 },
            { "Portalegre",16 },
            { "Porto",2 },
            { "Santarem",12 },
            { "Setubal",13 },
            { "Viana do Castelo",28 },
            { "Vila Real",6 },
            { "Viseu",8 },
            { "Outros Locais - Estrangeiro",29 }
        };

        //To avoid getting caught by rate limiting
        static private void VariableWaitTime(int min = 0,int max = 0)
        {
            int min_range = (min != 0?min : 2), max_range = (max != 0 ? max : 4);
            Thread.Sleep(rdn.Next(min_range * 1000, max_range * 1000));
        }

        #region HTMLAgilityPack

        public void Optimized_Search(string search_term, string city = "")
        {
            //set up for the save inside the DB Folder
            string processed_searchTerm = search_term.Trim().Replace(' ', '_');
            string processed_city = city.Trim().Replace(' ', '_');
            JSON_Name = processed_city != "" ? processed_searchTerm + '_' + processed_city + ".json" : processed_searchTerm + ".json";

            int zone = 0;

            if (city != "")
            {
                city = char.ToUpper(city[0]) + city.Substring(1);
                zone = regiao[city];
            }

            //initializing the HTTP Client
            HtmlWeb web = new HtmlWeb();

            string url = city != "" 
                ? $"https://www.net-empregos.com/pesquisa-empregos.asp?chaves={search_term}&cidade={city}&categoria=5&zona={zone}&tipo=0"
                : $"https://www.net-empregos.com/pesquisa-empregos.asp?chaves={search_term}&categoria=5&tipo=0";

            HtmlAgilityPack.HtmlDocument document = web.Load(url);

            Optimized_Page_Scrapper(ref document, url, element: current_job_XPath);

        }
        public bool Optimized_Page_Scrapper(ref HtmlAgilityPack.HtmlDocument document, string url, string element)
        {
           
            int page_index = 1;
            bool final_page = false;

            while (true)
            {
                HtmlNodeCollection elementList;
                elementList = document.DocumentNode.SelectNodes(current_job_XPath);

                foreach (HtmlNode html_element in elementList)
                {
                    Job current_job = new Job();

                    var titleNode = html_element.SelectSingleNode(".//a[@class='oferta-link']");
                    current_job.Title = titleNode != null ? HtmlEntity.DeEntitize(titleNode.InnerText) : "";

                    var companyNode = html_element.SelectSingleNode(".//li[i[contains(@class, 'flaticon-work')]]");
                    current_job.Company = companyNode != null ? HtmlEntity.DeEntitize(companyNode.InnerText) : "";

                    var locationNode = html_element.SelectSingleNode(".//li[i[contains(@class, 'flaticon-pin')]]");
                    current_job.Location = locationNode != null ? HtmlEntity.DeEntitize(locationNode.InnerText) : "";

                    current_job.Url = titleNode != null && titleNode.Attributes["href"] != null
                        ? HtmlEntity.DeEntitize(titleNode.Attributes["href"].Value)
                        : "";

                    current_job.Salary = null; //there isnt a salary description on the page

                    lock (job_list)
                    {
                        job_list.Add(current_job);
                    }

                }
                final_page = Optimized_Page_Turner(page_index: page_index, url);
                if (final_page)
                {
                    string complete_path = PROJECT_Directory + @"\JobFinder\DB\" + "NE_"+JSON_Name;
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

            return false; //should never reach here, but just in case
        }

        static public bool Optimized_Page_Turner(int page_index, string url)
        {
            //Human like wait time
            VariableWaitTime(1, 3);
            HtmlWeb client = new HtmlWeb();
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";
            string full_url = url + $"&page={page_index + 1}";
            HtmlAgilityPack.HtmlDocument doc = client.Load(full_url);

            HtmlNode lastPage = doc.DocumentNode.SelectSingleNode(last_page_marker);

            if (lastPage == null) return false;

            return true;
        }

        #endregion HTMLAgilityPack

        #region selenium

        public void Search(string search_term, string city = "")
        {
            //set up for the save inside the DB Folder
            string processed_searchTerm = search_term.Trim().Replace(' ', '_');
            string processed_city = city.Trim().Replace(' ', '_');
            JSON_Name = processed_city != "" ? processed_searchTerm + '_' + processed_city + ".json" : processed_searchTerm + ".json";

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");

            ChromeDriver driver = new ChromeDriver(options/*PROJECT_Directory + @"\chromedriver.exe"*/);
            city = char.ToUpper(city[0]) + city.Substring(1);
            int zone = regiao[city];

            driver.Navigate().GoToUrl($"https://www.net-empregos.com/pesquisa-empregos.asp");

            Disable_Initial_Cookies(ref driver);

            driver.Navigate().GoToUrl(city != "" ? $"https://www.net-empregos.com/pesquisa-empregos.asp?chaves={search_term}&cidade={city}&categoria=5&zona={zone}&tipo=0"
                : $"https://www.net-empregos.com/pesquisa-empregos.asp?chaves={search_term}&categoria=5&tipo=0");

            VariableWaitTime();

            Page_Scrapper(ref driver, driver.Url, By.XPath(current_job_XPath));

            VariableWaitTime();

            return;
        }

        public static bool Disable_Initial_Cookies(ref ChromeDriver driver)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            int index = 0;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            while (index < 5)
            {
                try
                {
                    var cookieBtn =  wait.Until(driver => driver.FindElement(By.Id("CybotCookiebotDialogBodyLevelButtonLevelOptinAllowallSelection")));
                    cookieBtn.Click();
                }
                catch
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                    index++;
                }
            }
            if (index > 5)
            {
                return false;
            }
            index = 0;
            while(index < 5)
            {
                try
                {
                    var element = driver.FindElement(By.XPath("//button[contains(text(), 'Não Ativar')]"));
                    element.Click();
                }
                catch
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                    index++;
                }
            }
            return false;
        }

        static public bool Dom_HTML_Processing(ref ChromeDriver driver, By shadowDom, string shadowRootID)
        {
            bool validation = false;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            int i = 0;
            while (i < 5)
            {
                try
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                    // Step 1: Locate the shadow host element
                    IWebElement shadowHost = driver.FindElement(shadowDom);
                    // Step 2: Access the shadow root
                    //var shadowRootElement = (IWebElement)js.ExecuteScript("return arguments[0].shadowRoot.querySelector('#cmpbntyestxt')", shadowHost);
                    // Step 3: Locate the button inside the shadow DOM
                    var button = (IWebElement)js.ExecuteScript($"return arguments[0].shadowRoot.querySelector('#{shadowRootID}')", shadowHost);
                    button.Click();
                    validation = true;
                    break;
                }
                catch (NoSuchElementException)//verificar que exceptions é que o javascript retorna
                {
                    i++;
                }
                catch (OpenQA.Selenium.ElementClickInterceptedException)
                {
                    i++;
                }
                catch (OpenQA.Selenium.StaleElementReferenceException)
                {
                    i++;
                }
                catch (OpenQA.Selenium.ElementNotInteractableException)
                {
                    i++;
                }

            }
            return validation;
        }

        public bool Page_Scrapper(ref ChromeDriver driver,string url ,By element)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            url= driver.Url;
            int page_index = 1;
            bool final_page = false;

            while (true)
            {
                dynamic elementList;
                try
                {
                     elementList= driver.FindElements(element);
                }
                catch(Exception ex)
                {
                    //TODO:eventually replace with a logger, the console.writeline only works for debugging
                    Console.WriteLine($"Error interacting with element: {ex.Message}");
                    continue;
                }
                //it runs all job postings of the chosen search term and city till theres no more pages
                //TODO: Implement Paralellism either by scrapping all pages first then threadpool/Parallel.ForEachAsync or by scrapping with multiple instances of Chrome at the same time
                //With Paralellism
                //ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 4 };
                //Parallel.ForEachAsync((IReadOnlyCollection<IWebElement>)elementList, options,
                //    async (element) =>
                //    {
                //        Job current_job = new Job();
                //        current_job.Title = html_element.FindElement(By.XPath(".//a[@class='oferta-link']")).Text;
                //        current_job.Company = html_element.FindElement(By.XPath(".//li[i[contains(@class, 'flaticon-work')]]")).Text.Trim();
                //        current_job.Location = html_element.FindElement(By.XPath(".//li[i[contains(@class, 'flaticon-pin')]]")).Text.Trim();
                //        current_job.Url = html_element.FindElement(By.XPath(".//a[@class='oferta-link']")).GetAttribute("href");

                //        current_job.Salary = null; //there isnt a salary description on the page

                //        lock (job_list)
                //        {
                //            job_list.Add(current_job);
                //        }

                //    });
                foreach (IWebElement html_element in elementList)
                {
                    Job current_job = new Job();
                    current_job.Title = html_element.FindElement(By.XPath(".//a[@class='oferta-link']")).Text;
                    current_job.Company = html_element.FindElement(By.XPath(".//li[i[contains(@class, 'flaticon-work')]]")).Text.Trim();
                    current_job.Location = html_element.FindElement(By.XPath(".//li[i[contains(@class, 'flaticon-pin')]]")).Text.Trim();
                    current_job.Url = html_element.FindElement(By.XPath(".//a[@class='oferta-link']")).GetAttribute("href");

                    current_job.Salary = null; //there isnt a salary description on the page

                    lock (job_list)
                    {
                        job_list.Add(current_job);
                    }

                }
                final_page = Page_Turner(ref driver, page_index: page_index,url);
                if (final_page)
                {
                    string complete_path = PROJECT_Directory + @"\JobFinder\DB\" + JSON_Name;
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

            return false; //should never reach here, but just in case
        }

        static public bool Page_Turner(ref ChromeDriver driver, int page_index, string url)
        {
            //Human like wait time
            VariableWaitTime(1, 3);
            driver.Navigate().GoToUrl(url + $"&page={page_index+1}");
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    //time to load the page
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                    //check the page for the "page non existent, ºgo backº button"
                    var nextPageElement = driver.FindElement(By.XPath("//button[@id='btn-voltar']"));
                    if (nextPageElement != null)
                    {
                        return true; // Pepa pig very happy, page exists
                    }
                }
                //TODO: implement a logger ,exceptions are heavy on preformance
                catch (NoSuchElementException)
                {
                    //Pepa pig very sad, page does not exist
                    continue;
                }
                catch (OpenQA.Selenium.StaleElementReferenceException)
                {
                    //Pepa pig happy and sad, page exists but the element is stale
                    return true;
                }
                catch (OpenQA.Selenium.ElementNotInteractableException)
                {
                    //Pepa pig very sad, page does not exist
                    continue;
                }
            }
            return false;
        }
        static public bool HTML_Interactor(ref ChromeDriver driver, By element, HTML_Action action, string value = "")
        {
            try
            {
                IWebElement webElement = driver.FindElement(element);
                switch (action)
                {
                    case HTML_Action.Click:
                        webElement.Click();
                        break;
                    case HTML_Action.Form:
                        webElement.SendKeys(value);
                        break;
                    case HTML_Action.Radio:
                        if (!webElement.Selected)
                            webElement.Click();
                        break;
                    case HTML_Action.ComboBox:
                        var selectElement = new OpenQA.Selenium.Support.UI.SelectElement(webElement);
                        selectElement.SelectByText(value);
                        break;
                    default:
                        throw new ArgumentException("Invalid action specified.");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error interacting with element: {ex.Message}");
                return false;
            }
        }
    }
    #endregion selenium 
}
