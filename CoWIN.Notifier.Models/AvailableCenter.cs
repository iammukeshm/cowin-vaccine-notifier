using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoWIN.Notifier.Models
{
    public class AvailableCenter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public int AvailableVaccines { get; set; }
        public int MinimumAge { get; set; }
        public DateTime DateTime { get; set; }
    }
}
