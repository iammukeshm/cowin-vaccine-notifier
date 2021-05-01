using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoWIN.Notifier.Models
{
    public class CoWINConfiguration
    {
        public int IntervalInMinutes { get; set; }
        public int DistrictId { get; set; }
        public string IFTTT_ApiKey { get; set; }
    }
}
