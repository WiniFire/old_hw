using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ProductCacheApp.Models;
using ProductCacheApp.Services;

namespace ProductCacheApp.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductStore _store;

    public ProductsController(ProductStore store)
    {
        _store = store;
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var product = _store.Find(id);
        if (product is null) return NotFound();

        IList<EntityTagHeaderValue>? requestedTags;
        try
        {
            requestedTags = Request.GetTypedHeaders().IfNoneMatch;
        }
        catch (FormatException)
        {
            return BadRequest(new { message = "Некорректный заголовок If-None-Match." });
        }

        var tag = new EntityTagHeaderValue($"\"v{product.Version}\"");
        var headers = Response.GetTypedHeaders();
        headers.ETag = tag;
        headers.CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(60)
        };

        if (requestedTags is not null && requestedTags.Any(value =>
            value.Tag == "*" || value.Compare(tag, useStrongComparison: false)))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return Ok(new { product.Id, product.Name, product.Price });
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UpdateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Название не может быть пустым." });

        var product = _store.Update(id, request.Name, request.Price!.Value);
        if (product is null) return NotFound();

        Response.Headers.ETag = $"\"v{product.Version}\"";
        Response.Headers.CacheControl = "no-store";
        return Ok(new { product.Id, product.Name, product.Price });
    }
}
