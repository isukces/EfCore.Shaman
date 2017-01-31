namespace EfCore.Shaman.ModelScanner
{
    public class ValueInfo
    {
        #region Static Methods

        public static ValueInfo FromClrValue(object clrValue)
        {
            return new ValueInfo
            {
                ClrValue = clrValue,
                Kind = ValueInfoKind.Clr
            };
        }

        public static ValueInfo FromSqlValue(string sqlValue)
        {
            return new ValueInfo
            {
                SqlValue = sqlValue,
                Kind = ValueInfoKind.Sql
            };
        }

        #endregion

        #region Properties

        public object ClrValue { get; private set; }

        public string SqlValue { get; private set; }

        public ValueInfoKind Kind { get; private set; }

        #endregion
    }

    public enum ValueInfoKind
    {
        Clr,
        Sql
    }
}