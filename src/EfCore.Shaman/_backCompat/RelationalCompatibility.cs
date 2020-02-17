using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfCore.Shaman
{
    public sealed class RelationalCompatibility
    {
        public RelationalCompatibility(IEntityType entity)
        {
            _entity = entity;
        }

        public string TableName
        {
            get
            {
                return _entity.GetTableName();
            }
            set
            {
                if (_entity is IMutableAnnotatable ma)
                    ma[RelationalAnnotationNames.TableName] = value;
                else
                    throw new Exception("Unable to change table name to " + value);
            }
        }

        public string Schema
        {
            get
            {
                return _entity.GetSchema();
            }
        }

        private readonly IEntityType _entity;
    }
}