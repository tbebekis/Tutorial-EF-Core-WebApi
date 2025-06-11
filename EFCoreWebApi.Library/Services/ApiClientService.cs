namespace EFCoreWebApi.Services
{
    public class ApiClientService: DataService<AppClient>
    {
        /// <summary>
        /// Validates the specified user credentials and returns a <see cref="IRequestor"/> on success, else null.
        /// </summary>
        public async Task<ItemResult<IAppClient>> ValidateApiClientCredentials(string ClientId, string PlainTextSecret)
        {
            ItemResult<IAppClient> Result = new();
            ItemResult<AppClient> ClientResult = await GetByProcAsync(c => c.ClientId == ClientId);

            if (ClientResult.Item == null || !ClientResult.Succeeded)
            {
                Result.ErrorResult(ApiStatusCodes.InvalidCredentials);
                return Result;
            }

            if (!Hasher.Validate(PlainTextSecret, ClientResult.Item.Secret, ClientResult.Item.SecretSalt))
            {
                Result.ErrorResult(ApiStatusCodes.InvalidCredentials);
                return Result;
            }

            Result.Item = ClientResult.Item;
 
            return Result;
        }
    }
}
