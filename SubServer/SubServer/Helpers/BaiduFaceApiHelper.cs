using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;

namespace FacialRecognizedDoorClient.Helpers
{
    class BaiduFaceApiHelper
    {
        #region 实例变量
        private string accessToken;
        JObject webStatus;//它表示网络异常情况
        JObject tokenStatus;
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>

        public BaiduFaceApiHelper()
        {
            
        }


        #region Method

        public async Task<JObject> FaceIdentifyRequestAsync(StorageFile photoFolder)//人脸识别
        {
           
            string access_Token = await GetTokenAsync();//获取access token
            if (access_Token == null)
            {
                webStatus = null;
                return webStatus;//网络异常空对象
            }
            else if (accessToken == "notoken")
            {
                tokenStatus = new JObject();
                tokenStatus.Add("error","1");
                return tokenStatus;
            }

            else
            {
                string image = await ImageToStringAsync(photoFolder);//将图片进行bash64编码后所得的字符串
                JObject result = null;//请求结果
                string groupId = "AnJiaSmartLock_Test_1";

                List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("access_token", access_Token),
                new KeyValuePair<string, string>("group_id", groupId),
                new KeyValuePair<string, string>("images", image),
                new KeyValuePair<string, string>("ext_fields", "faceliveness")
            };
                result = await CreateHttpRequestAsync(FaceApiURL.FACE_SEARCH_IDENTIFY_URL, paraList);
                //接下来解析JSon获取有用的信息
                return result;
            }
                

        }

        /// <summary>
        /// 用于创建不同类型的HTTP请求
        /// 请求方式：POST
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="list">请求参数列表</param>
        /// <returns></returns>
        private async Task<JObject> CreateHttpRequestAsync(string url, List<KeyValuePair<String, String>> list)//创建http请求
        {

            try
            {
                HttpClient hc = new HttpClient();

                using (var content = new HttpFormUrlEncodedContent(list))
                {
                    var response = await hc.PostAsync(new Uri(url), content);
                    var resdata = await response.Content.ReadAsStringAsync();
                    JObject result = (JObject)JsonConvert.DeserializeObject(resdata);
                    
                    return result;
                }
            }
            catch
            {
                webStatus = null;
                return webStatus ;
            }

        }

        private async Task<string> ImageToStringAsync(StorageFolder photoFolder)//用于将图片进行Bash64编码
        {
            string imagesString = "";
            await Task.Run(async () =>
            {

                var files = await photoFolder.GetFilesAsync();

                foreach (var file in files)
                {
                    if (File.Exists(file.Path))
                    {
                        var image = File.ReadAllBytes(file.Path);
                        imagesString = Convert.ToBase64String(image);
                    }
                }



            });
            return imagesString;

        }

        private async Task<string> ImageToStringAsync(StorageFile photoFile)
        {
            string imagesString = "";
            await Task.Run(() =>
            {
                if (File.Exists(photoFile.Path))
                {
                    var image = File.ReadAllBytes(photoFile.Path);
                    imagesString = Convert.ToBase64String(image);
                }

            });
            return imagesString;

        }

        /// <summary>
        /// 获取accessToken
        /// </summary>
        /// <returns>string</returns>
        private async Task<string> GetTokenAsync()
        {

            string token_url = "https://aip.baidubce.com/oauth/2.0/token";
            string clientId = GeneralConstants.FaceAPIKey;//apiKey
            string clientSecret = GeneralConstants.FaceSecretKey;//secretKey

            HttpClient hc = new HttpClient();
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            };

            JObject result = await CreateHttpRequestAsync(token_url, paraList);

            //解析Json
            if (result == null)
            {
                accessToken = null;
            }
            else
            {
                try
                {
                    accessToken = result["access_token"].ToString();
                }
                catch (Exception)
                {
                    accessToken = "notoken";
                    
                }

     
            }
   


            return accessToken;

        }

        #endregion
    }
}
