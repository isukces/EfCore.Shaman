using System;

namespace EfCore.Shaman
{
    public class InvalidIndexFilterException : Exception
    {
        public InvalidIndexFilterException(string message, ITableIndexInfo indexInfo)
            : base(message)
        {
            IndexInfo = indexInfo;
        }

        public ITableIndexInfo IndexInfo { get; }
    }
}