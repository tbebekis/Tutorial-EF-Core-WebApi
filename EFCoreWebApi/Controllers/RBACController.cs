namespace EFCoreWebApi.Controllers
{

    [Permission("RBAC.Admin")]
    [Tags("Security")]
    [Route("rbac")]
    public class RBACController : WebApiController
    {
        // ● actions
        [EndpointDescription("Returns the list of registered clients.")]
        [HttpGet("client/list", Name = "Client.List"), Produces<ListResult<IAppClient>>]
        public async Task<ListResult<IAppClient>> List()
        {
            ListResult<IAppClient> Result = new();

            DataService <AppClient> Service = new DataService<AppClient>();
            ListResult<AppClient> DataList = await Service.GetAllAsync();
            string JsonText = JsonSerializer.Serialize(DataList);
            if (DataList.Succeeded)
            {
                List<IAppClient> List = DataList.List.Cast<AppClient>().Select(x => x as IAppClient).ToList();
                Result.List = List;
            }
            else
            {
                Result.CopyErrors(DataList);
            }

           return Result;
        }

        [EndpointDescription("Insert a new AppClient.")]
        [HttpPost("client", Name = "Client.Insert"), Produces<ItemResult<IAppClient>>]
        public async Task<ItemResult<IAppClient>> InsertClient(AppClient Model)
        {
            // TODO: Validation

            //
            AppClient Client = new AppClient(Model.ClientId, Model.Secret, Model.Name);
            DataService<AppClient> Service = new DataService<AppClient>();
            ItemResult<AppClient> ResultClient = await Service.InsertAsync(Client);

            ItemResult<IAppClient> Result = new();
            Result.CopyErrors(ResultClient);
            Result.Item = ResultClient.Item;
            return Result;
        }
    }
}
