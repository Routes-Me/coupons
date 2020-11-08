using CouponService.Abstraction;
using CouponService.Helper.Abstraction;
using CouponService.Helper.Model;
using CouponService.Models;
using CouponService.Models.DBModels;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Obfuscation;
using System;
using System.Collections.Generic;
using System.Linq;
using static CouponService.Models.ReturnResponse;

namespace CouponService.Repository
{
    public class PromotionsRepository : IPromotionsRepository
    {
        private readonly couponserviceContext _context;
        private readonly AppSettings _appSettings;
        private readonly IIncludedRepository _includedRepository;
        public PromotionsRepository(IOptions<AppSettings> appSettings, couponserviceContext context, IIncludedRepository includedRepository)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _includedRepository = includedRepository;
        }

        public dynamic DeletePromotions(string id)
        {
            try
            {
                int promotionIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(id), _appSettings.PrimeInverse);
                var promotion = _context.Promotions.Include(x => x.PromotionsPlaces).Where(x => x.PromotionId == promotionIdDecrypted).FirstOrDefault();
                if (promotion == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.PromotionsNotFound, StatusCodes.Status404NotFound);

                promotion.AdvertisementId = 0;
                _context.Promotions.Update(promotion);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.PromotionsDelete, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic GetPromotions(string id, Pagination pageInfo, string includeType)
        {
            PromotionsGetResponse response = new PromotionsGetResponse();
            int totalCount = 0;
            try
            {
                int promotionIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(id), _appSettings.PrimeInverse);
                Transposition transposition = new Transposition();
                List<PromotionsModel> promotionsModelList = new List<PromotionsModel>();

                if (promotionIdDecrypted == 0)
                {
                    promotionsModelList = (from promotion in _context.Promotions
                                           select new PromotionsModel()
                                           {
                                               PromotionId = ObfuscationClass.EncodeId(promotion.PromotionId, _appSettings.Prime).ToString(),
                                               AdvertisementId = ObfuscationClass.EncodeId(Convert.ToInt32(promotion.AdvertisementId), _appSettings.Prime).ToString(),
                                               CreatedAt = promotion.CreatedAt,
                                               EndAt = promotion.EndAt,
                                               InstitutionId = ObfuscationClass.EncodeId(Convert.ToInt32(promotion.InstitutionId), _appSettings.Prime).ToString(),
                                               IsSharable = promotion.IsSharable,
                                               LogoUrl = promotion.LogoUrl,
                                               StartAt = promotion.StartAt,
                                               Subtitle = promotion.Subtitle,
                                               Title = promotion.Title,
                                               UpdatedAt = promotion.UpdatedAt,
                                               UsageLimit = promotion.UsageLimit,
                                               Type = promotion.Type,
                                               Code = promotion.Code
                                           }).AsEnumerable().OrderBy(a => a.PromotionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Promotions.ToList().Count();
                }
                else
                {
                    promotionsModelList = (from promotion in _context.Promotions
                                           where promotion.PromotionId == promotionIdDecrypted
                                           select new PromotionsModel()
                                           {
                                               PromotionId = ObfuscationClass.EncodeId(promotion.PromotionId, _appSettings.Prime).ToString(),
                                               AdvertisementId = ObfuscationClass.EncodeId(Convert.ToInt32(promotion.AdvertisementId), _appSettings.Prime).ToString(),
                                               CreatedAt = promotion.CreatedAt,
                                               EndAt = promotion.EndAt,
                                               InstitutionId = ObfuscationClass.EncodeId(Convert.ToInt32(promotion.InstitutionId), _appSettings.Prime).ToString(),
                                               IsSharable = promotion.IsSharable,
                                               LogoUrl = promotion.LogoUrl,
                                               StartAt = promotion.StartAt,
                                               Subtitle = promotion.Subtitle,
                                               Title = promotion.Title,
                                               UpdatedAt = promotion.UpdatedAt,
                                               UsageLimit = promotion.UsageLimit,
                                               Type = promotion.Type,
                                               Code = promotion.Code
                                           }).AsEnumerable().OrderBy(a => a.PromotionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Promotions.Where(x => x.PromotionId == promotionIdDecrypted).ToList().Count();
                }
                var page = new Pagination
                {
                    offset = pageInfo.offset,
                    limit = pageInfo.limit,
                    total = totalCount
                };

                dynamic includeData = new JObject();
                if (!string.IsNullOrEmpty(includeType))
                {
                    string[] includeArr = includeType.Split(',');
                    if (includeArr.Length > 0)
                    {
                        foreach (var item in includeArr)
                        {
                            if (item.ToLower() == "advertisement" || item.ToLower() == "advertisements")
                            {
                                includeData.advertisements = _includedRepository.GetAdvertisementsIncludedData(promotionsModelList);
                            }
                            else if (item.ToLower() == "institution" || item.ToLower() == "institutions")
                            {
                                includeData.institutions = _includedRepository.GetInstitutionsIncludedData(promotionsModelList);
                            }
                        }
                    }
                }

                if (((JContainer)includeData).Count == 0)
                    includeData = null;

                response.status = true;
                response.message = CommonMessage.PromotionsRetrived;
                response.pagination = page;
                response.data = promotionsModelList;
                response.included = includeData;
                response.statusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic GetPromotionsByAdvertisementsId(string id, Pagination pageInfo)
        {
            PromotionsForContentGetResponse response = new PromotionsForContentGetResponse();
            int totalCount = 0;
            try
            {
                int advertisementIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(id), _appSettings.PrimeInverse);
                List<PromotionsModel> promotionsModelList = new List<PromotionsModel>();
                if (advertisementIdDecrypted == 0)
                {
                    promotionsModelList = (from promotion in _context.Promotions
                                           select new PromotionsModel()
                                           {
                                               PromotionId = ObfuscationClass.EncodeId(promotion.PromotionId, _appSettings.Prime).ToString(),
                                               AdvertisementId = ObfuscationClass.EncodeId(Convert.ToInt32(promotion.AdvertisementId), _appSettings.Prime).ToString(),
                                               CreatedAt = promotion.CreatedAt,
                                               EndAt = promotion.EndAt,
                                               InstitutionId = ObfuscationClass.EncodeId(Convert.ToInt32(promotion.InstitutionId), _appSettings.Prime).ToString(),
                                               IsSharable = promotion.IsSharable,
                                               LogoUrl = promotion.LogoUrl,
                                               StartAt = promotion.StartAt,
                                               Subtitle = promotion.Subtitle,
                                               Title = promotion.Title,
                                               UpdatedAt = promotion.UpdatedAt,
                                               UsageLimit = promotion.UsageLimit,
                                               Type = promotion.Type,
                                               Code = promotion.Code
                                           }).AsEnumerable().OrderBy(a => a.PromotionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Promotions.ToList().Count();
                }
                else
                {
                    promotionsModelList = (from promotion in _context.Promotions
                                           where promotion.AdvertisementId == advertisementIdDecrypted
                                           select new PromotionsModel()
                                           {
                                               PromotionId = ObfuscationClass.EncodeId(promotion.PromotionId, _appSettings.Prime).ToString(),
                                               AdvertisementId = ObfuscationClass.EncodeId(Convert.ToInt32(promotion.AdvertisementId), _appSettings.Prime).ToString(),
                                               CreatedAt = promotion.CreatedAt,
                                               EndAt = promotion.EndAt,
                                               InstitutionId = ObfuscationClass.EncodeId(Convert.ToInt32(promotion.InstitutionId), _appSettings.Prime).ToString(),
                                               IsSharable = promotion.IsSharable,
                                               LogoUrl = promotion.LogoUrl,
                                               StartAt = promotion.StartAt,
                                               Subtitle = promotion.Subtitle,
                                               Title = promotion.Title,
                                               UpdatedAt = promotion.UpdatedAt,
                                               UsageLimit = promotion.UsageLimit,
                                               Type = promotion.Type,
                                               Code = promotion.Code
                                           }).AsEnumerable().OrderBy(a => a.PromotionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Promotions.Where(x => x.AdvertisementId == advertisementIdDecrypted).ToList().Count();
                }

                var page = new Pagination
                {
                    offset = pageInfo.offset,
                    limit = pageInfo.limit,
                    total = totalCount
                };

                response.status = true;
                response.message = CommonMessage.PromotionsRetrived;
                response.pagination = page;
                response.data = promotionsModelList;
                response.statusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic InsertPromotions(PromotionsPostModel model)
        {
            PromotionsPostResponse response = new PromotionsPostResponse();
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                if (string.IsNullOrEmpty(model.Subtitle))
                    model.Subtitle = null;

                if (string.IsNullOrEmpty(model.InstitutionId))
                    model.InstitutionId = null;

                if (string.IsNullOrEmpty(model.Code))
                    model.Code = null;
                else if (model.Code.Length > 5)
                    model.Code = model.Code.Substring(0, 5);

                if (!string.IsNullOrEmpty(model.Type) && model.Type.ToString().ToLower() == "links")
                {
                    if (model.Links == null)
                    {
                        return ReturnResponse.ErrorResponse(CommonMessage.LinksRequired, StatusCodes.Status400BadRequest);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.Links.Web))
                            return ReturnResponse.ErrorResponse(CommonMessage.WebLinkRequired, StatusCodes.Status400BadRequest);
                    }
                }

                if (!string.IsNullOrEmpty(model.Type) && model.Type.ToLower() == "coupons")
                {
                    if (string.IsNullOrEmpty(Convert.ToString(model.UsageLimit)))
                        model.UsageLimit = 1000;

                    if (string.IsNullOrEmpty(Convert.ToString(model.StartAt)))
                        model.StartAt = DateTime.Now;

                    if (string.IsNullOrEmpty(Convert.ToString(model.EndAt)))
                        model.EndAt = DateTime.Now.AddMonths(1);
                }

                int advertisementIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.AdvertisementId), _appSettings.PrimeInverse);
                int institutionIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.InstitutionId), _appSettings.PrimeInverse);

                Promotions promotion = new Promotions()
                {
                    AdvertisementId = advertisementIdDecrypted,
                    LogoUrl = model.LogoUrl,
                    EndAt = model.EndAt,
                    IsSharable = model.IsSharable,
                    InstitutionId = institutionIdDecrypted,
                    CreatedAt = DateTime.Now,
                    StartAt = model.StartAt,
                    Subtitle = model.Subtitle,
                    Title = model.Title,
                    UsageLimit = model.UsageLimit,
                    Type = model.Type,
                    Code = model.Code
                };
                _context.Promotions.Add(promotion);
                _context.SaveChanges();

                if (!string.IsNullOrEmpty(model.PlaceId))
                {
                    int placeIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.PlaceId), _appSettings.PrimeInverse);
                    PromotionsPlaces promotionsPlace = new PromotionsPlaces()
                    {
                        PromotionId = promotion.PromotionId,
                        PlaceId = placeIdDecrypted
                    };
                    _context.PromotionsPlaces.Add(promotionsPlace);
                    _context.SaveChanges();
                }

