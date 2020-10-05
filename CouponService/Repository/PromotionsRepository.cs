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
using System;
using System.Collections.Generic;
using System.Drawing;
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
                var promotion = _context.Promotions.Include(x => x.PromotionsPlaces).Where(x => x.PromotionId == Convert.ToInt32(id)).FirstOrDefault();
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
                Transposition transposition = new Transposition();
                List<PromotionsModel> promotionsModelList = new List<PromotionsModel>();

                if (Convert.ToInt32(id) == 0)
                {
                    promotionsModelList = (from promotion in _context.Promotions
                                           select new PromotionsModel()
                                           {
                                               PromotionId = promotion.PromotionId.ToString(),
                                               AdvertisementId = promotion.AdvertisementId.ToString(),
                                               CreatedAt = promotion.CreatedAt,
                                               EndAt = promotion.EndAt,
                                               InstitutionId = promotion.InstitutionId.ToString(),
                                               IsSharable = promotion.IsSharable,
                                               LogoUrl = promotion.LogoUrl,
                                               StartAt = promotion.StartAt,
                                               Subtitle = promotion.Subtitle,
                                               Title = promotion.Title,
                                               UpdatedAt = promotion.UpdatedAt,
                                               UsageLimit = promotion.UsageLimit
                                           }).OrderBy(a => a.PromotionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Promotions.ToList().Count();
                }
                else
                {
                    promotionsModelList = (from promotion in _context.Promotions
                                           where promotion.PromotionId == Convert.ToInt32(id)
                                           select new PromotionsModel()
                                           {
                                               PromotionId = promotion.PromotionId.ToString(),
                                               AdvertisementId = promotion.AdvertisementId.ToString(),
                                               CreatedAt = promotion.CreatedAt,
                                               EndAt = promotion.EndAt,
                                               InstitutionId = promotion.InstitutionId.ToString(),
                                               IsSharable = promotion.IsSharable,
                                               LogoUrl = promotion.LogoUrl,
                                               StartAt = promotion.StartAt,
                                               Subtitle = promotion.Subtitle,
                                               Title = promotion.Title,
                                               UpdatedAt = promotion.UpdatedAt,
                                               UsageLimit = promotion.UsageLimit
                                           }).OrderBy(a => a.PromotionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Promotions.Where(x => x.PromotionId == Convert.ToInt32(id)).ToList().Count();
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
                            if (item.ToLower() == "advertisement")
                            {
                                includeData.advertisements = _includedRepository.GetAdvertisementsIncludedData(promotionsModelList);
                            }
                            else if (item.ToLower() == "institution")
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
                List<PromotionsModel> promotionsModelList = new List<PromotionsModel>();
                if (Convert.ToInt32(id) == 0)
                {
                    promotionsModelList = (from promotion in _context.Promotions
                                           select new PromotionsModel()
                                           {
                                               PromotionId = promotion.PromotionId.ToString(),
                                               AdvertisementId = promotion.AdvertisementId.ToString(),
                                               CreatedAt = promotion.CreatedAt,
                                               EndAt = promotion.EndAt,
                                               InstitutionId = promotion.InstitutionId.ToString(),
                                               IsSharable = promotion.IsSharable,
                                               LogoUrl = promotion.LogoUrl,
                                               StartAt = promotion.StartAt,
                                               Subtitle = promotion.Subtitle,
                                               Title = promotion.Title,
                                               UpdatedAt = promotion.UpdatedAt,
                                               UsageLimit = promotion.UsageLimit
                                           }).OrderBy(a => a.PromotionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Promotions.ToList().Count();
                }
                else
                {
                    promotionsModelList = (from promotion in _context.Promotions
                                           where promotion.AdvertisementId == Convert.ToInt32(id)
                                           select new PromotionsModel()
                                           {
                                               PromotionId = promotion.PromotionId.ToString(),
                                               AdvertisementId = promotion.AdvertisementId.ToString(),
                                               CreatedAt = promotion.CreatedAt,
                                               EndAt = promotion.EndAt,
                                               InstitutionId = promotion.InstitutionId.ToString(),
                                               IsSharable = promotion.IsSharable,
                                               LogoUrl = promotion.LogoUrl,
                                               StartAt = promotion.StartAt,
                                               Subtitle = promotion.Subtitle,
                                               Title = promotion.Title,
                                               UpdatedAt = promotion.UpdatedAt,
                                               UsageLimit = promotion.UsageLimit
                                           }).OrderBy(a => a.PromotionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Promotions.Where(x => x.AdvertisementId == Convert.ToInt32(id)).ToList().Count();
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
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                Promotions promotion = new Promotions()
                {
                    AdvertisementId = Convert.ToInt32(model.AdvertisementId),
                    LogoUrl = model.LogoUrl,
                    EndAt = model.EndAt,
                    IsSharable = model.IsSharable,
                    InstitutionId = Convert.ToInt32(model.InstitutionId),
                    CreatedAt = DateTime.Now,
                    StartAt = model.StartAt,
                    Subtitle = model.Subtitle,
                    Title = model.Title,
                    UpdatedAt = model.UpdatedAt,
                    UsageLimit = model.UsageLimit
                };
                _context.Promotions.Add(promotion);
                _context.SaveChanges();

                PromotionsPlaces promotionsPlace = new PromotionsPlaces()
                {
                    PromotionId = promotion.PromotionId,
                    PlaceId = Convert.ToInt32(model.PlaceId)
                };
                _context.PromotionsPlaces.Add(promotionsPlace);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.PromotionsInsert, true);
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

                var promotion = _context.Promotions.Include(x => x.PromotionsPlaces).Where(x => x.PromotionId == Convert.ToInt32(model.PromotionId)).FirstOrDefault();
                if (promotion == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.PromotionsNotFound, StatusCodes.Status404NotFound);

                var place = _context.Places.Where(x => x.PlaceId == Convert.ToInt32(model.PlaceId)).FirstOrDefault();
                if (place == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.PlacesNotFound, StatusCodes.Status404NotFound);

                var promotionPlace = promotion.PromotionsPlaces.Where(x => x.PromotionId == Convert.ToInt32(model.PromotionId)).FirstOrDefault();

                promotion.AdvertisementId = Convert.ToInt32(model.AdvertisementId);
                promotion.LogoUrl = model.LogoUrl;
                promotion.EndAt = model.EndAt;
                promotion.IsSharable = model.IsSharable;
                promotion.InstitutionId = Convert.ToInt32(model.InstitutionId);
                promotion.CreatedAt = model.CreatedAt;
                promotion.StartAt = model.StartAt;
                promotion.Subtitle = model.Subtitle;
                promotion.Title = model.Title;
                promotion.UpdatedAt = model.UpdatedAt;
                promotion.UsageLimit = model.UsageLimit;

                promotionPlace.PlaceId = Convert.ToInt32(model.PlaceId);

                _context.Promotions.Update(promotion);
                _context.PromotionsPlaces.Update(promotionPlace);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.PlacesUpdate, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
    }
}
