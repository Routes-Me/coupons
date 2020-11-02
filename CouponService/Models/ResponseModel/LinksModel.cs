using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponService.Models.ResponseModel
{
    public class LinksModel
    {
        public string LinkId { get; set; }
        public string PromotionId { get; set; }
        public string Web { get; set; }
        public string Ios { get; set; }
        public string Android { get; set; }
    }
}
