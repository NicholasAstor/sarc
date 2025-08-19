using Microsoft.AspNetCore.Mvc;
using Sarc.Model.Entity;
using Sarc.Service.Interface;

namespace Sarc.Controllers;

[ApiController]                              // <- isso ajuda o Swagger e validação automática
[Route("api/[controller]")]                  // <- agora fica /api/hello
public class HelloController : ControllerBase // <- API usa ControllerBase
{
    private readonly IHelloService _service;

    public HelloController(IHelloService service)
    {
        _service = service;
    }

    /// <summary>Retorna uma saudação (endpoint de exemplo).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(Hello), StatusCodes.Status200OK)]
    public ActionResult<Hello> Get()
    {
        var greeting = _service.Hello();
        return Ok(greeting);
    }
}
