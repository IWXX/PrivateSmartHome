using FacialRecognizedDoorClient.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.WiFi;
using Windows.Networking.Sockets;
using IoTCoreDefaultApp.Utils;
using FacialRecognizedDoorClient.Presenters;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Security.Credentials;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.Storage.Streams;
using System.IO;
using FacialRecognizedDoorClient.JSON;

using Newtonsoft.Json;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Input.Preview.Injection;
using Windows.Networking.Connectivity;
// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace FacialRecognizedDoorClient
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region websocket变量声明
        Uri websocketserver = new Uri(GeneralConstants.websocketAPI);
        private MessageWebSocket messageWebSocket;
        private DataWriter messageWriter;
        private bool busy;
        private string acToken;
        private string subserID;
        JObject ControlMSG;//
        #endregion

        #region 蓝牙相关变量声明
        private StreamSocket socket;
        private DataWriter writer;
        private RfcommServiceProvider rfcommProvider;
        private StreamSocketListener socketListener;
        private DeviceWatcher deviceWatcher = null;
        public InputInjector inputInjector = null;
        private DispatcherTimer timer1;
        private bool ispaired;
        private bool btPairSuccess = false;
        private bool btConnectSuccess = false;
        #endregion

        #region WIFI相关变量的声明
        private WiFiAdapter firstAdapter;
        private WiFiNetworkReport wiFiNetworkReport=null;
        private string WifiSSid = null;
        private string WifiPwd = null;
        Error_code error_Code = new Error_code//定义错误码
        {
            error_code = "",
            msg = "未预料到的错误"

        };
        WiFiInfo wiFiInfo = new WiFiInfo
        {
            Ssid=" ",
            Password=" "
        };
        public string ErrorMsgToJson(string errorCode, string msg)
        {
            error_Code.error_code = errorCode;
            error_Code.msg = "The subserver has been connected to the specified WIFI ";
            string errormsg = JsonConvert.SerializeObject(error_Code);
            return msg;
        }
        #endregion

        #region 人脸识别变量声明
        private WebcamHelper webcam;
        private BaiduFaceApiHelper faceClient;
        private BaiduSpeechApiHelper speechClient;
        // GPIO Related Variables:
        private GpioHelper gpioHelper;
        private bool gpioAvailable;
        private bool doorbellJustPressed = false;
        #endregion

        #region TCP连接模块变量声明

        public List<StreamSocket> ClientSockets;
        public List<StreamSocket> PhoneSockets;
   
        public Dictionary<string,StreamSocket> DeviceSockets;
        StreamSocketListener Listener;
        DispatcherTimer UpdateClients;//定时维护锁具端列表
        DispatcherTimer UpdatePhone;//定时维护手机端列表
        #endregion

        #region UI时钟相关变量
        private CoreDispatcher MainPageDispatcher;
        private DispatcherTimer timer;
        #endregion

        #region CortanaUI相关变量
        DispatcherTimer dispatcherTimer;//该计时器用户切换Cortana表情
        DispatcherTimer dispatcherTimer_Textblock;
        DateTimeOffset startTime;
        DateTimeOffset lastTime;
        DateTimeOffset stopTime;
        int timesTicked = 1;
        int timesToTick = 10;
        int flag = 0;

        

        enum FaceChangeSwitch
        {
            Normal = 0,
            Good = 1,
            Bad = 2
        }
        #endregion

        #region UI时钟相关属性
        public CoreDispatcher UIThreadDispatcher
        {
            get
            {
                return MainPageDispatcher;
            }

            set
            {
                MainPageDispatcher = value;
            }
        }
        #endregion

        private SpeechHelper speechHelper;//用于设备联网之前的

        #region 初始化
        public MainPage()
        {
            InitializeComponent();
             
            NavigationCacheMode = NavigationCacheMode.Enabled;

            MainPageDispatcher = Window.Current.Dispatcher;

            InitializeGpio();
            faceClient = new BaiduFaceApiHelper();

            this.WIFIAdapterInitAsync();//连接互联网

            #region 时钟相关操作

            timer = new DispatcherTimer();//new一个定时器对象
            timer.Tick += Timer_Tick;//给定时器对象绑定事件方法
            timer.Interval = TimeSpan.FromSeconds(1);//
            Loaded += async (sender, e) =>
            {
                await MainPageDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    // UpdateBoardInfo();
                    // UpdateNetworkInfo();
                    UpdateDateTime();
                    // UpdateConnectedDevices();

                    timer.Start();
                });
            };
            this.Unloaded += (sender, e) =>
            {
                timer.Stop();
            };
            this.TcpserverAsync();
            #endregion

            #region 初始化手机列表更新器

            //UpdatePhone = new DispatcherTimer();

            //UpdatePhone.Tick += null;

            //UpdatePhone.Interval = TimeSpan.FromSeconds(5);//每五秒触发一次

            #endregion

            #region 初始化设备列表更新器

            //UpdateClients = new DispatcherTimer();

            //UpdateClients.Tick += null;

            //UpdateClients.Interval = TimeSpan.FromSeconds(5);

            #endregion

            #region 连接websocket
            
            #endregion

        }
        #endregion

        #region websocket相关

        private async void OnConnectAsync(object sender, RoutedEventArgs e)
        {
         //   await gpioHelper.NetLEDBlink();
            string SubserverId = await CreatAndReadSubserverIDFile();//在子服务器连接云服务器时即初始化subserverId 
            await ConnectAsync();
            if (messageWebSocket == null)
            {
                
            }
            else
            {
                messageWriter = new DataWriter(messageWebSocket.OutputStream);
                await SendAsync(GeneralConstants.websocketToken, GeneralConstants.subServerID);
            }

        }

        /// <summary>
        /// 创建存储Token的文件
        /// </summary>
        /// <param name="Tokenstring"></param>
        private async Task<string> CreatAndReadTokenFile(string Tokenstring)
        {
            
            StorageFolder TokenFolder = KnownFolders.MusicLibrary;
            CreationCollisionOption collisionOption = CreationCollisionOption.FailIfExists;//如果文件已经存在则引发异常
            try
            {
                StorageFile SubserverFile = await TokenFolder.CreateFileAsync("TokenFile.txt", collisionOption);
                string subServerID = Guid.NewGuid().ToString();
                await FileIO.WriteTextAsync(SubserverFile, subServerID);
                return subServerID;
            }
            catch (Exception)
            {
                StorageFile SubserverFile = await TokenFolder.GetFileAsync("TokenFile.txt");
                string subServerID = await FileIO.ReadTextAsync(SubserverFile);
                return subServerID;
            }

        }

