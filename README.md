# [PrivateSmartHome  私有智能家居系统]http://privatesmarthome.readthedocs.io/en/latest/get-started/index.html)

## 概述

这是是一个简易版的智能家居系统，可以实现使用手机APP对物联网设备的控制。没有使用知名的物联网协议，整个系统所使用的协议只有 TCP，HTTP，webSocket。
系统主要分一下几部分：
* 远程服务器

* 子服务器

* 手机APP

* 物联网设备

## 远程服务器

远程服务器应用是一个ASP.Net Core Mvc 项目，可以跨平台部署。目前也只测试在ubuntu server上部署成功，其他平台理论上可是可以的。

## 子服务器

子服务器应用是一个UWP应用，部署在 Windows10 IOT Core 上。推荐使用树莓派作为开发板。

## 物联网设备

物联网设备的基本要求是必须有蓝牙模块以及wifi模块。简单起见，推荐使用 Arduino开发板+ESP8266WIFI模块+蓝牙模块的方式。编写程序需要参照所给示例。
