﻿using CouponService.Helper.Abstraction;
using CouponService.Helper.Model;
using CouponService.Models;
using CouponService.Models.DBModels;
using CouponService.Models.ResponseModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static CouponService.Models.ReturnResponse;

namespace CouponService.Helper.Repository
{
    public class IncludedRepository : IIncludedRepository
    {
        private readonly AppSettings _appSettings;
        private readonly Dependencies _dependencies;
        private readonly couponserviceContext _context;

        public IncludedRepository(IOptions<AppSettings> appSettings, couponserviceContext context, IOptions<Dependencies> dependencies)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _dependencies = dependencies.Value;
        }

        public dynamic GetAdvertisementsIncludedData(List<PromotionsModel> promotionsModelList)
        {
            List<AdvertisementsModel> lstAdvertisements = new List<AdvertisementsModel>();
            foreach (var item in promotionsModelList)
            {
                var client = new RestClient(_appSettings.Host + _dependencies.AdvertisementsUrl + item.InstitutionId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var advertisementsData = JsonConvert.DeserializeObject<AdvertisementData>(result);
                    lstAdvertisements.AddRange(advertisementsData.data);
                }
            }
            var advertisementsList = lstAdvertisements.GroupBy(x => x.InstitutionId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(advertisementsList.Cast<dynamic>().ToList());
        }

        public dynamic GetInstitutionsIncludedData(List<AuthoritiesModel> authoritiesModelList)
        {
            List<InstitutionsModel> lstInstitutions = new List<InstitutionsModel>();
            foreach (var item in authoritiesModelList)
            {
                var client = new RestClient(_appSettings.Host + _dependencies.InstitutionUrl + item.InstitutionId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var institutionsData = JsonConvert.DeserializeObject<InstitutionsData>(result);
                    lstInstitutions.AddRange(institutionsData.data);
                }
            }
            var institutionsList = lstInstitutions.GroupBy(x => x.InstitutionId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(institutionsList.Cast<dynamic>().ToList());
        }

        public dynamic GetInstitutionsIncludedData(List<PromotionsModel> authoritiesModelList)
        {
            List<InstitutionsModel> lstInstitutions = new List<InstitutionsModel>();
            foreach (var item in authoritiesModelList)
            {
                var client = new RestClient(_appSettings.Host + _dependencies.InstitutionUrl + item.InstitutionId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var institutionsData = JsonConvert.DeserializeObject<InstitutionsData>(result);
                    lstInstitutions.AddRange(institutionsData.data);
                }
            }
            var institutionsList = lstInstitutions.GroupBy(x => x.InstitutionId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(institutionsList.Cast<dynamic>().ToList());
        }

        public dynamic GetCouponIncludedData(List<RedemptionGetModel> redemptionModelList)
        {
            List<CouponWithUser> lstCouponWithUser = new List<CouponWithUser>();
            List<Coupons> coupons = new List<Coupons>();
            List<UserModel> lstUsers = new List<UserModel>();
            foreach (var item in redemptionModelList)
            {
                var couponsDetails = (from coupon in _context.Coupons
                                      where coupon.CouponId == Convert.ToInt32(item.CouponId)
                                      select new Coupons()
                                      {
                                          CouponId = coupon.CouponId,
                                          PromotionId = coupon.PromotionId,
                                          UserId = coupon.UserId,
                                          CreatedAt = coupon.CreatedAt,
                                          Promotion = coupon.Promotion
                                      }).FirstOrDefault();
                coupons.Add(couponsDetails);
            }
            var couponsList = coupons.GroupBy(x => x.CouponId).Select(a => a.First()).ToList();
            foreach (var item in couponsList)
            {
                var client = new RestClient(_appSettings.Host + _dependencies.UserUrl + item.UserId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var userData = JsonConvert.DeserializeObject<UserData>(result);
                    lstUsers.AddRange(userData.data);
                }
            }
            var usersList = lstUsers.GroupBy(x => x.UserId).Select(a => a.First()).ToList();
            CouponWithUser couponWithUser = new CouponWithUser();
            couponWithUser.coupons = couponsList;
            couponWithUser.users = usersList;
            lstCouponWithUser.Add(couponWithUser);
            return Common.SerializeJsonForIncludedRepo(lstCouponWithUser.Cast<dynamic>().ToList());
        }

        public dynamic GetPromotionIncludedData(List<CouponsModel> couponsModelList)
        {
            List<PromotionsModel> promotion = new List<PromotionsModel>();
            foreach (var item in couponsModelList)
            {
                var couponsDetails = (from promotions in _context.Promotions
                                      where promotions.PromotionId == Convert.ToInt32(item.PromotionId)
                                      select new PromotionsModel()
                                      {
                                          PromotionId = Convert.ToString(promotions.PromotionId),
                                          Title = promotions.Title,
                                          Subtitle = promotions.Subtitle,
                                          CreatedAt = promotions.CreatedAt,
                                          UpdatedAt = promotions.UpdatedAt,
                                          StartAt = promotions.StartAt,
                                          EndAt = promotions.EndAt,
                                          UsageLimit = promotions.UsageLimit,
                                          AdvertisementId = Convert.ToString(promotions.AdvertisementId),
                                          InstitutionId = Convert.ToString(promotions.InstitutionId),
                                          IsSharable = promotions.IsSharable,
                                          LogoUrl = promotions.LogoUrl,
                                      }).FirstOrDefault();

                promotion.Add(couponsDetails);
            }
            var promotionList = promotion.GroupBy(x => x.PromotionId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(promotionList.Cast<dynamic>().ToList());
        }

        public dynamic GetUsersIncludedData(List<CouponsModel> couponsModelList)
        {
            List<UserModel> lstUsers = new List<UserModel>();
            foreach (var item in couponsModelList)
            {
                var client = new RestClient(_appSettings.Host + _dependencies.UserUrl + item.UserId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var userData = JsonConvert.DeserializeObject<UserData>(result);
                    lstUsers.AddRange(userData.data);
                }
            }
            var usersList = lstUsers.GroupBy(x => x.UserId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(usersList.Cast<dynamic>().ToList());
        }

        public dynamic GetOfficerIncludedData(List<RedemptionGetModel> redemptionModelList)
        {
            List<OfficerModel> lstOfficer = new List<OfficerModel>();
            foreach (var item in redemptionModelList)
            {
                var client = new RestClient(_appSettings.Host + _dependencies.OfficersUrl + item.OfficerId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var officerData = JsonConvert.DeserializeObject<OfficerData>(result);
                    lstOfficer.AddRange(officerData.data);
                }
            }
            var officerList = lstOfficer.GroupBy(x => x.UserId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(officerList.Cast<dynamic>().ToList());
        }

        public dynamic GetOfficerData(string officerId)
        {
            List<OfficerModel> lstOfficer = new List<OfficerModel>();
            var client = new RestClient(_appSettings.Host + _dependencies.OfficersUrl + officerId);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = response.Content;
                var officerData = JsonConvert.DeserializeObject<OfficerData>(result);
                lstOfficer.AddRange(officerData.data);
            }
            return lstOfficer;
        }

        public dynamic GetPinData(string institutionId)
        {
            List<AuthoritiesModel> lstAuthorities = new List<AuthoritiesModel>();
            var client = new RestClient((_appSettings.Host + _dependencies.AuthoritiesUrl).Replace("__id", institutionId));
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = response.Content;
                var authoritiesData = JsonConvert.DeserializeObject<AuthoritiesData>(result);
                lstAuthorities.AddRange(authoritiesData.data);
            }
            return lstAuthorities;
        }
    }
}
