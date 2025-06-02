namespace EFCoreWebApi.Controllers
{
    
    public class AuthController : WebApiController
    {
        AuthService Service;

        public AuthController(AuthService service)
        {
            Service = service;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        //public IActionResult Authenticate(string ClientId, string Secret, string CultureCode)         // POST api/v1/authenticate?ClientId=teo&Secret=webdesk&CultureCode=en-US
        public IActionResult Authenticate([FromBody] ApiClientCredentials M)
        {
            string ClientId = M.ClientId; 
            string Secret = M.Secret; 
            string CultureCode = M.Locale;

            if (string.IsNullOrWhiteSpace(CultureCode))
                CultureCode = Lib.Settings.Defaults.CultureCode;

            if (!Lib.Settings.Defaults.SupportedCultures.Contains(CultureCode))
                return BadRequest(new { message = "Invalid culture" });

            ApiItemResponse<IApiClient> DataResult = Service.ValidateApiClientCredentials(ClientId, Secret);
            IApiClient Client = DataResult.Item;

            if (DataResult.Succeeded && Client != null)
            {
                ApiItemResponse<ApiToken> TokenResponse = Lib.CreateAuthenticatedToken(Client, M.Locale);
                //OkObjectResult Result = new OkObjectResult(TokenResponse);

                return Json(TokenResponse);

            }

            return BadRequest(new { message = "Invalid ClientId or Secret." });
        }
    }
}
