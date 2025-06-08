namespace EFCoreWebApi.Controllers
{
    [Route("product")]
    [Tags("Products")]
    public class ProductController: WebApiSecureController
    {
        DataService<Product> Service = new();

        // ● construction
        public ProductController()
        {
        }

        // ● actions

        [EndpointDescription("Returns the list of all products.")]
        [Produces<ApiListResult<Product>>]        
        [HttpGet("list", Name = "Product.List")]
        public async Task<ApiListResult<Product>> List()
        {
            ApiListResult<Product> Result = await Service.GetAllAsync();
            return Result;
        }
    }
}
