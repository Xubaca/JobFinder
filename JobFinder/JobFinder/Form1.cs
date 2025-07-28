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
        static string PROJECT_Directory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

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
            //MessageBox.Show("Hello", "Window Class Names");
            SapoEmprego.Search();

        }
    }
}
