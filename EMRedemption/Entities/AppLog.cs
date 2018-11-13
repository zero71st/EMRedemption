using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class AppLog:BaseEntity
    {
        [MaxLength(50)]
        public string Application { get; set; }
        public DateTime Logged { get; set; }
        [MaxLength(50)]
        public string Level { get; set; }
        [MaxLength(512)]
        public string Message { get; set; }
        [MaxLength(250)]
        public string Logger { get; set; }
        [MaxLength(512)]
        public string Callsite { get; set; }
        [MaxLength(512)]
        public string Exception { get; set; }
    }
}
