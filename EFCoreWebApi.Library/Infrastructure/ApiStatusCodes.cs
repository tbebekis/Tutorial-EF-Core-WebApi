namespace EFCoreWebApi.Library
{
    /// <summary>
    /// API Status Codes.
    /// <para>Standard HTTP Status Codes</para>
    /// <list type="bullet">
    /// <item>100 – 199: Informational responses</item>
    /// <item>200 – 299: Successful responses   </item>
    /// <item>300 – 399: Redirection messages   </item>
    /// <item>400 – 499: Client error responses </item>
    /// <item>500 – 599: Server error responses </item>
    /// </list>
    /// <para>SEE: https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status</para>
    /// <para>SEE: https://www.rfc-editor.org/rfc/rfc2616#section-6.1.1 </para>
    /// </summary>
    static public class ApiStatusCodes
    {        
        public const int UnknownError = 1000;
        public const int Exception = 1001;
        public const int NoData = 1002;
    }

 
}
