using NewLife.IoT.Controllers;

var led = new FileOutputPort("/dev/led");

while (true)
{
    // 灭
    led.Write(false);

    Thread.Sleep(500);

    // 亮
    led.Write(true);

    Thread.Sleep(500);
}
