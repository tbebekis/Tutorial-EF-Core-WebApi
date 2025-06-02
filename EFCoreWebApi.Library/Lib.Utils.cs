﻿namespace EFCoreWebApi.Library
{
    static public partial class Lib
    {
        // ● Newtonsoft.Json
        static JsonSerializerSettings fJsonSerializerSettings;






        // ● strings 
        /// <summary>
        /// Case insensitive string equality.
        /// </summary>
        public static bool IsSameText(string A, string B)
        {
            return string.Compare(A, B, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        // ● files and folders
        /// <summary>
        /// Returns true if a specified path is a directory
        /// </summary>
        static public bool IsDirectory(string FolderPath)
        {
            return Directory.Exists(FolderPath);
        }
        /// <summary>
        /// Deletes a folder
        /// <para>Waits until the directory is actually deleted and not just marked as deleted</para>
        /// </summary>
        static public void DeleteFolder(string FolderPath)
        {
            Directory.Delete(FolderPath, true);
            int MaxIterations = 10;
            int Counter = 0;

            // wait until the directory is actually deleted
            // and not just marked as deleted
            while (Directory.Exists(FolderPath))
            {
                Counter += 1;

                if (Counter > MaxIterations)
                    return;

                Thread.Sleep(250);
            }
        }

        // ● to/from Base64  
        /// <summary>
        /// Encodes Value into a Base64 string using the specified Enc.
        /// If End is null, the Encoding.Unicode is used.
        /// </summary>
        static public string StringToBase64(string Value, Encoding Enc)
        {
            if (Enc == null)
                Enc = Encoding.Unicode;

            byte[] Data = Enc.GetBytes(Value);
            return Convert.ToBase64String(Data);
        }
        /// <summary>
        /// Decodes the Base64 string Value into a string using the specified Enc.
        /// If End is null, the Encoding.Unicode is used.
        /// </summary>
        static public string Base64ToString(string Value, Encoding Enc)
        {
            if (Enc == null)
                Enc = Encoding.Unicode;

            byte[] Data = Convert.FromBase64String(Value);
            return Enc.GetString(Data);
        }

        // ● miscs 
        /// <summary>
        /// Creates and returns a new Guid.
        /// <para>If UseBrackets is true, the new guid is surrounded by {}</para>
        /// </summary>
        static public string GenId(bool UseBrackets)
        {
            string format = UseBrackets ? "B" : "D";
            return Guid.NewGuid().ToString(format).ToUpper();
        }
        /// <summary>
        /// Creates and returns a new Guid WITHOUT surrounding brackets, i.e. {}
        /// </summary>
        static public string GenId()
        {
            return GenId(false);
        }

        // ● properties
        static public bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.WinCE;
        static public bool IsLinux => Environment.OSVersion.Platform == PlatformID.Unix;
        static public JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                if(fJsonSerializerSettings == null)
                {
                    fJsonSerializerSettings = new JsonSerializerSettings();
                    fJsonSerializerSettings.ContractResolver = new DefaultContractResolver() { NamingStrategy = new DefaultNamingStrategy() };   // no camelCase
                    fJsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    fJsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    fJsonSerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    fJsonSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
                    fJsonSerializerSettings.Converters.Add(new StringEnumConverter());
                }
                return fJsonSerializerSettings;
            }
        }
    }
}
