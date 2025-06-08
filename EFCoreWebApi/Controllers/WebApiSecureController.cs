namespace EFCoreWebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // 
    public class WebApiSecureController : WebApiController
    { 
    }
}
