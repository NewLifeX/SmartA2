using NewLife.IoT.Controllers;

namespace SmartA2;

/// <summary>继电器控制板</summary>
public class RelayControl
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
    public UInt16 ReadAddress() => Modbus.ReadRegister(0x00, 0, 1)[0];

    /// <summary>读取从机波特率</summary>
    /// <returns></returns>
    public UInt16 ReadBaudrate() => Modbus.ReadRegister(0xFF, 0x03E8, 1)[0];
}