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
                var redemption = _context.Redemptions.Include(x => x.Coupon).Where(x => x.RedemptionId == Convert.ToInt32(id)).FirstOrDefault();
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
                List<RedemptionGetModel> redemptionModelList = new List<RedemptionGetModel>();

                if (Convert.ToInt32(officerId) == 0)
                {
                    if (Convert.ToInt32(id) == 0)
                    {
                        redemptionModelList = (from redemption in _context.Redemptions
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = redemption.RedemptionId.ToString(),
                                                   CouponId = redemption.CouponId.ToString(),
                                                   OfficerId = redemption.OfficerId.ToString(),
                                                   CreatedAt = redemption.CreatedAt
                                               }).OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Redemptions.ToList().Count();
                    }
                    else
                    {
                        redemptionModelList = (from redemption in _context.Redemptions
                                               where redemption.RedemptionId == Convert.ToInt32(id)
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = redemption.RedemptionId.ToString(),
                                                   CouponId = redemption.CouponId.ToString(),
                                                   OfficerId = redemption.OfficerId.ToString(),
                                                   CreatedAt = redemption.CreatedAt
                                               }).OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Redemptions.Where(x => x.RedemptionId == Convert.ToInt32(id)).ToList().Count();
                    }
                }
                else
                {
                    if (Convert.ToInt32(id) == 0)
                    {
                        redemptionModelList = (from redemption in _context.Redemptions
                                               where redemption.OfficerId == Convert.ToInt32(officerId)
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = redemption.RedemptionId.ToString(),
                                                   CouponId = redemption.CouponId.ToString(),
                                                   OfficerId = redemption.OfficerId.ToString(),
                                                   CreatedAt = redemption.CreatedAt
                                               }).OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Redemptions.Where(x => x.OfficerId == Convert.ToInt32(officerId)).ToList().Count();
                    }
                    else
                    {
                        redemptionModelList = (from redemption in _context.Redemptions
                                               where redemption.RedemptionId == Convert.ToInt32(id) && redemption.OfficerId == Convert.ToInt32(officerId)
                                               select new RedemptionGetModel()
                                               {
                                                   RedemptionId = redemption.RedemptionId.ToString(),
                                                   CouponId = redemption.CouponId.ToString(),
                                                   OfficerId = redemption.OfficerId.ToString(),
                                                   CreatedAt = redemption.CreatedAt
                                               }).OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Redemptions.Where(x => x.RedemptionId == Convert.ToInt32(id) && x.OfficerId == Convert.ToInt32(officerId)).ToList().Count();
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
                            if (item.ToLower() == "coupon")
                            {
                                includeData.coupons = _includedRepository.GetCouponIncludedData(redemptionModelList);
                            }
                            else if (item.ToLower() == "officer")
                            {
                                includeData.coupons = _includedRepository.GetOfficerIncludedData(redemptionModelList);
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
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                var coupon = _context.Coupons.Include(x => x.Promotion).Include(x => x.Redemptions).Where(x => x.CouponId == Convert.ToInt32(model.CouponId)).FirstOrDefault();
                if (coupon == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.CouponsNotFound, StatusCodes.Status404NotFound);

                if (coupon.Redemptions.Count() > 0)
                {
                    var timeDifference = (coupon.Redemptions.OrderByDescending(x => x.CreatedAt).FirstOrDefault().CreatedAt - DateTime.Now.AddHours(-14));
                    if (timeDifference.Value.TotalMinutes > 0)
                        return ReturnResponse.ErrorResponse(CommonMessage.CouponsRedeemed, StatusCodes.Status400BadRequest);
                }

                List<AuthoritiesModel> authorities = new List<AuthoritiesModel>();
                authorities = _includedRepository.GetPinData(model.OfficerId);
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

                if (coupon.Promotion.InstitutionId != Convert.ToInt32(officers.FirstOrDefault().InstitutionId))
                    return ReturnResponse.ErrorResponse(CommonMessage.OfficerDoNotBelong, StatusCodes.Status400BadRequest);

             

                Redemptions redemption = new Redemptions()
                {
                    CreatedAt = DateTime.Now,
                    CouponId = Convert.ToInt32(model.CouponId),
                    OfficerId = Convert.ToInt32(model.OfficerId)
                };
                _context.Redemptions.Add(redemption);
                _context.SaveChanges();
                RedemptionResponse response = new RedemptionResponse();
                response.status = true;
                response.message = CommonMessage.RedemptionInsert;
                response.statusCode = StatusCodes.Status201Created;
                response.RedemptionId = Convert.ToString(redemption.RedemptionId);
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
    }
}
