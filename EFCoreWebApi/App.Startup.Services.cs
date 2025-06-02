

using EFCoreWebApi.Library;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization.Metadata;

namespace EFCoreWebApi
{

    static public partial class App
    {
        static public void AddServices(WebApplicationBuilder builder)
        {
            //JsonConvert.DefaultSettings = () => Lib.JsonSerializerSettings;

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

                TokenValidationParameters ValidationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = Lib.Settings.Jwt.Issuer,                  
                    ValidAudiences = new List<string> { Lib.Settings.Jwt.Audience },                    
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Lib.Settings.Jwt.Secret))
                };

                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = ValidationParams;

                o.Events = new ApiClientJwtBearerEvents();
            });

            // ● Authorization  
            builder.Services.AddAuthorization();




            /*
            MvcBuilder.AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();   // no camelCase
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            builder.Services.Configure<MvcNewtonsoftJsonOptions>(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();   // no camelCase
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
            */



            // .AddNewtonsoftJson(o => o.SerializerSettings.ContractResolver = new DefaultContractResolver())  NamingStrategy = new DefaultNamingStrategy()

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            // SEE: https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer(); // https://localhost:7025/swagger
            builder.Services.AddSwaggerGen(options => {
                //options.SwaggerDoc.
                //options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                options.SwaggerGeneratorOptions.DescribeAllParametersInCamelCase = false;
                //JsonSerializerOptionsProvider

            });



            /*
            builder.Services.AddSingleton(serviceProvider =>
            {
                MvcNewtonsoftJsonOptions JsonOptions = new();
                JsonOptions.SerializerSettings.ContractResolver = new DefaultContractResolver();   // no camelCase
                JsonOptions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                JsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                JsonOptions.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                JsonOptions.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
                JsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());

                return Options.Create(JsonOptions);
            });
            */


            //builder.Services.AddSwaggerGenNewtonsoftSupport();

            //builder.Services.AddSingleton<IConfigureOptions<MvcNewtonsoftJsonOptions>>(JsonOptions);

            // https://blog.stackademic.com/how-to-implement-and-personalize-swaggerui-in-a-net-9-web-api-c69824310fe0
            // https://github.com/domaindrivendev/Swashbuckle.AspNetCore



            // ● Controllers
            IMvcBuilder MvcBuilder = builder.Services.AddControllers();

            MvcBuilder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

            builder.Services.AddSingleton(serviceProvider =>
            {
                JsonOptions JsonOptions = new();
                JsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
                JsonOptions.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

                return Options.Create(JsonOptions);
            });
        }
    }
}
 