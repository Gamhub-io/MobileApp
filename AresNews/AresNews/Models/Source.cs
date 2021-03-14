using System;
using System.Collections.Generic;
using System.Text;

namespace AresNews.Models
{
    public class Source
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Domain { get; set; }
        public bool IsActive { get; set; }
    }
}
