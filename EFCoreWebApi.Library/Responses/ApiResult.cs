namespace EFCoreWebApi.Responses
{

    /// <summary>
    /// Base response
    /// </summary>
    public class ApiResult
    {
        List<string> fErrors;

        // ● constructor
        public ApiResult()
        {
        }

        // ● public
        /// <summary>
        /// Adds an error to the error list
        /// </summary>
        /// <param name="ErrorText"></param>
        public virtual void AddError(string ErrorText)
        {
            if (!string.IsNullOrWhiteSpace(ErrorText))
            {
                if (Errors == null)
                    Errors = new List<string>();

                if (!Errors.Contains(ErrorText))
                    Errors.Add(ErrorText);

                Message = "Check the Errors list";
            }
        }
        /// <summary>
        /// Clears the error list
        /// </summary>
        public virtual void ClearErrors()
        {
            if (Errors != null)
                Errors.Clear();
        }

        public virtual void SetResult(int Status = -1, string ErrorMessage = null, string Message = null)
        {
            if (Status >= StatusCodes.Status100Continue)
                this.Status = Status;

            if (!string.IsNullOrWhiteSpace(ErrorMessage))
                Errors.Add(ErrorMessage);

            if (!string.IsNullOrWhiteSpace(Message))
               this.Message = Message;
        }
        public virtual void ErrorResult(int Status, string ErrorMessage = null)
        {
            ErrorMessage = !string.IsNullOrWhiteSpace(ErrorMessage) ? ErrorMessage : $"Error: {Status}";
            SetResult(Status: Status, ErrorMessage: ErrorMessage, "Error");
        }
        public virtual void NotAuthenticated()
        {
            string ErrorMessage = "Invalid Token. A valid JTW access token is required.";
            ErrorResult(StatusCodes.Status400BadRequest, ErrorMessage);
        }
        public virtual void TokenExpired(string ErrorMessage)
        {
            ErrorMessage = !string.IsNullOrWhiteSpace(ErrorMessage) ? ErrorMessage : $"The Access Token is expired";
            ErrorResult(StatusCodes.Status401Unauthorized, ErrorMessage);
        }
        public virtual void ExceptionResult(Exception Ex)
        {
            if (Ex != null)
            {
                string ErrorMessage = $"Exception: {Ex.Message}";
                ErrorResult(CustomStatusCodes.Exception, ErrorMessage); 
            }
        }

        public virtual void BadRequest(string ErrorMessage = null)
        {
            ErrorMessage = !string.IsNullOrWhiteSpace(ErrorMessage) ? $"Bad request: {ErrorMessage}" : "Bad request";
            ErrorResult(StatusCodes.Status400BadRequest, ErrorMessage);
        }
        public virtual void NoDataResult(string Message = null)
        {
            Message = !string.IsNullOrWhiteSpace(Message) ? $"No data: {Message}" : "No data.";
            SetResult(Status: CustomStatusCodes.NoData, Message: Message);
        }

        // ● properties
        /// <summary>
        /// True if there are no errors
        /// </summary>
        [JsonIgnore]
        public bool Succeeded => fErrors == null || fErrors.Count == 0;
        /// <summary>
        /// Returns the text of all errors.
        /// </summary>
        [JsonIgnore]
        public string ErrorText => Succeeded ? string.Empty : string.Join(Environment.NewLine, Errors.ToArray());
 
        [Description("A Status numeric code. Could be a standard HTTP Status Code or a custom code.")]
        [DefaultValue(StatusCodes.Status200OK)]
        public int Status { get; set; } = StatusCodes.Status200OK;
        [Description("A response message. Defaults to OK.")]
        [DefaultValue("OK")]
        public string Message { get; set; } = "OK";
        [Description("The list of errors, if any.")]
        public List<string> Errors
        {
            get
            {
                if (fErrors == null)
                    fErrors = new List<string>();
                return fErrors;
            }
            set { fErrors = value; }
        }
    }
}
