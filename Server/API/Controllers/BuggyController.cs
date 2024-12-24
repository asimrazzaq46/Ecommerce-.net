using API.DTOs;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{

    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    {
        return Unauthorized();
    }
    
    [HttpGet("notfound")]
    public IActionResult GetNotFound()
    {
        return NotFound();
    }
    
    [HttpGet("badrequest")]
    public IActionResult GetBadRequest()
    {
        return BadRequest("Not a good request");
    }
    
    [HttpPost("validationerror")]
    public IActionResult GetValidationError(CreateProductDto product)
    {
        return Ok();
    }
    
    [HttpGet("internalerror")]
    public IActionResult GetInternalError()
    {
        throw new Exception("This is a test exception");
    }

}
