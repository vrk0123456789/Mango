using Mango.Web.Models;

namespace Mango.Web.Service
{
    public interface ICouponService
    {
        Task<ResponseDto?>  GetCouponAsync(string couponCode);
        Task<ResponseDto?> GetAllCouponsAsync();
        Task<ResponseDto?> GetCouponByIdAsync(string couponId);
        Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto);
        Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto);
        Task<ResponseDto?> DeleteCouponAsync(int couponId);
    }
}
