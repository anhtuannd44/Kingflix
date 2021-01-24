using System.Web.Http.ExceptionHandling;
using NLog;

namespace Kingflix.Website.Extensions.WebApi
{
    public class WebApiGlobalErrorLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Log(LogLevel.Error, context.Exception, context.Exception.Message);
        }
    }
}