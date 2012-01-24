namespace OoMapper.Tests.Banchmarks
{
    using System;
    using System.Diagnostics;
    using Xunit;

    public class SimpleTest:BenchmarkBase
    {
        public SimpleTest()
        {
            Mapper.CreateMap<B2, A2>();
        }

        [Fact]
        public void Run()
        {
            const int mappingsCount = 1000000;
            Console.WriteLine("OoMapper (simple): {0} milliseconds", OoMapperSimple(mappingsCount));
            Console.WriteLine("Handwritten Mapper (simple): {0} milliseconds", HandWtittenSimple(mappingsCount));
        }

        private static long HandWtittenSimple(int mappingsCount)
        {
            var s = new B2();
            var d = new A2();

            return Benchmark(mappingsCount, () => d = Map(s, d));
        }

        private static A2 Map(B2 s, A2 result)
        {
            result.str1 = s.str1;
            result.str2 = s.str2;
            result.str3 = s.str3;
            result.str4 = s.str4;
            result.str5 = s.str5;
            result.str6 = s.str6;
            result.str7 = s.str7;
            result.str8 = s.str8;
            result.str9 = s.str9;

            result.n1 = s.n1;
            result.n2 = (int) s.n2;
            result.n3 = s.n3;
            result.n4 = s.n4;
            result.n5 = (int) s.n5;
            result.n6 = (int) s.n6;
            result.n7 = s.n7;
            result.n8 = s.n8;

            return result;
        }

        private static long OoMapperSimple(int mappingsCount)
        {
            var s = new B2();
            var d = new A2();

            return Benchmark(mappingsCount, () => d = Mapper.Map(s, d));
        }

        public class A2
        {
            public int n1;
            public int n2;
            public int n3;
            public int n4;
            public int n5;
            public int n6;
            public int n7;
            public int n8;
            public string str1;
            public string str2;
            public string str3;
            public string str4;
            public string str5;
            public string str6;
            public string str7;
            public string str8;
            public string str9;
        }

        public class B2
        {
            public int n1 = 1;
            public long n2 = 2;
            public short n3 = 3;
            public byte n4 = 4;
            public decimal n5 = 5;
            public float n6 = 6;
            public int n7 = 7;
            public char n8 = 'a';
            public string str1 = "str1";
            public string str2 = "str2";
            public string str3 = "str3";
            public string str4 = "str4";
            public string str5 = "str5";
            public string str6 = "str6";
            public string str7 = "str7";
            public string str8 = "str8";
            public string str9 = "str9";
        }
    }
}