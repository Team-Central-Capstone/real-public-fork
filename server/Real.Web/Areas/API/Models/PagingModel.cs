
using System;

namespace Real.Web.Areas.API.Models {
    public class PagingModel {
        public static int PageSize { get; set; } = 20;
        public int Page { get; set; } = 1;
        public Int64 TotalRecords { get; set; } = -1;
    }
}