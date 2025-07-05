using BaseProject.Service.Core.Mapper;
using Microsoft.AspNetCore.Mvc;
using BaseProject.Model.Entities;
using BaseProject.Service.GroupUserRoleService;
using BaseProject.Service.GroupUserRoleService.Dto;
using BaseProject.Service.GroupUserRoleService.ViewModels;
using BaseProject.Service.Common;
using BaseProject.Api.Filter;
using CommonHelper.Excel;
using BaseProject.Web.Common;
using BaseProject.Api.ViewModels.Import;
using BaseProject.Service.TaiLieuDinhKemService;
using BaseProject.Api.Dto;
using MongoDB.Bson.Serialization.Serializers;
using BaseProject.Controllers;
using BaseProjectNetCore.Api.Dto;
using BaseProject.Model.Entities;

namespace BaseProject.Controllers
{
    [Route("api/[controller]")]
    public class GroupUserRoleController : BaseProjectController
    {
        private readonly IGroupUserRoleService _groupUserRoleService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupUserRoleController> _logger;

        public GroupUserRoleController(
            IGroupUserRoleService groupUserRoleService,
            ITaiLieuDinhKemService taiLieuDinhKemService,
            IMapper mapper,
            ILogger<GroupUserRoleController> logger
            )
        {
            this._groupUserRoleService = groupUserRoleService;
            this._taiLieuDinhKemService = taiLieuDinhKemService;
            this._mapper = mapper;
            _logger = logger;
        }

