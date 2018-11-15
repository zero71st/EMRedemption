using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.AppLogViewModels
{
    public class AppLogListViewModel
    {
        public List<AppLogViewModel> AppLogs { get; set; }
        public string Keyword { get; set; }
    }
}
