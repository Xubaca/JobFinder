using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobFinder.Model
{
    public class Job
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Url { get; set; }
        //it most likely wont appear , but should it be on the description i should not it
        public decimal? Salary { get; set; }

    }
}