/// <summary>
/// 创建subserverID文件并返回subserverID
/// </summary>
/// <returns>subserverID字符串</returns>
        private async Task<string> CreatAndReadSubserverIDFile()
        {
            StorageFolder SubserverFolder = KnownFolders.MusicLibrary;
            CreationCollisionOption collisionOption = CreationCollisionOption.FailIfExists;//如果文件已经存在则引发异常
            try
            {
                StorageFile SubserverFile = await SubserverFolder.CreateFileAsync("SubserverID.txt", collisionOption);

                string subServerID = Guid.NewGuid().ToString();
               await FileIO.WriteTextAsync(SubserverFile,subServerID);
                return subServerID;

            }
            catch (Exception)
            {
            
                StorageFile SubserverFile = await SubserverFolder.GetFileAsync("SubserverID.txt");
                string subServerID = await FileIO.ReadTextAsync(SubserverFile);
                return subServerID;
            }
         
        }
        /// <summary>
        /// 读取wifi信息文件，没有则创建
        /// </summary>
        /// <returns></returns>
        private async Task<string> CreatAndReadWiFiInfoFile()
        {
            StorageFolder SubserverFolder = KnownFolders.MusicLibrary;
            CreationCollisionOption collisionOption = CreationCollisionOption.FailIfExists;//如果文件已经存在则引发异常
            try
            {
                StorageFile SubserverFile = await SubserverFolder.CreateFileAsync("WifiInfo.json", collisionOption);

                string wfInfo = JsonConvert.SerializeObject(wiFiInfo);

                await FileIO.WriteTextAsync(SubserverFile, wfInfo);
                return "IniSuccess";

            }
            catch (Exception)
            {

                StorageFile SubserverFile = await SubserverFolder.GetFileAsync("WifiInfo.json");
                string wfInfo = await FileIO.ReadTextAsync(SubserverFile);
                return wfInfo;
            }
        }

        /// <summary>
        /// 将wifi信息写入文件
        /// </summary>
        /// <param name="ssid">wifi名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private async Task<bool> WriteToWiFiInfoFile(string ssid,string password)
        {
            StorageFolder SubserverFolder = KnownFolders.MusicLibrary;
            CreationCollisionOption collisionOption = CreationCollisionOption.FailIfExists;//如果文件已经存在则引发异常
            try
            {

                StorageFile SubserverFile = await SubserverFolder.GetFileAsync("WifiInfo.json");
                string wifiInfo1 =  await FileIO.ReadTextAsync(SubserverFile);
                JObject jObject = (JObject)JsonConvert.DeserializeObject(wifiInfo1);
                jObject["Ssid"] = ssid;
                jObject["Password"] = password;

                wifiInfo1=jObject.ToString();


                await FileIO.WriteTextAsync(SubserverFile, wifiInfo1);
                return true;

            }
            catch (Exception)
            {
                StorageFile SubserverFile = await SubserverFolder.CreateFileAsync("WifiInfo.json", collisionOption);
                string wifiInfo1 = JsonConvert.SerializeObject(wiFiInfo);
                JObject jObject = (JObject)JsonConvert.DeserializeObject(wifiInfo1);
                jObject["Ssid"] = ssid;
                jObject["Password"] = password;

                wifiInfo1 = jObject.ToString();
                await FileIO.WriteTextAsync(SubserverFile, wifiInfo1);
      
                return true;
            }


        }


        /// <summary>
        /// 用于连接之后子服务器向云服务器发送信息 我觉得可以使用HTTP
        /// </summary>
        /// <param name="Jsonstring"></param>
        private async void SendMsgToServer(string Jsonstring)
        {

        }

        private async Task ConnectAsync()
        {

            messageWebSocket = new MessageWebSocket();
            messageWebSocket.Control.MessageType = SocketMessageType.Utf8;
            messageWebSocket.MessageReceived += MessageReceived;


            try
            {
                await messageWebSocket.ConnectAsync(websocketserver);

            }
            catch (Exception) // For debugging
            {
                gpioHelper.NetLEDDark();
                //出现异常则自动重连
                WebcamStatus.Text = "离线，正在重连";
                await AutomaticReconnectionAsync();
                return;
            }


        }

        private void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            // Dispatch the event to the UI thread so we can update UI.
            var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    using (DataReader reader = args.GetDataReader())
                    {
                        reader.UnicodeEncoding = UnicodeEncoding.Utf8;

                        try
                        {
                            //远程开锁方式
                            string read = reader.ReadString(reader.UnconsumedBufferLength);
                            //此处获取到需要树莓派控制的具体设备IP
                            ControlMSG = (JObject)JsonConvert.DeserializeObject(read);
                            string DeviceIP = ControlMSG["DeviceIP"].ToString();

                            await UnlockDoorAsync(DeviceIP);
                          
                            WebcamStatus.Text = ControlMSG["DeviceIP"].ToString();

                        }
                        catch (Exception )
                        {

                        }
                    }
                }
                catch (Exception)
                {
                 
                   // WebcamStatus.Text = "服务器异常";
                   // await gpioHelper.NetLEDBlink();//表示没有连接上服务器
                    await AutomaticReconnectionAsync();
                }
               
            });
        }
        /// <summary>
        /// 发送websocket消息
        /// </summary>
        /// <returns></returns>
        async Task SendAsync(string token,string subserid)
        {
            //    string message = "{\"PiMAC\":\"00-23-5A-15-99-42\"}";//这里要改为序列化之后的JSON字符串


            WebSocketConn webSocketConn = new WebSocketConn
            {
                AccessToken = token,
                SubserverID = subserid
            };
            string message = JsonConvert.SerializeObject(webSocketConn);
         


            messageWriter.WriteString(message);
              // Send the data as one complete message.
            await messageWriter.StoreAsync();

        }


        #endregion

        #region wifi相关
        private NetworkPresenter networkPresenter = new NetworkPresenter();

        /// <summary>
        /// 初始化wifi适配器
        /// </summary>
        private async void WIFIAdapterInitAsync()
        {

           string info=await CreatAndReadWiFiInfoFile();

            JObject Jinfo = (JObject)JsonConvert.DeserializeObject(info);


           if( await ConnectToWiFi(Jinfo["Ssid"].ToString(), Jinfo["Password"].ToString()))
            {

            }
            else
            {

            }
     
        }

        /// <summary>
        /// 连接到指定的wifi网络
        /// </summary>
        /// <param name="wifiname">WiFi网络名</param>
        /// <param name="wifipassword">wifi密码</param>
        private async Task<bool> ConnectToWiFi(string wifiname, string wifipassword)
        {
            WiFiConnectionResult result = null;//连接wifi的结果
            int ssidCount = 0;
            var access = await WiFiAdapter.RequestAccessAsync();
            DeviceInformationCollection wiFiAdapters;
            if (access != WiFiAccessStatus.Allowed)
            {
                return false;//如果WiFi适配器不可用
            }
            else
            {
                wiFiAdapters = await DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());//查找wifi适配器
                if (wiFiAdapters.Count >= 1)
                {
                    firstAdapter = await WiFiAdapter.FromIdAsync(wiFiAdapters[0].Id);
                    await firstAdapter.ScanAsync();//开始扫描
                    wiFiNetworkReport = firstAdapter.NetworkReport;//获取扫描结果
                }
                else
                {
                    return false;
                }
            }

            WiFiReconnectionKind reconnectionKind = WiFiReconnectionKind.Automatic;//自动重连

            var credential = new PasswordCredential();
            if (!string.IsNullOrEmpty(wifipassword))
            {
                credential.Password = wifipassword;//只有证书的password部分会被提交
            }
            foreach (var network in wiFiNetworkReport.AvailableNetworks)
            {
                string ssid = network.Ssid;
                ssidCount++;
                if (ssid == wifiname)
                {
                    result = await firstAdapter.ConnectAsync(network, reconnectionKind, credential);

                    if (result.ConnectionStatus == WiFiConnectionStatus.Success)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            await speechHelper.Read(SpeechContants.WIFIConnected);//提示密码错误
                        });
                        return true;
                    }
                    else
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            await speechHelper.Read(SpeechContants.WIFIPwdWrong);//提示密码错误
                        });

                        return false; //wifi连接不成功
                    }
                }
                
            }
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await speechHelper.Read(SpeechContants.WIFINameWrong);//提示名称不存在
            });
            return false;

        }


        #endregion  

        #region 蓝牙相关

        #region 1.进行蓝牙配对

        /// <summary>
        /// 开始扫描
        /// </summary>
        private void StartWatcher()
        {
            btPairSuccess = false;
            gpioHelper.BTLEDBlinkFast(btPairSuccess);

      
            string[] requestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
            deviceWatcher = DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
                                                requestedProperties,
                                                DeviceInformationKind.AssociationEndpoint);
            // Hook up handlers for the watcher events before starting the watcher
            deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>((watcher, deviceInfo) =>
            {

                // Make sure device name isn't blank
                if (deviceInfo.Name == BlueToothConstants.BluetoothClientName)
                {
                    RfcommChatDeviceDisplay rfcommChatDeviceDisplay = new RfcommChatDeviceDisplay(deviceInfo);
                    PairPhone(rfcommChatDeviceDisplay);
                }

            });

            deviceWatcher.Updated += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                // Make sure device name isn't blank

            });

            deviceWatcher.Start();

        }

        /// <summary>
        /// 停止扫描
        /// </summary>
        private void StopWatcher()
        {
            gpioHelper.BTLEDDark();
            if (null != deviceWatcher)
            {
                if ((DeviceWatcherStatus.Started == deviceWatcher.Status ||
                     DeviceWatcherStatus.EnumerationCompleted == deviceWatcher.Status))
                {
                    deviceWatcher.Stop();
                }
                deviceWatcher = null;
            }
        }

        /// <summary>
        /// 开始配对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PairPhone(RfcommChatDeviceDisplay rfcommChatD)
        {
            DevicePairingKinds ceremoniesSelected = DevicePairingKinds.ConfirmOnly;

            DevicePairingProtectionLevel protectionLevel = DevicePairingProtectionLevel.None;//使用加密的方式可能使手机端的响应速度变慢
            
            DeviceInformationCustomPairing customPairing = rfcommChatD.DeviceInformation.Pairing.Custom;


            customPairing.PairingRequested += PairingRequestedHandler;
            ispaired = rfcommChatD.DeviceInformation.Pairing.IsPaired;
            if (ispaired)
            {
                await rfcommChatD.DeviceInformation.Pairing.UnpairAsync();
            }
            await AutoConfirmStartAsync();//开始计时
            
            DevicePairingResult result = await customPairing.PairAsync(ceremoniesSelected, protectionLevel);
         
            await AutoConfirmStopAsync();
            btPairSuccess = true;
            gpioHelper.BTLEDBlinkFast(btPairSuccess);//配对成功 快闪熄灭

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await speechHelper.Read(SpeechContants.BTPairSuccess);//提示配对成功
            });

             await InitializeRfcommServer();//初始化蓝牙服务器

            gpioHelper.BTLEDBlinkSlow(btConnectSuccess);//慢闪等待连接

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await speechHelper.Read(SpeechContants.BTConnecting);//提示等待连接
            });
         
            customPairing.PairingRequested -= PairingRequestedHandler;
           // StopWatcher();

        }

        /// <summary>
        /// 生成一个定时器，每十秒进行一次键盘的模拟输入
        /// </summary>
        /// <returns></returns>
        public async Task AutoConfirmStartAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                timer1 = new DispatcherTimer();
                timer1.Tick += AutoConfirm_Tick;
                timer1.Interval = TimeSpan.FromSeconds(10);
                timer1.Start();
            });

            
        }
        /// <summary>
        /// 用于自动确认弹出的蓝牙配对提示框的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AutoConfirm_Tick(object sender,object e)
        {
            InputInjector injector = null;
            injector = InputInjector.TryCreate();

            InjectedInputKeyboardInfo Left = new InjectedInputKeyboardInfo
            {
                VirtualKey = (ushort)37
                
            };
            InjectedInputKeyboardInfo EnterDown = new InjectedInputKeyboardInfo
            {
                ScanCode = (ushort)28,
              

            };
            InjectedInputKeyboardInfo EnterUP= new InjectedInputKeyboardInfo
            {
                ScanCode = (ushort)28,
                KeyOptions = InjectedInputKeyOptions.KeyUp
            };
            InjectedInputKeyboardInfo[] infos =
            {
                Left,
                EnterDown,
                EnterUP
            };

            injector.InjectKeyboardInput(infos);

        }
        public async Task AutoConfirmStopAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                timer1.Stop();
                timer1.Tick -= AutoConfirm_Tick;
            });

        }
        private void PairingRequestedHandler(  DeviceInformationCustomPairing sender,DevicePairingRequestedEventArgs args)
        {
            switch (args.PairingKind)
            {
                case DevicePairingKinds.ConfirmOnly:
                    // Windows itself will pop the confirmation dialog as part of "consent" if this is running on Desktop or Mobile
                    // If this is an App for 'Windows IoT Core' where there is no Windows Consent UX, you may want to provide your own confirmation.
                    args.Accept();
                    break;

                case DevicePairingKinds.DisplayPin:
                    // We just show the PIN on this side. The ceremony is actually completed when the user enters the PIN
                    // on the target device. We automatically accept here since we can't really "cancel" the operation
                    // from this side.
                    args.Accept();

                    // No need for a deferral since we don't need any decision from the user

                    break;

                case DevicePairingKinds.ProvidePin:
                    // A PIN may be shown on the target device and the user needs to enter the matching PIN on 
                    // this Windows device. Get a deferral so we can perform the async request to the user.
                    var collectPinDeferral = args.GetDeferral();


                    break;

                case DevicePairingKinds.ConfirmPinMatch:
                    // We show the PIN here and the user responds with whether the PIN matches what they see
                    // on the target device. Response comes back and we set it on the PinComparePairingRequestedData
                    // then complete the deferral.
                    var displayMessageDeferral = args.GetDeferral();


                    break;
            }
        }
        #endregion

        #region 2.等待手机端发起连接
        private async Task InitializeRfcommServer()
        {
            try
            {
                rfcommProvider = await RfcommServiceProvider.CreateAsync(RfcommServiceId.FromUuid(BlueToothConstants.RfcommDeviceServiceUuid));
            }

            //尝试捕获异常 HRESULT_FROM_WIN32(ERROR_DEVICE_NOT_AVAILABLE) 即，蓝牙设备不可用异常
            catch (Exception ex) when ((uint)ex.HResult == 0x800710DF)
            {
                Debug.Write("Make sure your Bluetooth Radio is on: " + ex.Message);
                return;
            }

            // Create a listener for this service and start listening
            socketListener = new StreamSocketListener();
            socketListener.ConnectionReceived += OnBlutoothConnectionReceived;
            var rfcomm = rfcommProvider.ServiceId.AsString();

            await socketListener.BindServiceNameAsync(rfcommProvider.ServiceId.AsString(),
                SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

            // Set the SDP attributes and start Bluetooth advertising
            InitializeServiceSdpAttributes(rfcommProvider);



            try
            {
                rfcommProvider.StartAdvertising(socketListener, true);
            }
            catch (Exception e)
            {
                Debug.Write(e);
                return;
            }

            Debug.Write("Listening for incoming connections");
        }

        private async void OnBlutoothConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {

            if (Listener == null)
            {

            }
            else
            {
                Listener.Dispose();//保证了在下一次服务启动之前不会有第二个连接
                Listener = null;
            }

            Debug.WriteLine("Connection received");

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BlutoothStatus.Text = "已连接";

            });

            try
            {
                socket = args.Socket;
            }
            catch (Exception e)
            {
                Debug.Write(e);
                Disconnect();
                return;
            }

            // Note - this is the supported way to get a Bluetooth device from a given socket
            var remoteDevice = await BluetoothDevice.FromHostNameAsync(socket.Information.RemoteHostName);

            writer = new DataWriter(socket.OutputStream);

            var reader = new DataReader(socket.InputStream);

            bool remoteDisconnection = false;

            Debug.Write("Connected to Client: " + remoteDevice.Name);


            string subserverID = await CreatAndReadSubserverIDFile();
            Blutooth_SendMessage($"subserverID:"+subserverID);
            btConnectSuccess = true;
            gpioHelper.BTLEDBlinkSlow(btConnectSuccess);//慢闪等待连接

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {

        
                await gpioHelper.BTLEDLight();//连接成功，常亮
                
                await speechHelper.Read(SpeechContants.BTConnected);//提示蓝牙已经连接
               
            });
  

            //连接成功后等待消息进入
            while (true)
            {
                try
                {
             
                    //基于我们自订的协议，第一个整数是消息的大

                    uint readLength = await reader.LoadAsync(sizeof(uint));//四个字节

                    // Check if the size of the data is expected (otherwise the remote has already terminated the connection)
                    if (readLength < sizeof(uint))
                    {
                        remoteDisconnection = true;
                        gpioHelper.BTLEDDark();
                        break;
                    }

                    var currentLength = reader.ReadUInt32();

                    // Load the rest of the message since you already know the length of the data expected.
                    readLength = await reader.LoadAsync(currentLength);

                    // Check if the size of the data is expected (otherwise the remote has already terminated the connection)
                    if (readLength < currentLength)
                    {
                        remoteDisconnection = true;
                        break;
                    }
                    string message = reader.ReadString(currentLength);
                    try
                    {
                        JObject wifimsg = (JObject)JsonConvert.DeserializeObject(message);
                     
                        WifiSSid = wifimsg["WiFiName"].ToString();
                        WifiPwd = wifimsg["Password"].ToString();


                        //wifi信息获取到了 现在开始连接wifi

                       bool connectResult= await ConnectToWiFi(WifiSSid, WifiPwd);

                        await WriteToWiFiInfoFile(WifiSSid,WifiPwd);
                        if (connectResult)
                        {


                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                await speechHelper.Read(SpeechContants.WIFIConnected);//提示wifi连接成功
                            });


                            gpioHelper.NetLEDLight();//点亮指示灯
                            string msg = ErrorMsgToJson("3000", "The subserver has been connected to the specified WIFI");
                            Blutooth_SendMessage(msg);


                        }
                        else
                        {
                            string msg = ErrorMsgToJson("3001", "password is wrong");
                            Blutooth_SendMessage(msg);
                        }


                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            wifiNameBlock.Text = WifiSSid;
                            wifiPasswodBlock.Text = WifiPwd;

                        });
                    }
                    catch (Exception)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            wifiNameBlock.Text = "消息接收异常";
                        });
                     
                    }


                    Debug.Write("Received: " + message);
                }
                // Catch exception HRESULT_FROM_WIN32(ERROR_OPERATION_ABORTED).
                catch (Exception ex) when ((uint)ex.HResult == 0x800703E3)
                {
                    gpioHelper.BTLEDDark();
                    Debug.Write("Client Disconnected Successfully");
                    break;
                }
            }

            reader.DetachStream();
            if (remoteDisconnection)
            {
                Disconnect();
                Debug.Write("Client disconnected");
            }
        }

        private void Disconnect()
        {
            if (writer != null)
            {
                writer.DetachStream();
                writer = null;
            }

            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
            Debug.Write("Disconected");
        }

        private void InitializeServiceSdpAttributes(RfcommServiceProvider rfcommProvider)
        {
            var sdpWriter = new DataWriter();

            // Write the Service Name Attribute.
            sdpWriter.WriteByte(BlueToothConstants.SdpServiceNameAttributeType);

            // The length of the UTF-8 encoded Service Name SDP Attribute.
            sdpWriter.WriteByte((byte)BlueToothConstants.SdpServiceName.Length);

            // The UTF-8 encoded Service Name value.
            sdpWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            sdpWriter.WriteString(BlueToothConstants.SdpServiceName);

            // Set the SDP Attribute on the RFCOMM Service Provider.
            rfcommProvider.SdpRawAttributes.Add(BlueToothConstants.SdpServiceNameAttributeId, sdpWriter.DetachBuffer());
        }

        //发送信息
        private async void Blutooth_SendMessage(string message)
        {
            // Make sure that the connection is still up and there is a message to send
            if (socket != null)
            {
                writer.WriteInt32((int)message.Length);
                writer.WriteString(message);

                await writer.StoreAsync();
            }
            else
            {
                Debug.Write("No clients connected, please wait for a client to connect before attempting to send a message");
            }
        }
        #endregion

        #endregion

        #region 时钟相关方法
        private void Timer_Tick(object sender, object e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            // Using DateTime.Now is simpler, but the time zone is cached. So, we use a native method insead.、
            //
            NativeTimeMethods.GetLocalTime(out SYSTEMTIME localTime);

            DateTime t = localTime.ToDateTime();
            CurrentTime.Text = t.ToString();
        }
        #endregion

        #region 加载事件

        /// <summary>
        /// 语音模块加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeechMediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (speechClient == null)
            {
                speechClient = new BaiduSpeechApiHelper(SpeechMediaElement);
                speechHelper = new SpeechHelper(SpeechMediaElement);
            }
            else
            {
                // Prevents media element from re-greeting visitor
                SpeechMediaElement.AutoPlay = false;

            }
        }  


        /// <summary>
        /// 相机模块加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WebcamFeed_Loaded(object sender, RoutedEventArgs e)
        {
            //放弃启动预览，因为树莓派的性能原因，画面总是撕裂
            if (webcam == null || !webcam.IsInitialized())
            {
                // Initialize Webcam Helper
                webcam = new WebcamHelper();
                await webcam.InitializeCameraAsync();
            }
        }

        #endregion

        #region 开锁相关方法

        /// <summary>
        /// 点击界面上虚拟门铃触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DoorbellButton_Click(object sender, RoutedEventArgs e)
        {

            if (!doorbellJustPressed)
            {
                doorbellJustPressed = true;
                await DoorbellPressed();
                // AnalysingVisitorGrid.Visibility = Visibility.Collapsed;
            }

        }

        /// <summary>
        /// 点击物理按钮或者虚拟按钮时触发
        /// </summary>
        private async Task DoorbellPressed()
        {
            //隐藏表情图
            faceLookImagePanel.Visibility = Visibility.Collapsed;
            // 显示进度条
            AnalysingVisitorGrid.Visibility = Visibility.Visible;
            await speechClient.DingDong();
            dispatcherTimer = new DispatcherTimer();//生成一个计时器
            
            JObject recognizedVisitors;//识别结果的Json对象

            // 确认摄像头就绪且人脸识别初始化成功
            if (webcam.IsInitialized())
            {
                // 截取一帧图片存入一个临时文件
                StorageFile image = await webcam.CapturePhoto();


                // 验证访客是否咋在白名单
 

                    recognizedVisitors = await faceClient.FaceIdentifyRequestAsync(image);

                    if(recognizedVisitors==null)//说明断网
                    {
                        NetProblem();
                    }
                else
                {

                    bool inlist = false;
                    try
                    {
                        int num = Convert.ToInt32(recognizedVisitors["result_num"].ToString());
                        inlist = true;
                    }
                    catch (Exception)
                    {
                        inlist = false;

                    }

                    //  statusBlock.Text = recognizedVisitors.ToString();
                    if (inlist)//在当前图片中检测到人脸且Group不为空时执行
                    {
                        double scores = 0.01;
                        double livescores = 2.0;

                        JToken users = recognizedVisitors["result"];
                        JToken faceliveness = recognizedVisitors["ext_info"];

                        scores = Convert.ToDouble(users[0]["scores"][0].ToString());

                        livescores = Convert.ToDouble(faceliveness["faceliveness"].ToString());
                        //  livescores = Convert.ToDouble();
                        if (scores > 80.0)//说明该人脸的匹配度达标
                        {
                            if (livescores > 0.4494)//活体检测
                            {
                                string UserInfo;
                                UserInfo = users[0]["user_info"].ToString();

                                ComeInBlock.Foreground = new SolidColorBrush(Colors.Black);

                                ComeInBlock.Text = "请进！";

                                DetailsBlock.Text = "访客为： " + UserInfo;


                                //隐藏进度条
                                AnalysingVisitorGrid.Visibility = Visibility.Collapsed;
                                //显示表情表情图
                                faceLookImagePanel.Visibility = Visibility.Visible;
                                ChangeFaceMethod(FaceChangeSwitch.Good);//切换到Good表情
                                RestoreFace();
                                await speechClient.GreetingToUser(UserInfo);

                                //开锁方式更改为使用GPIO输出高电平
                                UnlockDoorByGpio();

                            }
                            else
                            {
                                string UserInfo;
                                int count = 2;
                                UserInfo = users[0]["user_info"].ToString();


                                ComeInBlock.Foreground = new SolidColorBrush(Colors.Red);

                                ComeInBlock.Text = "警告！";

                                string reason = "活体检测未通过!\n试图冒充" + UserInfo + "\n" + count + "次后通知家主！";

                                DetailsBlock.Text = reason;

                                //隐藏进度条
                                AnalysingVisitorGrid.Visibility = Visibility.Collapsed;
                                //显示表情表情图
                                faceLookImage.Source = new BitmapImage(new Uri(GeneralConstants.FACELOOK_WARNING_URI, UriKind.RelativeOrAbsolute));

                                faceLookImagePanel.Visibility = Visibility.Visible;

                                RestoreFace();//恢复就绪表情
                                await speechClient.Livecheck(UserInfo);

                            }

                            // 
                        }
                        else//人脸特征匹配度不达标
                        {
                            ComeInBlock.Foreground = new SolidColorBrush(Colors.Black);
                            ComeInBlock.Text = "抱歉！";
                            string reason = "您似乎不在白名单";
                            DetailsBlock.Text = reason;
                            //隐藏进度条
                            AnalysingVisitorGrid.Visibility = Visibility.Collapsed;
                            //显示表情表情图
                            faceLookImagePanel.Visibility = Visibility.Visible;
                            ChangeFaceMethod(FaceChangeSwitch.Bad);//切换到Bad表情
                            RestoreFace();//恢复就绪表情


                            await speechClient.GreeCantIdentify();

                        }

                    }
                    else //1.图片中没有人脸(face not found), error_code= 216402，2.Group为空(no user in group), error_code= 216618
                    {
                        ComeInBlock.Foreground = new SolidColorBrush(Colors.Black);
                        ComeInBlock.Text = "抱歉！";
                        int errorCode = Convert.ToInt32(recognizedVisitors["error_code"].ToString());//错误码
                        string reason = "";//出错原因

                        switch (errorCode)//根据错误代码显示错误原因
                        {
                            case 216402:
                                {
                                    //隐藏进度条
                                    AnalysingVisitorGrid.Visibility = Visibility.Collapsed;
                                    reason = "未检测到人脸";
                                    faceLookImage.Source = new BitmapImage(new Uri(GeneralConstants.FACELOOK_QUESTION_URI, UriKind.RelativeOrAbsolute));
                                    faceLookImagePanel.Visibility = Visibility.Visible;
                                    RestoreFace();//恢复就绪表情
                                    await speechClient.CantFindFace();
                                }
                                break;
                            case 216618:
                                {
                                    //隐藏进度条
                                    AnalysingVisitorGrid.Visibility = Visibility.Collapsed;
                                    reason = "白名单中无用户";
                                    faceLookImage.Source = new BitmapImage(new Uri(GeneralConstants.FACELOOK_QUESTION_URI, UriKind.RelativeOrAbsolute));
                                    faceLookImagePanel.Visibility = Visibility.Visible;
                                    RestoreFace();//恢复就绪表情
                                    await speechClient.GreeGroupIsNull();
                                }
                                break;

                            default:
                                //隐藏进度条
                                AnalysingVisitorGrid.Visibility = Visibility.Collapsed;
                                reason = "出现未知错误\n请联系管理员";
                                faceLookImage.Source = new BitmapImage(new Uri(GeneralConstants.FACELOOK_WARNING_URI, UriKind.RelativeOrAbsolute));
                                faceLookImagePanel.Visibility = Visibility.Visible;
                                break;
                        }

                        DetailsBlock.Text = reason;

                    }
                }




            }
            else//如果摄像头没有成功初始化
            {
                WebcamStatus.Text = "初始化失败";
            }

            doorbellJustPressed = false;

        }

        private void UnlockDoorByGpio()
        {
            // Greet visitor
            //   await speech.Read(SpeechContants.GeneralGreetigMessage(visitorName));

            if (gpioAvailable)
            {
                // Unlock door for specified ammount of time
                gpioHelper.UnlockDoor();
            }
        }

        /// <summary>
        /// 物理按钮按下时触发
        /// </summary>
        private async void DoorBellPressed(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (!doorbellJustPressed)
            {
                // Checks to see if even was triggered from a press or release of button
                if (args.Edge == GpioPinEdge.FallingEdge)
                {
                    //Doorbell was just pressed
                    doorbellJustPressed = true;

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await DoorbellPressed();// 如果您是在辅助线程上并且要在用户界面线程上安排工作

                    });


                }
            }
        }

        /// <summary>
        /// Unlocks door and greets visitor
        /// </summary>
        private async Task UnlockDoorAsync(string DeviceIP)
        {
            // Greet visitor
            //   await speech.Read(SpeechContants.GeneralGreetigMessage(visitorName));
            try
            {
                if (DeviceSockets.Count != 0)
                {
                    
                    await MsgSenderAsync( DeviceIP,"unlock");
                }
                else
                {
                    await speechClient.NullDevices();
                }
            }
            catch (Exception)
            {
                await speechClient.DevicesReduce();
                DeviceSockets.Remove(DeviceIP);
            }
           

            //if (gpioAvailable)
            //{
            //    // Unlock door for specified ammount of time
            //    gpioHelper.UnlockDoor();
            //}
        }


        /// <summary>
        /// 首次启动时初始化Gpio接口
        /// </summary>
        public void InitializeGpio()
        {
            try
            {
                gpioHelper = new GpioHelper();

                gpioAvailable = gpioHelper.Initialize();
              //  GpioStatusBlock.Text = "Gpio初始化成功";
            }
            catch
            {
                gpioAvailable = false;
              //  GpioStatusBlock.Text = "Gpio初始化失败";
            }
            // If initialization was successfull, attach doorbell pressed event handler
            if (gpioAvailable)
            {
           //     gpioHelper.GetDoorBellPin().ValueChanged += DoorBellPressed;//检测引脚变化启动人脸识别
                gpioHelper.GetBluetoothPairPin().ValueChanged+= BTPairPressedAsync;//检测引脚变化
            }
        }

        private async void BTPairPressedAsync(GpioPin sender, GpioPinValueChangedEventArgs args)
        {

            bool isHoldDown = await gpioHelper.BTButtonHoldCheck();
            if (isHoldDown)
            {   //调用蓝牙配对
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await speechHelper.Read(SpeechContants.BTPairing);//提示正在配对

                });


                StartWatcher();
            }

        }

        #endregion

        #region 异常处理

        /// <summary>
        /// 云服务器重连程序
        /// </summary>
        private async Task AutomaticReconnectionAsync()
        {
            bool offline = true;
            while (offline)
            {
                messageWebSocket = new MessageWebSocket();
                messageWebSocket.Control.MessageType = SocketMessageType.Utf8;
                messageWebSocket.MessageReceived += MessageReceived;
                Uri websocketserver = new Uri(GeneralConstants.websocketAPI);

                try
                {
                    await messageWebSocket.ConnectAsync(websocketserver);
                    gpioHelper.NetLEDLight();
                    //连接之后没有发送消息
                    offline = false;
                    WebcamStatus.Text = "正常";
                    messageWriter = new DataWriter(messageWebSocket.OutputStream);
                    await SendAsync(GeneralConstants.websocketToken, GeneralConstants.subServerID);
                    return;
                }
                catch (Exception ex) // For debugging
                {
                    WebcamStatus.Text = "连接服务器失败，请检查网络";
                    messageWebSocket.Dispose();
                    messageWebSocket = null;
               
                }



            }  
        }

        private void NetProblem()
        {
           // DoorbellButton.Visibility = Visibility.Collapsed;
            wifiicon.Icon = new SymbolIcon((Symbol.ZeroBars));
            wifiicon.Label = "无信号";
            DetailsBlock.Text = "网络异常\n请检查网络连接";

            //开启一个计时器进行定时Ping百度，ping不通
        }

        //设备异常处理
        private void DeviceProblem()
        {
            //如果摄像头出现故障，那么在界面提示摄像头故障

        }
        #endregion  

        #region 网络相关

        #region TcpServer相关
        private async void SocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {

            #region 客户端分类

            var firstreader = new DataReader(args.Socket.InputStream);

            while (true)
            {
                //等待数据进来  
                var sizeFieldCount = await firstreader.LoadAsync(6);//4位四位的读

                string checkString = firstreader.ReadString(sizeFieldCount);

                if (checkString == "islock")
                {

                    firstreader.UnicodeEncoding = UnicodeEncoding.Utf8;
                    string read = firstreader.ReadString(firstreader.UnconsumedBufferLength);

                    string remoteIP = args.Socket.Information.RemoteAddress.ToString();


                    if (DeviceSockets.ContainsKey(remoteIP))
                    {
                        DeviceSockets.Remove(remoteIP);
                        DeviceSockets.Add(remoteIP, args.Socket);//添加进设备字典
                    }
                    else
                    {
                        DeviceSockets.Add(remoteIP, args.Socket);//添加进设备字典
                    }
                    

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await speechClient.DevicesAdd();//提示锁具上线
                    });
                    break;
                }

            }
            #endregion

         }


            #region 消息发送器
            /// <summary>
            /// 消息发送器
            /// </summary>
            /// <param name="streamSocket">目标socket</param>
            /// <param name="msg">发送的信息</param>
            /// <returns></returns>
            public async Task MsgSenderAsync(string DeviceIP,string msg)
        {
            StreamSocket streamSocket = DeviceSockets[DeviceIP];

            Stream streamOut = streamSocket.OutputStream.AsStreamForWrite();

            StreamWriter writer = new StreamWriter(streamOut);

            await writer.WriteLineAsync(msg);

            await writer.FlushAsync();
        }


        #endregion

        /// <summary>
        /// TCP服务器
        /// </summary>
        /// <returns></returns>
        private async void TcpserverAsync()
        {
            Listener = new StreamSocketListener();

            ClientSockets = new List<StreamSocket>();

            PhoneSockets = new List<StreamSocket>();

            DeviceSockets = new Dictionary<string, StreamSocket>();

            Listener = new StreamSocketListener()
            {
                Control = { KeepAlive = false }
            };

            Listener.ConnectionReceived += SocketListener_ConnectionReceived;  //新连接接入时的事件  

            await Listener.BindServiceNameAsync(TcpSocket.LISTENPORT.ToString());//IP为本机IP地址，监听端口为9999

        }

        #endregion

        #region Ethernet相关
        private void SetupEthernet()
        {
            var ethernetProfile = NetworkPresenter.GetDirectConnectionName();

            if (ethernetProfile == null)
            {
                //NoneFoundText.Visibility = Visibility.Visible;
                //DirectConnectionStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                //NoneFoundText.Visibility = Visibility.Collapsed;
                // DirectConnectionStackPanel.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #endregion

        #region 传感器相关

        #region 光照传感器

        #endregion

        #region 人体红外传感器

        #endregion

        #region 干簧管

        #endregion

        #endregion

        #region 表情切换

        public void RestoreFace()//切换回normal
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer_Textblock = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;//将dispatcherTimer_Tick绑定到事件
            dispatcherTimer_Textblock.Tick += DispatcherTimer_TextBlock_Tick;//
            dispatcherTimer_Textblock.Interval = new TimeSpan(0, 0, 4);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1200);
            dispatcherTimer.Start();//启动计时器1
            dispatcherTimer_Textblock.Start();//启动计时器2

        }
        void DispatcherTimer_Tick(object sender, object e)//每1.2秒触发此事件
        {
            ChangeFaceMethod(FaceChangeSwitch.Normal);//切换回Normal
            dispatcherTimer.Stop();//关闭计时器
        }

        private void ChangeFaceMethod(Enum a)
        {

            switch (Convert.ToInt32(a))
            {
                case 0: faceLookImage.Source = new BitmapImage(new Uri(GeneralConstants.FACELOOK_Normal_URI, UriKind.RelativeOrAbsolute)); break;
                case 1: faceLookImage.Source = new BitmapImage(new Uri(GeneralConstants.FACELOOK_GOOD_URI, UriKind.RelativeOrAbsolute)); break;
                case 2: faceLookImage.Source = new BitmapImage(new Uri(GeneralConstants.FACELOOK_BAD_URI, UriKind.RelativeOrAbsolute)); break;
                default:
                    faceLookImage.Source = new BitmapImage(new Uri(GeneralConstants.FACELOOK_Normal_URI, UriKind.RelativeOrAbsolute));
                    break;
            }

        }
        /// <summary>
        /// 每1.2秒刷新文本框 使状态与表情同步变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DispatcherTimer_TextBlock_Tick(object sender, object e)//每1.2秒触发此事件
        {
            ComeInBlock.Text = "";
            DetailsBlock.Text = "就绪";
            dispatcherTimer_Textblock.Stop();//关闭计时器
        }



        #endregion

        #region 测试代码


        #endregion

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //  await CreatAndReadWiFiInfoFile();
            await WriteToWiFiInfoFile("test1","1234sdfdsf");
        }
    }
    public class RfcommChatDeviceDisplay : INotifyPropertyChanged
    {
        private DeviceInformation deviceInfo;

        public RfcommChatDeviceDisplay(DeviceInformation deviceInfoIn)
        {
            deviceInfo = deviceInfoIn;

        }

        public DeviceInformationKind Kind
        {
            get
            {
                return deviceInfo.Kind;
            }
        }

        public string Id
        {
            get
            {
                return deviceInfo.Id;
            }
        }

        public string Name
        {
            get
            {
                return deviceInfo.Name;
            }
        }

        public BitmapImage GlyphBitmapImage
        {
            get;
            private set;
        }

        public bool CanPair
        {
            get
            {
                return deviceInfo.Pairing.CanPair;
            }
        }

        public bool IsPaired
        {
            get
            {
                return deviceInfo.Pairing.IsPaired;
            }
        }

        public IReadOnlyDictionary<string, object> Properties
        {
            get
            {
                return deviceInfo.Properties;
            }
        }

        public DeviceInformation DeviceInformation
        {
            get
            {
                return deviceInfo;
            }

            private set
            {
                deviceInfo = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

  
}
