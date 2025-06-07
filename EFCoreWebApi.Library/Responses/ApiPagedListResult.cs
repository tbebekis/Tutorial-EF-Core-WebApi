namespace EFCoreWebApi.Responses
{
    public class ApiPagedListResult<T>: ApiListResult<T>, IPaging
    {
 


        /// <summary>
        /// The number of total items when this is a paged response. 
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// The number of total pages.
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        public int TotalPages => Paging.GetTotalPages(TotalItems, PageSize);
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
