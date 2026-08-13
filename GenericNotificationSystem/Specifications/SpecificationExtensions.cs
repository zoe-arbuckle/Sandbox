namespace GenericNotificationSystem.Specifications
{
    public static class SpecificationExtensions
    {
        public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right) => new AndSpecification<T>(left, right);
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, ISpecification<T> spec) => source.Where(spec.IsSatisfiedBy);
    }
    internal record AndSpecification<T>(ISpecification<T> Left, ISpecification<T> Right) : ISpecification<T>
    {
        public bool IsSatisfiedBy(T candidate) => Left.IsSatisfiedBy(candidate) && Right.IsSatisfiedBy(candidate);
    }
}
