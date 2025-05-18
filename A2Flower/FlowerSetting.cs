using System.ComponentModel;

namespace A2Flower;

internal class FlowerSetting
{
    /// <summary>定时表达式。支持多个表达式（分号隔开），默认0 30 8-18 * * ? *，每天8到18点的30分执行</summary>
    [Description("定时表达式。支持多个表达式（分号隔开），默认0 30 8-18 * * ? *，每天8到18点的30分执行")]
    public String Cron { get; set; } = "0 30 8-18 * * ? *";

    /// <summary>
    /// 打开电源时间。默认15000ms
    /// </summary>
    [Description("打开电源时间。默认3000ms")]
    public Int32 PowerTime { get; set; } = 15000;
}
