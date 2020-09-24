using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponService.Models.ResponseModel
{
    public class AuthoritiesModel
    {
        public string AuthorityId { get; set; }
        public string InstitutionId { get; set; }
        public string Pin { get; set; }
    }
}
