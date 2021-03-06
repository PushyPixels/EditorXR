using Unity.XRTools.ModuleLoader;
using UnityEngine;

namespace Unity.EditorXR.Interfaces
{
    /// <summary>
    /// Gives decorated class access to grouping
    /// </summary>
    public interface IUsesGrouping : IFunctionalitySubscriber<IProvidesGrouping>
    {
    }

    /// <summary>
    /// Extension methods for implementors of IUsesGrouping
    /// </summary>
    public static class UsesGroupingMethods
    {
        /// <summary>
        /// Make this object, and its children into a group
        /// </summary>
        /// <param name="user">The functionality user</param>
        /// <param name="root">The root of the group</param>
        public static void MakeGroup(this IUsesGrouping user, GameObject root)
        {
#if !FI_AUTOFILL
            user.provider.MakeGroup(root);
#endif
        }
    }
}
