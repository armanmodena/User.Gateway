using System;
using System.Collections.Generic;

namespace User.Gateway.DTO
{
    [Serializable]
    public class PageResult<T>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPage { get; set; }

        public long TotalCount { get; set; }

        public IEnumerable<T> Data { get; set; }

        public PageResult()
        {

        }

        public PageResult(int page, int pageSize, int totalPage, long totalCount, IEnumerable<T> data)
        {
            Page = page;
            PageSize = pageSize;
            TotalPage = totalPage;
            TotalCount = totalCount;
            Data = data;
        }
    }
}
