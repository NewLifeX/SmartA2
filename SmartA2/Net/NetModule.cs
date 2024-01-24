using System.Net.Sockets;
using NewLife;
using NewLife.Log;
using NewLife.Serialization;

namespace SmartA2.Net;

/// <summary>网络模块</summary>
public class NetModule : DisposeBase
{
    #region 属性
    private Socket _socket;
    #endregion

    #region 构造
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);

        Close();
    }
    #endregion

    #region 方法
    public void Open()
    {
        var ep = new UnixDomainSocketEndPoint("\0phone_server");
        var sock = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP)
        {
            SendTimeout = 3_000,
            ReceiveTimeout = 3_000
        };
        sock.Connect(ep);

        _socket = sock;
    }

    public void Close()
    {
        _socket?.Close();
        _socket = null;
    }

    public NetResult Send(UInt16 cmd, Byte[] args)
    {
        //XTrace.WriteLine("Send {0} {1}", cmd, args.ToHex());

        var writer = new Binary { IsLittleEndian = false, EncodeInt = false };
        writer.WriteFixedString("HTS", 3);
        writer.WriteUInt16(cmd);
        writer.WriteUInt16((UInt16)args.Length);
        writer.Write(args, 0, args.Length);

        var data = writer.GetBytes();
#if DEBUG
        XTrace.WriteLine("=> {0}", data.ToHex("-"));
#endif

        var rs = new Byte[1024];
        if (_socket.Available > 0) _socket.Receive(rs);

        _socket.Send(data);

        var count = _socket.Receive(rs);
        if (count == 0) return null;

#if DEBUG
        XTrace.WriteLine("<= {0}", rs.ToHex("-", 0, count));
#endif

        var ms = new MemoryStream(rs, 0, count);
        var reader = new Binary { Stream = ms, IsLittleEndian = false, EncodeInt = false, TrimZero = true };
        if (reader.ReadFixedString(3) != "HTR") return null;

        var cmd2 = reader.ReadUInt16();
        var code = (ErrorCodes)reader.ReadUInt16();
        var str = reader.ReadFixedString(-1);
#if DEBUG
        XTrace.WriteLine("cmd={0} code={1} message={2}", cmd2, code, str);
#endif

        return new NetResult { Cmd = cmd2, Code = code, Message = str };
    }
    #endregion

    #region 功能
    public String GetInfo()
    {
        var rs = Send(51, new Byte[0]);
        if (rs == null || rs.Code != 0) return null;

        return rs.Message;
    }

    public String GetIMEI()
    {
        var rs = Send(52, new Byte[0]);
        if (rs == null) return null;

        return rs.Message;
    }

    public String GetIMSI()
    {
        var rs = Send(101, new Byte[0]);
        if (rs == null) return null;

        return rs.Message;
    }

    public String GetCSQ()
    {
        var rs = Send(102, new Byte[0]);
        if (rs == null) return null;

        return rs.Message;
    }

    public String GetCOPS()
    {
        var rs = Send(103, new Byte[0]);
        if (rs == null) return null;

        return rs.Message;
    }

    public String GetICCID()
    {
        var rs = Send(104, new Byte[0]);
        if (rs == null) return null;

        return rs.Message;
    }

    public String GetLACCI()
    {
        var rs = Send(111, new Byte[0]);
        if (rs == null) return null;

        return rs.Message;
    }

    public String GetLBS()
    {
        var rs = Send(452, new Byte[0]);
        if (rs == null) return null;

        return rs.Message;
    }

    public String GetVersion()
    {
        var rs = Send(911, new Byte[0]);
        if (rs == null) return null;

        return rs.Message;
    }

    public String GetState()
    {
        var rs = Send(912, new Byte[0]);
        if (rs == null) return null;

        return rs.Message;
    }
    #endregion

    #region 复合功能
    /// <summary>获取网络信息</summary>
    /// <returns></returns>
    public NetInfo GetNetInfo()
    {
        var inf = new NetInfo();

        inf.IMEI = GetIMEI();

        Thread.Sleep(100);
        inf.IMSI = GetIMSI();

        Thread.Sleep(100);
        inf.ICCID = GetICCID();

        Thread.Sleep(100);
        inf.COPS = GetCOPS();

        Thread.Sleep(100);
        inf.CSQ = GetCSQ().ToInt();

        Thread.Sleep(100);
        inf.LACCI = GetLACCI();

        return inf;
    }
    #endregion
}
