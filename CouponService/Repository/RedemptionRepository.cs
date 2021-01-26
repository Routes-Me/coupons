using CouponService.Abstraction;
using CouponService.Helper.Abstraction;
using CouponService.Helper.Model;
using CouponService.Models;
using CouponService.Models.DBModels;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoutesSecurity;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CouponService.Repository
{
    public class RedemptionRepository : IRedemptionRepository
    {
        private readonly couponserviceContext _context;
        private readonly AppSettings _appSettings;
        private readonly Dependencies _dependencies;
        private readonly IIncludedRepository _includedRepository;
        public RedemptionRepository(IOptions<AppSettings> appSettings, couponserviceContext context, IIncludedRepository includedRepository, IOptions<Dependencies> dependencies)
        {
            _context = context;
            _includedRepository = includedRepository;
            _appSettings = appSettings.Value;
            _dependencies = dependencies.Value;
        }

        public dynamic DeleteRedemption(string id)
        {
            try
            {
                int redemptionIdDecrypted = Obfuscation.Decode(id);
                var redemption = _context.Redemptions.Include(x => x.Coupon).Where(x => x.RedemptionId == redemptionIdDecrypted).FirstOrDefault();
                if (redemption == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.RedemptionNotFound, StatusCodes.Status404NotFound);

                if (redemption.Coupon != null)
                    return ReturnResponse.ErrorResponse(CommonMessage.RedemptionAssociated, StatusCodes.Status409Conflict);

                _context.Redemptions.Remove(redemption);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.RedemptionDelete, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic GetRedemption(string redemptionId, string officerId, Pagination pageInfo, string includedType)
        {
            RedemptionGetResponse response = new RedemptionGetResponse();
            int totalCount = 0;
            try
            {
                List<RedemptionGetModel> redemptionModelList = new List<RedemptionGetModel>();

                if (string.IsNullOrEmpty(officerId))
                {
                    if (string.IsNullOrEmpty(redemptionId))
                    {
                        redemptionModelList = (from redemption in _context.Redemptions
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = Obfuscation.Encode(redemption.RedemptionId),
                                                   CouponId = Obfuscation.Encode(Convert.ToInt32(redemption.CouponId)),
                                                   OfficerId = Obfuscation.Encode(Convert.ToInt32(redemption.OfficerId)),
                                                   CreatedAt = redemption.CreatedAt
                                               }).AsEnumerable().OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Redemptions.ToList().Count();
                    }
                    else
                    {
                        int redemptionIdDecrypted = Obfuscation.Decode(redemptionId);
                        redemptionModelList = (from redemption in _context.Redemptions
                                               where redemption.RedemptionId == redemptionIdDecrypted
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = Obfuscation.Encode(redemption.RedemptionId),
                                                   CouponId = Obfuscation.Encode(Convert.ToInt32(redemption.CouponId)),
                                                   OfficerId = Obfuscation.Encode(Convert.ToInt32(redemption.OfficerId)),
                                                   CreatedAt = redemption.CreatedAt
                                               }).AsEnumerable().OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Redemptions.Where(x => x.RedemptionId == redemptionIdDecrypted).ToList().Count();
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(redemptionId))
                    {
                        int officerIdDecrypted = Obfuscation.Decode(officerId);
                        redemptionModelList = (from redemption in _context.Redemptions
                                               where redemption.OfficerId == officerIdDecrypted
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = Obfuscation.Encode(redemption.RedemptionId),
                                                   CouponId = Obfuscation.Encode(Convert.ToInt32(redemption.CouponId)),
                                                   OfficerId = Obfuscation.Encode(Convert.ToInt32(redemption.OfficerId)),
                                                   CreatedAt = redemption.CreatedAt
                                               }).AsEnumerable().OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Redemptions.Where(x => x.OfficerId == officerIdDecrypted).ToList().Count();
                    }
                    else
                    {
                        int redemptionIdDecrypted = Obfuscation.Decode(redemptionId);
                        int officerIdDecrypted = Obfuscation.Decode(officerId);
                        redemptionModelList = (from redemption in _context.Redemptions
                                               where redemption.RedemptionId == redemptionIdDecrypted && redemption.OfficerId == officerIdDecrypted
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = Obfuscation.Encode(redemption.RedemptionId),
                                                   CouponId = Obfuscation.Encode(Convert.ToInt32(redemption.CouponId)),
                                                   OfficerId = Obfuscation.Encode(Convert.ToInt32(redemption.OfficerId)),
                                                   CreatedAt = redemption.CreatedAt
                                               }).AsEnumerable().OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Redemptions.Where(x => x.RedemptionId == redemptionIdDecrypted && x.OfficerId == officerIdDecrypted).ToList().Count();
                    }
                }

                dynamic includeData = new JObject();
                if (!string.IsNullOrEmpty(includedType))
                {
                    string[] includeArr = includedType.Split(',');
                    if (includeArr.Length > 0)
                    {
                        foreach (var item in includeArr)
                        {
                            if (item.ToLower() == "coupon" || item.ToLower() == "coupons")
                            {
                                includeData.coupons = _includedRepository.GetCouponIncludedData(redemptionModelList);
                            }
                            else if (item.ToLower() == "officer" || item.ToLower() == "officers")
                            {
                                includeData.officers = _includedRepository.GetOfficerIncludedData(redemptionModelList);
                            }
                            else if (item.ToLower() == "user" || item.ToLower() == "users")
                            {
                                includeData.users = _includedRepository.GetUserIncludedDataForRedemption(redemptionModelList);
                            }
                        }
                    }
                }

                if (((JContainer)includeData).Count == 0)
                    includeData = null;

                var page = new Pagination
                {
                    offset = pageInfo.offset,
                    limit = pageInfo.limit,
                    total = totalCount
                };

                response.status = true;
                response.message = CommonMessage.AuthoritiesRetrived;
                response.pagination = page;
                response.data = redemptionModelList;
                response.included = includeData;
                response.statusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic InsertRedemption(RedemptionModel model)
        {
            try
            {
                if (model.OfficerId == "") model.OfficerId = "0";
                int couponIdDecrypted = Obfuscation.Decode(model.CouponId);
                int officerIdDecrypted = Obfuscation.Decode(model.OfficerId);
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                var coupon = _context.Coupons.Include(x => x.Promotion).Include(x => x.Redemptions).Where(x => x.CouponId == couponIdDecrypted).FirstOrDefault();
                if (coupon == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.CouponsNotFound, StatusCodes.Status404NotFound);

                if (coupon.Redemptions.Count() > 0)
                {
                    var lastRedemptionDate = coupon.Redemptions.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                    if (Convert.ToDateTime(coupon.CreatedAt).AddHours(14) >= DateTime.Now)
                    {
                        TimeSpan remainHour = Convert.ToDateTime(Convert.ToDateTime(lastRedemptionDate.CreatedAt).AddHours(14)) - DateTime.Now;
                        return ReturnResponse.ErrorResponse("Coupons already redeemed. You can try again after " + Convert.ToDecimal(remainHour.TotalHours).ToString("#.##") + " hours.", StatusCodes.Status400BadRequest);
                    }
                }

                List<AuthoritiesModel> authorities = new List<AuthoritiesModel>();
                authorities = _includedRepository.GetPinData(model.InstitutionId);
                if (authorities == null || authorities.Count() == 0)
                    return ReturnResponse.ErrorResponse(CommonMessage.AuthoritiesNotFound, StatusCodes.Status404NotFound);

                if (authorities.FirstOrDefault().Pin != model.Pin)
                    return ReturnResponse.ErrorResponse(CommonMessage.PinInvalid, StatusCodes.Status404NotFound);

                if (coupon.Promotion.EndAt < DateTime.Now)
                    return ReturnResponse.ErrorResponse(CommonMessage.CouponsExpired, StatusCodes.Status400BadRequest);

                if (coupon.Promotion.StartAt > DateTime.Now)
                    return ReturnResponse.ErrorResponse(CommonMessage.CouponsBeforeStartDate, StatusCodes.Status400BadRequest);

                List<OfficerModel> officers = new List<OfficerModel>();
                officers = _includedRepository.GetOfficerData(model.OfficerId);
                if (officers == null || officers.Count() == 0)
                    return ReturnResponse.ErrorResponse(CommonMessage.OfficerNotFound, StatusCodes.Status404NotFound);

                int InstitutionIdDecrypted1 = Obfuscation.Decode(model.InstitutionId);

                int InstitutionIdDecrypted = Obfuscation.Decode(officers.FirstOrDefault().InstitutionId);
                if (coupon.Promotion.InstitutionId != InstitutionIdDecrypted)
                    return ReturnResponse.ErrorResponse(CommonMessage.OfficerDoNotBelong, StatusCodes.Status400BadRequest);



                Redemptions redemption = new Redemptions()
                {
                    CreatedAt = DateTime.Now,
                    CouponId = couponIdDecrypted,
                    OfficerId = officerIdDecrypted
                };
                _context.Redemptions.Add(redemption);
                _context.SaveChanges();
                RedemptionResponse response = new RedemptionResponse();
                response.status = true;
                response.message = CommonMessage.RedemptionInsert;
                response.statusCode = StatusCodes.Status201Created;
                response.RedemptionId = Obfuscation.Encode(redemption.RedemptionId);
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic SearchRedemption(string officerId, string q, Pagination pageInfo, string includedType)
        {
            RedemptionGetResponse response = new RedemptionGetResponse();
            int totalCount = 0;
            try
            {
                List<RedemptionGetModel> redemptionModelList = new List<RedemptionGetModel>();
                int officerIdDecrypted = Obfuscation.Decode(officerId);
                //int redemptionIdDecrypted = 0;
                //try
                //{
                //    redemptionIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(q), _appSettings.PrimeInverse);
                //}
                //catch (Exception)
                //{
                //    redemptionIdDecrypted = 0;
                //}
                
                //if (redemptionIdDecrypted == 0)
                //{
                    var redemptionsData = _context.Redemptions.Include(x => x.Coupon).Where(x => x.OfficerId == officerIdDecrypted && x.Coupon.Promotion.Title.Contains(q)).ToList();
                    redemptionModelList = GetRedemptionDetails(redemptionsData);
                    totalCount = _context.Redemptions.Where(x => x.OfficerId == officerIdDecrypted && x.Coupon.Promotion.Title.Contains(q)).ToList().Count();
                //}
                //else
                //{
                //    var redemptionsData = _context.Redemptions.Include(x => x.Coupon).Where(x => x.RedemptionId == redemptionIdDecrypted || x.OfficerId == officerIdDecrypted ).ToList();
                //    redemptionModelList = GetRedemptionDetails(redemptionsData);
                //    totalCount = _context.Redemptions.Where(x => x.RedemptionId == redemptionIdDecrypted || x.OfficerId == officerIdDecrypted).ToList().Count();
                //}

                dynamic includeData = new JObject();
                if (!string.IsNullOrEmpty(includedType))
                {
                    string[] includeArr = includedType.Split(',');
                    if (includeArr.Length > 0)
                    {
                        foreach (var item in includeArr)
                        {
                            if (item.ToLower() == "coupon" || item.ToLower() == "coupons")
                            {
                                includeData.coupons = _includedRepository.GetSearchCouponIncludedData(redemptionModelList, q);
                            }
                        }
                    }
                }

                if (((JContainer)includeData).Count == 0)
                    includeData = null;

                var page = new Pagination
                {
                    offset = pageInfo.offset,
                    limit = pageInfo.limit,
                    total = totalCount
                };

                response.status = true;
                response.message = CommonMessage.AuthoritiesRetrived;
                response.pagination = page;
                response.data = redemptionModelList;
                response.included = includeData;
                response.statusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        private List<RedemptionGetModel> GetRedemptionDetails(List<Redemptions> redemptionsData)
        {
            List<RedemptionGetModel> modelList = new List<RedemptionGetModel>();
            foreach (var item in redemptionsData)
            {
                RedemptionGetModel model = new RedemptionGetModel();
                model.RedemptionId = Obfuscation.Encode(item.RedemptionId);
                model.CouponId = Obfuscation.Encode(Convert.ToInt32(item.CouponId));
                model.OfficerId = Obfuscation.Encode(Convert.ToInt32(item.OfficerId));
                model.CreatedAt = item.CreatedAt;
                modelList.Add(model);
            }
            return modelList;
        }
    }
}
