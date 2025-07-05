using BaseProject.Service.Core.Mapper;
using Microsoft.AspNetCore.Mvc;
using BaseProject.Model.Entities;
using BaseProject.Service.FileManagerService;
using BaseProject.Service.FileManagerService.Dto;
using BaseProject.Service.FileManagerService.ViewModels;
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
using System.Net.WebSockets;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using CommonHelper.String;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Identity;
using Pipelines.Sockets.Unofficial;
using BaseProject.Service.FileSecurityService.Dto;
using DocumentFormat.OpenXml.Office2010.Excel;
using BaseProject.Service.AspNetUsersService;
using BaseProject.Service.RoleService;
using BaseProject.Service.DepartmentService;
using BaseProject.Controllers;
using BaseProjectNetCore.Api.Dto;
using System.Security.AccessControl;
using BaseProject.Model.Entities;


namespace BaseProject.Controllers
{
    [Route("api/[controller]")]
    public class FileManagerController : BaseProjectController
    {
        private readonly IFileManagerService _fileManagerService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IMapper _mapper;
        private readonly ILogger<FileManagerController> _logger;
        private readonly IAspNetUsersService _aspNetUsersService;
        private readonly IRoleService _roleService;
        private readonly IDepartmentService _departmentService;

        private readonly string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads/filemanager/uploads");
        private readonly string rootZipPathDownLoad = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads/filemanager/downloads");

        public FileManagerController(
            IFileManagerService fileManagerService,
            ITaiLieuDinhKemService taiLieuDinhKemService,
            IMapper mapper,
            ILogger<FileManagerController> logger
,
            IAspNetUsersService aspNetUsersService,
            IRoleService roleService,
            IDepartmentService departmentService)
        {
            this._fileManagerService = fileManagerService;
            this._taiLieuDinhKemService = taiLieuDinhKemService;
            this._mapper = mapper;
            _logger = logger;
            _aspNetUsersService = aspNetUsersService;
            _roleService = roleService;
            _departmentService = departmentService;
        }


        // Get all file/folder
        [HttpPost("GetData")]
        [ServiceFilter(typeof(LogActionFilter))]
        public async Task<DataResponse<List<FileManagerDto>>> GetData([FromBody] FileManagerSearch search)
        {
            search.UserID = UserId ?? new Guid();
            var data = await _fileManagerService.GetDataAll(search);
            return DataResponse<List<FileManagerDto>>.Success(data);
        }

        [HttpPost("Download")]
        public async Task<DataResponse<string>> Download([FromBody] List<Guid> fileIDs)
        {
            try
            {
                if (fileIDs == null && !fileIDs.Any())
                {
                    return DataResponse<string>.False("Dá»¯ liá»‡u nháº­n Ä‘Æ°á»£c chÆ°a Ä‘Ãºng");
                }
                // Chuáº©n hÃ³a Ä‘Æ°á»ng dáº«n tuyá»‡t Ä‘á»‘i
                var fullZipFolderPath = Path.GetFullPath(rootZipPathDownLoad);

                // Äáº£m báº£o thÆ° má»¥c tá»“n táº¡i
                if (!Directory.Exists(fullZipFolderPath))
                {
                    Directory.CreateDirectory(fullZipFolderPath);
                }
                var zipFilePath = await _fileManagerService.Download(fileIDs, rootPath, fullZipFolderPath);

                if (string.IsNullOrEmpty(zipFilePath) || !System.IO.File.Exists(zipFilePath))
                    return DataResponse<string>.False("KhÃ´ng táº¡o Ä‘Æ°á»£c file zip");

                var fileName = Path.GetFileName(zipFilePath);
                var downloadUrl = $"{Request.Scheme}://{Request.Host}/uploads/filemanager/downloads/{fileName}";

                return DataResponse<string>.Success(downloadUrl, "Láº¥y link táº£i xuá»‘ng thÃ nh cÃ´ng!");
            }
            catch (Exception)
            {
                return DataResponse<string>.False("Lá»—i táº£i xuá»‘ng file");
            }
        }



