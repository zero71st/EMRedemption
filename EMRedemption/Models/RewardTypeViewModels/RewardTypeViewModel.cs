using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RewardTypeViewModels
{
    public class RewardTypeViewModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Reword Code")]
        public string RewardCode { get; set; }
        [Required]
        [DisplayName("Reward Name")]
        public string RewardName { get; set; }
        [Required]
        [DisplayName("Reward Type Name")]
        public string RewardTypeName { get; set; }
    }
}
