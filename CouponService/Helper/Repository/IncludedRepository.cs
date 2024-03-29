﻿using CouponService.Helper.Abstraction;
using CouponService.Helper.Model;
using CouponService.Models;
using CouponService.Models.DBModels;
using CouponService.Models.ResponseModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoutesSecurity;
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
            List<Coupons> coupons = new List<Coupons>();
            foreach (var item in redemptionModelList)
            {
                var couponIdDecrypted = Obfuscation.Decode(item.CouponId);
                var couponsDetails = (from coupon in _context.Coupons
                                      where coupon.CouponId == couponIdDecrypted
                                      select new Coupons()
                                      {
                                          CouponId = Convert.ToInt32(Obfuscation.Encode(coupon.CouponId)),
                                          PromotionId = Convert.ToInt32(Obfuscation.Encode(Convert.ToInt32(coupon.PromotionId))),
                                          UserId = Convert.ToInt32(Obfuscation.Encode(Convert.ToInt32(coupon.UserId))),
                                          CreatedAt = coupon.CreatedAt,
                                          Promotion = coupon.Promotion
                                      }).AsEnumerable().FirstOrDefault();
                coupons.Add(couponsDetails);
            }
            var couponsList = coupons.GroupBy(x => x.CouponId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(couponsList.Cast<dynamic>().ToList());
        }


        public dynamic GetUserIncludedDataForRedemption(List<RedemptionGetModel> redemptionModelList)
        {
            List<string> users = new List<string>();
            List<UserModel> userModel = new List<UserModel>();
            foreach (var item in redemptionModelList)
            {
                var couponIdDecrypted = Obfuscation.Decode(item.CouponId);
                var couponsDetails = _context.Coupons.Where(x => x.CouponId == couponIdDecrypted).FirstOrDefault();
                if (couponsDetails != null)
                {
                    users.Add(Obfuscation.Encode(Convert.ToInt32(couponsDetails.UserId)));
                }
            }
            foreach (var item in users)
            {
                var client = new RestClient(_appSettings.Host + _dependencies.UserUrl + item);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var userData = JsonConvert.DeserializeObject<UserData>(result);
                    userModel.AddRange(userData.data);
                }
            }
            var usersList = userModel.GroupBy(x => x.UserId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(usersList.Cast<dynamic>().ToList());
        }

        public dynamic GetPromotionIncludedData(List<CouponsModel> couponsModelList)
        {
            List<PromotionsModel> promotion = new List<PromotionsModel>();
            foreach (var item in couponsModelList)
            {
                var promoIdDecrypted = Obfuscation.Decode(item.PromotionId);
                var couponsDetails = (from promotions in _context.Promotions
                                      where promotions.PromotionId == promoIdDecrypted
                                      select new PromotionsModel()
                                      {
                                          PromotionId = Obfuscation.Encode(promotions.PromotionId),
                                          Title = promotions.Title,
                                          Subtitle = promotions.Subtitle,
                                          CreatedAt = promotions.CreatedAt,
                                          UpdatedAt = promotions.UpdatedAt,
                                          StartAt = promotions.StartAt,
                                          EndAt = promotions.EndAt,
                                          UsageLimit = promotions.UsageLimit,
                                          AdvertisementId = Obfuscation.Encode(Convert.ToInt32(promotions.AdvertisementId)),
                                          InstitutionId = Obfuscation.Encode(Convert.ToInt32(promotions.InstitutionId)),
                                          IsSharable = promotions.IsSharable,
                                          LogoUrl = promotions.LogoUrl,
                                          Type = promotions.Type
                                      }).AsEnumerable().FirstOrDefault();

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
            UriBuilder uriBuilder = new UriBuilder(_appSettings.Host + _dependencies.InstitutionUrl + institutionId + "/authorities");
            var client = new RestClient(uriBuilder.Uri);
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

        public dynamic GetSearchCouponIncludedData(List<RedemptionGetModel> redemptionModelList, string search)
        {
            List<Coupons> coupons = new List<Coupons>();
            foreach (var item in redemptionModelList)
            {
                var couponIdDecrypted = Obfuscation.Decode(item.CouponId);
                var couponsDetails = (from coupon in _context.Coupons
                                      where coupon.CouponId == couponIdDecrypted
                                      select new Coupons()
                                      {
                                          CouponId = Convert.ToInt32(Obfuscation.Encode(coupon.CouponId)),
                                          PromotionId = Convert.ToInt32(Obfuscation.Encode(Convert.ToInt32(coupon.PromotionId))),
                                          UserId = Convert.ToInt32(Obfuscation.Encode(Convert.ToInt32(coupon.UserId))),
                                          CreatedAt = coupon.CreatedAt,
                                          Promotion = coupon.Promotion
                                      }).AsEnumerable().FirstOrDefault();
                coupons.Add(couponsDetails);
            }
            foreach (var item in coupons)
            {
                item.Promotion.Coupons = null;
                item.Promotion.PromotionsPlaces = null;
            }
            var couponsList = coupons.GroupBy(x => x.CouponId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(couponsList.Cast<dynamic>().ToList());
        }

        public dynamic GetLinksPromotionIncludedData(List<LinksModel> linkModelList)
        {
            List<PromotionsModel> promotion = new List<PromotionsModel>();
            foreach (var item in linkModelList)
            {
                var promoIdDecrypted = Obfuscation.Decode(item.PromotionId);
                var couponsDetails = (from promotions in _context.Promotions
                                      where promotions.PromotionId == promoIdDecrypted
                                      select new PromotionsModel()
                                      {
                                          PromotionId = Obfuscation.Encode(promotions.PromotionId),
                                          Title = promotions.Title,
                                          Subtitle = promotions.Subtitle,
                                          CreatedAt = promotions.CreatedAt,
                                          UpdatedAt = promotions.UpdatedAt,
                                          StartAt = promotions.StartAt,
                                          EndAt = promotions.EndAt,
                                          UsageLimit = promotions.UsageLimit,
                                          AdvertisementId = Obfuscation.Encode(Convert.ToInt32(promotions.AdvertisementId)),
                                          InstitutionId = Obfuscation.Encode(Convert.ToInt32(promotions.InstitutionId)),
                                          IsSharable = promotions.IsSharable,
                                          LogoUrl = promotions.LogoUrl,
                                          Type = promotions.Type
                                      }).AsEnumerable().FirstOrDefault();

                promotion.Add(couponsDetails);
            }
            var promotionList = promotion.GroupBy(x => x.PromotionId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(promotionList.Cast<dynamic>().ToList());
        }

        public dynamic GetLinksIncludedData(List<PromotionsModel> promotionsModelList)
        {
            List<LinksModel> linksModel = new List<LinksModel>();
            foreach (var item in promotionsModelList)
            {
                var promotionIdDecrypted = Obfuscation.Decode(item.PromotionId);
                var linksDetails = (from links in _context.Links
                                    where links.PromotionId == promotionIdDecrypted
                                    select new LinksModel()
                                    {
                                        LinkId = Obfuscation.Encode(links.LinkId),
                                        PromotionId = Obfuscation.Encode(Convert.ToInt32(links.PromotionId)),
                                        Web = links.Web,
                                        Ios = links.Ios,
                                        Android = links.Android
                                    }).AsEnumerable().FirstOrDefault();
                if (linksDetails != null)
                    linksModel.Add(linksDetails);
            }
            var linkList = linksModel.GroupBy(x => x.LinkId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(linkList.Cast<dynamic>().ToList());
        }

        public dynamic GetCouponsIncludedData(List<PromotionsModel> promotionsModelList)
        {
            List<CouponsModel> couponsModel = new List<CouponsModel>();
            foreach (var item in promotionsModelList)
            {
                var promotionIdDecrypted = Obfuscation.Decode(item.PromotionId);
                var couponsDetails = (from coupon in _context.Coupons
                                    where coupon.PromotionId == promotionIdDecrypted
                                    select new CouponsModel()
                                    {
                                        CouponId = Obfuscation.Encode(coupon.CouponId),
                                        PromotionId = Obfuscation.Encode(Convert.ToInt32(coupon.PromotionId)),
                                        UserId = Obfuscation.Encode(Convert.ToInt32(coupon.UserId)),
                                        CreatedAt = coupon.CreatedAt
                                    }).AsEnumerable().FirstOrDefault();
                if (couponsDetails != null)
                    couponsModel.Add(couponsDetails);
            }
            var couponsList = couponsModel.GroupBy(x => x.CouponId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(couponsList.Cast<dynamic>().ToList());
        }
    }
}
