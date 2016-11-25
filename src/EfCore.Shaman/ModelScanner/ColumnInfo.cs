using System.Collections.Generic;

namespace EfCore.Shaman.ModelScanner
{
    public class ColumnInfo
    {
        #region Constructors

        public ColumnInfo(int reflectionIndex, string propertyName)
        {
            ReflectionIndex = reflectionIndex;
            PropertyName = propertyName;
            ColumnName = propertyName;
            ForceFieldOrder = int.MaxValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Index of property scanned with GetProperties() method
        /// </summary>
        public int ReflectionIndex { get; }

        /// <summary>
        /// Name of property
        /// </summary>
        public string PropertyName { get; set; }


        /// <summary>
        /// Name of database column
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Field order set with ColumnAttribute
        /// </summary>
        public int ForceFieldOrder { get; set; }


        public List<ColumnIndexInfo> ColumnIndexes { get; } = new List<ColumnIndexInfo>();

        #endregion
    }
}