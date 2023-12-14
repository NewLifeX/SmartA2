using NewLife;
using NewLife.Data;
using NewLife.IoT.Controllers;
using NewLife.Log;
using NewLife.Net;
using SmartA2;

// 客户端，本地连接串口，远程连接服务端

internal class Program
{
    static ISerialPort _serial;
    static ISocketRemote _client;

    private static void Main(String[] args)
    {
        XTrace.UseConsole();

        var host = new A2();

        // 配置并打开串口COM1
        var serial = host.CreateSerial(Coms.COM1, 9600);
        serial.Received += Serial_Received;
        serial.Open();

        // 服务器地址，可保存在配置文件中，支持tcp/udp地址
        var server = "tcp://10.0.2.6:888";
        var uri = new NetUri(server);
        var client = uri.CreateRemote();
        client.Log = XTrace.Log;
        client.Received += OnReceiveSocket;
        client.Open();

        _serial = serial;
        _client = client;

        // 等待退出
        Console.ReadLine();
    }

    private static void Serial_Received(Object sender, ReceivedEventArgs e)
    {
        var pk = e.Packet;
        if (pk.Total == 0) return;

        var buf = pk.ReadBytes();

        // 发送串口数据到服务器
        var rs = new Packet(buf, 0, buf.Length);
        _client.Send(rs);
    }

    static void OnReceiveSocket(Object sender, ReceivedEventArgs e)
    {
        // 接收到服务器数据，转发到串口
        var pk = e.Packet;
        _serial.Write(pk.Data, pk.Offset, pk.Count);
    }
}