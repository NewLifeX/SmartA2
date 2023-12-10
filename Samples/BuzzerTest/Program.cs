using NewLife.IoT.Controllers;

var buzzer = new FileOutputPort("/dev/buzzer");

for (var i = 0; i < 5; i++)
{
    // 响
    buzzer.Write(true);

    Thread.Sleep(500);

    // 不响
    buzzer.Write(false);

    Thread.Sleep(500);
}