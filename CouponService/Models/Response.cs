using CouponService.Models.DBModels;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
 
namespace CouponService.Models
{
    public class Response
    {
        public bool status { get; set; }
        public string message { get; set; }
        public int statusCode { get; set; }
    }

    public class ReturnResponse
    {
        public static dynamic ExceptionResponse(Exception ex)
        {
            Response response = new Response();
            response.status = false;
            response.message = CommonMessage.ExceptionMessage + ex.Message;
            response.statusCode = StatusCodes.Status500InternalServerError;
            return response;
        }

        public static dynamic SuccessResponse(string message, bool isCreated)
        {
            Response response = new Response();
            response.status = true;
            response.message = message;
            if (isCreated)
                response.statusCode = StatusCodes.Status201Created;
            else
                response.statusCode = StatusCodes.Status200OK;
            return response;
        }

        public static dynamic ErrorResponse(string message, int statusCode)
        {
            Response response = new Response();
            response.status = false;
            response.message = message;
            response.statusCode = statusCode;
            return response;
        }
    }

    #region Redemption Reponse
    public class RedemptionGetResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<RedemptionGetModel> data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JObject included { get; set; }
    }

    public class RedemptionResponse : Response
    {
        public string RedemptionId { get; set; }
    }
    #endregion

    #region Places Reponse
    public class PlacesGetResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<PlacesModel> data { get; set; }
    }
    #endregion

    #region Places Reponse
    public class PromotionsGetResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<PromotionsModel> data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JObject included { get; set; }
    }

    public class PromotionsForContentGetResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<PromotionsModel> data { get; set; }
    }
    #endregion

    #region Coupon Reponse
    public class CouponGetResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<CouponsModel> data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JObject included { get; set; }
    }
    #endregion

    public class InstitutionsData
    {
        public Pagination pagination { get; set; }
        public List<InstitutionsModel> data { get; set; }
    }

    public class CouponWithUser
    {
        public List<Coupons> coupons { get; set; }
        public List<UserModel> users { get; set; }
    }

    public class UserData
    {
        public Pagination pagination { get; set; }
        public List<UserModel> data { get; set; }
    }

    public class OfficerData
    {
        public Pagination pagination { get; set; }
        public List<OfficerModel> data { get; set; }
    }

    public class AuthoritiesData
    {
        public Pagination pagination { get; set; }
        public List<AuthoritiesModel> data { get; set; }
    }

    public class AdvertisementData
    {
        public Pagination pagination { get; set; }
        public List<AdvertisementsModel> data { get; set; }
    }

    public class LinkResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<LinksModel> data { get; set; }
    }
    public class PromotionsPostResponse : Response
    {
        public string promotionsId { get; set; }
    }

    public class GetAnalyticsResponse : Response
    {
        public DateTime? CreatedAt { get; set; }
    }
}
