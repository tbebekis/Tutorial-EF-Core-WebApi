namespace EFCoreWebApi
{

    /// <summary>
    /// Base response
    /// </summary>
    public class ApiResult
    {
        // ● public
        /// <summary>
        /// Adds an error to the error list
        /// </summary>
        /// <param name="ErrorText"></param>
        public void AddError(string ErrorText)
        {
            if (Errors == null)
                Errors = new List<string>();

            Errors.Add(ErrorText);
        }
        /// <summary>
        /// Clears the error list
        /// </summary>
        public void ClearErrors()
        {
            if (Errors != null)
                Errors.Clear();
        }

        // ● properties
        /// <summary>
        /// True if there are no errors
        /// </summary>
        [JsonIgnore]
        public bool Succeeded => Errors == null || Errors.Count == 0;
        /// <summary>
        /// Returns the first error
        /// </summary>
        [JsonIgnore]
        public string Error => Errors != null && Errors.Count > 0 ? Errors[0] : string.Empty;
        /// <summary>
        /// The list of errors
        /// </summary>
        [Description("The list of errors, if any.")]
        public List<string> Errors { get; set; }
        /// <summary>
        /// Returns the text of all errors.
        /// </summary>
        [JsonIgnore]
        public string ErrorText => Succeeded? string.Empty: string.Join(Environment.NewLine, Errors.ToArray());
        [Description("The HTTP Status numeric code.")]
        [DefaultValue(StatusCodes.Status200OK)]
        public int HttpStatus { get; set; } = StatusCodes.Status200OK;
    }
}
