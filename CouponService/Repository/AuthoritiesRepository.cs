using CouponService.Abstraction;
using CouponService.Helper.Abstraction;
using CouponService.Helper.Model;
using CouponService.Models;
using CouponService.Models.DBModels;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using static CouponService.Models.ReturnResponse;

namespace CouponService.Repository
{
    public class AuthoritiesRepository : IAuthoritiesRepository
    {
        private readonly couponserviceContext _context;
        private readonly AppSettings _appSettings;
        private readonly IIncludedRepository _includedRepository;
        public AuthoritiesRepository(IOptions<AppSettings> appSettings, couponserviceContext context, IIncludedRepository includedRepository)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _includedRepository = includedRepository;
        }

        public dynamic DeleteAuthorities(string id)
        {
            try
            {
                var authorities = _context.Authorities.Where(x => x.AuthorityId == Convert.ToInt32(id)).FirstOrDefault();
                if (authorities == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.AuthoritiesNotFound, StatusCodes.Status404NotFound);

                _context.Authorities.Remove(authorities);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.AuthoritiesDelete, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic GetAuthorities(string id, Pagination pageInfo, string includeType)
        {
            AuthoritiesGetResponse response = new AuthoritiesGetResponse();
            int totalCount = 0;
            try
            {
               
                List<AuthoritiesModel> authoritiesModelList = new List<AuthoritiesModel>();
                if (Convert.ToInt32(id) == 0)
                {
                    authoritiesModelList = (from authority in _context.Authorities
                                                  select new AuthoritiesModel()
                                                  {
                                                      AuthorityId = authority.AuthorityId.ToString(),
                                                      InstitutionId = authority.InstitutionId.ToString(),
                                                      Pin = authority.Pin
                                                  }).OrderBy(a => a.AuthorityId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Authorities.ToList().Count();
                }
                else
                {
                    authoritiesModelList = (from authority in _context.Authorities
                                            where authority.AuthorityId == Convert.ToInt32(id)
                                            select new AuthoritiesModel()
                                            {
                                                AuthorityId = authority.AuthorityId.ToString(),
                                                InstitutionId = authority.InstitutionId.ToString(),
                                                Pin = authority.Pin
                                            }).OrderBy(a => a.AuthorityId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Authorities.Where(x => x.AuthorityId == Convert.ToInt32(id)).ToList().Count();
                }

                if (authoritiesModelList == null || authoritiesModelList.Count == 0)
                    return ReturnResponse.ErrorResponse(CommonMessage.AuthoritiesNotFound, StatusCodes.Status404NotFound);

                dynamic includeData = new JObject();
                if (!string.IsNullOrEmpty(includeType))
                {
                    string[] includeArr = includeType.Split(',');
                    if (includeArr.Length > 0)
                    {
                        foreach (var item in includeArr)
                        {
                            if (item.ToLower() == "institution")
                            {
                                includeData.institutions = _includedRepository.GetInstitutionsIncludedData(authoritiesModelList);
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
                response.data = authoritiesModelList;
                response.included = includeData;
                response.statusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic InsertAuthorities(AuthoritiesModel model)
        {
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                Authorities authority = new Authorities()
                {
                    AuthorityId = Convert.ToInt32(model.AuthorityId),
                    InstitutionId = Convert.ToInt32(model.InstitutionId),
                    Pin = model.Pin
                };
                _context.Authorities.Add(authority);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.AuthoritiesInsert, true);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic UpdateAuthorities(AuthoritiesModel model)
        {
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                var authorities = _context.Authorities.Where(x => x.AuthorityId == Convert.ToInt32(model.AuthorityId)).FirstOrDefault();
                if (authorities == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.AuthoritiesNotFound, StatusCodes.Status404NotFound);

                authorities.AuthorityId = Convert.ToInt32(model.AuthorityId);
                authorities.InstitutionId = Convert.ToInt32(model.InstitutionId);
                authorities.Pin = model.Pin;
                _context.Authorities.Update(authorities);
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
