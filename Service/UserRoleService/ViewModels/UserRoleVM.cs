using BaseProject.Service.DepartmentService.ViewModels;


namespace BaseProject.Service.UserRoleService.ViewModels
{
    public class UserRoleVM
    {
        public Guid UserId { get; set; }

        public List<DepartmentVM> Departments { get; set; } = new List<DepartmentVM>();
    }
}
