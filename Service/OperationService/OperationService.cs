﻿using BaseProject.Model.Entities;
using BaseProject.Repository.ModuleRepository;
using BaseProject.Repository.OperationRepository;
using BaseProject.Repository.RoleOperationRepository;
using BaseProject.Repository.RoleRepository;
using BaseProject.Repository.UserRoleRepository;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using BaseProject.Service.OperationService.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Service.Common;

namespace BaseProject.Service.OperationService
{
    public class OperationService : Service<Operation>, IOperationService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IRoleOperationRepository _roleOperationRepository;
        private readonly IOperationRepository _operationRepository;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public OperationService(
            IOperationRepository operationRepository,
            IUserRoleRepository userRoleRepository,
            IRoleRepository roleRepository,
            IModuleRepository moduleRepository,
            IRoleOperationRepository roleOperationRepository, IMemoryCache cache) : base(operationRepository)
        {
            _operationRepository = operationRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _moduleRepository = moduleRepository;
            _roleOperationRepository = roleOperationRepository;
            _cache = cache;
        }

        public async Task<PagedList<OperationDto>> GetData(OperationSearch search)
        {
            try
            {
                var query = from q in GetQueryable()
                    select new OperationDto
                    {
                        ModuleId = q.ModuleId,
                        CreatedId = q.CreatedId,
                        UpdatedId = q.UpdatedId,
                        Name = q.Name,
                        URL = q.URL,
                        Code = q.Code,
                        Css = q.Css,
                        Icon = q.Icon,
                        Order = q.Order,
                        IsShow = q.IsShow,
                        IsDelete = q.IsDelete,
                        Id = q.Id,
                        CreatedBy = q.CreatedBy,
                        UpdatedBy = q.UpdatedBy,
                        DeleteId = q.DeleteId,
                        CreatedDate = q.CreatedDate,
                        UpdatedDate = q.UpdatedDate,
                        DeleteTime = q.DeleteTime,
                        TrangThaiHienThi = q.IsShow ? "Hiển thị" : "Không hiển thị"
                    };

                if (search != null)
                {
                    if (search.ModuleId != null)
                        query = query.Where(x => x.ModuleId == search.ModuleId);

                    if (!string.IsNullOrEmpty(search.Name))
                        query = query.Where(x => x.Name.Contains(search.Name));

                    if (!string.IsNullOrEmpty(search.Code))
                        query = query.Where(x => x.Code.Contains(search.Code));

                    if (search.IsShow != null)
                        query = query.Where(x => x.IsShow == search.IsShow);
                }

                query = query.OrderBy(x => x.Order);
                return await PagedList<OperationDto>.CreateAsync(query, search);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve operation data: " + ex.Message);
            }
        }

        public async Task<OperationDto> GetDto(Guid id)
        {
            try
            {
                var item = await (from q in GetQueryable().Where(x => x.Id == id)
                    select new OperationDto
                    {
                        ModuleId = q.ModuleId,
                        CreatedId = q.CreatedId,
                        UpdatedId = q.UpdatedId,
                        Name = q.Name,
                        URL = q.URL,
                        Code = q.Code,
                        Css = q.Css,
                        Icon = q.Icon,
                        Order = q.Order,
                        IsShow = q.IsShow,
                        IsDelete = q.IsDelete,
                        Id = q.Id,
                        CreatedBy = q.CreatedBy,
                        UpdatedBy = q.UpdatedBy,
                        DeleteId = q.DeleteId,
                        CreatedDate = q.CreatedDate,
                        UpdatedDate = q.UpdatedDate,
                        DeleteTime = q.DeleteTime,
                    }).FirstOrDefaultAsync();

                return item ?? throw new Exception("Operation not found for ID: " + id);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve operation DTO: " + ex.Message);
            }
        }

