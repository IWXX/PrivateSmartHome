
============
设备开发示例
============


此节提供了开发的简单示例，旨在了解如何进行简单的开发。希望可以起到举一反三的作用


远程控制小灯（例1）
>>>>>>>>>>>>>

概述
:::::::::

在这里描述一下这个作品是干什么的

材料清单
:::::::::

1. aaaa

2. bbbbb

3. ccccc


接线图
:::::::::

.. image:: ../_static/

源代码
:::::::::


.. code-block :: c

    /* LED闪烁进阶

    让13引脚连接的LED闪烁起来而不使用delay()函数。这样就意味着其他的代码可以不受LED闪烁的干扰，在“同一时间”(译者注：其实应该是几乎同一时间)运行。

    电路这样搭:
    * LED连接到13引脚和GND。
    * 注：绝大多数Arduino已经在13引脚连接了板载LED。因而这个例子可能不需要多余LED也能看到效果。

    代码是公开的。
    */

    // 定义一个不会改变的整型常量。这里用来定义引脚号码:
    const int ledPin =  13;      // LED连接的引脚

    // 声明并定义可变的变量 :
    int ledState = LOW;             // LED的状态值

    // 一般来说，用 "unsigned long"类型的变量来存储时间值比较好。因为如果用int类型“装不下”这么大的数字。
    unsigned long previousMillis = 0;        // 存储上次LED状态被改变的时间

    // 又定义了一个常量 :
    const long interval = 1000;           // LED状态应该被改变的间隔时间(单位毫秒)

    void setup() {
    // 将数字引脚定义为输出模式：
    pinMode(ledPin, OUTPUT);
    }

    void loop() {
    //这里写你想要不断运行的代码。

    // 检查看看LED是否到了应该打开或关闭的时间; 就是说，检查下现在时间离开记录的时间是否超过了要求LED状态改变的间隔时间。
        unsigned long currentMillis = millis();

    if (currentMillis - previousMillis >= interval) {
        // 更新时间标记
        previousMillis = currentMillis;

        // 如果LED关闭则打开它，如果LED打开则关闭它:
        if (ledState == LOW) {
        ledState = HIGH;
        } else {
        ledState = LOW;
        }

        // 用以下代码设置LED状态:
        digitalWrite(ledPin, ledState);
    }
    }



示例2
>>>>>>>>>>>>

材料清单
:::::::::

1. aaaa

2. bbbbb

3. ccccc


接线图
:::::::::

.. image:: ../_static/

源代码
:::::::::


.. code-block :: c