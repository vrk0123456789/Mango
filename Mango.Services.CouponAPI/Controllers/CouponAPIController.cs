using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private ResponseDto _responseDto;
        private IMapper _mapper; 
        public CouponAPIController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> coupons = _appDbContext.Coupons.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<CouponDto>>(coupons);
                _responseDto.Message = "Successful";
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon coupon = _appDbContext.Coupons.First(c => c.CouponId == id);
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
                _responseDto.Message = "Successful";
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
        [HttpGet]
        [Route("GetByCouponCode/{couponCode}")]
        public ResponseDto GetByCouponCode(string couponCode)
        {
            try
            {
                Coupon coupon = _appDbContext.Coupons.First(c => c.CouponCode.ToLower() == couponCode.ToLower());
                
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
                _responseDto.Message = "Successful";
                
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
        [HttpPost]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _appDbContext.Coupons.Add(coupon);
                _appDbContext.SaveChanges();
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _appDbContext.Coupons.Update(coupon);
                _appDbContext.SaveChanges();
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
        [HttpDelete]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon coupon = _appDbContext.Coupons.First(c => c.CouponId == id);
                _appDbContext.Coupons.Remove(coupon);
                _appDbContext.SaveChanges();
                
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
    }
}
