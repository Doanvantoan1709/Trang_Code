using BaseProject.Model.Entities;
using BaseProject.Service.Common;
using BaseProject.Service.Constant;

namespace BaseProject.Service.ApiPermissionsService.Dto
{
    public class ApiPermissionGroupData
    {
        public string? Name { get; set; }
        public string? Path { get; set; }
        public bool Checked { get; set; }
        public List<ApiPermissionAction>? Actions { get; set; }
    }
    public class ApiPermissionAction
    {
        public string? Name { get; set; }
        public string? Path { get; set; }
        public bool Checked { get; set; }
    }


}
