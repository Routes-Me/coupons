using CouponService.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponService.Helper.Abstraction
{
    public interface IIncludedRepository
    {
        dynamic GetInstitutionsIncludedData(List<AuthoritiesModel> authoritiesModelList);
        dynamic GetUsersIncludedData(List<CouponsModel> objDriversModelList);
        dynamic GetAdvertisementsIncludedData(List<PromotionsModel> promotionsModelList);
        dynamic GetInstitutionsIncludedData(List<PromotionsModel> authoritiesModelList);
        dynamic GetCouponIncludedData(List<RedemptionModel> redemptionModelList);
        dynamic GetOfficerIncludedData(List<RedemptionModel> redemptionModelList);
        dynamic GetPromotionIncludedData(List<CouponsModel> placeModelList);
        string GetOfficerUserFromInstitution(string institutionId);
        //dynamic GetPromotionIncludedData(List<RedemptionModel> redemptionModelList);
    }
}
