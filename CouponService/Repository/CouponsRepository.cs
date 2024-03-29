﻿using CouponService.Abstraction;
using CouponService.Helper.Abstraction;
using CouponService.Helper.Model;
using CouponService.Models;
using CouponService.Models.DBModels;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RoutesSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using static CouponService.Models.ReturnResponse;

namespace CouponService.Repository
{
    public class CouponsRepository : ICouponsRepository
    {
        private readonly couponserviceContext _context;
        private readonly AppSettings _appSettings;
        private readonly IIncludedRepository _includedRepository;
        public CouponsRepository(IOptions<AppSettings> appSettings, couponserviceContext context, IIncludedRepository includedRepository)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _includedRepository = includedRepository;
        }

        public dynamic DeleteCoupons(string id)
        {
            try
            {
                int couponIdDecrypted = Obfuscation.Decode(id);
                var coupon = _context.Coupons.Include(x => x.Redemptions).Where(x => x.CouponId == couponIdDecrypted).FirstOrDefault();
                if (coupon == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.CouponsNotFound, StatusCodes.Status404NotFound);

                var redemption = coupon.Redemptions.Where(x => x.CouponId == couponIdDecrypted).FirstOrDefault();
                if (redemption != null)
                {
                    _context.Redemptions.Remove(redemption);
                    _context.SaveChanges();
                }

                _context.Coupons.Remove(coupon);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.CouponsDelete, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic GetCoupons(string couponId, string userId, string promotionsId, Pagination pageInfo, string includeType)
        {
            CouponGetResponse response = new CouponGetResponse();
            int totalCount = 0;
            try
            {
                List<CouponsModel> placeModelList = new List<CouponsModel>();

                if (string.IsNullOrEmpty(userId))
                {
                    if (string.IsNullOrEmpty(couponId))
                    {
                        if (string.IsNullOrEmpty(promotionsId))
                        {
                            placeModelList = (from coupon in _context.Coupons
                                              select new CouponsModel()
                                              {
                                                  CouponId = Obfuscation.Encode(coupon.CouponId),
                                                  PromotionId = Obfuscation.Encode(Convert.ToInt32(coupon.PromotionId)),
                                                  CreatedAt = coupon.CreatedAt,
                                                  UserId = Obfuscation.Encode(Convert.ToInt32(coupon.UserId))
                                              }).AsEnumerable().OrderBy(a => a.CouponId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                            totalCount = _context.Coupons.ToList().Count();
                        }
                        else
                        {
                            int promotionsIdDecrypted = Obfuscation.Decode(promotionsId);
                            placeModelList = (from coupon in _context.Coupons
                                              where coupon.PromotionId == promotionsIdDecrypted
                                              select new CouponsModel()
                                              {
                                                  CouponId = Obfuscation.Encode(coupon.CouponId),
                                                  PromotionId = Obfuscation.Encode(Convert.ToInt32(coupon.PromotionId)),
                                                  CreatedAt = coupon.CreatedAt,
                                                  UserId = Obfuscation.Encode(Convert.ToInt32(coupon.UserId))
                                              }).AsEnumerable().OrderBy(a => a.CouponId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                            totalCount = _context.Coupons.ToList().Count();
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(promotionsId))
                        {
                            int couponIdDecrypted = Obfuscation.Decode(couponId);
                            placeModelList = (from coupon in _context.Coupons
                                              where coupon.CouponId == couponIdDecrypted
                                              select new CouponsModel()
                                              {
                                                  CouponId = Obfuscation.Encode(coupon.CouponId),
                                                  PromotionId = Obfuscation.Encode(Convert.ToInt32(coupon.PromotionId)),
                                                  CreatedAt = coupon.CreatedAt,
                                                  UserId = Obfuscation.Encode(Convert.ToInt32(coupon.UserId))
                                              }).AsEnumerable().OrderBy(a => a.CouponId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                            totalCount = _context.Coupons.Where(x => x.CouponId == couponIdDecrypted).ToList().Count();
                        }
                        else
                        {
                            int couponIdDecrypted = Obfuscation.Decode(couponId);
                            int promotionsIdDecrypted = Obfuscation.Decode(promotionsId);
                            placeModelList = (from coupon in _context.Coupons
                                              where coupon.CouponId == couponIdDecrypted && coupon.PromotionId == promotionsIdDecrypted
                                              select new CouponsModel()
                                              {
                                                  CouponId = Obfuscation.Encode(coupon.CouponId),
                                                  PromotionId = Obfuscation.Encode(Convert.ToInt32(coupon.PromotionId)),
                                                  CreatedAt = coupon.CreatedAt,
                                                  UserId = Obfuscation.Encode(Convert.ToInt32(coupon.UserId))
                                              }).AsEnumerable().OrderBy(a => a.CouponId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                            totalCount = _context.Coupons.Where(x => x.CouponId == couponIdDecrypted).ToList().Count();
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(couponId))
                    {
                        if (string.IsNullOrEmpty(promotionsId))
                        {
                            int userIdDecrypted = Obfuscation.Decode(userId);
                            placeModelList = (from coupon in _context.Coupons
                                              where coupon.UserId == userIdDecrypted
                                              select new CouponsModel()
                                              {
                                                  CouponId = Obfuscation.Encode(coupon.CouponId),
                                                  PromotionId = Obfuscation.Encode(Convert.ToInt32(coupon.PromotionId)),
                                                  CreatedAt = coupon.CreatedAt,
                                                  UserId = Obfuscation.Encode(Convert.ToInt32(coupon.UserId))
                                              }).AsEnumerable().OrderBy(a => a.CouponId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                            totalCount = _context.Coupons.Where(x => x.UserId == userIdDecrypted).ToList().Count();
                        }
                        else
                        {
                            int userIdDecrypted = Obfuscation.Decode(userId);
                            int promotionsIdDecrypted = Obfuscation.Decode(promotionsId);
                            placeModelList = (from coupon in _context.Coupons
                                              where coupon.UserId == userIdDecrypted && coupon.PromotionId == promotionsIdDecrypted
                                              select new CouponsModel()
                                              {
                                                  CouponId = Obfuscation.Encode(coupon.CouponId),
                                                  PromotionId = Obfuscation.Encode(Convert.ToInt32(coupon.PromotionId)),
                                                  CreatedAt = coupon.CreatedAt,
                                                  UserId = Obfuscation.Encode(Convert.ToInt32(coupon.UserId))
                                              }).AsEnumerable().OrderBy(a => a.CouponId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                            totalCount = _context.Coupons.Where(x => x.UserId == userIdDecrypted).ToList().Count();
                        }

                    }
                    else
                    {
                        if (string.IsNullOrEmpty(promotionsId))
                        {
                            int couponIdDecrypted = Obfuscation.Decode(couponId);
                            int userIdDecrypted = Obfuscation.Decode(userId);
                            placeModelList = (from coupon in _context.Coupons
                                              where coupon.CouponId == couponIdDecrypted && coupon.UserId == userIdDecrypted
                                              select new CouponsModel()
                                              {
                                                  CouponId = Obfuscation.Encode(coupon.CouponId),
                                                  PromotionId = Obfuscation.Encode(Convert.ToInt32(coupon.PromotionId)),
                                                  CreatedAt = coupon.CreatedAt,
                                                  UserId = Obfuscation.Encode(Convert.ToInt32(coupon.UserId))
                                              }).AsEnumerable().OrderBy(a => a.CouponId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                            totalCount = _context.Coupons.Where(x => x.CouponId == couponIdDecrypted && x.UserId == userIdDecrypted).ToList().Count();
                        }
                        else
                        {
                            int couponIdDecrypted = Obfuscation.Decode(couponId);
                            int userIdDecrypted = Obfuscation.Decode(userId);
                            int promotionsIdDecrypted = Obfuscation.Decode(promotionsId);
                            placeModelList = (from coupon in _context.Coupons
                                              where coupon.CouponId == couponIdDecrypted && coupon.UserId == userIdDecrypted && coupon.PromotionId == promotionsIdDecrypted
                                              select new CouponsModel()
                                              {
                                                  CouponId = Obfuscation.Encode(coupon.CouponId),
                                                  PromotionId = Obfuscation.Encode(Convert.ToInt32(coupon.PromotionId)),
                                                  CreatedAt = coupon.CreatedAt,
                                                  UserId = Obfuscation.Encode(Convert.ToInt32(coupon.UserId))
                                              }).AsEnumerable().OrderBy(a => a.CouponId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                            totalCount = _context.Coupons.Where(x => x.CouponId == couponIdDecrypted && x.UserId == userIdDecrypted).ToList().Count();
                        }
                    }
                }

                dynamic includeData = new JObject();
                if (!string.IsNullOrEmpty(includeType))
                {
                    string[] includeArr = includeType.Split(',');
                    if (includeArr.Length > 0)
                    {
                        foreach (var item in includeArr)
                        {
                            if (item.ToLower() == "user" || item.ToLower() == "users")
                            {
                                includeData.users = _includedRepository.GetUsersIncludedData(placeModelList);
                            }
                            else if (item.ToLower() == "promotion" || item.ToLower() == "promotions")
                            {
                                includeData.promotions = _includedRepository.GetPromotionIncludedData(placeModelList);
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
                response.message = CommonMessage.CouponsRetrived;
                response.pagination = page;
                response.data = placeModelList;
                response.included = includeData;
                response.statusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic InsertCoupons(CouponsModel model)
        {
            try
            {
                int promotionIdDecrypted = Obfuscation.Decode(model.PromotionId);
                int userIdDecrypted = Obfuscation.Decode(model.UserId);
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                var promotion = _context.Promotions.Include(x => x.Coupons).Where(x => x.PromotionId == promotionIdDecrypted).FirstOrDefault();
                if (promotion == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.PromotionsNotFound, StatusCodes.Status404NotFound);

                if (promotion.Coupons.Count >= promotion.UsageLimit)
                    return ReturnResponse.ErrorResponse(CommonMessage.PromotionsUsageLimitExceed, StatusCodes.Status422UnprocessableEntity);

                var couponData = promotion.Coupons.Where(x => x.UserId == userIdDecrypted).OrderByDescending(x => x.CouponId).FirstOrDefault();
                if (couponData != null)
                {
                    if (Convert.ToDateTime(couponData.CreatedAt).AddHours(14) >= DateTime.Now)
                    {
                        TimeSpan remainHour = Convert.ToDateTime(Convert.ToDateTime(couponData.CreatedAt).AddHours(14)) - DateTime.Now;
                        return ReturnResponse.ErrorResponse("Coupons already generated for this promotion. You can try again after " + Convert.ToDecimal(remainHour.TotalHours).ToString("#.##") + " hours.", StatusCodes.Status400BadRequest);
                    }
                }

                Coupons coupon = new Coupons()
                {
                    CreatedAt = DateTime.Now,
                    PromotionId = promotionIdDecrypted,
                    UserId = userIdDecrypted
                };
                _context.Coupons.Add(coupon);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.CouponsInsert, true);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
    }
}
