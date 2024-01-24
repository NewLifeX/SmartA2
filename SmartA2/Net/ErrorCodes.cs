namespace SmartA2.Net;

/// <summary>错误代码</summary>
public enum ErrorCodes : UInt16
{
    /// <summary>成功</summary>
    Success = 0,

    Fail = 1,

    执行过程信息输出 = 5,

    参数错误 = 10,

    当前模式错误 = 11,

    Busy = 12,

    模块复位中 = 13,

    无此命令 = 20,
}
