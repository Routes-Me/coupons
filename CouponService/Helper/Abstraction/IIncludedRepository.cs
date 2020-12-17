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
        dynamic GetCouponIncludedData(List<RedemptionGetModel> redemptionModelList);
        dynamic GetOfficerIncludedData(List<RedemptionGetModel> redemptionModelList);
        dynamic GetOfficerData(string officerId);
        dynamic GetPinData(string institutionId);
        dynamic GetPromotionIncludedData(List<CouponsModel> placeModelList);
        dynamic GetUserIncludedDataForRedemption(List<RedemptionGetModel> redemptionModelList);
        dynamic GetSearchCouponIncludedData(List<RedemptionGetModel> redemptionModelList, string search);
        dynamic GetLinksPromotionIncludedData(List<LinksModel> linkModelList);
        dynamic GetLinksIncludedData(List<PromotionsModel> promotionsModelList);
        dynamic GetCouponsIncludedData(List<PromotionsModel> promotionsModelList);
    }
}
