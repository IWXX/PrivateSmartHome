using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace FacialRecognizedDoorClient.Helpers
{
    public class GpioHelper
    {
        private GpioController gpioController;
        private GpioPin doorbellPin;
        private GpioPin doorLockPin;
        private GpioPin BTPairPin;
        private GpioPin BTLEDPin;
        private GpioPin NetLEDPin;

        public bool Initialize()
        {
            gpioController = GpioController.GetDefault();//获取Gpio控制器
            if (gpioController == null)
            {
                return false;
            }
            #region 门铃按钮初始化
            doorbellPin = InitializeInputGPIO(doorbellPin,GpioConstants.DoorbellPinID);
            #endregion

            #region 蓝牙按钮引脚初始化
         //   BTPairPin = InitializeInputGPIO(BTPairPin,GpioConstants.BTPairPinID);
            BTPairPin = gpioController.OpenPin(GpioConstants.BTPairPinID);
            if (BTPairPin == null)
            {
                return false;
            }

            BTPairPin.DebounceTimeout = TimeSpan.FromMilliseconds(25);

            if (BTPairPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
            {
                // Take advantage of built in pull-up resistors of Raspberry Pi 2 and DragonBoard 410c
                //利用树莓派内置的上拉电阻
                BTPairPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            }
            else
            {
                BTPairPin.SetDriveMode(GpioPinDriveMode.Input);
            }

            #endregion

            #region 关锁引脚初始化
           doorLockPin =  InitializeOutputGPIO(doorLockPin,GpioConstants.DoorLockPinID,GpioPinValue.High);
            #endregion

            #region 蓝牙标识灯引脚初始化
           // BTLEDPin = InitializeOutputGPIO(BTLEDPin,GpioConstants.BTLEDPinID,GpioPinValue.High);
            BTLEDPin = gpioController.OpenPin(GpioConstants.BTLEDPinID);
            if (BTLEDPin == null)
            {
                return false;
            }

            BTLEDPin.SetDriveMode(GpioPinDriveMode.Output);

            //初始化引脚为低电平
            BTLEDPin.Write(GpioPinValue.Low);
            #endregion

            #region 网络标识灯引脚初始化
            NetLEDPin= gpioController.OpenPin(GpioConstants.NetLEDPinID);
            if (NetLEDPin == null)
            {
                return false;
            }

            NetLEDPin.SetDriveMode(GpioPinDriveMode.Output);

            //初始化引脚为低电平
            NetLEDPin.Write(GpioPinValue.Low);
            #endregion

            return true;

        }


        /// <summary>
        /// 用于对输出型的GPIO进行初始化
        /// </summary>
        /// <param name="gpioPin"></param>
        /// <param name="PinID">Gpio引脚号</param>
        /// <param name="gpioPinValue">引脚初始电平值</param>
        /// <returns></returns>
        private GpioPin InitializeOutputGPIO(GpioPin gpioPin, int PinID, GpioPinValue gpioPinValue)
        {
            gpioPin = gpioController.OpenPin(GpioConstants.DoorLockPinID);
            if (gpioPin == null)
            {
                return null;
            }

            gpioPin.SetDriveMode(GpioPinDriveMode.Output);

            //初始化引脚为低电平
            gpioPin.Write(gpioPinValue);
            return gpioPin;
        }
        private GpioPin InitializeInputGPIO(GpioPin gpioPin, int PinID)
        {
            gpioPin = gpioController.OpenPin(PinID);
            if (gpioPin == null)
            {
                return null;
            }

            gpioPin.DebounceTimeout = TimeSpan.FromMilliseconds(25);

            if (gpioPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
            {
                // Take advantage of built in pull-up resistors of Raspberry Pi 2 and DragonBoard 410c
                //利用树莓派内置的上拉电阻
                gpioPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            }
            else
            {
                gpioPin.SetDriveMode(GpioPinDriveMode.Input);
            }
            return gpioPin;
        }
        public GpioPin GetDoorBellPin()
        {
            return doorbellPin;
        }
        public async void UnlockDoor()

        {
            // Unlock door
            doorLockPin.Write(GpioPinValue.Low);
            // Wait for specified length
            //等待特定的时长
            await Task.Delay(TimeSpan.FromSeconds(GpioConstants.DoorLockOpenDurationSeconds));
            // Lock door 关锁
            //输出高电平
            doorLockPin.Write(GpioPinValue.High);
            
            return;
        }
        public async Task<bool> BTButtonHoldCheck()
        {
            await Task.Delay(TimeSpan.FromSeconds(GpioConstants.BTButttonHoldDurationSeconds));//
            if (BTPairPin.Read() == GpioPinValue.Low)
            {
                return true;
            }
            else
            {
                return false;
            }
        
        }
        /// <summary>
        /// 配对模式，急闪
        /// </summary>
        public async void BTLEDBlinkFast(bool pairSuccess)
        {
            while (!pairSuccess)
            {
                BTLEDPin.Write(GpioPinValue.High);
                await Task.Delay(TimeSpan.FromSeconds(GpioConstants.BTDurationSeconds_Fast));
                BTLEDPin.Write(GpioPinValue.Low);
                await Task.Delay(TimeSpan.FromSeconds(GpioConstants.BTDurationSeconds_Fast));
            }

        }

        /// <summary>
        /// 配对成功 等待连接  慢闪
        /// </summary>
        public async void BTLEDBlinkSlow(bool connectSuccess)
        {
            while (!connectSuccess)
            {
                BTLEDPin.Write(GpioPinValue.High);
                await Task.Delay(TimeSpan.FromSeconds(GpioConstants.BTDurationSeconds_Slow));//
                BTLEDPin.Write(GpioPinValue.Low);
            }

        }

        /// <summary>
        /// 连接成功 常亮
        /// </summary>

        /// <summary>
        /// 连接成功 常亮
        /// </summary>
        public async Task BTLEDLight()
        {
            BTLEDPin.Write(GpioPinValue.High);
        }

        /// <summary>
        /// 连接断开 蓝牙关闭 熄灭
        /// </summary>
        public void BTLEDDark()
        {
            BTLEDPin.Write(GpioPinValue.Low);
        }

        /// <summary>
        /// 网络正常 常量
        /// </summary>
        public void NetLEDLight()
        {
            NetLEDPin.Write(GpioPinValue.High);
        }
        /// <summary>
        /// 网络未连接 熄灭
        /// </summary>
        public void NetLEDDark()
        {
            NetLEDPin.Write(GpioPinValue.Low);
        }
        /// <summary>
        /// 连上网但是没有连上云服务器
        /// </summary>
        public async Task NetLEDBlink()
        {
            NetLEDPin.Write(GpioPinValue.High);
            await Task.Delay(TimeSpan.FromSeconds(GpioConstants.BTDurationSeconds_Slow));//
            NetLEDPin.Write(GpioPinValue.Low);
        }
        public GpioPin GetBluetoothPairPin()
        {
            return BTPairPin;
        }
    }
}
