using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace FacialRecognizedDoorClient.Helpers
{
    public class WebcamHelper
    {
        public MediaCapture mediaCapture;

        private bool initialized = false;

        public async Task InitializeCameraAsync()
        {
            if (mediaCapture == null)
            {
                // 尝试获取连接摄像头
                var cameraDevice = await FindCameraDevice();

                if (cameraDevice == null)
                {
                    // No camera found, report the error and break out of initialization
                    //找不到摄像头则报告错误并跳出初始化
                    Debug.WriteLine("No camera found!");
                    initialized = false;
                    return;
                }

                // Creates MediaCapture initialization settings with foudnd webcam device
                //用发现的webcam设备创建MediaCapture 初始化 设置对象
                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync(settings);
                initialized = true;
            }
        }
        /// <summary>
        /// 异步地寻找并返回发现的第一个camera设备
        /// 如果没有找到设备则返回 null
        /// </summary>
        /// <returns></returns>
        private static async Task<DeviceInformation> FindCameraDevice()
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);


            if (allVideoDevices.Count > 0)
            {
                // If there is a device attached, return the first device found
                return allVideoDevices[0];
            }
            else
            {
                // Else, return null
                return null;
            }
        }

        public async Task StartCameraPreview()
        {
            try
            {
                await mediaCapture.StartPreviewAsync();
            }
            catch
            {
                initialized = false;
                Debug.WriteLine("Failed to start camera preview stream");

            }
        }

        public async Task StopCameraPreview()
        {
            try
            {
                await mediaCapture.StopPreviewAsync();
            }
            catch
            {
                Debug.WriteLine("Failed to stop camera preview stream");
            }
        }

        public async Task<StorageFile> CapturePhoto()
        {
            //在本地磁盘创建存储文件
            string fileName = GenerateNewFileName() + ".jpg";
            CreationCollisionOption collisionOption = CreationCollisionOption.GenerateUniqueName;
            StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, collisionOption);

            // Captures and stores new Jpeg image file
            await mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), file);//将图片捕获到存储文件

            // Return image file
            return file;
        }

        private string GenerateNewFileName()
        {
            return DateTime.UtcNow.ToString("yyyy.MMM.dd HH-mm-ss") + " Facial Recognition Door";
        }

        public bool IsInitialized()
        {
            return initialized;
        }
    }
}
