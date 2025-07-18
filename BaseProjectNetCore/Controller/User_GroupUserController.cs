using BaseProject.Service.Core.Mapper;
using Microsoft.AspNetCore.Mvc;
using BaseProject.Model.Entities;
using BaseProject.Service.User_GroupUserService;
using BaseProject.Service.User_GroupUserService.Dto;
using BaseProject.Service.User_GroupUserService.ViewModels;
using BaseProject.Service.Common;
using BaseProject.Api.Filter;
using CommonHelper.Excel;
using BaseProject.Web.Common;
using BaseProject.Api.ViewModels.Import;
using BaseProject.Service.TaiLieuDinhKemService;
using BaseProject.Api.Dto;
using System.Net.WebSockets;
using BaseProject.Controllers;
using BaseProjectNetCore.Api.Dto;

namespace BaseProject.Controllers
{
    [Route("api/[controller]")]
    public class User_GroupUserController : BaseProjectController
    {
        private readonly IUser_GroupUserService _user_GroupUserService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IMapper _mapper;
        private readonly ILogger<User_GroupUserController> _logger;

        public User_GroupUserController(
            IUser_GroupUserService user_GroupUserService,
            ITaiLieuDinhKemService taiLieuDinhKemService,
            IMapper mapper,
            ILogger<User_GroupUserController> logger
            )
        {
            this._user_GroupUserService = user_GroupUserService;
            this._taiLieuDinhKemService = taiLieuDinhKemService;
            this._mapper = mapper;
            _logger = logger;
        }

        [HttpPost("Create")]
        public async Task<DataResponse<User_GroupUser>> Create([FromBody] User_GroupUserCreateVM model)
        {
            try
            {
                // nhÃ³m cÅ©
                var lstOlds = _user_GroupUserService.FindBy(x => x.UserId == model.UserId).ToList();

                if (model.GroupUserId != null && model.GroupUserId.Any())
                {
                    // thÃªm nhÃ³m má»›i
                    var userGroupUsers = new List<User_GroupUser>();
                    foreach (var item in model.GroupUserId)
                    {
                        var userGroupUser = new User_GroupUser();
                        userGroupUser.UserId = model.UserId;
                        userGroupUser.GroupUserId = item;
                        userGroupUsers.Add(userGroupUser);
                    }

                    if (userGroupUsers.Any())
                    {
                        await _user_GroupUserService.CreateAsync(userGroupUsers);
                    }
                }
                // xÃ³a háº¿t nhÃ³m cÅ© 
                if (lstOlds != null && lstOlds.Any())
                {
                    await _user_GroupUserService.DeleteAsync(lstOlds);
                }

                return DataResponse<User_GroupUser>.Success(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi táº¡o User_GroupUser");
                return DataResponse<User_GroupUser>.False("ÄÃ£ xáº£y ra lá»—i khi táº¡o dá»¯ liá»‡u.");
            }
        }

        [HttpPut("Update")]
        public async Task<DataResponse<User_GroupUser>> Update([FromBody] User_GroupUserEditVM model)
        {
            try
            {
                var entity = await _user_GroupUserService.GetByIdAsync(model.Id);
                if (entity == null)
                    return DataResponse<User_GroupUser>.False("User_GroupUser khÃ´ng tá»“n táº¡i");

                entity = _mapper.Map(model, entity);
                await _user_GroupUserService.UpdateAsync(entity);
                return DataResponse<User_GroupUser>.Success(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi cáº­p nháº­t User_GroupUser vá»›i Id: {Id}", model.Id);
                return new DataResponse<User_GroupUser>()
                {
                    Data = null,
                    Status = false,
                    Message = "ÄÃ£ xáº£y ra lá»—i khi cáº­p nháº­t dá»¯ liá»‡u."
                };
            }
        }

        [HttpGet("Get/{id}")]
        public async Task<DataResponse<User_GroupUserDto>> Get(Guid id)
        {
            var dto = await _user_GroupUserService.GetDto(id);
            return DataResponse<User_GroupUserDto>.Success(dto);
        }

        [HttpPost("GetData", Name = "Xem danh sÃ¡ch User_GroupUser há»‡ thá»‘ng")]
        [ServiceFilter(typeof(LogActionFilter))]
        public async Task<DataResponse<PagedList<User_GroupUserDto>>> GetData([FromBody] User_GroupUserSearch search)
        {
            var data = await _user_GroupUserService.GetData(search);
            return DataResponse<PagedList<User_GroupUserDto>>.Success(data);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<DataResponse> Delete(Guid id)
        {
            try
            {
                var entity = await _user_GroupUserService.GetByIdAsync(id);
                await _user_GroupUserService.DeleteAsync(entity);
                return DataResponse.Success(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi xÃ³a User_GroupUser vá»›i Id: {Id}", id);
                return DataResponse.False("ÄÃ£ xáº£y ra lá»—i khi xÃ³a dá»¯ liá»‡u.");
            }
        }

        [HttpGet("export")]
        public async Task<DataResponse> ExportExcel()
        {
            try
            {
                var search = new User_GroupUserSearch();
                var data = await _user_GroupUserService.GetData(search);
                var base64Excel = await ExportExcelHelperNetCore.Export<User_GroupUserDto>(data?.Items);
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
                var base64 = ExcelImportExtention.ConvertToBase64(rootPath, "User_GroupUser");
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
                ExcelImportExtention.CreateExcelWithDisplayNames<User_GroupUser>(rootPath, "User_GroupUser");
                var columns = ExcelImportExtention.GetColumnNamesWithOrder<User_GroupUser>();
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

                var importHelper = new ImportExcelHelperNetCore<User_GroupUser>();
                importHelper.PathTemplate = filePath;
                importHelper.StartCol = 1;
                importHelper.StartRow = 2;
                importHelper.ConfigColumn = new List<ConfigModule>();
                importHelper.ConfigColumn = ExcelImportExtention.GetConfigCol<User_GroupUser>(data.Collection);
                #endregion
                var rsl = importHelper.Import();

                var listImportReponse = new List<User_GroupUser>();
                if (rsl.ListTrue != null && rsl.ListTrue.Count > 0)
                {
                    listImportReponse.AddRange(rsl.ListTrue);
                    await _user_GroupUserService.CreateAsync(rsl.ListTrue);
                }

                var response = new ResponseImport<User_GroupUser>();


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
