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
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.FirstName = cartDto.CartHeader.FirstName;
            cart.CartHeader.LastName = cartDto.CartHeader.LastName;
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;

            var response = await _orderService.CreateOrderAsync(cart);
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result.ToString()));
            if(response != null && response.IsSuccess)
            {
                //get stripe session and redirect to stripe to place order
            }
            return View();
        }
        public async Task<IActionResult> Confirmation(int orderId)
        {
            return View(orderId);
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
