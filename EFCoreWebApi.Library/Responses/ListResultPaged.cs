namespace EFCoreWebApi.Responses
{

    [Description("A list of requested objects with pagination.")]
    public class ListResultPaged<T>: ListResult<T>, IPaging
    {

        /// <summary>
        /// The number of total items when this is a paged response. 
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        [Description("The number of total items when this is a paged response.")]
        public int TotalItems { get; set; }
        /// <summary>
        /// The number of total pages.
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        [Description("The number of total pages.")]
        public int TotalPages => Paging.GetTotalPages(TotalItems, PageSize);
        /// <summary>
        /// The number of items in a page.
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        [Description("The number of items in the page.")]
        public int PageSize { get; set; }
        /// <summary>
        /// The current page index. 0 based.
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        [Description("The current page index. 0 based.")]
        public int PageIndex { get; set; }
        /// <summary>
        /// The number of the page. 1-based
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        [Description("The number of the page. 1-based")]
        public int PageNumber => PageIndex + 1;
    }
}
