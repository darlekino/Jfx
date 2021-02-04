using System;

namespace Jfx.Test
{
    internal class EntryPoint
    {
        [STAThread]
        static void Main(string[] args) => new Client.Program().Run();
    }
}
