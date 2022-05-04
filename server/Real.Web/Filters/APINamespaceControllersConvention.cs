
using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Real.Web.Filters {
    public class APINamespaceControllersConvention : IActionModelConvention {

        public void Apply(ActionModel action) {
            action.ApiExplorer.IsVisible = action.Controller.DisplayName.Contains("Real.Web.Areas.API.Controllers");
        }

    }
}