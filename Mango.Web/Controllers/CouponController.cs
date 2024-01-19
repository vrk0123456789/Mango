using Mango.Web.Models;
using Mango.Web.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto>? list = new List<CouponDto>();
            ResponseDto? responseDto = await _couponService.GetAllCouponsAsync();
            if(responseDto != null && responseDto.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(responseDto.Result));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(list);
        }
        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto couponDto)
        {
            if(ModelState.IsValid)
            {
                ResponseDto? responseDto = await _couponService.CreateCouponAsync(couponDto);
                if (responseDto != null && responseDto.IsSuccess)
                {
                    TempData["success"] = "Coupon created successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = responseDto.Message;
                }
            }
            return View(couponDto);
        }
        public async Task<IActionResult> CouponDelete(int couponId)
        {
            ResponseDto? responseDto = await _couponService.GetCouponByIdAsync(couponId);
            
            if (responseDto != null && responseDto.IsSuccess)
            {
                CouponDto couponDto = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(responseDto.Result));
                return View(couponDto);
            }
            return NotFound("Coupon with given ID not found");
        }
        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto couponDto)
        {
            ResponseDto? responseDto = await _couponService.DeleteCouponAsync(couponDto.CouponId);

            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Coupon deleted successfully";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = responseDto.Message;
            }
            return View(couponDto);
        }
    }
}
