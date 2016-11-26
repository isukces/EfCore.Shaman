using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    class RequiredAttributeUpdater: IColumnInfoUpdateService
    {
        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<RequiredAttribute>();
            if (attribute == null) return;
            columnInfo.NotNull = true;
        }
    }
}
