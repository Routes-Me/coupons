using CouponService.Abstraction;
using CouponService.Helper.Model;
using CouponService.Models;
using CouponService.Models.DBModels;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using static CouponService.Models.ReturnResponse;

namespace CouponService.Repository
{
    public class PlacesRepository : IPlacesRepository
    {
        private readonly couponserviceContext _context;
        private readonly AppSettings _appSettings;
        public PlacesRepository(IOptions<AppSettings> appSettings, couponserviceContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }
        public dynamic DeletePlaces(string id)
        {
            try
            {
                var place = _context.Places.Include(x => x.PromotionsPlaces).Where(x => x.PlaceId == Convert.ToInt32(id)).FirstOrDefault();
                if (place == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.PlacesNotFound, StatusCodes.Status404NotFound);

                if (place.PromotionsPlaces.Count > 0)
                    return ReturnResponse.ErrorResponse(CommonMessage.PlacesConflict, StatusCodes.Status409Conflict);

                _context.Places.Remove(place);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.PlacesDelete, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic GetPlaces(string id, Pagination pageInfo)
        {
            PlacesGetResponse response = new PlacesGetResponse();
            int totalCount = 0;
            try
            {                
                List<PlacesModel> placeModelList = new List<PlacesModel>();
                if (Convert.ToInt32(id) == 0)
                {
                    placeModelList = (from place in _context.Places
                                      select new PlacesModel()
                                      {
                                          PlaceId = place.PlaceId.ToString(),
                                          Latitude = place.Latitude,
                                          Longitude = place.Longitude,
                                          Name = place.Name
                                      }).OrderBy(a => a.PlaceId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Authorities.ToList().Count();
                }
                else
                {
                    placeModelList = (from place in _context.Places
                                      where place.PlaceId == Convert.ToInt32(id)
                                       select new PlacesModel()
                                       {
                                           PlaceId = place.PlaceId.ToString(),
                                           Latitude = place.Latitude,
                                           Longitude = place.Longitude,
                                           Name = place.Name
                                       }).OrderBy(a => a.PlaceId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    totalCount = _context.Authorities.Where(x => x.AuthorityId == Convert.ToInt32(id)).ToList().Count();
                }


                var page = new Pagination
                {
                    offset = pageInfo.offset,
                    limit = pageInfo.limit,
                    total = totalCount
                };

                response.status = true;
                response.message = CommonMessage.PlacesRetrived;
                response.pagination = page;
                response.data = placeModelList;
                response.statusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic InsertPlaces(PlacesModel model)
        {
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                Places place = new Places()
                {
                    Name = model.Name,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude
                };
                _context.Places.Add(place);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.PlacesInsert, true);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic UpdatePlaces(PlacesModel model)
        {
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.BadRequest, StatusCodes.Status400BadRequest);

                var places = _context.Places.Where(x => x.PlaceId == Convert.ToInt32(model.PlaceId)).FirstOrDefault();
                if (places == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.AuthoritiesNotFound, StatusCodes.Status404NotFound);

                places.Name = model.Name;
                places.Latitude = model.Latitude;
                places.Longitude = model.Longitude;
                _context.Places.Update(places);
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
