#region using

using System.Collections.Generic;

#endregion

namespace EfCore.Shaman
{
    public class ShamanOptions
    {
        #region Static Properties

        public static ShamanOptions Default
            => new ShamanOptions().WithDefaultServices();

        #endregion

        #region Properties

        public IList<IShamanService> Services { get; } = new List<IShamanService>();

        #endregion
    }
}