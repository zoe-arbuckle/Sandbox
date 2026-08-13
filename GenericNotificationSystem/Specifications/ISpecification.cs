namespace GenericNotificationSystem.Specifications
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T specification);
    }

}
