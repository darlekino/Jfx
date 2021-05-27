using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Jfx.Mathematic;
using System.Threading;
using System.Threading.Tasks;

namespace Jfx.Benchmark
{

    [MemoryDiagnoser]
    public class MemoryBenchmarkerDemo
    {
        //const int NumberOfItems = 100;
        //VertexBuffer pBuff = new VertexBuffer(new Vector4F[NumberOfItems]);

        //public MemoryBenchmarkerDemo()
        //{
        //    for (int i = 0; i < pBuff.Count; i++)
        //    {
        //        pBuff.array[i] = new Vector4F(i, i + 1, i + 2);
        //    }
        //}

        //[Benchmark]
        //public unsafe float M02()
        //{
        //    float sum = 0;

        //    for (int i = 0; i < pBuff.Count; i++)
        //    {
        //        var v = pBuff[i];
        //        sum += (*v).X + (*v).Y + (*v).Z;
        //    }

        //    return sum;
        //}

        [Benchmark]
        public int M0()
        {
            int sum = 0;
            for (int i = 0; i < 1024 / 2; i++)
            {
                var index0 = (i << 1);
                var index1 = (i << 1) + 1;
                sum += index0 + index1;
            }

            return sum;
        }

        [Benchmark]
        public int M1()
        {
            int sum = 0;
            for (int i = 0; i < 1024 / 2; i++)
            {
                var index0 = (i * 2);
                var index1 = (i * 2) + 1;
                sum += index0 + index1;
            }

            return sum;
        }
    }

    class Program
    {
        unsafe static void Main(string[] args)
        {

            var summary = BenchmarkRunner.Run<MemoryBenchmarkerDemo>();
        }
    }
}