    public async Task<List<MenuDataDto>> GetListOperationOfUser(Guid userId)
{
    try
    {
        // Generate cache key for user operations
        string cacheKey = $"UserOperations_{userId}";

        // Try to get from cache first
        if (_cache.TryGetValue(cacheKey, out List<MenuDataDto> cachedResult))
        {
            return cachedResult;
        }

        // If not in cache, execute the original logic
        var listRoleIdOfUser = await _userRoleRepository.GetQueryable()
            .Where(x => x.UserId == userId)
            .Join(_roleRepository.GetQueryable(),
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => role.Id)
            .ToListAsync();

        var listOperationId = await _roleOperationRepository.GetQueryable()
            .AsNoTracking()
            .Where(x => x.IsAccess == 1 && listRoleIdOfUser.Contains(x.RoleId))
            .Select(x => x.OperationId)
            .ToListAsync();

        var result = await (from moduleTbl in _moduleRepository.GetQueryable().AsNoTracking()
                join operationTbl in GetQueryable().AsNoTracking()
                    on moduleTbl.Id equals operationTbl.ModuleId
                select new
                {
                    ModuleId = moduleTbl.Id,
                    ModuleName = moduleTbl.Name,
                    ModuleCode = moduleTbl.Code,
                    ModuleIcon = moduleTbl.Icon,
                    ModuleIsShow = moduleTbl.IsShow,
                    OperationId = operationTbl.Id,
                    OperationName = operationTbl.Name,
                    OperationUrl = operationTbl.URL,
                    OperationCode = operationTbl.Code
                })
            .GroupBy(x => new { x.ModuleId, x.ModuleName, x.ModuleCode, x.ModuleIcon, x.ModuleIsShow })
            .Select(g => new MenuDataDto
            {
                Id = g.Key.ModuleId,
                Name = g.Key.ModuleName,
                Code = g.Key.ModuleCode,
                Icon = g.Key.ModuleIcon,
                IsShow = g.Key.ModuleIsShow,
                ListMenu = g.Select(op => new MenuDto
                {
                    Id = op.OperationId,
                    Name = op.OperationName,
                    URL = op.OperationUrl,
                    Code = op.OperationCode,
                    IsAccess = listOperationId.Contains(op.OperationId)
                }).ToList()
            })
            .ToListAsync();

        // Store in cache
        _cache.Set(cacheKey, result, _cacheDuration);

        return result;
    }
    catch (Exception ex)
    {
        throw new Exception("Failed to retrieve operations of user: " + ex.Message);
    }
}

public async Task<List<ModuleMenuDTO>> GetListOperationOfRole(Guid roleId)
{
    try
    {
        // Generate cache key for role operations
        string cacheKey = $"RoleOperations_{roleId}";

        // Try to get from cache first
        if (_cache.TryGetValue(cacheKey, out List<ModuleMenuDTO> cachedResult))
        {
            return cachedResult;
        }

        // If not in cache, execute the original logic
        var listOperationId = _roleOperationRepository.GetQueryable()
            .Where(x => x.IsAccess == 1 && x.RoleId == roleId)
            .Select(x => x.OperationId)
            .ToList(); // Keeping as ToList() to maintain original behavior

        var result = await (from moduleTbl in _moduleRepository.GetQueryable()
                join operationTbl in GetQueryable()
                    on moduleTbl.Id equals operationTbl.ModuleId
                select new
                {
                    Id = moduleTbl.Id,
                    Name = moduleTbl.Name,
                    Code = moduleTbl.Code,
                    Icon = moduleTbl.Icon,
                    IsShow = moduleTbl.IsShow,
                    NameOpear = operationTbl.Name,
                    Url = operationTbl.URL,
                    codeOpe = operationTbl.Code,
                    Idoperation = operationTbl.Id
                })
            .GroupBy(x => new { x.Name, x.Code, x.Icon, x.Id, x.IsShow })
            .Select(x => new ModuleMenuDTO
            {
                Name = x.Key.Name,
                Code = x.Key.Code,
                Icon = x.Key.Icon,
                Id = x.Key.Id,
                IsShow = x.Key.IsShow,
                ListOperation = x.Select(y => new OperationDto
                {
                    Name = y.NameOpear,
                    URL = y.Url,
                    Code = y.codeOpe,
                    Id = y.Idoperation,
                    IsAccess = listOperationId.Any(z => z == y.Idoperation)
                }).ToList()
            })
            .ToListAsync();

        // Store in cache
        _cache.Set(cacheKey, result, _cacheDuration);

        return result;
    }
    catch (Exception ex)
    {
        throw new Exception("Failed to retrieve operations of role: " + ex.Message);
    }
}

        public async Task<List<MenuDataDto>> GetListMenu(Guid userId, List<string> roleCodes)
        {
            try
            {
                // Early exit if no role codes provided
                if (roleCodes == null || !roleCodes.Any())
                    return new List<MenuDataDto>();

                // Generate cache key based on user ID and role codes
                string cacheKey = $"UserMenu_{userId}_{string.Join("_", roleCodes.OrderBy(r => r))}";

                // Try to get from cache first
                if (_cache.TryGetValue(cacheKey, out List<MenuDataDto> cachedResult))
                {
                    return cachedResult;
                }

                // If not in cache, execute the original logic
                var result = await GetMenuDataFromDatabase(roleCodes);

                // Store in cache
                _cache.Set(cacheKey, result, _cacheDuration);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve menu list: " + ex.Message);
            }
        }