        // Táº¡o folder
        [HttpPost("Create")]
        public async Task<DataResponse<FileManager>> Create([FromBody] FileManagerCreateVM model)
        {
            try
            {
                var entity = _mapper.Map<FileManagerCreateVM, FileManager>(model);

                if (await _fileManagerService.CreateFolderFromDB(entity))
                {
                    return DataResponse<FileManager>.Success(entity, "Táº¡o folder má»›i thÃ nh cÃ´ng");
                }
                else
                {
                    return DataResponse<FileManager>.False("Táº¡o folder má»›i khÃ´ng thÃ nh cÃ´ng");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi táº¡o FileManager");
                return DataResponse<FileManager>.False("ÄÃ£ xáº£y ra lá»—i khi táº¡o dá»¯ liá»‡u.");
            }
        }

        // Upload file
        [HttpPost("UploadFile")]
        public async Task<DataResponse<FileManager>> UploadFile([FromForm] Guid? parentId, [FromForm] IFormFile file)
        {
            try
            {
                if (file.Length <= 0) return DataResponse<FileManager>.False("KhÃ´ng nháº­n Ä‘Æ°á»£c tÃ i liá»‡u");

                var newFile = await _fileManagerService.CreateFile(rootPath, parentId, file);
                if (newFile != null)
                {
                    return DataResponse<FileManager>.Success(newFile, "Táº£i lÃªn tÃ i liá»‡u thÃ nh cÃ´ng");
                }
                return DataResponse<FileManager>.False("Táº£i lÃªn tÃ i liá»‡u thÃ nh cÃ´ng");
            }
            catch (Exception)
            {   
                return DataResponse<FileManager>.False("ÄÃ£ xáº£y ra lá»—i khi upload file");
            }
        }

        // Copy file/folder
        [HttpPost("Copy")]
        public async Task<DataResponse> Copy([FromBody] FileManagerCopyVM model)
        {
        // Copy file/folder
            try
            {
                if (model == null || model.SourceItems == null || !model.SourceItems.Any())
                    return DataResponse.False("ÄÃ£ cÃ³ lá»—i trong khi truyá»n dá»¯ liá»‡u");

                var allFile = await _fileManagerService.GetQueryable().ToListAsync();
                foreach (var item in model.SourceItems)
                {
                    await _fileManagerService.CopyFileOrFolder(item, model.DestinationFolder, allFile, rootPath);
                }

                return DataResponse.Success(null, "Sao chÃ©p tÃ i liá»‡u thÃ nh cÃ´ng");

            }
            catch (Exception)
            {
                return DataResponse.False("ÄÃ£ xáº£y ra lá»—i trong khi sao chÃ©p tÃ i liá»‡u");
            }
        }

        // Move file/folder
        [HttpPost("Move")]
        public async Task<DataResponse> Move([FromBody] FileManagerCopyVM model)
        {
            try
            {
                if (model == null || model.SourceItems == null || !model.SourceItems.Any())
                    return DataResponse.False("ÄÃ£ cÃ³ lá»—i trong khi truyá»n dá»¯ liá»‡u");

                // copy 
                var allFile = await _fileManagerService.GetQueryable().ToListAsync();
                foreach (var item in model.SourceItems)
                {
                    await _fileManagerService.MoveFileOrFolder(item, model.DestinationFolder, allFile);
                }
                return DataResponse.Success(null, "Di chuyá»ƒn tÃ i liá»‡u thÃ nh cÃ´ng");

            }
            catch (Exception)
            {
                return DataResponse.False("ÄÃ£ xáº£y ra lá»—i trong khi sao chÃ©p tÃ i liá»‡u");
            }
        }

        // Delete file/folder     
        [HttpPost("Delete")]
        public async Task<DataResponse> Delete([FromBody] List<Guid> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return DataResponse.False("Dá»¯ liá»‡u nháº­n Ä‘Æ°á»£c khÃ´ng Ä‘Ãºng");
                }
                if (await _fileManagerService.DeleteFileorFolders(ids, rootPath))
                {
                    return DataResponse.Success(null, "XÃ³a thÃ nh cÃ´ng");
                }

                return DataResponse.False("XÃ³a khÃ´ng thÃ nh cÃ´ng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi xÃ³a FileManager vá»›i Ids: {@Ids}", ids);
                return DataResponse.False("ÄÃ£ xáº£y ra lá»—i khi xÃ³a dá»¯ liá»‡u.");
            }
        }

