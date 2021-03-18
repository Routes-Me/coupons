using CouponService.Models;
using CouponService.Models.ResponseModel;

namespace CouponService.Abstraction
{
    public interface IPlacesRepository
    {
        dynamic GetPlaces(string placeId, Pagination pageInfo);
        dynamic UpdatePlaces(PlacesModel model);
        dynamic DeletePlaces(string id);
        dynamic InsertPlaces(PlacesModel model);
    }
}
