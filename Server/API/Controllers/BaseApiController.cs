using API.RequestHelpers;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    protected async Task<ActionResult> CreatePageResult<T>(IGenericRepositery<T> repo,
        ISpecification<T> spec, int pageIndex, int pageSize) where T : BaseModel
    {
        var items  = await repo.GetListBySpecAsync(spec);
        var count = await repo.CountAsync(spec);
        var pagination = new Pagination<T>(pageIndex, pageSize, count, items);

        return Ok(pagination);
    }
}