        // Rename file/folder
        [HttpGet("Rename")]
        public async Task<DataResponse> Rename(Guid id, string newName)
        {
            try
            {
                var file = _fileManagerService.FindBy(x => x.Id == id).FirstOrDefault();
                if (file == null) return DataResponse.False("KhÃ´ng tÃ¬m tháº¥y tÃ i liá»‡u");
                if (string.IsNullOrWhiteSpace(newName))
                    return DataResponse.False("TÃªn má»›i bá»‹ rá»—ng");

                var newFile = await _fileManagerService.RenameFileOrFolder(file, newName);

                return DataResponse.Success(newFile, "Äá»•i tÃªn thÃ nh cÃ´ng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi Ä‘á»•i tÃªn FileManager vá»›i Id: {@id}", id);
                return DataResponse.False("ÄÃ£ xáº£y ra lá»—i Ä‘á»•i tÃªn.");
            }
        }

        // Get securities file/folder
        [HttpGet("GetSecurity/{id}")]
        public async Task<DataResponse<List<FileSecurityDto>>> GetSecurity(Guid id)
        {
            try
            {
                var lstData = await _fileManagerService.GetShare(id, UserId ?? new Guid());
                return DataResponse<List<FileSecurityDto>>.Success(lstData, "Láº¥y thÃ´ng tin thÃ nh cÃ´ng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi láº¥y thÃ´ng tin security file vá»›i Id: {@id}", id);
                return DataResponse<List<FileSecurityDto>>.False("ÄÃ£ xáº£y ra lá»—i Ä‘á»•i tÃªn.");
            }
        }

        // Save securities file/folder
        [HttpPost("SaveSecurity")]
        public async Task<DataResponse> SaveSecurity([FromBody] List<BaseProject.Model.Entities.FileSecurity> fileSecurities, [FromQuery] Guid fileID)
        {
            try
            {
                var lstdata = fileSecurities ?? new List<BaseProject.Model.Entities.FileSecurity>();
                if (lstdata.Any())
                {
                    foreach (var item in fileSecurities)
                    {
                        item.SharedByID = UserId ?? new Guid();
                    }
                }
                await _fileManagerService.SaveSecurity(lstdata, fileID);
                return DataResponse.Success("PhÃ¢n quyá»n/ chia sáº» thÃ nh cÃ´ng");
            }
            catch (Exception)
            {
                return DataResponse.False("ÄÃ£ xáº£y ra lá»—i khi phÃ¢n quyá»n/ chia sáº».");
            }
        }

        // Get object to security
        [HttpGet("GetDropdownObject")]
        public async Task<DataResponse<Object>> GetDropdownObject()
        {
            try
            {
                var Data = new
                {
                    dropdownUser = _aspNetUsersService.GetDropdownOptions(x => x.Name, x => x.Id),
                    dropdownRole = _roleService.GetDropdownOptions(x => x.Name, x => x.Id),
                    dropdownDepartment = _departmentService.GetDropdownOptions(x => x.Name, x => x.Id)
                };
                return DataResponse<Object>.Success(Data, "láº¥y thÃ´ng tin thÃ nh cÃ´ng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi láº¥y thÃ´ng tin dropdown object");
                return DataResponse<Object>.False("ÄÃ£ xáº£y ra lá»—i khi láº¥y dropdownObject security.");
            }
        }


        [HttpGet("export")]
        public async Task<DataResponse> ExportExcel()
        {
            try
            {
                var search = new FileManagerSearch();
                var data = await _fileManagerService.GetData(search);
                var base64Excel = await ExportExcelHelperNetCore.Export<FileManagerDto>(data?.Items);
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
                var base64 = ExcelImportExtention.ConvertToBase64(rootPath, "FileManager");
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
                ExcelImportExtention.CreateExcelWithDisplayNames<FileManager>(rootPath, "FileManager");
                var columns = ExcelImportExtention.GetColumnNamesWithOrder<FileManager>();
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

                var importHelper = new ImportExcelHelperNetCore<FileManager>();
                importHelper.PathTemplate = filePath;
                importHelper.StartCol = 1;
                importHelper.StartRow = 2;
                importHelper.ConfigColumn = new List<ConfigModule>();
                importHelper.ConfigColumn = ExcelImportExtention.GetConfigCol<FileManager>(data.Collection);
                #endregion
                var rsl = importHelper.Import();

                var listImportReponse = new List<FileManager>();
                if (rsl.ListTrue != null && rsl.ListTrue.Count > 0)
                {
                    listImportReponse.AddRange(rsl.ListTrue);
                    await _fileManagerService.CreateAsync(rsl.ListTrue);
                }

                var response = new ResponseImport<FileManager>();


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
