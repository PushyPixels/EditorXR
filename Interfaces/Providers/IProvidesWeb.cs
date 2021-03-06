#if INCLUDE_UNITY_WEB_REQUEST
using System;
using Unity.XRTools.ModuleLoader;
using UnityEngine.Networking;

namespace Unity.EditorXR.Interfaces
{
    /// <summary>
    /// Provide access to web functionality
    /// </summary>
    public interface IProvidesWeb : IFunctionalityProvider
    {
        /// <summary>
        /// Download a resource at the given URL and call a method on completion, providing the UnityWebRequest
        /// </summary>
        /// <param name="url">The URL of the resource</param>
        /// <param name="completed">The completion callback</param>
        void Download(string url, Action<UnityWebRequest> completed);

        /// <summary>
        /// Download a resource at the given URL using a custom download handler and call a method on completion, providing the UnityWebRequest
        /// </summary>
        /// <typeparam name="THandler">The type of download handler to use</typeparam>
        /// <param name="url">The URL of the resource</param>
        /// <param name="completed">The completion callback</param>
        void Download<THandler>(string url, Action<UnityWebRequest> completed) where THandler : DownloadHandler, new();

        /// <summary>
        /// Download a resource at the given URL to the given destination file and call a method on completion
        /// </summary>
        /// <param name="url">The URL of the resource</param>
        /// <param name="destination">The destination file path</param>
        /// <param name="completed">The completion callback</param>
        void Download(string url, string destination, Action completed);
    }
}
#endif
