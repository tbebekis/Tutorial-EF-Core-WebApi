namespace EFCoreWebApi.Controllers
{
    [Route("product")]
    [Tags("Products")]
    public class ProductController: WebApiController
    {
        DataService<Product> Service = new();

        // ● construction
        public ProductController()
        {
        }

        // ● actions
        [EndpointDescription("Returns the list of all products.")]
        [Produces<ListResult<Product>>]        
        [HttpGet("list", Name = "Product.List")]
        public async Task<ListResult<Product>> List()
        {
            ListResult<Product> Result = await Service.GetAllAsync();
            return Result;
        }
    }
}
