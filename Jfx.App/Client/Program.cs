using Jfx.App.UI;
using Jfx.Mathematic;
using Jfx.ThreeDEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Jfx.App.Client
{
    internal class Program : System.Windows.Application, IDisposable
    {
        private IWindow window;

        static IModel[] models;

        public static IEnumerable<Vector3F> StreamPointCloud_XYZ(string filePath)
        {
            using (var inputStream = new FileStream(filePath, FileMode.Open))
            {
                var pointCount = inputStream.Length / (4 * 3); // 4 bytes per float, 3 floats per vertex
                using (var reader = new BinaryReader(inputStream))
                {
                    for (var i = 0L; i < pointCount; i++)
                    {
                        yield return new Vector3F(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    }
                }
            }
        }

        static Program()
        {
            var matrix = Matrix4F.Scale(10) * Matrix4F.Rotate(UnitVector3F.XAxis.ToVector(), MathF.PI * 0.5f);

            // point cloud source: http://graphics.stanford.edu/data/3Dscanrep/
            var bunny1 = StreamPointCloud_XYZ(@"..\..\..\..\bunny.xyz")
                .Select(x => Vector3F.Transform(x, matrix))
                .ToArray();

            var bunny2 = StreamPointCloud_XYZ(@"..\..\..\..\bunny.xyz")
                .Select(x => Vector3F.Transform(x, matrix * Matrix4F.Rotate(UnitVector3F.XAxis.ToVector(), MathF.PI * 0.5f)))
                .ToArray();

            var bunny3 = StreamPointCloud_XYZ(@"..\..\..\..\bunny.xyz")
                .Select(x => Vector3F.Transform(x, matrix * Matrix4F.Rotate(UnitVector3F.XAxis.ToVector(), MathF.PI * 1.0f)))
                .ToArray();

            var bunny4 = StreamPointCloud_XYZ(@"..\..\..\..\bunny.xyz")
                .Select(x => Vector3F.Transform(x, matrix * Matrix4F.Rotate(UnitVector3F.XAxis.ToVector(), MathF.PI * 1.5f)))
                .ToArray();

            models = new IModel[] { new Model(bunny1), new Model(bunny2), new Model(bunny3), new Model(bunny4) };
        }

        public Program()
        {
            Startup += (_, _) => Initialize();
            Exit += (_, _) => Dispose();
        }

        private static float GetDeltaTime(DateTime timestamp, TimeSpan periodDuration)
        {
            var result = (timestamp.Second * 1000 + timestamp.Millisecond) % periodDuration.TotalMilliseconds / periodDuration.TotalMilliseconds;
            return (float)result;
        }

        private void Initialize()
        {
            window = WindowFactory.CreateDefaultWindow();

            while (!Dispatcher.HasShutdownStarted)
            {
                //DateTime utcNow = DateTime.UtcNow;
                //const int radius = 2;
                //float t = GetDeltaTime(utcNow, new TimeSpan(0, 0, 0, 10));
                //float angle = t * MathF.PI * 2;

                //window.Camera.MoveTo(new Mathematic.JfxVector3F(MathF.Sin(angle) * radius, MathF.Cos(angle) * radius, 1));

                window.Render(models);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        public void Dispose()
        {
            window.Dispose();
        }
    }
}
