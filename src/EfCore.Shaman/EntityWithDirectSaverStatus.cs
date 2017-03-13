namespace EfCore.Shaman
{
    public abstract class EntityWithDirectSaverStatus
    {
        public static EntityWithDirectSaverStatus<T> Clean<T>(T item)
        {
            return new EntityWithDirectSaverStatus<T>(item, DirectSaverEntityStatus.Clean);
        }

        public static EntityWithDirectSaverStatus<T> MustBeInserted<T>(T item)
        {
            return new EntityWithDirectSaverStatus<T>(item, DirectSaverEntityStatus.MustBeInserted);
        }

        public static EntityWithDirectSaverStatus<T> MustBeRemoved<T>(T item)
        {
            return new EntityWithDirectSaverStatus<T>(item, DirectSaverEntityStatus.MustBeRemoved);
        }

        public static EntityWithDirectSaverStatus<T> MustBeUpdated<T>(T item)
        {
            return new EntityWithDirectSaverStatus<T>(item, DirectSaverEntityStatus.MustBeUpdated);
        }

        public DirectSaverEntityStatus Status { get; set; }
    }

    public class EntityWithDirectSaverStatus<T> : EntityWithDirectSaverStatus
    {
        public EntityWithDirectSaverStatus(T item, DirectSaverEntityStatus status)
        {
            Item = item;
            Status = status;
        }

        public T Item { get; set; }
    }

    public enum DirectSaverEntityStatus
    {
        Clean,
        MustBeInserted,
        MustBeUpdated,
        MustBeRemoved
    }
}