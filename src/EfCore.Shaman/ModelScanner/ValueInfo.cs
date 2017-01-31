namespace EfCore.Shaman.ModelScanner
{
    public class ValueInfo
    {
        #region Constructors

        public ValueInfo(object clrValue)
        {
            ClrValue = clrValue;
        }

        #endregion

        #region Properties

        public object ClrValue { get; }

        #endregion
    }
}