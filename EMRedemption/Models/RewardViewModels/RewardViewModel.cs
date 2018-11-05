using EMRedemption.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RewardViewModels
{
    public class RewardViewModel
    {
        public int Id { get; set; }

        public int LineNo { get; set; }

        [Required]
        [DisplayName("Reward Code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("Serial Number")]
        public string SerialNo { get; set; }

        [Required]
        [DisplayName("Reward Name")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Quantity")]
        public int Quantity { get; set; }

        [DisplayName("Expire Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime ExpireDate { get; set; }

        [Required]
        [DisplayName("Reward Type")]
        public string RewardType { get; set; }

        [Required]
        [DisplayName("Lot No.")]
        public string LotNo { get; set; }

        [DisplayName("Added By")]
        public string AddBy { get; set; }

        [DisplayName("Added Date")]
        public DateTime AddDate { get; set; }

        public bool IsUsed { get; set; }

        public RewardViewModel()
        {

        }

        public RewardViewModel(Reward reward):this()
        {
            Code = reward.Code;
            SerialNo = reward.SerialNo;
            Description = reward.Description;
            ExpireDate = reward.ExpireDate;
            Quantity = reward.Quantity;
            RewardType = reward.RewardType;
            LotNo = reward.LotNo;
        }
    }
}
