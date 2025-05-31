namespace EFCoreWebApi.Library
{
    public class AppSettings
    {

        public DefaultSettings Defaults { get; set; } = new DefaultSettings();
        public JwtSettings Jwt { get; set; } = new JwtSettings();
    }
}
