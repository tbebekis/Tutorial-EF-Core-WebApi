namespace EFCoreWebApi.Controllers
{
    
    public class AuthController : WebApiController
    {
        AuthService Service;

        public AuthController(AuthService service)
        {
            Service = service;
        }

        [EndpointDescription("Authenticates a client.")]
        [Produces<ApiItemResult<ApiToken>>]
        [Tags("Security")]
        [HttpPost("authenticate"), AllowAnonymous]
        public ApiItemResult<ApiToken> Authenticate([FromBody] ApiClientCredentials M)
        {
            string ClientId = M.ClientId; 
            string Secret = M.Secret; 
            string CultureCode = M.Locale;

            if (string.IsNullOrWhiteSpace(CultureCode))
                CultureCode = Lib.Settings.Defaults.CultureCode;

            if (!Lib.Settings.Defaults.SupportedCultures.Contains(CultureCode))
                return ApiItemResult<ApiToken>.BadRequest("Requested locale not supported.");

            ApiItemResult<IApiClient> DataResult = Service.ValidateApiClientCredentials(ClientId, Secret);
            IApiClient Client = DataResult.Item;

            if (DataResult.Succeeded && Client != null)
            {
                ApiItemResult<ApiToken> TokenResponse = Lib.CreateAuthenticatedToken(Client, M.Locale);
                return TokenResponse;
            }
            else
            {
                ApiItemResult<ApiToken>.ErrorResult(StatusCodes.Status401Unauthorized, "Wrong credentials.");
            }

            return ApiItemResult<ApiToken>.NoDataResult();
        }
    }
}
