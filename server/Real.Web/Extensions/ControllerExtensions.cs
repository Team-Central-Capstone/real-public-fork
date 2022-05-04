using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Real.Web.Areas.API.Models;

namespace Real.Web {
    public static partial class Extensions {
        
        public static string GetUserFirebaseId(this Controller context) {
            return context.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))?.Value;
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> query, PagingModel paging) {
            return query
                .Skip((paging.Page - 1) * PagingModel.PageSize)
                .Take(PagingModel.PageSize);
        }
    }
}