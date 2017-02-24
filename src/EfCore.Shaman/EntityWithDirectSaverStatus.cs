namespace EfCore.Shaman
{
    public class EntityWithDirectSaverStatus<T>
    {
        #region Constructors

        public EntityWithDirectSaverStatus(T item, DirectSaverEntityStatus status)
        {
            Item = item;
            Status = status;
        }

        #endregion

        #region Static Methods

        public static EntityWithDirectSaverStatus<T> Clean(T item)
        {
            return new EntityWithDirectSaverStatus<T>(item, DirectSaverEntityStatus.Clean);
        }

        #endregion

        #region Properties

        public T Item { get; set; }
        public DirectSaverEntityStatus Status { get; set; }

        #endregion
    }

    public enum DirectSaverEntityStatus
    {
        Clean,
        MustBeInserted,
        MustBeUpdated,
        MustBeRemoved
    }
}