                if (!string.IsNullOrEmpty(model.Type) && model.Type.ToString().ToLower() == "links")
                {
                    if (model.Links != null)
                    {
                        if (!string.IsNullOrEmpty(model.Links.Android) || !string.IsNullOrEmpty(model.Links.Web) || !string.IsNullOrEmpty(model.Links.Ios))
                        {
                            Links modelLinks = new Links()
                            {
                                PromotionId = promotion.PromotionId,
                                Web = model.Links.Web,
                                Ios = model.Links.Ios,
                                Android = model.Links.Android,
                            };
                            _context.Links.Add(modelLinks);
                            _context.SaveChanges();
                        }
                    }
                }
                response.status = true;
                response.statusCode = StatusCodes.Status201Created;
                response.message = CommonMessage.PromotionsInsert;
                response.promotionsId = ObfuscationClass.EncodeId(promotion.PromotionId, _appSettings.Prime).ToString();
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic UpdatePromotions(PromotionsPostModel model)
        {
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                if (string.IsNullOrEmpty(model.Subtitle))
                    model.Subtitle = null;

                if (string.IsNullOrEmpty(model.InstitutionId))
                    model.InstitutionId = null;

                if (string.IsNullOrEmpty(model.Code))
                    model.Code = null;
                else if (model.Code.Length > 5)
                    model.Code = model.Code.Substring(0, 5);

                if (!string.IsNullOrEmpty(model.Type) && model.Type.ToString().ToLower() == "links")
                {
                    if (model.Links == null)
                    {
                        return ReturnResponse.ErrorResponse(CommonMessage.LinksRequired, StatusCodes.Status400BadRequest);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.Links.Web))
                            return ReturnResponse.ErrorResponse(CommonMessage.WebLinkRequired, StatusCodes.Status400BadRequest);
                    }
                }

                if (!string.IsNullOrEmpty(model.Type) && model.Type.ToLower() == "coupons")
                {
                    if (string.IsNullOrEmpty(Convert.ToString(model.UsageLimit)))
                        model.UsageLimit = 1000;

                    if (string.IsNullOrEmpty(Convert.ToString(model.StartAt)))
                        model.StartAt = DateTime.Now;

                    if (string.IsNullOrEmpty(Convert.ToString(model.EndAt)))
                        model.EndAt = DateTime.Now.AddMonths(1);
                }

                int promotionIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.PromotionId), _appSettings.PrimeInverse);
                int advertisementIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.AdvertisementId), _appSettings.PrimeInverse);
                int institutionIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.InstitutionId), _appSettings.PrimeInverse);
                int placeIdDecrypted = 0;

                var promotion = _context.Promotions.Include(x => x.PromotionsPlaces).Include(x => x.Links).Where(x => x.PromotionId == promotionIdDecrypted).FirstOrDefault();
                if (promotion == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.PromotionsNotFound, StatusCodes.Status404NotFound);

                if (!string.IsNullOrEmpty(model.PlaceId))
                {
                    placeIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.PlaceId), _appSettings.PrimeInverse);
                    var place = _context.Places.Where(x => x.PlaceId == placeIdDecrypted).FirstOrDefault();
                    if (place == null)
                        return ReturnResponse.ErrorResponse(CommonMessage.PlacesNotFound, StatusCodes.Status404NotFound);
                }

                promotion.AdvertisementId = advertisementIdDecrypted;
                promotion.LogoUrl = model.LogoUrl;
                promotion.EndAt = model.EndAt;
                promotion.IsSharable = model.IsSharable;
                promotion.InstitutionId = institutionIdDecrypted;
                promotion.StartAt = model.StartAt;
                promotion.Subtitle = model.Subtitle;
                promotion.Title = model.Title;
                promotion.UpdatedAt = DateTime.Now;
                promotion.UsageLimit = model.UsageLimit;
                promotion.Type = model.Type;
                promotion.Code = model.Code;
                _context.Promotions.Update(promotion);
                _context.SaveChanges();

                if (!string.IsNullOrEmpty(model.PlaceId))
                {
                    var promotionPlace = promotion.PromotionsPlaces.Where(x => x.PromotionId == promotionIdDecrypted).FirstOrDefault();
                    promotionPlace.PlaceId = placeIdDecrypted;
                    _context.PromotionsPlaces.Update(promotionPlace);
                    _context.SaveChanges();
                }
                if (!string.IsNullOrEmpty(model.Type) && model.Type.ToString().ToLower() == "links")
                {
                    if (model.Links != null)
                    {
                        var promotionLinks = promotion.Links.Where(x => x.PromotionId == promotionIdDecrypted).FirstOrDefault();
                        if (promotionLinks != null)
                        {
                            _context.Links.Remove(promotionLinks);
                            _context.SaveChanges();
                        }

                        Links modelLinks = new Links()
                        {
                            PromotionId = promotionIdDecrypted,
                            Web = model.Links.Web,
                            Ios = model.Links.Ios,
                            Android = model.Links.Android,
                        };
                        _context.Links.Add(modelLinks);
                        _context.SaveChanges();
                    }
                }
                return ReturnResponse.SuccessResponse(CommonMessage.PromotionsUpdate, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
    }
}
