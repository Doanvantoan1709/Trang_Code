using BaseProject.Api.Filter;
using BaseProject.Model.Entities;
using BaseProject.Service.Common;
using BaseProject.Service.EmailCheckService;
using BaseProject.Service.EmailCheckService.Dto;
using BaseProject.Service.EmailCheckService.ViewModels;
using BaseProjectNetCore.Api.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseProjectNetCore.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailCheckController : ControllerBase
    {
        private readonly IEmailCheckService _emailCheckService;

        public EmailCheckController( IEmailCheckService emailCheckService)
        {
            _emailCheckService = emailCheckService;
        }

        [HttpGet("Get/{id}")]
        public async Task<DataResponse<Incoming_emails>> Get(Guid id)
        {
            try
            {
                var data = await _emailCheckService.GetByIdAsync(id);
                return new DataResponse<Incoming_emails>()
                {
                    Message = "Lấy thông tin thành công",
                    Data = data,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<Incoming_emails>()
                {
                    Message = "Lỗi lấy thông tin Template",
                    Status = true,
                    Errors = new string[] { ex.Message }
                };
            }
        }

        [HttpPost("GetData", Name = "Xem danh sách Email hệ thống")]
        [ServiceFilter(typeof(LogActionFilter))]
        public async Task<DataResponse<PagedList<EmailCheckDto>>> GetData([FromBody] EmailCheckSearch search)
        {
            try
            {
                var res = await _emailCheckService.GetData(search);
                return new DataResponse<PagedList<EmailCheckDto>>()
                {
                    Data = res,
                    Message = "Lấy thông tin thành công",
                    Status = true

                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedList<EmailCheckDto>>()
                {
                    Message = "Lỗi lấy thông tin Template",
                    Status = true,
                    Errors = new string[] { ex.Message }
                };
            }
        }
    }
}
