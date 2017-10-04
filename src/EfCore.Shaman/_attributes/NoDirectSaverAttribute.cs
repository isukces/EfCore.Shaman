using System;

namespace EfCore.Shaman
{
    /// <summary>
    ///     Skip using DirectSaver features.
    ///     Shaman is sometimes unable to prepare property reader/writer to support DirectSaver
    ///     i.e. for classes derived from IdentityUser
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NoDirectSaverAttribute : Attribute
    {
    }
}