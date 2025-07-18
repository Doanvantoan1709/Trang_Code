//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.EntityFrameworkCore;
//using Model;
//using System.Security.Claims;

//namespace BaseProjectNetCore.Filter
//{
//    public class AuthorizeAttribute : TypeFilterAttribute
//    {
//        public AuthorizeAttribute(string _functionCode, string _permission) : base(typeof(AuthorizeActionFilter))
//        {
//            Arguments = new object[] { _functionCode, _permission };
//        }

//        public class AuthorizeActionFilter : IAsyncAuthorizationFilter
//        {
//            private string _functionCode { get; set; }
//            private string _permission { get; set; }
//            private readonly BaseProjectContext _dbContext;

//            public AuthorizeActionFilter(string functionCode, string permission, BaseProjectContext dbContext)
//            {
//                _functionCode = functionCode;
//                _permission = permission;
//                _dbContext = dbContext;
//            }

//            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
//            {
//                var identity = context.HttpContext.User.Identity as ClaimsIdentity;

//                if (identity == null || !identity.IsAuthenticated)
//                {
//                    // Nếu không có xác thực thì trả về unauthorized
//                    context.HttpContext.Response.StatusCode = 401; // Error code: Unauthorize
//                    context.HttpContext.Response.ContentType = "application/json";
//                    context.Result = new JsonResult(new
//                    {
//                        message = "Please login to access the feature"
//                    });
//                    return;
//                }

//                var userClaims = identity.Claims;
//                var userID = Convert.ToInt32(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.PrimarySid)?.Value);
//                var isAdmin = Convert.ToInt32(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value);


//                if (isAdmin != 1)
//                {
//                    // Lấy FunctionId từ FunctionCode
//                    var function = await _dbContext.function.FirstOrDefaultAsync(x => x.FunctionCode == _functionCode);
//                    if (function == null)
//                    {
//                        context.Result = new JsonResult(new { message = "Function not found" }) { StatusCode = 403 };
//                        return;
//                    }

//                    // Lấy quyền theo AccountId + FunctionId
//                    var permission = await _dbContext.permission.FirstOrDefaultAsync(x => x.AccountId == userID && x.FunctionId == function.FunctionId);

//                    if (permission == null)
//                    {
//                        context.Result = new JsonResult(new { message = "Access denied: No permission record" }) { StatusCode = 403 };
//                        return;
//                    }

//                    // khác admin thì check quyền theo switch
//                    switch (_permission.ToUpper())
//                    {
//                        case "ISVIEW": // Check user quyền view
//                            if (permission.IsView != 1)
//                            {
//                                context.Result = new JsonResult(new { message = "Access denied: No view permission" }) { StatusCode = 403 };
//                                return;
//                            }
//                            break;

//                        case "ISINSERT": // Check user quyền insert
//                            if (permission.IsInsert != 1)
//                            {
//                                context.Result = new JsonResult(new { message = "Access denied: No insert permission" }) { StatusCode = 403 };
//                                return;
//                            }
//                            break;

//                        case "ISUPDATE": // Check user quyền update
//                            if (permission.IsUpdate != 1)
//                            {
//                                context.Result = new JsonResult(new { message = "Access denied: No update permission" }) { StatusCode = 403 };
//                                return;
//                            }
//                            break;

//                        case "ISDELETE": // Check user quyền delete
//                            if (permission.IsDelete != 1)
//                            {
//                                context.Result = new JsonResult(new { message = "Access denied: No delete permission" }) { StatusCode = 403 };
//                                return;
//                            }
//                            break;

//                        default:
//                            context.Result = new JsonResult(new { message = "Invalid permission type" }) { StatusCode = 400 };
//                            return;
//                    }
//                }
//            }
//        }
//    }
//}
