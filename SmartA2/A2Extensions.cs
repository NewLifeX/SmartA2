using NewLife.IoT.Controllers;
using SmartA2;

namespace NewLife.Model;

/// <summary>A2硬件工厂扩展</summary>
public static class A2Extensions
{
    /// <summary>在A2工业计算机中注册IBoard服务</summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static Boolean AddA2(this IObjectContainer services)
    {
        // 检测本机如果是A2，则注入服务
        var flag = false;
        var machineName = Environment.MachineName;
        if (!flag && (machineName == "A2" || machineName.StartsWithIgnoreCase("A2-"))) flag = true;
        if (!flag)
        {
            var mi = MachineInfo.GetCurrent();
            if (!flag && (mi.Product == "A2" || mi.Product.StartsWithIgnoreCase("A2-"))) flag = true;
            if (!flag && mi.Board.StartsWithIgnoreCase("A2-")) flag = true;
        }
        if (!flag) return false;

        services.AddSingleton<IBoard, A2>();

        return true;
    }
}
