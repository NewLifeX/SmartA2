using SmartA2;

var led = new OutputPort { FileName = "/dev/led" };

while (true)
{
    led.Write(false);

    Thread.Sleep(500);

    led.Write(true);

    Thread.Sleep(500);
}
