using System.IO.Ports;
using NewLife;
using NewLife.IoT.Controllers;
using NewLife.IoT.Protocols;
using NewLife.Log;
using NewLife.Model;
using NewLife.Threading;
using SmartA2;

namespace FullTest;

internal class TestWorker(IBoard board) : IHostedService
{
    private TimerX _timer;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        //_ = Task.Run(DoWork);
        _timer = new TimerX(DoWork, null, 1000, 60000) { Async = true };

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task DoWork(Object state)
    {
        var a2 = board as A2;

        XTrace.WriteLine("输入输出测试");
        {
            var key = a2.Key;
            key.KeyDown += (s, e) => XTrace.WriteLine("按键按下");
            key.KeyUp += (s, e) => XTrace.WriteLine("按键弹起");

            // 闪烁灯光、蜂鸣器若干次
            var led = a2.Led;
            var led2 = a2.Led2;
            var buzzer = a2.Buzzer;
            for (var i = 0; i < 5; i++)
            {
                led.Write(true);
                led2.Write(true);
                buzzer.Write(true);
                await Task.Delay(500);

                led.Write(false);
                led2.Write(false);
                buzzer.Write(false);
                await Task.Delay(500);
            }
        }

        XTrace.WriteLine("串口测试");
        {
            // 列出所有串口
            var ports = SerialPort.GetPortNames();
            XTrace.WriteLine("串口列表[{0}]：", ports.Length);
            foreach (var item in ports)
            {
                XTrace.WriteLine(item);
            }

            // 打开串口并发送数据，然后关闭串口
            using var port = a2.CreateSerial(Coms.COM1, 115200);
            port.Open();

            var buf = "Hello NewLife".GetBytes();
            port.Write(buf, 0, buf.Length);
        }

        XTrace.WriteLine("温湿度传感器测试");
        {
            // 打开Modbus读取数据
            using var modbus = a2.CreateModbus(Coms.COM2, 9600) as Modbus;
#if DEBUG
            //modbus.Log = XTrace.Log;
#endif
            var sensor = new TemperatureSensor { Modbus = modbus };

            XTrace.WriteLine("地址：{0:X2}", sensor.ReadAddress());
            XTrace.WriteLine("标识：{0}", sensor.ReadID().ToHex());

            var (hard, soft) = sensor.ReadVersion();
            XTrace.WriteLine("硬件：{0}", hard);
            XTrace.WriteLine("软件：{0}", soft);

            var (temp, humi) = sensor.ReadValues();
            XTrace.WriteLine("温度：{0:n1}", temp);
            XTrace.WriteLine("湿度：{0:p1}", humi);
        }

        XTrace.WriteLine("继电器测试");
        {
            // 打开Modbus读取数据
            using var modbus = a2.CreateModbus(Coms.COM3, 9600) as Modbus;
#if DEBUG
            modbus.Log = XTrace.Log;
#endif
            var relay = new RelayControl { Modbus = modbus, Host = 0xFF };
            XTrace.WriteLine("地址：{0:X2}", relay.ReadAddress());
            //XTrace.WriteLine("波特率：{0:X2}", relay.ReadBaudrate());

            XTrace.WriteLine("点位：{0}", relay.ReadAll(4).Join());

            var flag = true;
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    //relay.Invert(j);
                    relay.Write(j, flag);
                }
                flag = !flag;

                await Task.Delay(500);
            }
        }
    }
}
