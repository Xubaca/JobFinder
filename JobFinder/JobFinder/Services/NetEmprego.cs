using JobFinder.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.DevTools.V136.IndexedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
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

        public enum HTML_Action
        {
            Click = 1,
            Form = 2,
            Radio = 3,
            ComboBox = 4
        }

        static public List<Job> job_list = new List<Job>();

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
            Task.Delay(rdn.Next(min_range, max_range));
        }

        public static void Search(string search_term, string city)
        {
            //when i release it ill make it headless for a preformance boost, TODO:look into installing adblock on the driver
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");

            ChromeDriver driver = new ChromeDriver(options/*PROJECT_Directory + @"\chromedriver.exe"*/);
            city = char.ToUpper(city[0]) + city.Substring(1);
            int zone = regiao[city];

            driver.Navigate().GoToUrl($"https://www.net-empregos.com/pesquisa-empregos.asp");

            Disable_Initial_Cookies(ref driver);

            driver.Navigate().GoToUrl($"https://www.net-empregos.com/pesquisa-empregos.asp?chaves={search_term}&cidade={city}&categoria=5&zona={zone}&tipo=0");
            //Para testes é mais facil porque usar java como search_term sem especificar a localização é certo que vai ter mais que uma página
            //driver.Navigate().GoToUrl($"https://www.net-empregos.com/pesquisa-empregos.asp?chaves={search_term}&categoria=5");

            VariableWaitTime();

            Page_Scrapper(ref driver, driver.Url, By.XPath(current_job_XPath));

            VariableWaitTime();
            //throw new NotImplementedException("NetEmprego search functionality is not implemented yet.");
            //*[@id="CybotCookiebotDialogBodyLevelButtonLevelOptinAllowallSelection"]
            return;
        }

        public static bool Disable_Initial_Cookies(ref ChromeDriver driver)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            int index = 0;
            while (index < 5)
            {
                try
                {
                    var element = driver.FindElement(By.XPath("//button[@id='CybotCookiebotDialogBodyLevelButtonLevelOptinAllowallSelection']"));
                    element.Click();
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
            #region oldCode
            //while (index < 5)
            //{
            //    IWebElement permitirSelecao;
            //    try
            //    {
            //        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2000);
            //        // i dont for it via "id" or class name since this way is future proof so that in case some changes are made the text is the least likely part of the html element
            //        //to change
            //        permitirSelecao = driver.FindElement(by: By.XPath("//button[normalize-space(text())='Permitir seleção']"));

            //        permitirSelecao.Click();
            //        index++;
            //    }
            //    catch
            //    {
            //        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2000);
            //        index++;
            //        continue;
            //    }
            //    if (index > 1)
            //    {
            //        index = 0;
            //        break;
            //    }
            //}
            //while (index < 5)
            //{
            //    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);
            //    IWebElement ativarAlertas;
            //    try
            //    {
            //        ativarAlertas = driver.FindElement(by: By.XPath("//button[normalize-space(text())='Não Ativar']"));

            //        ativarAlertas.Click();
            //        index++;

            //    }
            //    catch
            //    {
            //        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1500);
            //        index++;
            //        continue;
            //    }
            //}
            #endregion oldCode
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

        static public bool Page_Scrapper(ref ChromeDriver driver,string url ,By element)
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
                foreach (IWebElement html_element in elementList)
                {
                    Job current_job = new Job();
                    current_job.Title = html_element.FindElement(By.XPath("//a[@class='oferta-link']")).Text;
                    var companyElement = driver.FindElement(By.XPath("//li[i[contains(@class, 'flaticon-work')]]"));
                    current_job.Company = companyElement.Text.Trim();
                    current_job.Location = html_element.FindElement(By.XPath("//li[i[contains(@class, 'flaticon-pin')]]")).Text.Trim();
                    current_job.Url = html_element.FindElement(By.XPath("//a[@class='oferta-link']")).GetAttribute("href");
                    current_job.Salary = null; //there isnt a salary description on the page

                    lock (job_list)
                    {
                        job_list.Add(current_job);
                    }

                }
                final_page = Page_Turner(ref driver, page_index: page_index,url);
                if (final_page)
                {
                    //TODO: Save the json , i should organize via {search_term}_{city}.json EXEMPLE: ".NET_Lisboa.json", i should also replace all spaces with underscores
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
}
