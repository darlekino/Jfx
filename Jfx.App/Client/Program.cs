using Jfx.App.UI;
using Jfx.Mathematic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Jfx.App.Client
{
    static class ColorExtension
    {
        public static Vector4F ToVector(this Color color)
            => new Vector4F(color.R / (float)byte.MaxValue, color.G / (float)byte.MaxValue, color.B / (float)byte.MaxValue, color.A / (float)byte.MaxValue);
    }

    internal class Program : System.Windows.Application, IDisposable
    {
        private IWindow window;

        static Visual[] visuals;

        

        static Program()
        {
            var matrix = Matrix4F.Scale(10) * Matrix4F.Rotate(UnitVector3F.XAxis.ToVector(), MathF.PI * 0.5f);

            // point cloud source: http://graphics.stanford.edu/data/3Dscanrep/
            var bunny1 = Seed.Bunny().Select(x => Vector3F.Transform(x, matrix)).ToArray();

            var x = new Model(new Vector3F(0, 0, 0), new Vector3F(1, 0, 0));
            var y = new Model(new Vector3F(0, 0, 0), new Vector3F(0, 1, 0));
            var z = new Model(new Vector3F(0, 0, 0), new Vector3F(0, 0, 1));

            Visual[] cube = Seed.Cube()
                .Select(polyline => new Model(polyline.Select(v => Vector3F.Transform(v, Matrix4F.Translate(-0.5f, -0.5f, -0.5f))).ToArray()))
                .Select(m => new Visual(m, new Shaders(PrimitiveTopology.LineStrip, Processing.Sequential, Interpolation.Undefined, Color.White.ToVector())))
                .ToArray();

            visuals = new Visual[]
            {
                new Visual(new Model(bunny1), new Shaders(PrimitiveTopology.PointList, Processing.Parallel, Interpolation.Undefined)),
                new Visual(x, new Shaders(PrimitiveTopology.LineList, Processing.Sequential, Interpolation.Undefined, Color.Red.ToVector())),
                new Visual(y, new Shaders(PrimitiveTopology.LineList, Processing.Sequential, Interpolation.Undefined, Color.LawnGreen.ToVector())),
                new Visual(z, new Shaders(PrimitiveTopology.LineList, Processing.Sequential, Interpolation.Undefined, Color.Blue.ToVector()))
            }
            .Concat(cube)
            .Concat(Seed.Triangles())
            .ToArray();
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

                window.Render(visuals);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        public void Dispose()
        {
            window.Dispose();
            
            foreach (var v in visuals) 
                v.Dispose();
        }
    }
}
