namespace Sirkadirov.Overtest.TestingDaemon.TestingServices.Skeleton
{
    public interface IExecutable<out T>
    {
        T Execute();
    }
}