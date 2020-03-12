using SqlKata;

namespace Dapper.Specification
{
    public class And<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public And(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Query ToQuery(Query q) => q.Where(_right.ToQuery(_left.ToQuery(q)));
        
        public override Query ToQuery() => _left.ToQuery().Where(q => _right.ToQuery());
    }
}