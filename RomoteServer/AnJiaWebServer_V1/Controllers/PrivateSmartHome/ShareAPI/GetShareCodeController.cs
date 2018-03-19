using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using AnJiaWebServer_V1.JSON;
using Newtonsoft.Json;
using QRCoder;
using System.DrawingCore;
using static QRCoder.Base64QRCode;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome.AccountAPI
{
    /// <summary>
    /// 接口说明
    /// ReturnCode: 获取分享码
    /// 
    /// 分享码考虑只针对某一个设备进行分享
    /// 注意：
    /// 
    /// 返回：经base64编码的
    /// </summary>
    [Produces("application/json")]
    [Route("PrivateSmartHome/api/V1/[controller]")]
    public class GetShareCodeController : Controller
    {
      //  [Authorize]
        [HttpGet("{username}/{deviceID}")]
        public JObject GetShareCode(string username,string deviceID)
        {
            JObject result;
            #region 验证
            //可以考虑将DeviceID和用户名放到数据库中 查询 ID是否存在以及
            //确认deviceID确实属于该用户
            //代码暂略

            #endregion

            if (!Check())//检测注销状态
            {
                //没有对应项则说明已经注销登录
                result = null;
                return result;
            }



            var shareCode = JwtManager.GetJwtManager().GenerateShareCode(username,deviceID);//生成分享码
            SharecodeJson sharecode = new SharecodeJson()
            {
                ReturnCode="0000",
                QRShareCode = CreateQrcode(shareCode)
            };
            var code= JsonConvert.SerializeObject(sharecode);
              result = (JObject)JsonConvert.DeserializeObject(code);
            return result;
        }
        private string CreateQrcode(string shareCode)//创建Base64格式的二维码
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(shareCode, QRCodeGenerator.ECCLevel.H);//生成的二维码内容为分享码
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);

            Color darkColor = Color.DarkBlue;
            Color lightColor = Color.White;
            bool drawQuietZones = true;
            ImageType imageType = ImageType.Jpeg;

            string qrCodeImageAsBase64 =  qrCode.GetGraphic(2, darkColor, lightColor, drawQuietZones, imageType);
            
            return qrCodeImageAsBase64;
        }
        private  ClaimsPrincipal GetToken()
        {
            StringValues JwtBearer;
            Request.Headers.TryGetValue("Authorization", out JwtBearer);
            string JwtBearerString = JwtBearer.ToString();
            string[] sArray = JwtBearerString.Split(' ');
            string acToken = sArray[1];//分离出Token
 
            var claimsPrincipal = JwtManager.GetPrincipal(acToken);//对Token
            return claimsPrincipal;
        }

        private bool Check()//检查Redis表中是否有对应
        {


            var actoken = JwtManager.GetRequestTokenString(Request);//获取Token
            //下面是 

            if (actoken == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

    }

}
