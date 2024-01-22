using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto>? list = new List<ProductDto>();
            ResponseDto? responseDto = await _productService.GetAllProductsAsync();
            if(responseDto != null && responseDto.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responseDto.Result));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(list);
        }
        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto productDto)
        {
            if(ModelState.IsValid)
            {
                ResponseDto? responseDto = await _productService.CreateProductAsync(productDto);
                if (responseDto != null && responseDto.IsSuccess)
                {
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = responseDto.Message;
                }
            }
            return View(productDto);
        }
        public async Task<IActionResult> ProductEdit(int productId)
        {
            ResponseDto? responseDto = await _productService.GetProductByIdAsync(productId);

            if (responseDto != null && responseDto.IsSuccess)
            {
                ProductDto productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseDto.Result));
                return View(productDto);
            }
            return NotFound("Product with given ID not found");
        }
        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto productDto)
        {
            ResponseDto? responseDto = await _productService.UpdateProductAsync(productDto);

            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Product updated successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = responseDto.Message;
            }
            return View(productDto);
        }
        public async Task<IActionResult> ProductDelete(int productId)
        {
            ResponseDto? responseDto = await _productService.GetProductByIdAsync(productId);
            
            if (responseDto != null && responseDto.IsSuccess)
            {
                ProductDto productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseDto.Result));
                return View(productDto);
            }
            return NotFound("Product with given ID not found");
        }
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto productDto)
        {
            ResponseDto? responseDto = await _productService.DeleteProductAsync(productDto.ProductId);

            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = responseDto.Message;
            }
            return View(productDto);
        }
    }
}
