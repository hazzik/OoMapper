namespace OoMapper.Tests.Banchmarks
{
    using System;
#if !SILVERLIGHT
    using System.Diagnostics;
#endif
    public abstract class BenchmarkBase
    {
        protected static long Benchmark(int mappingsCount, Action action)
        {
#if !SILVERLIGHT
            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < mappingsCount; ++i)
                action();
            sw.Stop();
            return sw.ElapsedMilliseconds;
#else
            var start = DateTime.Now;

            for (int i = 0; i < mappingsCount; i++)
                action();

            return (long) DateTime.Now.Subtract(start).TotalMilliseconds;
#endif
        }
    }
}