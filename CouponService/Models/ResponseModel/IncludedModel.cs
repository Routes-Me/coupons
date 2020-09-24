using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponService.Models.ResponseModel
{
    public class InstitutionsModel
    {
        public string InstitutionId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryIso { get; set; }
    }

    public class AdvertisementsModel
    {
        public string AdvertisementId { get; set; }
        public string ResourceName { get; set; }
        public string InstitutionId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string MediaId { get; set; }
    }

    public class UserModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Application { get; set; }
        public string Description { get; set; }
    }
    
    public class OfficerModel
    {
        public string OfficerId { get; set; }
        public string UserId { get; set; }
        public string InstitutionId { get; set; }
    }
}
