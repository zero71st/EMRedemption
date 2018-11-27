using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RewardViewModels
{
    public class ImportRewardViewModel
    {
        public int RewardTypeId { get; set; }
        [DisplayName("Reward Type")]
        public SelectList RewardTypes { get; set; }
        [DisplayName("Lot No.")]
        public DateTime LotDate { get; set; }
     
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Decrypt Password")]
        public string Password { get; set; }
        public string Description { get; set; }
    }
}
