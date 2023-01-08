using System.IO.Ports;
using NewLife;
using NewLife.IoT.Protocols;
using NewLife.Serial.Protocols;

namespace SmartA2;

/// <summary>A2硬件工厂</summary>
public class A2
{
    #region 属性
    /// <summary>SYS LED 指示灯</summary>
    public OutputPort Led { get; } = new OutputPort { FileName = "/dev/led" };

    /// <summary>SYS LED 指示灯</summary>
    public OutputPort Buzzer { get; } = new OutputPort { FileName = "/dev/buzzer" };

    /// <summary>FUN按键</summary>
    public InputPort Key { get; } = new InputPort { FileName = "/dev/key" };

    /// <summary>USB电源</summary>
    public OutputPort UsbPower { get; } = new OutputPort { FileName = "/dev/usbpwr" };

    /// <summary>看门狗</summary>
    public Watchdog Watchdog { get; } = new Watchdog { FileName = "/dev/watchdog" };

    /// <summary>串口名</summary>
    public String[] ComNames = new[] { "/dev/ttyAMA1", "/dev/ttyAMA0", "/dev/ttyAMA2", "/dev/ttyAMA3" };
    #endregion

    #region 串口
    /// <summary>创建串口</summary>
    /// <param name="com"></param>
    /// <param name="baudrate"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public SerialPort CreateSerial(Int32 com, Int32 baudrate = 9600)
    {
        if (com <= 0 || com > 4) throw new ArgumentOutOfRangeException(nameof(com), $"无效串口COM{com}，支持COM1/COM2/COM3/COM4");

        return new SerialPort(ComNames[com], baudrate);
    }

    /// <summary>创建Modbus</summary>
    /// <param name="com"></param>
    /// <param name="baudrate"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Modbus CreateModbus(Int32 com, Int32 baudrate = 9600)
    {
        if (com <= 0 || com > 4) throw new ArgumentOutOfRangeException(nameof(com), $"无效串口COM{com}，支持COM1/COM2/COM3/COM4");

        return new ModbusRtu { PortName = ComNames[com], Baudrate = baudrate };
    }
    #endregion

    #region 方法
    /// <summary>
    /// 设置主机名
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void SetHostName(String name)
    {
        name = name?.Trim();
        if (name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(name));

        // 修改主机名
        var file = "/etc/hostname";
        File.WriteAllText(file, name + Environment.NewLine);

        // 在hosts文件中添加主机名解析
        file = "/etc/hosts";
        if (File.Exists(file))
        {
            var lines = File.ReadAllLines(file);
            var flag = false;
            foreach (var line in lines)
            {
                var ss = line.Split(' ', '\t');
                if (ss[0] == "127.0.0.1" && ss[ss.Length - 1] == name)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag) File.AppendAllText(file, $"\r\n127.0.0.1\t{name}\t{name}\r\n");
        }
    }
    #endregion
}