using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AnJiaWebServer_V1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Newtonsoft.Json;
using System.Net.WebSockets;

using System.Data.Common;
using AnJiaWebServer_V1.JSON;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AnJiaWebServer_V1.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using IdentityModel;
using Microsoft.AspNetCore.SpaServices.Webpack;
using AnJiaWebServer_V1.Models;
using Microsoft.AspNetCore.Identity;

namespace AnJiaWebServer_V1
{
    public class Startup
    {


        bool macAvailable=false;
  

        JObject result;
   

        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        
        // This method gets called by the runtime. Use this method to add services to the container.
        //运行时调用此方法，使用这个方法添加服务
        //上下文注册依赖关系注入
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<AnJiaContext>(options =>
                options.UseMySql (Configuration.GetConnectionString("DefaultConnection")));// options.UserMySQL 指明了要使用MYSQL数据库 
   
            services.AddAuthentication( x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {

                o.IncludeErrorDetails = true;
                o.Audience = Constants.Audience;
                o.RequireHttpsMetadata = false;

                o.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType ="yuancong",
                    
                    RoleClaimType = JwtClaimTypes.Role,                 
                    ValidIssuer=Constants.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Constants.SecretKey))//这个Key是后台用来计算Signature 的


                    /***********************************TokenValidationParameters的参数默认值***********************************/
                    // RequireSignedTokens = true,
                    // SaveSigninToken = false,
                    // ValidateActor = false,
                    // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                    // ValidateAudience = true,
                    // ValidateIssuer = true, 
                    // ValidateIssuerSigningKey = false,
                    // 是否要求Token的Claims中必须包含Expires
                    // RequireExpirationTime = true,
                    // 允许的服务器时间偏移量
                    // ClockSkew = TimeSpan.FromSeconds(300),
                    // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比              
                };
            });


            services.AddMvc();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);
            loggerFactory.AddDebug(LogLevel.Debug);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();

            }
            app.UseAuthentication();
            app.UseWebSockets();

#if UseOptions
            #region UseWebSocketsOptions
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);
            #endregion
#endif
            
            #region AcceptWebSocket
            app.Use(async (context, next) =>
            {

                if (context.Request.Path == "/subserver/ws")//这里可以定义路径的格式
                {
    
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                        var buffer = new byte[1024 * 4];
                        WebSocketReceiveResult webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        string restring = Encoding.ASCII.GetString(buffer);

                        JObject jObject = (JObject)JsonConvert.DeserializeObject(restring);//序列化结果

                        //  string PiToken = jObject["PiToken"].ToString();
                        //验证PiToken
                        string subserverId = jObject["SubserverID"].ToString();

                        macAvailable = true;
                        ErrorRootobject error = new ErrorRootobject
                        {
                            error_code = "1001",
                            msg = "JSON format error"
                        };
                        string serial = JsonConvert.SerializeObject(error);

                        result = (JObject)JsonConvert.DeserializeObject(serial);

                        if (macAvailable)
                        {

                            var conn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection"));
                
                            conn.Open();
                            var command = conn.CreateCommand();
                            string query = "SELECT  Username"
                + " FROM UserToSubserver "
                + "WHERE SubserverID = '" + subserverId + "'";

                            command.CommandText = query;
                            DbDataReader BindedReader = await command.ExecuteReaderAsync();

                            if (BindedReader.HasRows)
                            {
                                //如果查询到被绑定
                                //我担心会查出一个MAC地址绑定了多个用户的情况
                                //后期可以设置让MAC地址为主键
                                BindedReader.Read();
                                string buser = BindedReader["Username"].ToString();
                                error.error_code = "1005";
                                error.msg = " This Subserver is owned by " + buser;

                                serial = JsonConvert.SerializeObject(error);
                                result = (JObject)JsonConvert.DeserializeObject(serial);
                                var buffer1 = Encoding.ASCII.GetBytes(serial);
                                //将
                                await webSocket.SendAsync(new ArraySegment<byte>(buffer1), webSocketReceiveResult.MessageType, webSocketReceiveResult.EndOfMessage, CancellationToken.None);
                                conn.Close();//关闭连接

                                //加入到字典中
                                WebsocketClient.AddToDictionary(subserverId,webSocket);
             

                            }
                            else
                            {
                                var buffer1= new byte[1024 * 4];
                                error.error_code = "1006";
                                error.msg = " SubserverID is not bound  ";

                                serial = JsonConvert.SerializeObject(error);
                                result = (JObject)JsonConvert.DeserializeObject(serial);
                                buffer1 = Encoding.ASCII.GetBytes(serial);
                                //将
                                int a = result.Count;
                                
                                await webSocket.SendAsync(new ArraySegment<byte>(buffer1), webSocketReceiveResult.MessageType, webSocketReceiveResult.EndOfMessage, CancellationToken.None);
                            }
                            conn.Close();

                        }
                        //启动监听
                        await ListenToSubserver(context, webSocket);

                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });
            #endregion
            app.UseFileServer();
  
            app.UseStaticFiles();



            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        #region ListenMothed
        private async Task ListenToSubserver(HttpContext context, WebSocket webSocket)
        {
            WebSocketReceiveResult result;
            var buffer = new byte[1024 * 4];
            try
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                string ass = Encoding.ASCII.GetString(buffer);//获得通信内容

                string reciMsg = ass.TrimEnd('\0');

                JObject msg = (JObject)JsonConvert.DeserializeObject(reciMsg);//将获得的字符串转换为JSON对象  
                //如果result不含值则循环 监听
                while (!result.CloseStatus.HasValue)
                {

                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch (Exception)
            {

                webSocket.Dispose();
                return;
            }
        
 
       
         //   string s = msg["PiMAC"].ToString();

            #region 解析Json包
            





            #endregion



            //将该子服务器的状态设置为掉线




        }
        #endregion


    }

    public static class WebsocketClient
    {

       static Dictionary<string, WebSocket> dictionary = new Dictionary<string, WebSocket>();

       static byte[] buffer1 = new byte[1024 * 4];

        public static void AddToDictionary(string mac,WebSocket webSocket)
        {
            if(dictionary.ContainsKey(mac))
            {
                dictionary.Remove(mac);
                dictionary.Add(mac, webSocket);
            }
            else
            {
                dictionary.Add(mac, webSocket);
            }
         
        }

        public static async Task<bool> SendToSubserverAsync(string subServerId, ControlMsgRootobject controlMsg)
        {
            string serial = JsonConvert.SerializeObject(controlMsg);
            buffer1 = Encoding.ASCII.GetBytes(serial);
            try
            {
                await dictionary[subServerId].SendAsync(new ArraySegment<byte>(buffer1), WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                //如果子服务器离线
                //将子服务器从字典中删除
                dictionary.Remove(subServerId);
                return false;


            }
            

        }



    }
}
