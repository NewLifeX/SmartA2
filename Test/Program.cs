using NewLife.IoT.Drivers;
using NewLife.IoT.ThingModels;
using NewLife.Log;
using SmartA2;
using SmartA2.Drivers;

XTrace.UseConsole();

var driver = new A2Driver();
var pm = new PCParameter();

//var points = driver.GetDefaultPoints();
//foreach (var item in points)
//{
//    XTrace.WriteLine("{0}={1}", item.Name, item.Address);
//}
var spec = driver.GetSpecification();
XTrace.WriteLine(spec.ToJson());

var node = driver.Open(null, pm);

var point = new PointModel
{
    Name = "newlife",
    Address = "newlifex.com",
    Type = "Int32",
};
var point2 = new PointModel
{
    Name = "google",
    Address = "google.com",
    Type = "Int32",
};

var dic = driver.Read(node, new[] { point, point2 });
foreach (var item in dic)
{
    XTrace.WriteLine("{0}\t= {1}", item.Key, item.Value);
}

driver.Close(node);

var a2 = new A2();
var usb = a2.UsbPower;
for (var i = 0; i < 100; i++)
{
    usb.Write(true);
    Thread.Sleep(500);
    usb.Write(false);
    Thread.Sleep(500);
}