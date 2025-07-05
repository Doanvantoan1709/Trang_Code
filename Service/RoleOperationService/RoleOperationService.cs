﻿using BaseProject.Model.Entities;
using BaseProject.Repository.RoleOperationRepository;
using BaseProject.Service.Common.Service;
using BaseProject.Service.RoleOperationService.Dto;
using BaseProject.Service.Common;
using Microsoft.EntityFrameworkCore;
using BaseProject.Repository.RoleRepository;
using BaseProject.Repository.OperationRepository;
using Service.Common;

namespace BaseProject.Service.RoleOperationService
{
    public class RoleOperationService : Service<RoleOperation>, IRoleOperationService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IOperationRepository _operationRepository;

        public RoleOperationService(
            IRoleOperationRepository roleOperationRepository,
            IRoleRepository roleRepository,
            IOperationRepository operationRepository) : base(roleOperationRepository)
        {
            _roleRepository = roleRepository;
            _operationRepository = operationRepository;
        }

        public async Task<PagedList<RoleOperationDto>> GetData(RoleOperationSearch search)
        {
            try
            {
                var query = from q in GetQueryable()
                            select new RoleOperationDto
                            {
                                CreatedId = q.CreatedId,
                                UpdatedId = q.UpdatedId,
                                RoleId = q.RoleId,
                                IsAccess = q.IsAccess,
                                OperationId = q.OperationId,
                                CreatedBy = q.CreatedBy,
                                UpdatedBy = q.UpdatedBy,
                                IsDelete = q.IsDelete,
                                DeleteId = q.DeleteId,
                                CreatedDate = q.CreatedDate,
                                UpdatedDate = q.UpdatedDate,
                                DeleteTime = q.DeleteTime,
                                Id = q.Id,
                            };

                query = query.OrderByDescending(x => x.CreatedDate);
                return await PagedList<RoleOperationDto>.CreateAsync(query, search);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve role operation data: " + ex.Message);
            }
        }

        public async Task<List<RoleOperationViewModel>> GetOperationByRoleId(Guid? id)
        {
            if (id == null)
                throw new Exception("Role ID is required");

            try
            {
                return await (from role in _roleRepository.GetQueryable().Where(x => x.Id == id)
                              join roleOperation in GetQueryable()
                              on role.Id equals roleOperation.RoleId
                              into roleOperationGr
                              from roleOperationData in roleOperationGr.DefaultIfEmpty()
                              join operation in _operationRepository.GetQueryable()
                              on roleOperationData.OperationId equals operation.Id
                              select new RoleOperationViewModel
                              {
                                  RoleId = role.Id,
                                  OperationId = operation.Id,
                                  IsAccess = roleOperationData.IsAccess == 1 ? true : false,
                                  RoleName = role.Name,
                                  OperationName = operation.Name
                              }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve operations by role ID: " + ex.Message);
            }
        }

        public async Task<RoleOperationDto> GetDto(Guid id)
        {
            try
            {
                var item = await (from q in GetQueryable().Where(x => x.Id == id)
                                  select new RoleOperationDto
                                  {
                                      CreatedId = q.CreatedId,
                                      UpdatedId = q.UpdatedId,
                                      RoleId = q.RoleId,
                                      IsAccess = q.IsAccess,
                                      OperationId = q.OperationId,
                                      CreatedBy = q.CreatedBy,
                                      UpdatedBy = q.UpdatedBy,
                                      IsDelete = q.IsDelete,
                                      DeleteId = q.DeleteId,
                                      CreatedDate = q.CreatedDate,
                                      UpdatedDate = q.UpdatedDate,
                                      DeleteTime = q.DeleteTime,
                                      Id = q.Id,
                                  }).FirstOrDefaultAsync();

                return item ?? throw new Exception("Role operation not found for ID: " + id);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve role operation DTO: " + ex.Message);
            }
        }

        public List<RoleOperation> GetByRoleId(Guid RoleId)
        {
            return GetQueryable().Where(x => x.RoleId == RoleId).ToList();
        }
    }
}
