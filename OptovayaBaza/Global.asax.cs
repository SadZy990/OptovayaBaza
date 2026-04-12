using OptovayaBaza.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;
using log4net.Config;

namespace OptovayaBaza
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Инициализация log4net из отдельного файла
            XmlConfigurator.Configure(new System.IO.FileInfo(Server.MapPath("~/log4net.config")));
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}