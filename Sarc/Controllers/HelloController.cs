using Microsoft.AspNetCore.Mvc;
using Sarc.Model.Entity;
using Sarc.Service.Interface;

namespace Sarc.Controllers
{
    [Route("[controller]")]
    public class HelloController : Controller
    {
        private readonly IHelloService _service;

        public HelloController(IHelloService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<Hello> Get()
        {
            var greeting = _service.Hello();
            return Ok(greeting);
        }
    }
}