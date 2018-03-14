using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1.Middleware
{
    public class RequestIPMiddleware
    {
        private readonly RequestDelegate _next; //定义请求委托  
        private readonly ILogger _logger; //定义日志  

        public RequestIPMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestIPMiddleware>();
        }

        /// <summary>  
        /// 中间件核心功能  
        /// 做什么功能，都在该方法中处理  
        /// </summary>  
        /// <param name="context"></param>  
        /// <returns>Task(void)</returns>  
        public async Task Invoke(HttpContext context)
        {
            string ip = context.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"User IP：{ip}");
            await _next.Invoke(context);
            await context.Response.WriteAsync($"Hello World!User IP：{ip}");  
        }
    }
    /// <summary>  
    /// 请求IP扩展类  
    /// </summary>  
    public static class RequestIPExtensions
    {
        /// <summary>  
        /// 引入请求IP中间件  
        /// </summary>  
        /// <param name="builder">扩展类型</param>  
        /// <returns>IApplicationBuilder</returns>  
        public static IApplicationBuilder UseRequestIP(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestIPMiddleware>();
        }
    }
}
