using System;
using SqlKata;

namespace Dapper.Specification
{
    public abstract class Specification<T>
    {
        public Specification<T> And(Specification<T> specification) =>
            new And<T>(this, specification);

        public Specification<T> Or(Specification<T> specification) =>
            new Or<T>(this, specification);

        public Specification<T> Not()
        {
            return this;
        }

        public abstract Query ToQuery(Query q);
        public abstract Query ToQuery();

        public static implicit operator Func<Query, Query>(Specification<T> specification) =>
            specification.ToQuery;
    }

    public class ConcreteSpec<T> : Specification<T>
    {
        private Query? _query;
        private Func<Query, Query>? _queryFunc;

        public Specification<T> WithQuery(Func<Query> func)
        {
            _query = func();
            return this;
        }

        public Specification<T> WithQuery(Func<Query, Query> func)
        {
            _queryFunc = func;
            return this;
        }

        public Specification<T> WithQuery(Func<Query, int, Query> func, int number)
        {
            _queryFunc = q => func(q, number);
            return this;
        }

        public Specification<T> WithQuery(Func<Query, Query> func, Query query)
        {
            _query = func(query);
            return this;
        }

        public Specification<T> WithQuery(Func<int, Query> func, int p)
        {
            _query = func(p);
            return this;
        }

        public Specification<T> WithQuery(Func<string, Query> func, string p)
        {
            _query = func(p);
            return this;
        }

        public override Query ToQuery(Query q)
        {
            if (_queryFunc != null)
                return q.Where(_queryFunc(q));
            return q.Where(_query);
        }

        public override Query ToQuery()
        {
            return _query;
        }
    }

    public static class FuncExtension
    {
        public static Func<A, C> ComposeWith<A, B, C>(this Func<A, B> input, Func<B, C> f)
        {
            return a => f(input(a));
        }

        //public static Func<C, A> ComposeWith<A, B, C>(this Func<A, B, C> input, Func<C, A> f, B inParam)
        //{
        //    return a => f(input(a, inParam));
        //}
    }
}
