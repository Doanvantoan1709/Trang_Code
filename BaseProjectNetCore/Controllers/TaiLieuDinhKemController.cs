﻿using Azure;
using BaseProject.Controllers;
using BaseProject.Model.Entities;
using BaseProjectNetCore.Api.Dto;
using BaseProject.Api.Dto;
using BaseProject.Api.Filter;
using BaseProject.Api.Hellper;
using BaseProject.Model.Entities;
using BaseProject.Service.Common;
using BaseProject.Service.Core.Mapper;
using BaseProject.Service.TaiLieuDinhKemService;
using BaseProject.Service.TaiLieuDinhKemService.Dto;
using BaseProject.Service.TaiLieuDinhKemService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject.Controllers
{
    [Route("api/[controller]")]
    public class TaiLieuDinhKemController : BaseProjectController
    {
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IMapper _mapper;
        private readonly ILogger<TaiLieuDinhKemController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TaiLieuDinhKemController(
            ITaiLieuDinhKemService taiLieuDinhKemService,
            IMapper mapper,
            ILogger<TaiLieuDinhKemController> logger
,
            IWebHostEnvironment webHostEnvironment)
        {
            this._taiLieuDinhKemService = taiLieuDinhKemService;
            this._mapper = mapper;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("Create")]
        public async Task<DataResponse<TaiLieuDinhKem>> Create([FromBody] TaiLieuDinhKemCreateVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = _mapper.Map<TaiLieuDinhKemCreateVM, TaiLieuDinhKem>(model);
                    await _taiLieuDinhKemService.CreateAsync(entity);
                    return new DataResponse<TaiLieuDinhKem>() { Data = entity, Status = true };
                }
                catch (Exception ex)
                {
                    return DataResponse<TaiLieuDinhKem>.False("Error", new string[] { ex.Message });
                }
            }
            return DataResponse<TaiLieuDinhKem>.False("Some properties are not valid", ModelStateError);
        }

        [HttpGet("AddPathPdf")]
        public async Task<DataResponse<TaiLieuDinhKem?>> PreView([FromQuery] string AttachId)
        {
            try
            {
                var attachs = await _taiLieuDinhKemService.GetByIdsAsync(AttachId);
                if (attachs != null && attachs.Any())
                {
                    //kiểm tra file type
                    var attach = attachs.FirstOrDefault();
                    //convert image
                    if (ConvertPDFHelper.AllowedTypeImage.Contains(attach.DinhDangFile))
                    {
                        var convert = new ConvertPDFHelper(_taiLieuDinhKemService, _webHostEnvironment);
                        _ = await convert.ConvertImageToPdf(attach?.DuongDanFile, attach?.Id.ToString());
                    }

                    return DataResponse<TaiLieuDinhKem>.Success(attach, "Success");
                }
                else
                {
                    return DataResponse<TaiLieuDinhKem?>.False("Not found Attach by Ids");
                }
            }
            catch (Exception ex)
            {
                return DataResponse<TaiLieuDinhKem?>.False(ex.Message);
            }
        }

        [HttpPost("Update")]
        public async Task<DataResponse<TaiLieuDinhKem>> Update([FromBody] TaiLieuDinhKemEditVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = await _taiLieuDinhKemService.GetByIdAsync(model.Id);
                    if (entity == null)
                        return DataResponse<TaiLieuDinhKem>.False("TaiLieuDinhKem not found");

                    entity = _mapper.Map(model, entity);
                    await _taiLieuDinhKemService.UpdateAsync(entity);
                    return new DataResponse<TaiLieuDinhKem>() { Data = entity, Status = true };
                }
                catch (Exception ex)
                {
                    DataResponse<TaiLieuDinhKem>.False(ex.Message);
                }
            }
            return DataResponse<TaiLieuDinhKem>.False("Some properties are not valid", ModelStateError);
        }

        [HttpGet("Get/{id}")]
        public async Task<DataResponse<TaiLieuDinhKemDto>> Get(Guid id)
        {
            var result = await _taiLieuDinhKemService.GetDto(id);
            return new DataResponse<TaiLieuDinhKemDto>
            {
                Data = result,
                Message = "Get TaiLieuDinhKemDto thành công",
                Status = true
            };
        }

        [HttpPost("GetData", Name = "Xem danh sách tài liệu đính kèm")]
        [ServiceFilter(typeof(LogActionFilter))]
        public async Task<DataResponse<PagedList<TaiLieuDinhKemDto>>> GetData([FromBody] TaiLieuDinhKemSearch search)
        {
            if (search.IsDonVi.HasValue && search.IsDonVi.Value) search.Item_ID = DonViId.ToString();
            var result = await _taiLieuDinhKemService.GetData(search);
            return new DataResponse<PagedList<TaiLieuDinhKemDto>>
            {
                Data = result,
                Message = "GetData PagedList<TaiLieuDinhKemDto> thành công",
                Status = true
            };
        }

        [HttpDelete("Delete/{id}")]
        public async Task<DataResponse> Delete(Guid id)
        {
            try
            {
                var entity = await _taiLieuDinhKemService.GetByIdAsync(id);
                await _taiLieuDinhKemService.DeleteAsync(entity);
                return DataResponse.Success(null);
            }
            catch (Exception ex)
            {
                return DataResponse.False(ex.Message);
            }
        }

        [HttpPost("GetKySo")]
        public async Task<DataResponse<List<TaiLieuDinhKem>>> GetKySo(List<Guid> Ids)
        {
            try
            {
                var entity = await _taiLieuDinhKemService.GetKySo(Ids);
                return new DataResponse<List<TaiLieuDinhKem>>()
                {
                    Data = entity,
                    Status = true,
                };
            }
            catch (Exception ex)
            {
                return DataResponse<List<TaiLieuDinhKem>>.False("Some properties are not valid", ModelStateError);
            }
        }
        [HttpGet("GetByIds/{id}")]
        public async Task<DataResponse<List<TaiLieuDinhKem>>> GetByIds(string id)
        {
            var result = await _taiLieuDinhKemService.GetByIdsAsync(id);
            return new DataResponse<List<TaiLieuDinhKem>>
            {
                Data = result,
                Message = "Get TaiLieuDinhKemDto thành công",
                Status = true
            };
        }


        [HttpPost("upload-multi")]
        public async Task<DataResponse<List<TaiLieuDinhKemDto>>> UploadMulti([FromForm] List<IFormFile> files, [FromForm] string loaiTaiLieu, [FromForm] string itemId)
        {
            var result = new DataResponse<List<TaiLieuDinhKemDto>>("Upload file thành công!");

            if (files == null || files.Count == 0)
            {
                result.MessageFalse("Không có file nào được upload.");
                return result;
            }

            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".jpg", ".png" };
            const long maxFileSize = 10 * 1024 * 1024; // 10MB

            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/" + loaiTaiLieu);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);
            var listFile = new List<TaiLieuDinhKemDto>();

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    result.MessageFalse($"File {file.FileName} không đúng định dạng cho phép.");
                    return result;
                }

                if (file.Length > maxFileSize)
                {
                    result.MessageFalse($"File {file.FileName} vượt quá dung lượng tối đa 10MB.");
                    return result;

                }
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var entity = new TaiLieuDinhKem
                {
                    TenTaiLieu = file.FileName,
                    DinhDangFile = ext,
                    DuongDanFile = $"/uploads/{fileName}",
                    KichThuoc = file.Length,
                    NgayPhatHanh = DateTime.Now,
                    LoaiTaiLieu = loaiTaiLieu,
                    Item_ID = itemId != null ? Guid.Parse(itemId) : Guid.Empty
                    // ... các trường khác nếu có
                };
                await _taiLieuDinhKemService.CreateAsync(entity);

                listFile.Add(new TaiLieuDinhKemDto
                {
                    Id = entity.Id,
                    TenTaiLieu = entity.TenTaiLieu,
                    DinhDangFile = entity.DinhDangFile,
                    DuongDanFile = entity.DuongDanFile,
                    KichThuoc = entity.KichThuoc,
                    NgayPhatHanh = entity.NgayPhatHanh,
                    LoaiTaiLieu = loaiTaiLieu,
                    Item_ID = itemId != null ? Guid.Parse(itemId) : Guid.Empty
                    // ... các trường khác nếu có
                });
            }
            result.Data = listFile;
            return result;
        }
        
    }
}
