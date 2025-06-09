namespace EFCoreWebApi.Library
{

    /// <summary>
    /// Helper with paging information
    /// </summary>
    public class Paging: IPaging
    {

        /// <summary>
        /// Returns the total pages given the total items and the page size.
        /// </summary>
        static public int GetTotalPages(int TotalItems, int PageSize)
        {
            if (TotalItems <= PageSize)
                return 1;

            int Result = TotalItems / PageSize;
            int Remainder = TotalItems % PageSize;
            if (Remainder > 0)
                Result++;

            return Result;
        }

        /// <summary>
        /// The number of total items when this is a paged response. 
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// The number of total pages.
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        public int TotalPages => GetTotalPages(TotalItems, PageSize);
        /// <summary>
        /// The number of items in a page.
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// The current page index. 0 based.
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// The number of the page. 1-based
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        public int PageNumber => PageIndex + 1;
    }


}
