namespace CouponService.Models
{
    public class CommonMessage
    {
        public static string BadRequest = "Provide valid date.";
        public static string AuthoritiesRetrived = "Authorities Retrived successfully.";
        public static string AuthoritiesNotFound = "Authorities not found.";
        public static string AuthoritiesDelete = "Authorities deleted successfully.";
        public static string AuthoritiesInsert = "Authorities inserted successfully.";
        public static string AuthoritiesUpdate = "Authorities updated successfully.";
        public static string PlacesRetrived = "Places Retrived successfully.";
        public static string PlacesNotFound = "Places not found.";
        public static string PlacesDelete = "Places deleted successfully.";
        public static string PlacesInsert = "Places inserted successfully.";
        public static string PlacesUpdate = "Places updated successfully.";
        public static string PlacesConflict = "Places are associated with promotions.";
        public static string PromotionsRetrived = "Promotions Retrived successfully.";
        public static string PromotionsNotFound = "Promotions not found.";
        public static string PromotionsDelete = "Promotions deleted successfully.";
        public static string PromotionsInsert = "Promotions inserted successfully.";
        public static string PromotionsUpdate = "Promotions updated successfully.";
        public static string PromotionsUsageLimitExceed = "Promotions redeem limit exceeded.";
        public static string CouponsRetrived = "Coupons Retrived successfully.";
        public static string CouponsNotFound = "Coupons not found.";
        public static string CouponsDelete = "Coupons deleted successfully.";
        public static string CouponsInsert = "Coupons inserted successfully.";
        public static string CouponsUpdate = "Coupons updated successfully.";
        public static string CouponsRedeemed = "Coupons already redeemed. You can try again after 14 hours.";
        public static string CouponsExpired = "Coupons already expired.";
        public static string CouponsBeforeStartDate = "Coupons can not be redeemed before start date.";
        public static string RedemptionRetrived = "Redemption Retrived successfully.";
        public static string RedemptionNotFound = "Redemption not found.";
        public static string RedemptionDelete = "Redemption deleted successfully.";
        public static string RedemptionInsert = "Redemption inserted successfully.";
        public static string RedemptionUpdate = "Redemption updated successfully.";
        public static string RedemptionAssociated = "Redemption is associated with other coupons.";
        public static string PinInvalid = "Pin is invalid.";
        public static string OfficerDoNotBelong = "Officer does not belong to this institution.";
        public static string OfficerNotFound = "Officer not found.";
        public static string ExceptionMessage = "Something went wrong. Error Message - ";

        public static string LinksRetrived = "Links Retrived successfully.";
        public static string LinksNotFound = "Links not found.";
        public static string LinksDelete = "Links deleted successfully.";
        public static string LinksInsert = "Links inserted successfully.";
        public static string LinksUpdate = "Links updated successfully.";
        public static string LinksConflict = "Links are associated with promotions.";
        public static string CodeLength = "Code length should not more than 5 characters.";
        
    }
}
