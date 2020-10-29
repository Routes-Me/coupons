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
using Obfuscation;
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
                int redemptionIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(id), _appSettings.PrimeInverse);
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

        public dynamic GetRedemption(string id, string officerId, Pagination pageInfo, string includedType)
        {
            RedemptionGetResponse response = new RedemptionGetResponse();
            int totalCount = 0;
            try
            {
                int redemptionIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(id), _appSettings.PrimeInverse);
                int officerIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(officerId), _appSettings.PrimeInverse);
                List<RedemptionGetModel> redemptionModelList = new List<RedemptionGetModel>();

                if (officerIdDecrypted == 0)
                {
                    if (redemptionIdDecrypted == 0)
                    {
                        redemptionModelList = (from redemption in _context.Redemptions
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = ObfuscationClass.EncodeId(redemption.RedemptionId, _appSettings.Prime).ToString(),
                                                   CouponId = ObfuscationClass.EncodeId(Convert.ToInt32(redemption.CouponId), _appSettings.Prime).ToString(),
                                                   OfficerId = ObfuscationClass.EncodeId(Convert.ToInt32(redemption.OfficerId), _appSettings.Prime).ToString(),
                                                   CreatedAt = redemption.CreatedAt
                                               }).AsEnumerable().OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Redemptions.ToList().Count();
                    }
                    else
                    {
                        redemptionModelList = (from redemption in _context.Redemptions
                                               where redemption.RedemptionId == redemptionIdDecrypted
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = ObfuscationClass.EncodeId(redemption.RedemptionId, _appSettings.Prime).ToString(),
                                                   CouponId = ObfuscationClass.EncodeId(Convert.ToInt32(redemption.CouponId), _appSettings.Prime).ToString(),
                                                   OfficerId = ObfuscationClass.EncodeId(Convert.ToInt32(redemption.OfficerId), _appSettings.Prime).ToString(),
                                                   CreatedAt = redemption.CreatedAt
                                               }).AsEnumerable().OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Redemptions.Where(x => x.RedemptionId == redemptionIdDecrypted).ToList().Count();
                    }
                }
                else
                {
                    if (redemptionIdDecrypted == 0)
                    {
                        redemptionModelList = (from redemption in _context.Redemptions
                                               where redemption.OfficerId == officerIdDecrypted
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = ObfuscationClass.EncodeId(redemption.RedemptionId, _appSettings.Prime).ToString(),
                                                   CouponId = ObfuscationClass.EncodeId(Convert.ToInt32(redemption.CouponId), _appSettings.Prime).ToString(),
                                                   OfficerId = ObfuscationClass.EncodeId(Convert.ToInt32(redemption.OfficerId), _appSettings.Prime).ToString(),
                                                   CreatedAt = redemption.CreatedAt
                                               }).AsEnumerable().OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Redemptions.Where(x => x.OfficerId == officerIdDecrypted).ToList().Count();
                    }
                    else
                    {
                        redemptionModelList = (from redemption in _context.Redemptions
                                               where redemption.RedemptionId == redemptionIdDecrypted && redemption.OfficerId == officerIdDecrypted
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = ObfuscationClass.EncodeId(redemption.RedemptionId, _appSettings.Prime).ToString(),
                                                   CouponId = ObfuscationClass.EncodeId(Convert.ToInt32(redemption.CouponId), _appSettings.Prime).ToString(),
                                                   OfficerId = ObfuscationClass.EncodeId(Convert.ToInt32(redemption.OfficerId), _appSettings.Prime).ToString(),
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
                int couponIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.CouponId), _appSettings.PrimeInverse);
                int officerIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.OfficerId), _appSettings.PrimeInverse);
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

                int InstitutionIdDecrypted1 = ObfuscationClass.DecodeId(Convert.ToInt32(model.InstitutionId), _appSettings.PrimeInverse);

                int InstitutionIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(officers.FirstOrDefault().InstitutionId), _appSettings.PrimeInverse);
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
                response.RedemptionId = ObfuscationClass.EncodeId(redemption.RedemptionId, _appSettings.Prime).ToString();
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
    }
}
