namespace EFCoreWebApi
{
    /// <summary>
    /// Base response
    /// </summary>
    public class ApiResponse
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
        public List<string> Errors { get; set; }
        /// <summary>
        /// Returns the text of all errors.
        /// </summary>
        [JsonIgnore]
        public string ErrorText => Succeeded? string.Empty: string.Join(Environment.NewLine, Errors.ToArray());
 
    }
}
