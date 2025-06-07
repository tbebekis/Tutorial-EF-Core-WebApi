namespace EFCoreWebApi
{


    static public partial class App
    {
        static void SetupJsonSerializerOptions(JsonSerializerOptions JsonOptions)
        {
            JsonOptions.PropertyNamingPolicy = new JsonNamingPolicyAsIs();
            JsonOptions.DictionaryKeyPolicy = JsonOptions.PropertyNamingPolicy;
            JsonOptions.PropertyNameCaseInsensitive = true;
            JsonOptions.WriteIndented = true;
            JsonOptions.IgnoreReadOnlyProperties = true;
            JsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // JsonIgnoreCondition.Always;
            JsonOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            JsonOptions.AllowTrailingCommas = true;
            JsonOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            JsonOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // or ReferenceHandler.IgnoreCycles
            JsonOptions.Converters.Insert(0, new JsonStringEnumConverter(JsonOptions.PropertyNamingPolicy));
        }

        static public void AddServices(WebApplicationBuilder builder)
        {

            // ● AppSettings
            App.Configuration = builder.Configuration;
            builder.Configuration.Bind(nameof(AppSettings), Lib.Settings);

            // ● DbContext
            // AddDbContextPool() singleton service
            // AddDbContext() scoped servicea
            // SEE: https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics
            //builder.Services.AddDbContextPool<AppDbContext>(context => context.UseSqlite(), poolSize: 1024);
            builder.Services.AddDbContext<AppDbContext>(context => { context.UseInMemoryDatabase(AppDbContext.SMemoryDatabase); });

            // ● custom services 
            builder.Services.AddScoped<ApiClientContext>();
            builder.Services.AddScoped<AuthService>();

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

                JwtSettings Jwt = Lib.Settings.Jwt;

                TokenValidationParameters ValidationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = Jwt.Issuer,                  
                    ValidAudiences = new List<string> { Jwt.Audience },                    
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.EncryptionKey))
                };

            
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = ValidationParams;

                o.Events = new ApiClientJwtBearerEvents();
            });

            // ● Authorization  
            builder.Services.AddAuthorization();

            // ● OpenApi
            builder.Services.AddOpenApi();
            //builder.Services.AddEndpointsApiExplorer(); // https://localhost:7025/swagger
            // The Microsoft.AspNetCore.OpenApi document generator doesn't use the MVC JSON options.
            // SEE: https://github.com/scalar/scalar/discussions/5828
            builder.Services.ConfigureHttpJsonOptions(options => SetupJsonSerializerOptions(options.SerializerOptions));

            // ● Controllers
            IMvcBuilder MvcBuilder = builder.Services.AddControllers();

            MvcBuilder.AddJsonOptions(options => SetupJsonSerializerOptions(options.JsonSerializerOptions));
        }
    }
}
 