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
using static CouponService.Models.ReturnResponse;

namespace CouponService.Repository
{
    public class RedemptionRepository : IRedemptionRepository
    {
        private readonly couponserviceContext _context;
        private readonly AppSettings _appSettings;
        private readonly Dependencies _dependencies;
        private readonly IIncludedRepository _includedRepository;
        public RedemptionRepository(IOptions<AppSettings> appSettings, couponserviceContext context, IIncludedRepository includedRepository, Dependencies dependencies)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _includedRepository = includedRepository;
            _dependencies = dependencies;
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

        public dynamic GetRedemption(string id, Pagination pageInfo, string includedType)
        {
            RedemptionGetResponse response = new RedemptionGetResponse();
            int totalCount = 0;
            try
            {
                List<RedemptionModel> redemptionModelList = new List<RedemptionModel>();
                if (Convert.ToInt32(id) == 0)
                {
                    redemptionModelList = (from redemption in _context.Redemptions
                                            select new RedemptionModel()
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
                                           select new RedemptionModel()
                                           {
                                               RedemptionId = redemption.RedemptionId.ToString(),
                                               CouponId = redemption.CouponId.ToString(),
                                               OfficerId = redemption.OfficerId.ToString(),
                                               CreatedAt = redemption.CreatedAt
                                           }).OrderBy(a => a.RedemptionId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Redemptions.Where(x => x.RedemptionId == Convert.ToInt32(id)).ToList().Count();
                }

                if (redemptionModelList == null || redemptionModelList.Count == 0)
                    return ReturnResponse.ErrorResponse(CommonMessage.RedemptionNotFound, StatusCodes.Status404NotFound);

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

                var coupon = _context.Coupons.Include(x => x.Promotion).Where(x => x.CouponId == Convert.ToInt32(model.CouponId)).FirstOrDefault();
                if (coupon == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.CouponsNotFound, StatusCodes.Status404NotFound);

                OfficerModel officer = new OfficerModel();
                var client = new RestClient(_appSettings.Host + _dependencies.OfficersUrl + model.OfficerId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var officerData = JsonConvert.DeserializeObject<OfficerData>(result);
                    officer = officerData.data.FirstOrDefault();
                }

                if (Convert.ToInt32(officer.InstitutionId) == coupon.Promotion.InstitutionId)
                    return ReturnResponse.ErrorResponse(CommonMessage.OfficerDoNotBelong, StatusCodes.Status400BadRequest);

                if (coupon.Promotion.EndAt < DateTime.Now)
                    return ReturnResponse.ErrorResponse(CommonMessage.CouponsExpired, StatusCodes.Status400BadRequest);

                if (coupon.Promotion.StartAt > DateTime.Now)
                    return ReturnResponse.ErrorResponse(CommonMessage.CouponsBeforeStartDate, StatusCodes.Status400BadRequest);

                Redemptions redemption = new Redemptions()
                {
                    CreatedAt = DateTime.Now,
                    CouponId = Convert.ToInt32(model.CouponId),
                    OfficerId = Convert.ToInt32(model.OfficerId)
                };
                _context.Redemptions.Add(redemption);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.AuthoritiesInsert, true);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic UpdateRedemption(RedemptionModel model)
        {
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                var redemptions = _context.Redemptions.Where(x => x.RedemptionId == Convert.ToInt32(model.RedemptionId)).FirstOrDefault();
                if (redemptions == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.RedemptionNotFound, StatusCodes.Status404NotFound);

                redemptions.OfficerId = Convert.ToInt32(model.OfficerId);
                redemptions.CouponId = Convert.ToInt32(model.CouponId);
                _context.Redemptions.Update(redemptions);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.AuthoritiesUpdate, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
    }
}
