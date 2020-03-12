using System.Collections.Generic;
using System.Linq;
using SqlKata;

namespace Dapper.Specification.Test
{
    public abstract class TestSupport
    {
        protected readonly TestCompilersContainer Compilers = new TestCompilersContainer();

        /// <summary>
        /// For legacy test support
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected IReadOnlyDictionary<string, string> Compile(Query query)
        {
            return Compilers.Compile(query).ToDictionary(s => s.Key, v => v.Value.ToString());
        }
    }
}