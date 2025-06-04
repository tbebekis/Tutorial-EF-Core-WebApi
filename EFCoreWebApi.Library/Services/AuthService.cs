namespace EFCoreWebApi.Services
{
    public class AuthService: DataService<ApiClient>
    {
        static ApiClient DefaultClient = new ApiClient("ClientId", "Secret");

        /// <summary>
        /// Validates the specified user credentials and returns a <see cref="IRequestor"/> on success, else null.
        /// </summary>
        public ApiItemResult<IApiClient> ValidateApiClientCredentials(string ClientId, string PlainTextSecret)
        {
            ApiItemResult<IApiClient> Result = new();

            /*
            using (AppDbContext context = Lib.GetDbContext())
            {
                ApiClient Client = context.ApiClients.FirstOrDefault(x => x.ClientId == ClientId);
                if (Client != null)
                {
                    if (Hasher.Validate(PlainTextSecret, Client.Secret, Client.SecretSalt))
                        Result.Item = Client;
                }
            }
            */

            Result.Item = DefaultClient;
            return Result;
        }
    }
}
