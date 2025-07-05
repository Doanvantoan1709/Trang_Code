﻿using BaseProject.Service.Core.Mapper;
using Microsoft.AspNetCore.Mvc;
using BaseProject.Model.Entities;
using BaseProject.Service.RoleOperationService;
using BaseProject.Service.RoleOperationService.Dto;
using BaseProject.Service.RoleOperationService.ViewModels;
using BaseProject.Service.Common;
using BaseProject.Service.Dto;
using BaseProject.Api.Dto;
using BaseProjectNetCore.Api.Dto;
using BaseProject.Controllers;

namespace BaseProject.Controllers
{
    [Route("api/[controller]")]
    public class RoleOperationController : BaseProjectController
    {
        private readonly IRoleOperationService _roleOperationService;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleOperationController> _logger;

        public RoleOperationController(
            IRoleOperationService roleOperationService,
            IMapper mapper,
            ILogger<RoleOperationController> logger
            )
        {
            this._roleOperationService = roleOperationService;
            this._mapper = mapper;
            _logger = logger;
        }

        [HttpPost("Create")]
        public async Task<DataResponse<List<RoleOperation>>> Create([FromBody] RoleOperationCreateVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model == null)
                    {
                        return new DataResponse<List<RoleOperation>>() { Data = null, Status = false };
                    }

                    //Xóa hết các roleOperation cũ
                    var oldRoleOperations = _roleOperationService.FindBy(x => x.RoleId == model.RoleId);
                    await _roleOperationService.DeleteAsync(oldRoleOperations);
                    var entity = new List<RoleOperation>();
                    if (model.ListOperationCreateVM.Any())
                    {
                        entity = model.ListOperationCreateVM.Where(x => x.IsAccess == 1).Select(x => new RoleOperation
                        {
                            RoleId = model.RoleId,
                            OperationId = x.OperationId,
                        }).ToList();
                    }

                    await _roleOperationService.CreateAsync(entity);
                    return new DataResponse<List<RoleOperation>>() { Data = entity, Status = true };
                }
                catch (Exception ex)
                {
                    return DataResponse<List<RoleOperation>>.False("Error", new string[] { ex.Message });
                }
            }
            return DataResponse<List<RoleOperation>>.False("Some properties are not valid", ModelStateError);
        }

        [HttpPost("Update")]
        public async Task<DataResponse<RoleOperation>> Update([FromBody] RoleOperationEditVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = await _roleOperationService.GetByIdAsync(model.Id);
                    if (entity == null)
                        return DataResponse<RoleOperation>.False("RoleOperation not found");

                    entity = _mapper.Map(model, entity);
                    await _roleOperationService.UpdateAsync(entity);
                    return new DataResponse<RoleOperation>() { Data = entity, Status = true };
                }
                catch (Exception ex)
                {
                    DataResponse<RoleOperation>.False(ex.Message);
                }
            }
            return DataResponse<RoleOperation>.False("Some properties are not valid", ModelStateError);
        }

        [HttpGet("Get/{id}")]
        public async Task<DataResponse<RoleOperationDto>> Get(Guid id)
        {
            var result = await _roleOperationService.GetDto(id);
            return new DataResponse<RoleOperationDto>
            {
                Data = result,
                Message = "Get RoleOperationDto thành công",
                Status = true
            };
        }

        [HttpPost("GetData")]
        public async Task<DataResponse<PagedList<RoleOperationDto>>> GetData([FromBody] RoleOperationSearch search)
        {
            var result = await _roleOperationService.GetData(search);
            return new DataResponse<PagedList<RoleOperationDto>>
            {
                Data = result,
                Message = "GetData PagedList<RoleOperationDto> thành công",
                Status = true
            };
        }

        [HttpDelete("Delete/{id}")]
        public async Task<DataResponse> Delete(Guid id)
        {
            try
            {
                var entity = await _roleOperationService.GetByIdAsync(id);
                await _roleOperationService.DeleteAsync(entity);
                return DataResponse.Success(null);
            }
            catch (Exception ex)
            {
                return DataResponse.False(ex.Message);
            }
        }

        [HttpGet("GetOperationByRoleId/{id}")]
        public async Task<DataResponse<List<RoleOperationViewModel>>> GetOperationByRoleId(Guid? id)
        {
            var result = await _roleOperationService.GetOperationByRoleId(id);
            return new DataResponse<List<RoleOperationViewModel>>
            {
                Data = result,
                Message = "GetOperationByRoleId PList<RoleOperationViewModel> thành công",
                Status = true
            };
        }
    }
}
