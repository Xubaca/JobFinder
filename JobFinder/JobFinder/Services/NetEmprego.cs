using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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
        };

        public static async Task Search(string search_term, string city)
        {
            ChromeDriver driver = new ChromeDriver(PROJECT_Directory + @"\chromedriver.exe");
            driver.Navigate().GoToUrl("https://www.net-empregos.com/pesquisa-empregos.asp?chaves=.net&cidade=Porto&categoria=5&zona=2&tipo=0");
            // Implement the search logic for NetEmprego here
            // This is a placeholder for the actual implementation
            throw new NotImplementedException("NetEmprego search functionality is not implemented yet.");
            return;
        }
    }
}
