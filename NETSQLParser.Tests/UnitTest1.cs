using System;
using Xunit;
using NETSQLParser;
using System.IO;

namespace NETSQLParser.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Parse_Test1()
        {
            var res = NETSQLParser.Parse("../../../tests_in/test1.sql");
            Assert.True(res);
        }

        [Fact]
        public void Parse_Test2()
        {
            var res = NETSQLParser.Parse("../../../tests_in/test2.sql");
            Assert.False(res);
        }        
        [Fact]
        public void Parse_Query1()
        {
            var success = NETSQLParser.Parse("../../../tests_in/test_select_query1.sql");
            Assert.True(success);

            var result = NETSQLParser.SQL2XML();
            Assert.True(result.Length > 0);

            using (var wr = new StreamWriter("../../../tests_out/test_select_query1.xml"))
            {
                wr.Write(result);
                wr.Flush();
            }
        }
        [Fact]
        public void Parse_Query2()
        {
            var success = NETSQLParser.Parse("../../../tests_in/test_select_query2.sql");
            Assert.True(success);

            var result = NETSQLParser.SQL2XML();
            Assert.True(result.Length > 0);

            using (var wr = new StreamWriter("../../../tests_out/test_select_query2.xml"))
            {
                wr.Write(result);
                wr.Flush();
            }
        }
        [Fact]
         public void Parse_Query3()
        {
            var success = NETSQLParser.Parse("../../../tests_in/test_insert_query1.sql");
            Assert.True(success);

            var result = NETSQLParser.SQL2XML();
            Assert.True(result.Length > 0);

            using (var wr = new StreamWriter("../../../tests_out/test_insert_query1.xml"))
            {
                wr.Write(result);
                wr.Flush();
            }
        }
   }
}
