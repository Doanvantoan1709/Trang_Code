using BaseProject.Service.Core.Mapper;
using Microsoft.AspNetCore.Mvc;
using BaseProject.Model.Entities;
using BaseProject.Service.GroupUserService;
using BaseProject.Service.GroupUserService.Dto;
using BaseProject.Service.GroupUserService.ViewModels;
using BaseProject.Service.Common;
using BaseProject.Api.Filter;
using CommonHelper.Excel;
using BaseProject.Web.Common;
using BaseProject.Api.ViewModels.Import;
using BaseProject.Service.TaiLieuDinhKemService;
using BaseProject.Api.Dto;
using BaseProject.Service.Dto;
using BaseProject.Service.Constant;
using BaseProject.Controllers;
using BaseProjectNetCore.Api.Dto;
using BaseProject.Model.Entities;

namespace BaseProject.Controllers
{
    [Route("api/[controller]")]
    public class GroupUserController : BaseProjectController
    {
        private readonly IGroupUserService _groupUserService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupUserController> _logger;

        public GroupUserController(
            IGroupUserService groupUserService,
            ITaiLieuDinhKemService taiLieuDinhKemService,
            IMapper mapper,
            ILogger<GroupUserController> logger
            )
        {
            this._groupUserService = groupUserService;
            this._taiLieuDinhKemService = taiLieuDinhKemService;
            this._mapper = mapper;
            _logger = logger;
        }

        [HttpPost("Create")]
        public async Task<DataResponse<GroupUser>> Create([FromBody] GroupUserCreateVM model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.Code))
                {
                    if (await _groupUserService.AnyAsync(x => x.Code.Trim().ToLower().Equals(model.Code.Trim().ToLower())))
                    {
                        return DataResponse<GroupUser>.False("MÃ£ nhÃ³m ngÆ°á»i sá»­ dá»¥ng Ä‘Ã£ tá»“n táº¡i");
                    }
                }

                var entity = _mapper.Map<GroupUserCreateVM, GroupUser>(model);
                if (!HasRole(VaiTroConstant.Admin)) entity.DepartmentId = DonViId;
                await _groupUserService.CreateAsync(entity);
                return DataResponse<GroupUser>.Success(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi táº¡o GroupUser");
                return DataResponse<GroupUser>.False("ÄÃ£ xáº£y ra lá»—i khi táº¡o dá»¯ liá»‡u.");
            }
        }


        [HttpPut("Update")]
        public async Task<DataResponse<GroupUser>> Update([FromBody] GroupUserEditVM model)
        {
            try
            {
                var entity = await _groupUserService.GetByIdAsync(model.Id);
                if (entity == null)
                    return DataResponse<GroupUser>.False("NhÃ³m ngÆ°á»i sá»­ dá»¥ng khÃ´ng tá»“n táº¡i");

                if (!string.IsNullOrWhiteSpace(model.Code))
                {
                    if (await _groupUserService.AnyAsync(x => x.Id != model.Id && x.Code.Trim().ToLower().Equals(model.Code.Trim().ToLower())))
                    {
                        return DataResponse<GroupUser>.False("MÃ£ nhÃ³m ngÆ°á»i sá»­ dá»¥ng Ä‘Ã£ tá»“n táº¡i");
                    }
                }

                entity = _mapper.Map(model, entity);
                if (!HasRole(VaiTroConstant.Admin)) entity.DepartmentId = DonViId;
                await _groupUserService.UpdateAsync(entity);
                return DataResponse<GroupUser>.Success(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi cáº­p nháº­t GroupUser vá»›i Id: {Id}", model.Id);
                return new DataResponse<GroupUser>()
                {
                    Data = null,
                    Status = false,
                    Message = "ÄÃ£ xáº£y ra lá»—i khi cáº­p nháº­t dá»¯ liá»‡u."
                };
            }
        }

        [HttpGet("Get/{id}")]
        public async Task<DataResponse<GroupUserDto>> Get(Guid id)
        {
            var dto = await _groupUserService.GetDto(id);
            return DataResponse<GroupUserDto>.Success(dto);
        }

        [HttpPost("GetData", Name = "Xem danh sÃ¡ch GroupUser há»‡ thá»‘ng")]
        [ServiceFilter(typeof(LogActionFilter))]
        public async Task<DataResponse<PagedList<GroupUserDto>>> GetData([FromBody] GroupUserSearch search)
        {
            if (search == null)
            {
                search = new GroupUserSearch();
            }
            if (!HasRole(VaiTroConstant.Admin))
            {
                search.DepartmentId = DonViId;
            }
            var data = await _groupUserService.GetData(search);
            return DataResponse<PagedList<GroupUserDto>>.Success(data);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<DataResponse> Delete(Guid id)
        {
            try
            {
                var entity = await _groupUserService.GetByIdAsync(id);
                await _groupUserService.DeleteAsync(entity);
                return DataResponse.Success(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi xÃ³a GroupUser vá»›i Id: {Id}", id);
                return DataResponse.False("ÄÃ£ xáº£y ra lá»—i khi xÃ³a dá»¯ liá»‡u.");
            }
        }

        [HttpGet("export")]
        public async Task<DataResponse> ExportExcel()
        {
            try
            {
                var search = new GroupUserSearch();
                var data = await _groupUserService.GetData(search);
                var base64Excel = await ExportExcelHelperNetCore.Export<GroupUserDto>(data?.Items);
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
                var base64 = ExcelImportExtention.ConvertToBase64(rootPath, "GroupUser");
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
                ExcelImportExtention.CreateExcelWithDisplayNames<GroupUser>(rootPath, "GroupUser");
                var columns = ExcelImportExtention.GetColumnNamesWithOrder<GroupUser>();
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

                var importHelper = new ImportExcelHelperNetCore<GroupUser>();
                importHelper.PathTemplate = filePath;
                importHelper.StartCol = 1;
                importHelper.StartRow = 2;
                importHelper.ConfigColumn = new List<ConfigModule>();
                importHelper.ConfigColumn = ExcelImportExtention.GetConfigCol<GroupUser>(data.Collection);
                #endregion
                var rsl = importHelper.Import();

                var listImportReponse = new List<GroupUser>();
                if (rsl.ListTrue != null && rsl.ListTrue.Count > 0)
                {
                    listImportReponse.AddRange(rsl.ListTrue);
                    await _groupUserService.CreateAsync(rsl.ListTrue);
                }

                var response = new ResponseImport<GroupUser>();


                response.ListTrue = listImportReponse;
                response.lstFalse = rsl.lstFalse;

                return DataResponse.Success(response);
            }
            catch (Exception)
            {
                return DataResponse.False("Import tháº¥t báº¡i");
            }
        }

        [HttpGet("GetDropdown")]
        public async Task<DataResponse<List<DropdownOption>>> GetDropdown()
        {
            var data = await _groupUserService.GetDropdownOptions(x => x.Name, x => x.Id);
            return DataResponse<List<DropdownOption>>.Success(data);
        }
    }
}
