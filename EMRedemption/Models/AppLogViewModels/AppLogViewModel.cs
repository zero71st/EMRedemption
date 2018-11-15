using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.AppLogViewModels
{
    public class AppLogViewModel
    {
        public int Id { get; set; }
        [DisplayName("Time")]
        public DateTime Logged { get; set; }
        [DisplayName("Type")]
        public string Level { get; set; }
        [DisplayName("Discription")]
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Callsite { get; set; }
    }
}
