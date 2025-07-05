using BaseProject.Model.Entities;
using BaseProject.Repository.GroupUserRoleRepository;
using BaseProject.Service.Common.Service;
using BaseProject.Service.GroupUserRoleService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Dto;
using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using Service.Common;

namespace BaseProject.Service.GroupUserRoleService
{
    public class GroupUserRoleService : Service<GroupUserRole>, IGroupUserRoleService
    {
        public GroupUserRoleService(
            IGroupUserRoleRepository groupUserRoleRepository
            ) : base(groupUserRoleRepository)
        {
        }

        public async Task<PagedList<GroupUserRoleDto>> GetData(GroupUserRoleSearch search)
        {
            var query = from q in GetQueryable()
                        select new GroupUserRoleDto()
                        {
                            GroupUserId = q.GroupUserId,
							RoleId = q.RoleId,
                            CreatedBy = q.CreatedBy,
                            UpdatedBy = q.UpdatedBy,
                            IsDelete = q.IsDelete,
                            DeleteId = q.DeleteId,
                            CreatedDate = q.CreatedDate,
                            UpdatedDate = q.UpdatedDate,
                            DeleteTime = q.DeleteTime,
                            Id = q.Id,
                        };
            if(search != null )
            {
                if(search.GroupUserId.HasValue)
				{
					query = query.Where(x => x.GroupUserId == search.GroupUserId);
				}
				if(search.RoleId.HasValue)
				{
					query = query.Where(x => x.RoleId == search.RoleId);
				}
            }
            query = query.OrderByDescending(x=>x.CreatedDate);
            var result = await PagedList<GroupUserRoleDto>.CreateAsync(query, search);
            return result;
        }

        public async Task<GroupUserRoleDto?> GetDto(Guid id)
        {
            var item = await (from q in GetQueryable().Where(x=>x.Id == id)
                        select new GroupUserRoleDto()
                        {
                            GroupUserId = q.GroupUserId,
							RoleId = q.RoleId,
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
