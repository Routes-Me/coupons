using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponService.Helper.Model
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string IV { get; set; }
        public string KEY { get; set; }
        public string RedeedInterval { get; set; }
        public string Host { get; set; }
    }
}
    