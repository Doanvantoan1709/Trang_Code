using BaseProject.Service.Core.Mapper;
using Microsoft.AspNetCore.Mvc;
using BaseProject.Model.Entities;
using BaseProject.Service.ApiPermissionsService;
using BaseProject.Service.ApiPermissionsService.Dto;
using BaseProject.Service.ApiPermissionsService.ViewModels;
using BaseProject.Service.Common;
using BaseProject.Api.Filter;
using CommonHelper.Excel;
using CommonHelper.Extenions;
using BaseProject.Web.Common;
using BaseProject.Api.ViewModels.Import;
using BaseProject.Service.TaiLieuDinhKemService;
using BaseProject.Api.Dto;
using BaseProject.Service.Dto;
using BaseProject.Service.Constant;
using System.Reflection;
using BaseProjectNetCore.Api.Dto;
using BaseProject.Controllers;


namespace BaseProject.Controllers
{
    [Route("api/[controller]")]
    public class ApiPermissionsController : BaseProjectController
    {
        private readonly IApiPermissionsService _apiPermissionsService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IMapper _mapper;
        private readonly ILogger<ApiPermissionsController> _logger;

        public ApiPermissionsController(
            IApiPermissionsService apiPermissionsService,
            ITaiLieuDinhKemService taiLieuDinhKemService,
            IMapper mapper,
            ILogger<ApiPermissionsController> logger
            )
        {
            this._apiPermissionsService = apiPermissionsService;
            this._taiLieuDinhKemService = taiLieuDinhKemService;
            this._mapper = mapper;
            _logger = logger;
        }

        [HttpPost("Save")]
        public async Task<DataResponse> Save([FromBody] ApiPermissionsSaveVM model)
        {
            try
            {
                await _apiPermissionsService.Save(model);
                return DataResponse.Success(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi táº¡o ApiPermissions");
                return DataResponse.False("ÄÃ£ xáº£y ra lá»—i khi táº¡o dá»¯ liá»‡u.");
            }
        }



        [HttpGet("Get/{id}")]
        public async Task<DataResponse<ApiPermissionsDto>> Get(Guid id)
        {
            var dto = await _apiPermissionsService.GetDto(id);
            return DataResponse<ApiPermissionsDto>.Success(dto);
        }

        [HttpGet("GetByRoleId/{roleId}")]
        public async Task<DataResponse<List<ApiPermissionGroupData>>> GetByRoleId(Guid roleId)
        {
            var items = await _apiPermissionsService.GetByRoleId(roleId);
            var allChecked = items.Any(i => i.Path == "/api");
            var controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "BaseProject.Controllers" && t.Name.EndsWith("Controller"))
                .Select(t =>
                {
                    var name = t.Name.Replace("Controller", "").ToLower();
                    var groupChecked = allChecked || items.Any(i => i.Path == $"/api/{name}");
                    return new ApiPermissionGroupData
                    {
                        Name = t.Name,
                        Path = $"/api/{name}",
                        Checked = groupChecked,
                        Actions = t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                .Where(m => m.GetCustomAttributes(typeof(HttpGetAttribute), false).Any()
                                            || m.GetCustomAttributes(typeof(HttpPostAttribute), false).Any()
                                            || m.GetCustomAttributes(typeof(HttpPutAttribute), false).Any()
                                            || m.GetCustomAttributes(typeof(HttpDeleteAttribute), false).Any())
                        .Select(m =>
                        {
                            var name = $"{t.Name.Replace("Controller", "")}/{m.Name}".ToLower();
                            var actionChecked = groupChecked || items.Any(i => i.Path == $"/api/{name}".ToLower());
                            return new ApiPermissionAction
                            {
                                Name = m.Name,
                                Path = $"/api/{name}",
                                Checked = actionChecked
                            };
                        }).ToList()
                    };
                }).ToList();

            return DataResponse<List<ApiPermissionGroupData>>.Success(controllers);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<DataResponse> Delete(Guid id)
        {
            try
            {
                var entity = await _apiPermissionsService.GetByIdAsync(id);
                await _apiPermissionsService.DeleteAsync(entity);
                return DataResponse.Success(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi xÃ³a ApiPermissions vá»›i Id: {Id}", id);
                return DataResponse.False("ÄÃ£ xáº£y ra lá»—i khi xÃ³a dá»¯ liá»‡u.");
            }
        }

        [HttpGet("GetDropdowns")]
        public async Task<DataResponse<Dictionary<string, List<DropdownOption>>>> GetDropdowns([FromQuery] string[] types)
        {
            var result = new Dictionary<string, List<DropdownOption>>()
            {
            };

            return DataResponse<Dictionary<string, List<DropdownOption>>>.Success(result);
        }

    }
}
