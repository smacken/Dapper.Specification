using System.Collections.Generic;
using System.Collections.ObjectModel;
using SqlKata;

namespace Dapper.Specification.Test
{
    public class TestSqlResultContainer : ReadOnlyDictionary<string, SqlResult>
    {
        public TestSqlResultContainer(IDictionary<string, SqlResult> dictionary) : base(dictionary)
        {

        }
    }
}