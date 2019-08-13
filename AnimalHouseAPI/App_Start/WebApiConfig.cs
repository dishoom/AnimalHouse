using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AnimalHouseAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //remove default xml formatter
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            //add json formatter
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

        }
    }
}
