namespace SmartA2.Net;

public class NetResult
{
    public UInt16 Cmd { get; set; }

    public ErrorCodes Code { get; set; }

    public String Message { get; set; }
}
