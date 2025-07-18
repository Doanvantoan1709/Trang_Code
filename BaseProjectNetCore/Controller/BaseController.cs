using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BaseProject.Service.Common.Service;
using BaseProject.Model.Entities;
using BaseProject.Service.Core.Mapper;
using BaseProject.Api.Dto;
using BaseProject.Service.Common;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Service.Common;
using BaseProjectNetCore.Api.Dto;
namespace BaseProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController<T, TCreateVM, TUpdateVM, TDto> : ControllerBase
        where T : class, IEntity
        where TDto : class
        where TCreateVM : class
        where TUpdateVM : IHasId<Guid>

    {
        protected readonly IService<T> service;
        protected readonly IMapper mapper;
        protected BaseController(IService<T> service,IMapper mapper)
        {
            this.service = service;
            this.mapper = mapper;
        }


        protected Guid? UserId
        {
            get
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(id, out var userId))
                {
                    return userId;
                }
                return null;
            }
        }
        protected Guid? DonViId
        {
            get
            {
                var id = User.FindFirst(ClaimTypes.Locality)?.Value;
                if (Guid.TryParse(id, out var donViId))
                {
                    return donViId;
                }
                return null;
            }
        }


        protected List<string>? Roles
        {
            get
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var rs = new List<string>();
                if (!string.IsNullOrWhiteSpace(role))
                {
                    rs = role.Split(",").ToList();
                }
                return rs;
            }
        }

        protected bool HasRole(string role)
        {
            var lstRole = Roles;
            return lstRole != null && lstRole.Any() && lstRole.Contains(role);
        }



        [HttpPost("Create")]
        public virtual async Task<DataResponse<T>> Create([FromBody] TCreateVM model)
        {
            if (!ModelState.IsValid)
                return DataResponse<T>.False("Dữ liệu không hợp lệ", ModelStateError);

            try
            {
                var entity = mapper.Map<TCreateVM,T>(model);
                await service.CreateAsync(entity);
                return new DataResponse<T> { Data = entity, Status = true };
            }
            catch (Exception ex)
            {
                return DataResponse<T>.False("Lỗi hệ thống", new[] { ex.Message });
            }
        }

        [HttpPut("Update")]
        public virtual async Task<DataResponse<T>> Update([FromBody] TUpdateVM model)
        {
            try
            {
                var entity = await service.GetByIdAsync(model.Id);
                if (entity == null)
                    return DataResponse<T>.False("Không tìm thấy dữ liệu để cập nhật");

                entity = mapper.Map(model, entity);
                await service.UpdateAsync(entity);
                return new DataResponse<T> { Data = entity, Status = true };
            }
            catch (Exception ex)
            {
                return DataResponse<T>.False("Lỗi khi cập nhật", new[] { ex.Message });
            }
        }

        [HttpGet("Get/{id}")]
        public virtual async Task<DataResponse<TDto>> Get([FromRoute] Guid id)
        {
                var entity = await service.GetByIdAsync(id);
                if (entity == null)
                    return DataResponse<TDto>.False("Không tìm thấy dữ liệu");

                var result = mapper.Map<T, TDto>(entity);
                return new DataResponse<TDto> { Data = result, Status = true, Message = "Lấy dữ liệu thành công" };
        }
        [HttpDelete("Delete/{id}")]
        public virtual async Task<DataResponse> Delete([FromRoute] Guid id)
        {
           
                var entity = await service.GetByIdAsync(id);
                if (entity == null)
                    return DataResponse.False("Không tìm thấy dữ liệu để xóa");

                await service.DeleteAsync(entity);
                return DataResponse.Success(null);
        }
        #region Helper
        protected virtual string[] ModelStateError =>
            ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
        #endregion
    }
}
