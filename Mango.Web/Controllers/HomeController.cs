using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
		private readonly IProductService _productService;
		public HomeController(IProductService productService)
		{
			_productService = productService;
		}

		public async Task<IActionResult> Index()
        {
			List<ProductDto>? list = new List<ProductDto>();
			ResponseDto? responseDto = await _productService.GetAllProductsAsync();
			if (responseDto != null && responseDto.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responseDto.Result));
			}
			else
			{
				TempData["error"] = responseDto?.Message;
			}
			return View(list);
		}
        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDto? productDto = new ProductDto();
            ResponseDto? responseDto = await _productService.GetProductByIdAsync(productId);
            if (responseDto != null && responseDto.IsSuccess)
            {
                productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseDto.Result));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(productDto);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
