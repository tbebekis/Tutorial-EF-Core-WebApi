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
        [Produces<ApiItemResult<TokenData>>]
        [Tags("Security")]
        [HttpPost("authenticate"), AllowAnonymous]
        public ApiItemResult<TokenData> Authenticate([FromBody] TokenRequest M)
        {
            ApiItemResult<TokenData> Result = new();

            string ClientId = M.ClientId; 
            string Secret = M.Secret; 
            string CultureCode = M.Locale;

            if (string.IsNullOrWhiteSpace(CultureCode))
                CultureCode = Lib.Settings.Defaults.CultureCode;

            if (!Lib.Settings.Defaults.SupportedCultures.Contains(CultureCode))
            {
                Result.BadRequest("Requested locale not supported.");
            }
            else
            {
                ApiItemResult<IApiClient> DataResult = Service.ValidateApiClientCredentials(ClientId, Secret);
                IApiClient Client = DataResult.Item;

                if (DataResult.Succeeded && Client != null)
                {
                    Result = Lib.CreateAuthenticatedToken(Client, CultureCode);
                    return Result;
                }
                else
                {
                    Result.ErrorResult(StatusCodes.Status401Unauthorized, "Wrong credentials.");
                }
            }

            Result.NoDataResult();
            return Result;
        }
    }
}
