﻿using BaseProject.Model.Entities;
using BaseProject.Repository.GroupUserRepository;
using BaseProject.Service.Common.Service;
using BaseProject.Service.GroupUserService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Dto;
using Microsoft.EntityFrameworkCore;
using BaseProject.Repository.RoleRepository;
using BaseProject.Repository.GroupUserRoleRepository;
using BaseProject.Model.Entities;
using Service.Common;

namespace BaseProject.Service.GroupUserService
{
    public class GroupUserService : Service<GroupUser>, IGroupUserService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IGroupUserRoleRepository _groupUserRoleRepository;
        public GroupUserService(
            IGroupUserRepository groupUserRepository
, IRoleRepository roleRepository,
IGroupUserRoleRepository groupUserRoleRepository) : base(groupUserRepository)
        {
            _roleRepository = roleRepository;
            _groupUserRoleRepository = groupUserRoleRepository;
        }

        public async Task<PagedList<GroupUserDto>> GetData(GroupUserSearch search)
        {
            var groupUserRoles = _roleRepository.GetQueryable()
                .Join(_groupUserRoleRepository.GetQueryable(),
                role => role.Id,
                gur => gur.RoleId,
                (role, gur) => new { roles = role, groupUserRoles = gur });

            var query = GetQueryable()
                        .GroupJoin(groupUserRoles,
                        q => q.Id,
                        gur => gur.groupUserRoles.GroupUserId,
                        (q, gur) => new GroupUserDto
                        {
                            Name = q.Name,
                            Code = q.Code,
                            CreatedBy = q.CreatedBy,
                            UpdatedBy = q.UpdatedBy,
                            IsDelete = q.IsDelete,
                            DeleteId = q.DeleteId,
                            CreatedDate = q.CreatedDate,
                            UpdatedDate = q.UpdatedDate,
                            DeleteTime = q.DeleteTime,
                            Id = q.Id,
                            DepartmentId = q.DepartmentId,
                            RoleIds = gur.Any() ? gur.Select(x => x.groupUserRoles.RoleId).ToList() : null,
                            RoleNames = gur.Any() ? gur.Select(x => x.roles.Name).ToList() : null
                        });

            if (search != null)
            {
                if (!string.IsNullOrEmpty(search.Name))
                {
                    query = query.Where(x => EF.Functions.Like(x.Name, $"%{search.Name}%"));
                }
                if (!string.IsNullOrEmpty(search.Code))
                {
                    query = query.Where(x => EF.Functions.Like(x.Code, $"%{search.Code}%"));
                }
                if (search.DepartmentId != null)
                {
                    query = query.Where(x => x.DepartmentId == search.DepartmentId);
                }
            }
            query = query.OrderByDescending(x => x.CreatedDate);
            var result = await PagedList<GroupUserDto>.CreateAsync(query, search);



            return result;
        }

        public async Task<GroupUserDto?> GetDto(Guid id)
        {
            var item = await (from q in GetQueryable().Where(x => x.Id == id)
                              select new GroupUserDto()
                              {
                                  Name = q.Name,
                                  Code = q.Code,
                                  CreatedBy = q.CreatedBy,
                                  UpdatedBy = q.UpdatedBy,
                                  IsDelete = q.IsDelete,
                                  DeleteId = q.DeleteId,
                                  CreatedDate = q.CreatedDate,
                                  UpdatedDate = q.UpdatedDate,
                                  DeleteTime = q.DeleteTime,
                                  Id = q.Id,
                              }).FirstOrDefaultAsync();

            return item;
        }

    }
}
