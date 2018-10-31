using EMRedemption.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RewardViewModels
{
    public class RewardViewModel
    {
        public int Id { get; set; }

        public int LineNo { get; set; }

        [DisplayName("Reward Code")]
        public string Code { get; set; }

        [DisplayName("Serial Number")]
        public string SerialNo { get; set; }

        [DisplayName("Reward Description")]
        public string Description { get; set; }

        [DisplayName("Reward Type")]
        public string RewardType { get; set; }

        [DisplayName("Expire Date")]
        public DateTime ExpireDate { get; set; }

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
            RewardType = reward.RewardType;
            ExpireDate = reward.ExpireDate;
        }
    }
}
