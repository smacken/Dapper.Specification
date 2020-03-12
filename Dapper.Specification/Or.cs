using SqlKata;

namespace Dapper.Specification
{
    public class Or<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public Or(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Query ToQuery(Query q)
        {
            throw new System.NotImplementedException();
        }

        public override Query ToQuery()
        {
            return _left.ToQuery().OrWhere(q => _right.ToQuery()).Or();
        }
    }
}