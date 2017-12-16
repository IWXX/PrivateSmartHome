using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacialRecognizedDoorClient
{
    public static class GeneralConstants
    {
        // This variable should be set to false for devices(设备), unlike the Raspberry Pi, that have GPU support
        public const bool DisableLiveCameraFeed = false;
        #region 百度云API_Key
        public const string FaceAPIKey = "Z2E7xx944efVaRncBaLnGrip";//百度云人脸识别APIKey
       // public const string FaceAPIKey = "ZRMSNuZLtBKHbfMzWICKGhDD";//百度云人脸识别APIKey
        public const string FaceSecretKey = "0Xp8fqsg4LEmD793DjwLF0ETGxQePqUs";//百度云人脸识别SecretKey
      //  public const string FaceSecretKey = "AUjpOSRKfdGNZka9VasPGosv38myxnEm";//百度云人脸识别SecretKey

        public const string SpeechAPIKey = "yzNCMsf3xNGo0moDSccVatyV";//百度云语音合成APIKey
        public const string SpeechSecretKey = "lNSWCTYFmZ9P28xTnaGRVe9Nk8ClLaIk";// 百度云语音合成SecretKey

        public const string WhiteListFolderName = "Facial Recognition Door Whitelist";
        public const string VolFolderName = "Speech file";

        public const string ACCESS_TOKEN_URL = "https://aip.baidubce.com/oauth/2.0/token";
        #endregion
         
        #region Cortana图像资源URI
        public const string FACELOOK_GOOD_URI = "ms-appx:///Image/yes.gif";
        public const string FACELOOK_Normal_URI = "ms-appx:///Image/normal.gif";
        public const string FACELOOK_BAD_URI = "ms-appx:///Image/coquestion.gif";
        public const string FACELOOK_WARNING_URI = "ms-appx:///Image/warning.png";
        public const string FACELOOK_QUESTION_URI = "ms-appx:///Image/question.png";
        #endregion

        //public const string websocketAPI = "ws://localhost:19217/subserver/ws";//连接地址
        public const string websocketAPI = "ws://www.writebug.site/subserver/ws";
        public const string websocketToken = "ae20ea77-ad6b-4029-b820-acfe0954e8ac";
        public const string subServerID = "ae20ea77-ad6b-4029-b820-acfe0954e8ac";

    }

    public static class BlueToothConstants
    {
        // The Chat Server's custom service Uuid: 34B1CF4D-1069-4AD6-89B6-E161D79BE4D8
        public static readonly Guid RfcommDeviceServiceUuid = Guid.Parse("34B1CF4D-1069-4AD6-89B6-E161D79BE4D9");

        // The Id of the Service Name SDP attribute
        public const UInt16 SdpServiceNameAttributeId = 0x100;

        // The SDP Type of the Service Name SDP attribute.
        // The first byte in the SDP Attribute encodes the SDP Attribute Type as follows :
        //    -  the Attribute Type size in the least significant 3 bits,
        //    -  the SDP Attribute Type value in the most significant 5 bits.
        public const byte SdpServiceNameAttributeType = (4 << 3) | 5;

        // The value of the Service Name SDP attribute
        public const string SdpServiceName = "Bluetooth Rfcomm Chat Service";

        public const string BluetoothClientName= "SubServerMaster";
    }

    public static class TcpSocket
    {
        public const int LISTENPORT = 9999;//监听端口
        public const int POSTPORT = 8885;//发送端口

    }

    public static class SpeechContants
    {
        public const string CanNotIdentify = "抱歉！您不在白名单列表中！";
        public const string CanNotFindFace = "抱歉！没有检测到人脸";
        public const string GroupIsNull = "抱歉！白名单列表中未添加任何人！";
        public const string FaceNotLive = "活体检测未通过。";

        public const string BTPairSuccess = "pair succeeded";//
        public const string BTPairing = "Waiting for pairing";//
        public const string BTConnecting = "Waiting for connection";//等待蓝牙连接
        public const string BTConnected = "connection succeeded";//连接成功
        public const string BTDisConn = "disconnection";//连接断开

        public const string WIFIConnected = "WIFI connection success";//路由器连接成功
        public const string WIFINameWrong = "WiFi  do not exist";//找不到指定wifi
        public const string WIFIPwdWrong = "Password error";//wifi密码错误

        public const string NetConnectedWrong = "Unable to connect to the Internet";//无法连接到互联网





    }

    #region 百度云API_Adress

    public static class FaceApiURL
    {
        public const string FACE_SEARCH_IDENTIFY_URL = "https://aip.baidubce.com/rest/2.0/face/v2/identify";
    }

    public static class SpeechApiURL
    {
        public const string BAIDU_TTS_URL = "http://tsn.baidu.com/text2audio";
    }
    #endregion


    public static class GpioConstants
    {
        // The GPIO pin that the doorbell button is attached to
        //门铃按钮连接的GPIO引脚 
        public const int DoorbellPinID = 5;
        //门锁链接的GPIO引脚
        // The GPIO pin that the door lock is attached to
        public const int DoorLockPinID = 6;

        public const int BTPairPinID = 26;//

        public const int NetLEDPinID = 12;//用于提示已经成功接入wifi

        public const int BTLEDPinID = 13;//用于提示蓝牙已经启动

   

        public const int BTButttonHoldDurationSeconds = 3;
        public const int BTDurationSeconds_Slow = 2;
        public const double BTDurationSeconds_Fast = 1;



        // The amount of time in seconds that the door will remain unlocked for
        //保持开锁的时间（秒）
        public const int DoorLockOpenDurationSeconds = 5;
    }
}
