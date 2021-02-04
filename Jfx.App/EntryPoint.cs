using System;

namespace Jfx.App
{
    internal class EntryPoint
    {
        [STAThread]
        static void Main(string[] args) => new Client.Program().Run();
    }
}
