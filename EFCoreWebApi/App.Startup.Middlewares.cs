namespace EFCoreWebApi
{
    static public partial class App
    {

        static public void AddMiddlewares(WebApplication app)
        {
            var RootServiceProvider = (app as IApplicationBuilder).ApplicationServices;
            var HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
            var WebHostEnvironment = app.Environment;

            

            // ● Lib initialization
            Lib.Initialize(RootServiceProvider, HttpContextAccessor, WebHostEnvironment, Configuration);

            // ● AppSettings
            // get an IOptionsMonitor<AppSettings> service instance
            // IOptionsMonitor is a singleton service
            IOptionsMonitor<AppSettings> AppSettingsMonitor = app.Services.GetRequiredService<IOptionsMonitor<AppSettings>>();

            // Lib App.SetupAppSettingsMonitor to hook into IOptionsMonitor<AppSettings>.OnChange()
            Lib.SetupAppSettingsMonitor(AppSettingsMonitor);


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
        }
    }
}
