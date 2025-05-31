namespace EFCoreWebApi
{

    static public partial class App
    {
        static public void AddServices(WebApplicationBuilder builder)
        {
            // ● AppSettings
            App.Configuration = builder.Configuration;
            builder.Configuration.Bind(nameof(AppSettings), Lib.Settings);

            // ● DbContext
            // AddDbContextPool() singleton service
            // AddDbContext() scoped servicea
            // SEE: https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics
            builder.Services.AddDbContextPool<AppDbContext>(context => context.UseSqlite(), poolSize: 1024);
            //builder.Services.AddDbContext<AppDbContext>(context => { context.UseInMemoryDatabase(AppDbContext.SMemoryDatabase); });

            // ● custom services 
            builder.Services.AddScoped<ApiClientContext>();


            // ● HttpContext - NOTE: is singleton
            builder.Services.AddHttpContextAccessor();

            // ● ActionContext - see: https://github.com/aspnet/mvc/issues/3936
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();  // see: https://github.com/aspnet/mvc/issues/3936

            // ● Memory Cache - NOTE: is singleton
            // NOTE: Distributed Cache is required for Session to function properly
            // SEE: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state#configure-session-state
            builder.Services.AddDistributedMemoryCache(); // AddMemoryCache(); 

            // ● Authentication  
            AuthenticationBuilder AuthBuilder = builder.Services.AddAuthentication(o => {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = o.DefaultScheme;
                o.DefaultChallengeScheme = o.DefaultScheme;
            });

            AuthBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o => {

                TokenValidationParameters ValidationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Lib.Settings.Jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudiences = new List<string> {
                            Lib.Settings.Jwt.Audience
                    },

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Lib.Settings.Jwt.Secret)),

                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };


                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = ValidationParams;

                //o.Events = new JwtBearerEvents();
                //o.Events.OnMessageReceived = MessageReceived;
                //o.Events.OnTokenValidated = TokenValidated;
            });

            // ● Authorization  
            builder.Services.AddAuthorization();

            // ● Controllers
            builder.Services.AddControllers();


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            // SEE: https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer(); // https://localhost:7025/swagger
            builder.Services.AddSwaggerGen();
        }
    }
}
