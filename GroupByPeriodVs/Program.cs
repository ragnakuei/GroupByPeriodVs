using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace GroupByPeriodVs
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TestRunner>();
        }
    }

    [ClrJob, MonoJob, CoreJob] // 可以針對不同的 CLR 進行測試
    [MinColumn, MaxColumn]
    [MemoryDiagnoser]
    public class TestRunner
    {
        private readonly TestClass _test = new TestClass();
        private          List<int> _data;
        private          List<int> _periods;

        public TestRunner()
        {
            _data = new List<int> { 61, 82, 60, 70, 81, 58, 59, 71, 72 };

            _periods = new List<int> { 60, 70, 80 };
        }

        [Benchmark]
        public void TestMethod1() => _test.Method1(_data, _periods);

        [Benchmark]
        public void TestMethod2() => _test.Method2(_data, _periods);
    }

    public class TestClass
    {
        public void Method1(List<int> data, List<int> periods)
        {
            data.GroupBy(d => periods.OrderByDescending(p => p)
                                     .FirstOrDefault(p => p <= d))
                .ToDictionary(p => p.Key, p => p.ToArray());
        }

        public void Method2(List<int> data, List<int> periods)
        {
            Dictionary<int, List<int>> result = periods.OrderByDescending(p => p)
                                                       .ToDictionary(p => p, p => new List<int>());
            result.Add(0, new List<int>());

            foreach ( var d in data )
            {
                var key = result.FirstOrDefault(r => r.Key <= d).Key;

                if ( result.ContainsKey(key) == false )
                {
                    result[0].Add(d);
                }
                else
                {
                    result[key].Add(d);
                }
            }
        }
    }
}