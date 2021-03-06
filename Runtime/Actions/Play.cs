using UnityEditor;

namespace Unity.EditorXR
{
    [ActionMenuItem("Play")]
    [SpatialMenuItem("Play", "Actions", "Enter Play-Mode")]
    sealed class Play : BaseAction
    {
        public override void ExecuteAction()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = true;
#endif
        }
    }
}
