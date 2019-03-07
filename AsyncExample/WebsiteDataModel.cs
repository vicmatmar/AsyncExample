using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncExample
{
    public class WebsiteDataModel
    {
        public string WebsiteUrl { get; set; } = "";
        public string WebsiteData { get; set; } = "";
        public double time_ms { get; set; } = 0.0;
    }
}
