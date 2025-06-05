namespace EFCoreWebApi
{

    /// <summary>
    /// Base response
    /// </summary>
    public class ApiResult
    {
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

        public virtual void SetResult(int HttpStatus = -1, string ErrorMessage = null, string Message = null)
        {
            if (HttpStatus >= StatusCodes.Status100Continue)
                this.HttpStatus = HttpStatus;

            if (!string.IsNullOrWhiteSpace(ErrorMessage))
                Errors.Add(ErrorMessage);

            if (!string.IsNullOrWhiteSpace(Message))
               this.Message = Message;
        }
        public virtual void ErrorResult(int HttpStatus, string ErrorMessage = null)
        {
            ErrorMessage = !string.IsNullOrWhiteSpace(ErrorMessage) ? ErrorMessage : $"Error: {HttpStatus}";
            SetResult(HttpStatus: HttpStatus, ErrorMessage: ErrorMessage);
        } 
        public virtual void BadRequest(string ErrorMessage = null)
        {
            ErrorMessage = !string.IsNullOrWhiteSpace(ErrorMessage) ? $"Bad request: {ErrorMessage}" : "Bad request";
            ErrorResult(StatusCodes.Status400BadRequest, ErrorMessage);
        }
        public virtual void NoDataResult(string Message = null)
        {
            Message = !string.IsNullOrWhiteSpace(Message) ? $"No data: {Message}" : "No data.";
            SetResult(HttpStatus: CustomStatusCodes.Status150NoData, Message: Message);
        }

        // ● properties
        /// <summary>
        /// True if there are no errors
        /// </summary>
        [JsonIgnore]
        public bool Succeeded => Errors == null || Errors.Count == 0;
        /// <summary>
        /// Returns the text of all errors.
        /// </summary>
        [JsonIgnore]
        public string ErrorText => Succeeded ? string.Empty : string.Join(Environment.NewLine, Errors.ToArray());
 
        [Description("The HTTP Status numeric code.")]
        [DefaultValue(StatusCodes.Status200OK)]
        public int HttpStatus { get; set; } = StatusCodes.Status200OK;
        [Description("A response message. Defaults to OK.")]
        [DefaultValue("OK")]
        public string Message { get; set; } = "OK";
        [Description("The list of errors, if any.")]
        public List<string> Errors { get; set; }
    }
}
