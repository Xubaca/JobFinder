using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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

        public static async Task Search(string search_term, string city)
        {
            ChromeDriver driver = new ChromeDriver(/*PROJECT_Directory + @"\chromedriver.exe"*/);
            city = char.ToUpper(city[0]) + city.Substring(1);
            int zone = regiao[city];
            driver.Navigate().GoToUrl($"https://www.net-empregos.com/pesquisa-empregos.asp?chaves={search_term}&cidade={city}&categoria=5&zona={zone}&tipo=0");

            Disable_Initial_Cookies(ref driver);
            // Implement the search logic for NetEmprego here
            // This is a placeholder for the actual implementation
            throw new NotImplementedException("NetEmprego search functionality is not implemented yet.");
            //*[@id="CybotCookiebotDialogBodyLevelButtonLevelOptinAllowallSelection"]
            return;
        }

        public static bool Disable_Initial_Cookies(ref ChromeDriver driver)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15000);
            int index = 0;
            while (index < 5)
            {
                IWebElement permitirSelecao;
                try
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2000);
                    // i dont for it via "id" or class name since this way is future proof so that in case some changes are made the text is the least likely part of the html element
                    //to change
                    permitirSelecao = driver.FindElement(by: By.XPath("//button[normalize-space(text())='Permitir seleção']"));

                    permitirSelecao.Click();
                    index++;
                }
                catch
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2000);
                    index++;
                    continue;
                }
                if (index > 1)
                {
                    index = 0;
                    break;
                }
            }
            while (index < 5)
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);
                IWebElement ativarAlertas;
                try
                {
                    ativarAlertas = driver.FindElement(by: By.XPath("//button[normalize-space(text())='Não Ativar']"));

                    ativarAlertas.Click();
                    index++;

                }
                catch
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1500);
                    index++;
                    continue;
                }
            }
            return false;
        }
    }
}
