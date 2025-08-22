using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using JobFinder.Services;

namespace JobFinder
{
    public partial class Form1 : Form
    {
        // This will get the current WORKING directory (i.e. \bin\Debug)
        static string workingDirectory = Environment.CurrentDirectory;
        // or: Directory.GetCurrentDirectory() gives the same result

        // This will get the current PROJECT bin directory (ie ../bin/)
        static string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;

        // This will get the current PROJECT directory
        static string PROJECT_Directory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;

        static string CurrentDirectory = Directory.GetCurrentDirectory();

        static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        //might add more browers like "Brave" to the program later
        //IWebDriver chrome_driver = new ChromeDriver();
        public Form1()
        {
            InitializeComponent();
        }


        private void btn_Begin_Click(object sender, EventArgs e)
        {
            string[] search_terms = this.txtb_Search.Text.Trim().Split(new string[] { Environment.NewLine }, StringSplitOptions.TrimEntries);
            string[] cities = this.textBox1.Text.Split(';', StringSplitOptions.TrimEntries);
            //In case the user didnt get the memo:
            if (search_terms.Length == 0 || search_terms[0] == "")
            {
                MessageBox.Show("Please enter a search term.");
                return;
            }
            if (cities.Length == 0 || cities[0] == "")
            {
                MessageBox.Show("Please enter a city.");
                return;
            }

            //remove empty entries
            cities = cities.Where(c => c != "").ToArray();
            search_terms = search_terms.Where(st => st != "").ToArray();
            Random rnd = new Random();
            //TODO: implement said ThreadPool , set max to 4 in order to not break the 
            ThreadPool.SetMaxThreads(4, 4);
            SapoEmprego sapoEmprego = new SapoEmprego();
            NetEmprego netEmprego = new NetEmprego();
            for (int i = 0; i < cities.Length; i++)
            {
                for (int j = 0; j < search_terms.Length; j++)
                {
                    //Since one runs on curl_cffi and the other on a chrome driver i can run them in parallel!
                    netEmprego.Search(search_terms[j], cities[i]);
                    //ia meter estes pedido async dentro de uma workerThread mas acho que o overhead de dar manage as cookies não vale o esforço.
                    //int sec_to_wait = rnd.Next(1000, 5000);
                    //if we're too fast will get our cookies blocked, forcing us to get new ones, delaying the program due to the cost of creating a new sellenium driver
                    //Task.Delay(sec_to_wait).Wait();
                    //por causa de como os processadores funcionam é muito mais rapido fazer assim doque search terms primeiro e depois cidades
                    sapoEmprego.Search(search_terms[j], cities[i]);
                }
            }
            Task.WaitAll();
        }

        private void btn_Teste_Click(object sender, EventArgs e)
        {
            SapoEmprego sapoEmprego = new SapoEmprego();
            NetEmprego netEmprego = new NetEmprego();
            ITJobs itJobs = new();

            //netEmprego.Search(search_term: ".NET", city: "Porto");
            //SapoEmprego.Search(search_term: ".NET", city: "Porto");
            //netEmprego.Optimized_Search(search_term: "Java", city: "Lisboa");
            //netEmprego.Optimized_Search(search_term: "Java");
            //sapoEmprego.Search(search_term: "Java");
            itJobs.Optimized_Search(searchTerm: ".Net", city: "Porto");
        }
    }
}
