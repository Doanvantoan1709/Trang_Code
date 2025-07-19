using BaseProject.Repository.TESTDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseProjectNetCore.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ITest _test;
        public TestsController(ITest test)
        {
            _test = test;
        }

        [HttpGet("Get_all_incoming")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var res = await _test.GetAllAsync();
                return Ok(res);
            }catch(Exception ex)
            {
                throw;
            }
        }
    }
}
