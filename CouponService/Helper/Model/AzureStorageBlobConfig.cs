using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponService.Helper.Model
{
    public class AzureStorageBlobConfig
    {
        public string StorageConnection { get; set; }
        public string Container { get; set; }
    }
}