        private async Task<List<MenuDataDto>> GetMenuDataFromDatabase(List<string> roleCodes)
        {
            // Get role IDs and operation IDs in fewer queries
            var roleIds = await _roleRepository.GetQueryable()
                .Where(r => roleCodes.Contains(r.Code))
                .Select(r => r.Id)
                .ToListAsync();

            if (roleIds.Count == 0)
                return new List<MenuDataDto>();

            // Create a HashSet for faster lookups
            var userOperationIds = new HashSet<Guid>(
                await _roleOperationRepository.GetQueryable()
                    .AsNoTracking()
                    .Where(ro => roleIds.Contains(ro.RoleId))
                    .Select(ro => ro.OperationId)
                    .ToListAsync()
            );

            // Join modules and operations in a single query
            var moduleWithOperations = await (
                    from module in _moduleRepository.GetQueryable()
                        .Where(m => m.IsShow)
                        .OrderBy(m => m.Order)
                        .AsNoTracking()
                    join operation in GetQueryable()
                            .Where(o => o.IsShow)
                            .AsNoTracking()
                        on module.Id equals operation.ModuleId
                    select new
                    {
                        ModuleId = module.Id,
                        ModuleName = module.Name,
                        ModuleCode = module.Code,
                        ModuleIcon = module.Icon,
                        ModuleClassCss = module.ClassCss,
                        ModuleIsShow = module.IsShow,
                        ModuleLink = module.Link,
                        ModuleOrder = module.Order,
                        OperationId = operation.Id,
                        OperationName = operation.Name,
                        OperationUrl = operation.URL,
                        OperationCode = operation.Code,
                        OperationOrder = operation.Order,
                        OperationIsShow = operation.IsShow
                    })
                .ToListAsync();

            // Process the data in memory which is more efficient
            return moduleWithOperations
                .GroupBy(x => new
                {
                    x.ModuleId, x.ModuleName, x.ModuleCode, x.ModuleIcon,
                    x.ModuleIsShow, x.ModuleLink, x.ModuleOrder, x.ModuleClassCss
                })
                .OrderBy(g => g.Key.ModuleOrder)
                .Select(g =>
                {
                    var operations = g.OrderBy(op => op.OperationOrder)
                        .Select(op => new MenuDto
                        {
                            Id = op.OperationId,
                            Name = op.OperationName,
                            URL = op.OperationUrl,
                            Code = op.OperationCode,
                            IsShow = op.OperationIsShow,
                            IsAccess = userOperationIds.Contains(op.OperationId)
                        })
                        .ToList();

                    return new MenuDataDto
                    {
                        Id = g.Key.ModuleId,
                        Name = g.Key.ModuleName,
                        Code = g.Key.ModuleCode,
                        Icon = g.Key.ModuleIcon,
                        ClassCss = g.Key.ModuleClassCss,
                        IsShow = g.Key.ModuleIsShow,
                        Link = g.Key.ModuleLink,
                        ListMenu = operations,
                        IsAccess = operations.Any(op => op.IsAccess)
                    };
                })
                .ToList();
        }


        public async Task<dynamic> GetOperationWithModuleByUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            var result = await _operationRepository.GetQueryable()
                .Where(x => x.URL.Equals(url))
                .Join(
                    _moduleRepository.GetQueryable(),
                    operation => operation.ModuleId,
                    module => module.Id,
                    (operation, module) => new
                    {
                        OperationName = operation.Name,
                        OperationUrl = operation.URL,
                        ModuleName = module.Name,
                        ModuleIcon = module.Icon,
                    })
                // .TagWith("GetOperationWithModuleByUrl")
                .FirstOrDefaultAsync();

            return result;
        }
        
        
        public override async Task CreateAsync(Operation entity)
        {
            ClearModuleCaches();
            await base.CreateAsync(entity);
        }

        public override async Task UpdateAsync(Operation entity)
        {
            ClearModuleCaches();
            _cache.Remove($"Module_Dto_{entity.Id}");
            _cache.Remove($"Module_Detail_{entity.Id}");
            await base.UpdateAsync(entity);
        }

        public override async Task DeleteAsync(Operation entity)
        {
            ClearModuleCaches();
            _cache.Remove($"Module_Dto_{entity.Id}");
            _cache.Remove($"Module_Detail_{entity.Id}");
            await base.DeleteAsync(entity);
        }

        private void ClearModuleCaches()
        {
            // Xóa cache chung
            _cache.Remove("Module_DropDown");
            _cache.Remove("Module_GroupData");
        
            // Các cache khác sẽ hết hạn theo thời gian
        }
    }
}
