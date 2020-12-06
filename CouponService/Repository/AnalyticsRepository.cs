using CouponService.Abstraction;
using CouponService.Helper.Model;
using CouponService.Models;
using CouponService.Models.DBModels;
using CouponService.Models.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Obfuscation;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CouponService.Repository
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AppSettings _appSettings;
        private readonly couponserviceContext _context;
        private readonly Dependencies _dependencies;

        public AnalyticsRepository(IOptions<AppSettings> appSettings, couponserviceContext context, IOptions<Dependencies> dependencies)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _dependencies = dependencies.Value;
        }
        public void InsertAnalytics()
        {
            try
            {
                DateTime? lastCouponDate = null;
                var client = new RestClient(_appSettings.Host + _dependencies.GetAnalyticsUrl + "coupons");
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var analyticsData = JsonConvert.DeserializeObject<GetAnalyticsResponse>(result);
                    if (analyticsData != null)
                        lastCouponDate = analyticsData.CreatedAt;
                }

                if (lastCouponDate != null)
                {
                    List<PromotionAnalytics> promotionAnalyticsList = new List<PromotionAnalytics>();
                    var redemptions = _context.Redemptions.Include(x => x.Coupon).Include(x => x.Coupon.Promotion).Where(x => x.CreatedAt > lastCouponDate).ToList();
                    if (redemptions != null && redemptions.Count > 0)
                    {
                        foreach (var group in redemptions.GroupBy(x => x.CouponId))
                        {
                            var items = group.FirstOrDefault();
                            PromotionAnalytics promotionAnalytics = new PromotionAnalytics();
                            promotionAnalytics.PromotionId = ObfuscationClass.EncodeId(items.Coupon.Promotion.PromotionId, _appSettings.Prime).ToString();
                            promotionAnalytics.AdvertismentId = ObfuscationClass.EncodeId(items.Coupon.Promotion.AdvertisementId.GetValueOrDefault(), _appSettings.Prime).ToString();
                            promotionAnalytics.CreatedAt = DateTime.Now;
                            promotionAnalytics.Count = group.Key;
                            promotionAnalytics.Type = "coupons";
                            promotionAnalyticsList.Add(promotionAnalytics);
                        }
                    }

                    if (promotionAnalyticsList != null && promotionAnalyticsList.Count > 0)
                    {
                        AnalyticsModel analyticsModel = new AnalyticsModel()
                        {
                            analytics = promotionAnalyticsList
                        };

                        var postClient = new RestClient(_appSettings.Host + _dependencies.PostAnalyticsUrl);
                        var postRequest = new RestRequest(Method.POST);
                        string jsonToSend = JsonConvert.SerializeObject(analyticsModel);
                        postRequest.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
                        postRequest.RequestFormat = DataFormat.Json;
                        IRestResponse institutionResponse = postClient.Execute(postRequest);
                        if (institutionResponse.StatusCode != HttpStatusCode.Created)
                        {

                        }
                    }
                }
                else
                {
                    List<PromotionAnalytics> promotionAnalyticsList = new List<PromotionAnalytics>();
                    var redemptions = _context.Redemptions.Include(x => x.Coupon).Include(x => x.Coupon.Promotion).ToList();
                    if (redemptions != null)
                    {
                        foreach (var group in redemptions.GroupBy(x => x.CouponId))
                        {
                            var items = group.FirstOrDefault();
                            PromotionAnalytics promotionAnalytics = new PromotionAnalytics();
                            promotionAnalytics.PromotionId = ObfuscationClass.EncodeId(items.Coupon.Promotion.PromotionId, _appSettings.Prime).ToString();
                            promotionAnalytics.AdvertismentId = ObfuscationClass.EncodeId(items.Coupon.Promotion.AdvertisementId.GetValueOrDefault(), _appSettings.Prime).ToString();
                            promotionAnalytics.CreatedAt = DateTime.Now;
                            promotionAnalytics.Count = group.Key;
                            promotionAnalytics.Type = "coupons";
                            promotionAnalyticsList.Add(promotionAnalytics);
                        }
                    }

                    if (promotionAnalyticsList != null && promotionAnalyticsList.Count > 0)
                    {
                        AnalyticsModel analyticsModel = new AnalyticsModel()
                        {
                            analytics = promotionAnalyticsList
                        };

                        var postClient = new RestClient(_appSettings.Host + _dependencies.PostAnalyticsUrl);
                        var postRequest = new RestRequest(Method.POST);
                        string jsonToSend = JsonConvert.SerializeObject(analyticsModel);
                        postRequest.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
                        postRequest.RequestFormat = DataFormat.Json;
                        IRestResponse institutionResponse = postClient.Execute(postRequest);
                        if (institutionResponse.StatusCode != HttpStatusCode.Created)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
