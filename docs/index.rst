
====
开始
====

该文档主要是将本项目从部署到原理进行讲解，作为人生中第一次写文档的练手作品



概述
==================
PrivateSmartHome是一个简易版的物联网家居系统。你可以自己DIY物联网设备，自己进行部署，
实现私有化的同时也可以对设备进行共享，让被授权的其他用户也能操作你的设备。
作为一个智能家居系统它提供了设备远程控制以及传感器数据采集整理的服务。此外系统还整合了人脸识别功能，可以此实现人脸识别开锁功能。



工具及材料
=========
要部署PrivateSmartPhone你需要：

* 一台装有UbuntuServer的云服务器
* 一台装有Windows10 IOT Core 的树莓派(Rasperbery Pi)


=======
API说明
=======

.. toctree::
    :maxdepth: 1

    ESP32 DevKitC <get-started-devkitc>
    ESP-WROVER-KIT <get-started-wrover-kit>
    ESP32-PICO-KIT <get-started-pico-kit>