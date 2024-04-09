namespace Preparation.Utility
{
    public interface IAddable<T>
    {
        public void Add(T value);
    }

    public interface IIntAddable
    {
        public void Add(int value);
    }

    public interface IDoubleAddable
    {
        public void Add(double value);
    }
}