        [HttpPost("Create")]
        public async Task<DataResponse<GroupUserRole>> Create([FromBody] GroupUserRoleCreateVM model)
        {
            try
            {
                var oldRoles = _groupUserRoleService.FindBy(x => x.GroupUserId == model.GroupUserId).ToList();

                if (model.RoleId != null && model.RoleId.Any())
                {
                    var lstGroupUserRole = new List<GroupUserRole>();
                    foreach (var item in model.RoleId)
                    {
                        var groupUserRole = new GroupUserRole();
                        groupUserRole.GroupUserId = model.GroupUserId;
                        groupUserRole.RoleId = item;
                        lstGroupUserRole.Add(groupUserRole);
                    }
                    if (lstGroupUserRole.Any())
                    {
                        await _groupUserRoleService.CreateAsync(lstGroupUserRole);
                    }
                }

                if (oldRoles != null && oldRoles.Any())
                {
                    await _groupUserRoleService.DeleteAsync(oldRoles);
                }

                return DataResponse<GroupUserRole>.Success(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi táº¡o GroupUserRole");
                return DataResponse<GroupUserRole>.False("ÄÃ£ xáº£y ra lá»—i khi táº¡o dá»¯ liá»‡u.");
            }
        }


        [HttpPut("Update")]
        public async Task<DataResponse<GroupUserRole>> Update([FromBody] GroupUserRoleEditVM model)
        {
            try
            {
                var entity = await _groupUserRoleService.GetByIdAsync(model.Id);
                if (entity == null)
                    return DataResponse<GroupUserRole>.False("GroupUserRole khÃ´ng tá»“n táº¡i");

                entity = _mapper.Map(model, entity);
                await _groupUserRoleService.UpdateAsync(entity);
                return DataResponse<GroupUserRole>.Success(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi cáº­p nháº­t GroupUserRole vá»›i Id: {Id}", model.Id);
                return new DataResponse<GroupUserRole>()
                {
                    Data = null,
                    Status = false,
                    Message = "ÄÃ£ xáº£y ra lá»—i khi cáº­p nháº­t dá»¯ liá»‡u."
                };
            }
        }

        [HttpGet("Get/{id}")]
        public async Task<DataResponse<GroupUserRoleDto>> Get(Guid id)
        {
            var dto = await _groupUserRoleService.GetDto(id);
            return DataResponse<GroupUserRoleDto>.Success(dto);
        }

        [HttpPost("GetData", Name = "Xem danh sÃ¡ch GroupUserRole há»‡ thá»‘ng")]
        [ServiceFilter(typeof(LogActionFilter))]
        public async Task<DataResponse<PagedList<GroupUserRoleDto>>> GetData([FromBody] GroupUserRoleSearch search)
        {
            var data = await _groupUserRoleService.GetData(search);
            return DataResponse<PagedList<GroupUserRoleDto>>.Success(data);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<DataResponse> Delete(Guid id)
        {
            try
            {
                var entity = await _groupUserRoleService.GetByIdAsync(id);
                await _groupUserRoleService.DeleteAsync(entity);
                return DataResponse.Success(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi xÃ³a GroupUserRole vá»›i Id: {Id}", id);
                return DataResponse.False("ÄÃ£ xáº£y ra lá»—i khi xÃ³a dá»¯ liá»‡u.");
            }
        }


        [HttpGet("export")]
        public async Task<DataResponse> ExportExcel()
        {
            try
            {
                var search = new GroupUserRoleSearch();
                var data = await _groupUserRoleService.GetData(search);
                var base64Excel = await ExportExcelHelperNetCore.Export<GroupUserRoleDto>(data?.Items);
                if (string.IsNullOrEmpty(base64Excel))
                {
                    return DataResponse.False("Káº¿t xuáº¥t tháº¥t báº¡i hoáº·c dá»¯ liá»‡u trá»‘ng");
                }
                return DataResponse.Success(base64Excel);
            }
            catch (Exception ex)
            {
                return DataResponse.False("Káº¿t xuáº¥t tháº¥t báº¡i");
            }
        }

        [HttpGet("ExportTemplateImport")]
        public async Task<DataResponse<string>> ExportTemplateImport()
        {
            try
            {
                string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var base64 = ExcelImportExtention.ConvertToBase64(rootPath, "GroupUserRole");
                if (string.IsNullOrEmpty(base64))
                {
                    return DataResponse<string>.False("Káº¿t xuáº¥t tháº¥t báº¡i hoáº·c dá»¯ liá»‡u trá»‘ng");
                }
                return DataResponse<string>.Success(base64);
            }
            catch (Exception)
            {
                return DataResponse<string>.False("Káº¿t xuáº¥t tháº¥t báº¡i");
            }
        }

        [HttpGet("Import")]
        public async Task<DataResponse> Import()
        {
            try
            {
                string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                ExcelImportExtention.CreateExcelWithDisplayNames<GroupUserRole>(rootPath, "GroupUserRole");
                var columns = ExcelImportExtention.GetColumnNamesWithOrder<GroupUserRole>();
                return DataResponse.Success(columns);
            }
            catch (Exception)
            {
                return DataResponse.False("Láº¥y dá»¯ liá»‡u mÃ n hÃ¬nh import tháº¥t báº¡i");
            }
        }

        [HttpPost("ImportExcel")]
        public async Task<DataResponse> ImportExcel([FromBody] DataImport data)
        {
            try
            {
                #region Config Ä‘á»ƒ import dá»¯ liá»‡u    
                var filePathQuery = await _taiLieuDinhKemService.GetPathFromId(data.IdFile);
                string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string filePath = rootPath + filePathQuery;

                var importHelper = new ImportExcelHelperNetCore<GroupUserRole>();
                importHelper.PathTemplate = filePath;
                importHelper.StartCol = 1;
                importHelper.StartRow = 2;
                importHelper.ConfigColumn = new List<ConfigModule>();
                importHelper.ConfigColumn = ExcelImportExtention.GetConfigCol<GroupUserRole>(data.Collection);
                #endregion
                var rsl = importHelper.Import();

                var listImportReponse = new List<GroupUserRole>();
                if (rsl.ListTrue != null && rsl.ListTrue.Count > 0)
                {
                    listImportReponse.AddRange(rsl.ListTrue);
                    await _groupUserRoleService.CreateAsync(rsl.ListTrue);
                }

                var response = new ResponseImport<GroupUserRole>();


                response.ListTrue = listImportReponse;
                response.lstFalse = rsl.lstFalse;

                return DataResponse.Success(response);
            }
            catch (Exception)
            {
                return DataResponse.False("Import tháº¥t báº¡i");
            }
        }
    }
}
