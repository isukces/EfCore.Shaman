﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    class ColumnAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
            if (attribute == null) return;
            if (!string.IsNullOrEmpty(attribute.Name))
                columnInfo.ColumnName = attribute.Name;
            if (attribute.Order >= 0)
                columnInfo.ForceFieldOrder = attribute.Order;
        }
    }
}
