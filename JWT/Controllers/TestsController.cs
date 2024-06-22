using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWT.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestsController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }

    [HttpGet("exception")]
    public IActionResult ThrowException()
    {
        throw new Exception("");
    }
}