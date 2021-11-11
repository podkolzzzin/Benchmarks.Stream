using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Benchmarks.Stream
{
    public class SleepVSDelayBenchmark
    {
        [Params(1, 5, 50)]
        public int Duration;

        [Benchmark]
        public void ThreadSleep() => Thread.Sleep(Duration);

        [Benchmark]
        public Task TaskDelay() => Task.Delay(Duration);
    }

    public class ThreadStartVSThreadPoolQueueVSTaskRunBenchmark
    {
        [Benchmark(Baseline = true)]
        public void ThreadStart()
        {
            bool b = false;
            var thread = new Thread(() =>
            {
                b = true;
            });
            thread.Start();

            while (!Volatile.Read(ref b))
                ;
        }

        [Benchmark]
        public void ThreadPoolQueue()
        {
            bool b = false;
            ThreadPool.QueueUserWorkItem(o =>
            {
                b = true;
            });

            while (!Volatile.Read(ref b))
                ;
        }

        [Benchmark]
        public void TaskRun()
        {
            bool b = false;
            Task.Run(() =>
            {
                b = true;
            });

            while (!Volatile.Read(ref b))
                ;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ThreadStartVSThreadPoolQueueVSTaskRunBenchmark>(DefaultConfig.Instance.AddColumn(StatisticColumn.P95));
        }
    }
}
