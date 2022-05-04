
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Real.Web.Helpers {

    /// <summary>
    /// Handles adding the 'active' class on the navbar when the associated action is invoked
    /// </summary>
    [HtmlTargetElement("a")]
    public class ActiveNavLinkTagHelper : TagHelper {

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected HttpRequest Request => ViewContext.HttpContext.Request;


        public override void Process(TagHelperContext context, TagHelperOutput output) {
            base.Process(context, output);

            if (!output.Attributes.TryGetAttributes("class", out IReadOnlyList<TagHelperAttribute> classAttribute))
                return;
                
            var classList = classAttribute.Select(x => x.Value).Select(x => x.ToString().ToLower()).ToList();
            if (!classList.Contains("nav-link") && !classList.Contains("dropdown-item")) {
                return;
            }

            context.AllAttributes.TryGetAttribute("asp-area", out TagHelperAttribute targetArea);
            context.AllAttributes.TryGetAttribute("asp-controller", out TagHelperAttribute targetController);
            context.AllAttributes.TryGetAttribute("asp-action", out TagHelperAttribute targetAction);

            var target = (targetArea?.Value.ToString().ToLower(), targetController?.Value.ToString().ToLower(), targetAction?.Value.ToString().ToLower());
            var actual = (Request.RouteValues["area"]?.ToString().ToLower() ?? "", Request.RouteValues["controller"]?.ToString().ToLower() ?? "", Request.RouteValues["action"]?.ToString().ToLower() ?? "");
            
            if (target.Equals(actual)) {
                classList.Add("active");
                var list = String.Join(" ", classList);
                output.Attributes.SetAttribute("class", list);
            }
        }
    }
}