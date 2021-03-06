﻿using System;
using System.Linq;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    public  class IndexAttributeUpdater : IColumnInfoUpdateService
    {
        private static ColumnIndexInfo GetOrCreateColumnIndexInfo(ColumnInfo columnInfo,
            AbstractIndexAttribute indexAttribute)
        {
            var indexName = indexAttribute.Name?.Trim() ?? "";
            var indexInfo = columnInfo.ColumnIndexes.SingleOrDefault(a => a.IndexName == indexName);
            if (indexInfo != null)
                return indexInfo;
            indexInfo = new ColumnIndexInfo(indexName)
            {
#if EF200
                Filter = indexAttribute.Filter
#endif
            };
            columnInfo.ColumnIndexes.Add(indexInfo);
            return indexInfo;
        }

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var indexAttributes = columnInfo.ClrProperty?.GetCustomAttributes<AbstractIndexAttribute>()?.ToArray();
            if (indexAttributes == null || !indexAttributes.Any()) return;
            foreach (var indexAttribute in indexAttributes)
            {
                var indexInfo = GetOrCreateColumnIndexInfo(columnInfo, indexAttribute)
                    .WithInfoFromAbstractIndexAttribute(indexAttribute)
                    .WithInfoFromUniqueIndexAttribute(indexAttribute as UniqueIndexAttribute)
                    .WithInfoFromFullTextIndexAttribute(indexAttribute as FullTextIndexAttribute);
                var target = $"column {dbSetInfo.Schema}.{dbSetInfo.TableName}.{columnInfo.ColumnName}";
                logger.Log(typeof(IndexAttributeUpdater), nameof(UpdateColumnInfoOnModelCreating),
                    $"Set indexInfo: Order={indexInfo.Order}, IsDescending={indexInfo.IsDescending}, IndexType={indexInfo.IndexType} for {target}");
            }
        }

        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
            
        }
    }

    public static class IndexAttributeUpdaterExtension
    {
        public static ColumnIndexInfo WithInfoFromAbstractIndexAttribute(this ColumnIndexInfo indexInfo,
            AbstractIndexAttribute indexAttribute)
        {
            indexInfo.Order = indexAttribute.Order;
            indexInfo.IsDescending = indexAttribute.IsDescending;
            return indexInfo;
        }

        public static ColumnIndexInfo WithInfoFromFullTextIndexAttribute(this ColumnIndexInfo indexInfo,
            FullTextIndexAttribute indexAttribute)
        {
            if (indexAttribute == null)
                return indexInfo;
            indexInfo.IndexType = IndexType.FullTextIndex;
            var tmp = indexInfo.FullTextCatalog ?? new FullTextCatalogInfo();
            tmp.Name = indexAttribute.FullTextCatalogName;
            indexInfo.FullTextCatalog = tmp;
            return indexInfo;
        }

        public static ColumnIndexInfo WithInfoFromUniqueIndexAttribute(this ColumnIndexInfo indexInfo,
            UniqueIndexAttribute attribute)
        {
            if (attribute != null)
                indexInfo.IndexType = IndexType.UniqueIndex;
            return indexInfo;
        }
    }
}