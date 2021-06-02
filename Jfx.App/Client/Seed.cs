using Jfx.App.UI;
using Jfx.Mathematic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.App.Client
{
    public static class Seed
    {
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

        public static Vector3F[] Bunny()
        {
            return StreamPointCloud_XYZ(@"..\..\..\..\bunny.xyz").ToArray();
        }

        public static Vector3F[][] Cube()
        {
            return new[]
             {
                new[]
                {
                    new Vector3F(0, 0, 0),
                    new Vector3F(1, 0, 0),
                    new Vector3F(1, 1, 0),
                    new Vector3F(0, 1, 0),
                    new Vector3F(0, 0, 0),
                },
                new[]
                {
                    new Vector3F(0, 0, 1),
                    new Vector3F(1, 0, 1),
                    new Vector3F(1, 1, 1),
                    new Vector3F(0, 1, 1),
                    new Vector3F(0, 0, 1),
                },
                new[] { new Vector3F(0, 0, 0), new Vector3F(0, 0, 1), },
                new[] { new Vector3F(1, 0, 0), new Vector3F(1, 0, 1), },
                new[] { new Vector3F(1, 1, 0), new Vector3F(1, 1, 1), },
                new[] { new Vector3F(0, 1, 0), new Vector3F(0, 1, 1), },
            };
        }

        public static Visual[] Triangles()
        {
            var tr0 = new Model(new Vector3F(0, 0, 0), new Vector3F(1, 0, 0), new Vector3F(0, 1, 0), new Vector3F(1, 1, 0));
            var tr1 = new Model(
                new Vector3F(-2, 0, 0),
                new Vector3F(-2, 1, 0),
                new Vector3F(-1, 0, 0),
                new Vector3F(-4, 0, 0),
                new Vector3F(-4, 1, 0),
                new Vector3F(-3, 0, 0)
            );

            return new Visual[]
            {
                new Visual(tr0, new Shaders(PrimitiveTopology.TriangleStrip, Processing.Parallel, Interpolation.Undefined, Color.Goldenrod.ToVector())),
                new Visual(tr1, new Shaders(PrimitiveTopology.TriangleList, Processing.Parallel, Interpolation.Undefined, Color.Cyan.ToVector())),
            };
        }
    }
}
