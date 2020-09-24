using System;
using System.Collections.Generic;

namespace CouponService.Models.DBModels
{
    public partial class Authorities
    {
        public int AuthorityId { get; set; }
        public int? InstitutionId { get; set; }
        public string Pin { get; set; }
    }
}
