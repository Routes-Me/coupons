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
using System.Threading.Tasks;

namespace CouponService.Repository
{
    public class LinksRepository : ILinksRepository
    {
        private readonly couponserviceContext _context;
        private readonly AppSettings _appSettings;
        private readonly IIncludedRepository _includedRepository;
        public LinksRepository(IOptions<AppSettings> appSettings, couponserviceContext context, IIncludedRepository includedRepository)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _includedRepository = includedRepository;
        }
        public dynamic DeleteLinks(string id)
        {
            try
            {
                int linkIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(id), _appSettings.PrimeInverse);
                var link = _context.Links.Include(x => x.Promotion).Where(x => x.LinkId == linkIdDecrypted).FirstOrDefault();
                if (link == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.LinksNotFound, StatusCodes.Status404NotFound);

                if (link.Promotion != null)
                    return ReturnResponse.ErrorResponse(CommonMessage.LinksConflict, StatusCodes.Status409Conflict);

                _context.Links.Remove(link);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.LinksDelete, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic GetLinks(string id, Pagination pageInfo, string includeType)
        {
            LinkResponse response = new LinkResponse();
            int totalCount = 0;
            try
            {
                int linkIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(id), _appSettings.PrimeInverse);
                List<LinksModel> linkModelList = new List<LinksModel>();
                if (linkIdDecrypted == 0)
                {
                    linkModelList = (from link in _context.Links
                                     select new LinksModel()
                                     {
                                         LinkId = ObfuscationClass.EncodeId(link.LinkId, _appSettings.Prime).ToString(),
                                         PromotionId = ObfuscationClass.EncodeId(link.PromotionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                         Web = link.Web,
                                         Ios = link.Ios,
                                         Android = link.Android
                                     }).AsEnumerable().OrderBy(a => a.LinkId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Links.ToList().Count();
                }
                else
                {
                    linkModelList = (from link in _context.Links
                                     where link.LinkId == linkIdDecrypted
                                     select new LinksModel()
                                     {
                                         LinkId = ObfuscationClass.EncodeId(link.LinkId, _appSettings.Prime).ToString(),
                                         PromotionId = ObfuscationClass.EncodeId(link.PromotionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                         Web = link.Web,
                                         Ios = link.Ios,
                                         Android = link.Android
                                     }).AsEnumerable().OrderBy(a => a.LinkId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Links.Where(x => x.LinkId == linkIdDecrypted).ToList().Count();
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
                            if (item.ToLower() == "promotion" || item.ToLower() == "promotions")
                            {
                                includeData.promotions = _includedRepository.GetLinksPromotionIncludedData(linkModelList);
                            }
                        }
                    }
                }

                response.status = true;
                response.message = CommonMessage.LinksRetrived;
                response.pagination = page;
                response.data = linkModelList;
                response.statusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic InsertLinks(LinksModel model)
        {
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                int promotionIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.PromotionId), _appSettings.PrimeInverse);
                Links link = new Links()
                {
                    PromotionId = promotionIdDecrypted,
                    Web = model.Web,
                    Ios = model.Ios,
                    Android = model.Android
                };
                _context.Links.Add(link);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.LinksInsert, true);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic UpdateLinks(LinksModel model)
        {
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                int linkIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.LinkId), _appSettings.PrimeInverse);
                int promotionIdDecrypted = ObfuscationClass.DecodeId(Convert.ToInt32(model.PromotionId), _appSettings.PrimeInverse);

                var links = _context.Links.Where(x => x.LinkId == linkIdDecrypted).FirstOrDefault();
                if (links == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.LinksNotFound, StatusCodes.Status404NotFound);

                links.PromotionId = promotionIdDecrypted;
                links.Web = model.Web;
                links.Ios = model.Ios;
                links.Android = model.Android;

                _context.Links.Update(links);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.LinksUpdate, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
    }
}
