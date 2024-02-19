using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? responseDto = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return RedirectToAction(nameof(CartIndex));
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            ResponseDto? responseDto = await _cartService.ApplyCouponAsync(cartDto);
            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Coupon applied successfully";
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return RedirectToAction(nameof(CartIndex)); ;
        }
        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = string.Empty;
            ResponseDto? responseDto = await _cartService.ApplyCouponAsync(cartDto);
            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Coupon removed successfully";
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return RedirectToAction(nameof(CartIndex)); ;
        }
        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? responseDto = await _cartService.GetCartByUserIdAsync(userId);
            if (responseDto != null && responseDto.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(responseDto.Result));
                return cartDto;
            }
            return new CartDto();
        }
        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDto? responseDto = await _cartService.EmailCart(cart);
            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly.";
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return RedirectToAction(nameof(CartIndex)); ;
        }
    }
}
