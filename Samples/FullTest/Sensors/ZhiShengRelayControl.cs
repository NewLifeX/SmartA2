using NewLife;
using NewLife.IoT.Controllers;

namespace FullTest.Sensors;

/// <summary>智胜继电器控制板</summary>
/// <remarks>
/// 智胜电子的4路继电器控制板，使用Modbus协议进行通信。
/// 购买于2019~2020年，https://item.taobao.com/item.htm?id=549504261336
/// 默认波特率9600，8N1，Modbus地址1。
/// </remarks>
public class ZhiShengRelayControl
{
    /// <summary>Modbus对象</summary>
    public IModbus Modbus { get; set; }

    /// <summary>主机地址</summary>
    public Byte Host { get; set; } = 1;

    /// <summary>控制指定点位</summary>
    /// <param name="address"></param>
    /// <param name="flag"></param>
    public void Write(Int32 address, Boolean flag) => Modbus.WriteCoil(Host, (UInt16)address, (UInt16)(flag ? 0xFF00 : 0x0000));

    /// <summary>翻转指定点位</summary>
    /// <param name="address"></param>
    public void Invert(Int32 address) => Modbus.WriteCoil(Host, (UInt16)address, 0x5500);

    /// <summary>控制指定点位</summary>
    /// <param name="flag"></param>
    public void WriteAll(Boolean flag) => Modbus.WriteCoil(Host, 0x00FF, (UInt16)(flag ? 0xFFFF : 0x0000));

    /// <summary>翻转指定点位</summary>
    public void InvertAll() => Modbus.WriteCoil(Host, 0x00FF, 0x5a00);

    /// <summary>读取指定点位</summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public Boolean Read(Int32 address) => Modbus.ReadCoil(Host, (UInt16)address, 1)[0];

    /// <summary>读取所有点位</summary>
    /// <returns></returns>
    public Boolean[] ReadAll(UInt16 count = 8) => Modbus.ReadCoil(Host, 0, count);

    /// <summary>读取从机地址</summary>
    /// <returns></returns>
    public UInt16 ReadAddress() => Modbus.ReadRegister(0x00, 0x4000, 1)[0];

    /// <summary>写入从机地址</summary>
    public Int32 WriteAddress(UInt16 address) => Modbus.WriteRegister(0x00, 0x4000, address);

    /// <summary>读取软件版本</summary>
    public String ReadSoftwareVersion()
    {
        var day = Modbus.ReadRegister(0x00, 0x0002, 1)[0];
        var month = Modbus.ReadRegister(0x00, 0x0004, 1)[0];
        var year = Modbus.ReadRegister(0x00, 0x0008, 1)[0];
        var time = Modbus.ReadRegister(0x00, 0x0010, 1)[0];

        return $"{year:X4}-{month.GetBytes().ToStr()}-{(day >> 8):X2} {(time >> 8):X2}:{(time & 0xFF):X2}";
    }

    /// <summary>读取硬件版本</summary>
    public String ReadHardwareVersion()
    {
        var vs = Modbus.ReadRegister(0x00, 0x0020, 1);
        if (vs == null || vs.Length == 0) return null;

        var v = new Version(vs[0] / 100, vs[0] % 100);

        return v.ToString();
    }

    /// <summary>读取从机波特率</summary>
    /// <returns></returns>
    public UInt16 ReadBaudrate() => Modbus.ReadRegister(0xFF, 0x0002, 1)[0];
}