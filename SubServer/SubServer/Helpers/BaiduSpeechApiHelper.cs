using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace FacialRecognizedDoorClient.Helpers
{
    class BaiduSpeechApiHelper
    {
        #region 实例变量
        private string accessToken;
        private string contentType;
        private MediaElement mediaElement;
        StorageFolder folder;
        #endregion
        public BaiduSpeechApiHelper(MediaElement media)
        {
            mediaElement = media;

        }

        #region PrivateMethod
        private async Task GetTts(string message)
        {
            string access_Token = await GetTokenAsync();
            string lan_setting = "zh";//必填	语言选择,目前只有中英文混合模式，填写固定值zh
            string ctp_setting = "1"; //必填 客户端类型选择，web端填写固定值1
            string spd_setting = "5";//选填	语速，取值0-9，默认为5中语速
            string pit_setting = "5"; //选填 音调，取值0 - 9，默认为5中语调
            string vol_setting = "5"; //选填 音量，取值0 - 15，默认为5中音量
            string per_setting = "3"; //选填 发音人选择, 0为普通女声，1为普通男生，3为情感合成 - 度逍遥，4为情感合成 - 度丫丫，默认为普通女声
            string tex_seting = message;//必填	合成的文本，使用UTF-8编码，请注意文本长度必须小于1024字节 

            List<KeyValuePair<String, String>> paralist = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("tex",tex_seting),
                new KeyValuePair<string, string>("lan",lan_setting),
                new KeyValuePair<string, string>("tok",access_Token),
                new KeyValuePair<string, string>("ctp",ctp_setting),
                new KeyValuePair<string, string>("cuid","cuid"),
                new KeyValuePair<string, string>("spd",spd_setting),
                new KeyValuePair<string, string>("pit",pit_setting),
                new KeyValuePair<string, string>("vol",vol_setting),
                new KeyValuePair<string, string>("per",per_setting)
            };

            IBuffer TtsResult = await GetTtsResultAsync(SpeechApiURL.BAIDU_TTS_URL, paralist);//获取mp3文件
            await PlayAudio(TtsResult);


        }

        private async Task PlayAudio(IBuffer result)//最终播放音频
        {
            folder = await KnownFolders.MusicLibrary.CreateFolderAsync("Greeting", CreationCollisionOption.OpenIfExists);//创建文件夹
            StorageFile storageFile = await folder.CreateFileAsync("语音文件.mp3", CreationCollisionOption.ReplaceExisting);//创建文件 再次创建居然没有权限
            await FileIO.WriteBufferAsync(storageFile, result);//从缓冲写入文件
            storageFile = await folder.GetFileAsync("语音文件.mp3");
            using (IRandomAccessStream readStream = await storageFile.OpenAsync(FileAccessMode.Read))
            {
                if (mediaElement != null)
                {
                    mediaElement.AutoPlay = true;
                    mediaElement.SetSource(readStream, "");
                    mediaElement.Play(); ;
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetTokenAsync()
        {

            string token_url = "https://aip.baidubce.com/oauth/2.0/token";
            string clientId = GeneralConstants.SpeechAPIKey;//apiKey
            string clientSecret = GeneralConstants.SpeechSecretKey;//secretKey

            HttpClient hc = new HttpClient();
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            };

            JObject result = await CreateHttpRequestAsync(token_url, paraList);
            //解析Json

            accessToken = result["access_token"].ToString();


            return accessToken;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task<JObject> CreateHttpRequestAsync(string url, List<KeyValuePair<String, String>> list)//创建http请求 获取Token
        {
            //注：尚未进行断网异常处理 
            JObject state = null;
            try
            {
                HttpClient hc = new HttpClient();

                using (var content = new HttpFormUrlEncodedContent(list))
                {
                    var response = await hc.PostAsync(new Uri(url), content);
                    string contentType = response.Content.Headers.ContentType.ToString();
                    var resdata = await response.Content.ReadAsStringAsync();
                    JObject result = (JObject)JsonConvert.DeserializeObject(resdata);
                    return result;
                }
            }
            catch
            {
                return state;
            }

        }
        private async Task<IBuffer> GetTtsResultAsync(string url, List<KeyValuePair<String, String>> list)// 获取文件
        {

            HttpClient hc = new HttpClient();
            using (var content = new HttpFormUrlEncodedContent(list))
            {
                var response = await hc.PostAsync(new Uri(url), content);
                IBuffer buffer = await response.Content.ReadAsBufferAsync();

                return buffer;
            }






        }
        private string GetPcMacAddress()
        {
            string mac = "123";

            return mac;
        }

        #endregion

        #region PublicMethod

        public async Task DingDong()
        {
            // folder = await KnownFolders.MusicLibrary.CreateFolderAsync("Greeting", CreationCollisionOption.OpenIfExists);//创建文件夹
            StorageFolder storageFolder = KnownFolders.MusicLibrary;//创建文件夹

            StorageFile storageFile = await storageFolder.GetFileAsync("1.mp3");
         //   await FileIO.WriteBufferAsync(storageFile, result);//从缓冲写入文件
     
            using (IRandomAccessStream readStream = await storageFile.OpenAsync(FileAccessMode.Read))
            {
                if (mediaElement != null)
                {
                    mediaElement.SetSource(readStream, "");
                    mediaElement.AutoPlay = true;
                    mediaElement.Play(); ;
                }
            }
        }

        public async Task DingDongnull()
        {
            StorageFolder storageFolder = KnownFolders.MusicLibrary;//创建文件夹

            StorageFile storageFile = await storageFolder.GetFileAsync("1.mp3");
   
            using (IRandomAccessStream readStream = await storageFile.OpenAsync(FileAccessMode.Read))
            {
                if (mediaElement != null)
                {
                    mediaElement.AutoPlay = true;
                    mediaElement.SetSource(readStream, "");
                    mediaElement.Play(); ;
                }
            }
        }
        public async Task GreetingToUser(string uinfo)//识别成功跟访客打招呼
        {
            string msg = "你好！" + uinfo + "请进！";
            await GetTts(msg);

        }

        public async Task Livecheck(string usname)//识别成功跟访客打招呼
        {
            string msg = "活体检测未通过，试图冒充"+usname;
            await GetTts(msg);

        }

        public async Task NullDevices()//识别成功跟访客打招呼
        {
            string msg = "当前没有锁具在线！";
            await GetTts(msg);

        }

        public async Task DevicesAdd()//识别成功跟访客打招呼
        {
            string msg = "锁具已上线";
            await GetTts(msg);

        }
        public async Task PhoneAdd()//识别成功跟访客打招呼
        {
            string msg = "手机已上线！";
            await GetTts(msg);

        }

        public async Task PhoneReduce()//识别成功跟访客打招呼
        {
            string msg = "手机下线！";
            await GetTts(msg);

        }
        public async Task DevicesReduce()//识别成功跟访客打招呼
        {
            string msg = "锁具下线";
            await GetTts(msg);
        }
            public async Task GreeCantIdentify()//无法识别
        {
            string msg = SpeechContants.CanNotIdentify;
            await GetTts(msg);
        }

        public async Task GreeGroupIsNull()//白名单为空
        {
            string msg = SpeechContants.GroupIsNull;
            await GetTts(msg);
        }

        public async Task CantFindFace()//没有发现人脸
        {
            string msg = SpeechContants.CanNotFindFace;
            await GetTts(msg);
        }

        #endregion
    }
}
