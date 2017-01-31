#region using

using System;
using System.Collections.Generic;

#endregion

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

        #region Instance Methods

        public override string ToString() =>
            $"Property {PropertyName} => {ColumnName}";

        #endregion

        #region Properties

        /// <summary>
        ///     Index of property scanned with GetProperties() method
        /// </summary>
        public int ReflectionIndex { get; }

        /// <summary>
        ///     Name of property
        /// </summary>
        public string PropertyName { get; set; }


        /// <summary>
        ///     Name of database column
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        ///     Field order set with ColumnAttribute
        /// </summary>
        public int ForceFieldOrder { get; set; }

        /// <summary>
        /// Decorated with NotMappedAttribute
        /// </summary>
        public bool IsNotMapped { get; set; }

        public List<ColumnIndexInfo> ColumnIndexes { get; } = new List<ColumnIndexInfo>();

        public IDictionary<string, object> Annotations { get; } =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public bool NotNull { get; set; }
        public int? MaxLength { get; set; }
        public int? DecimalPlaces { get; set; }
        public bool IsInPrimaryKey { get; set; }
        public object DefaultValue { get; set; }
        public string DefaultValueSql { get; set; }
        /// <summary>
        /// Decorated with ForeignKeyAttribute or InversePropertyAttribute
        /// </summary>
        public bool IsNavigationProperty { get; set; }

        public bool IsDatabaseGenerated { get; set; }
        public bool IsIdentity { get; set; }
        public IPropertyValueReader ValueReader { get; set; }
        public IPropertyValueWriter ValueWriter { get; set; }
        public ValueInfo DefaultValue { get; set; }

        #endregion
    }
}