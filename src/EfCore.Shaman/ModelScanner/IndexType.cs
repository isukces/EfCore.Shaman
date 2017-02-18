namespace EfCore.Shaman.ModelScanner
{
    public enum IndexType
    {
        Index,
        UniqueIndex,
        FullTextIndex
    }

    public static class IndexTypeExtension
    {
        #region Static Methods

        /// <summary>
        /// Check if indextype is Index or UniqueIndex (opposite to FullTextIndex)
        /// </summary>
        /// <param name="idxIndexType"></param>
        /// <returns></returns>
        public static bool IsNormalIndex(this IndexType idxIndexType)
        {
            return idxIndexType == IndexType.Index || idxIndexType == IndexType.UniqueIndex;
        }

        #endregion
    }
}