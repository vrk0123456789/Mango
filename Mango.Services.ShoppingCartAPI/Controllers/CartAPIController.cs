using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private ResponseDto _responseDto;
        private IMapper _mapper;
        public CartAPIController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }
        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> Upsert(CartDto cartDto)
        {
            return null;
        }
    }
}
