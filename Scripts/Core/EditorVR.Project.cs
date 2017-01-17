#if UNITY_EDITORVR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.EditorVR;

namespace UnityEditor.Experimental.EditorVR
{
	partial class EditorVR
	{
		// Maximum time (in ms) before yielding in CreateFolderData: should be target frame time
		const float kMaxFrameTime = 0.01f;

		// Minimum time to spend loading the project folder before yielding
		const float kMinProjectFolderLoadTime = 0.005f;

		readonly List<IFilterUI> m_FilterUIs = new List<IFilterUI>();

		readonly List<IUsesProjectFolderData> m_ProjectFolderLists = new List<IUsesProjectFolderData>();
		List<FolderData> m_FolderData;
		readonly HashSet<string> m_AssetTypes = new HashSet<string>();
		float m_ProjectFolderLoadStartTime;
		float m_ProjectFolderLoadYieldTime;

		List<string> GetFilterList()
		{
			return m_AssetTypes.ToList();
		}

		List<FolderData> GetFolderData()
		{
			if (m_FolderData == null)
				m_FolderData = new List<FolderData>();

			return m_FolderData;
		}

		void UpdateProjectFolders()
		{
			m_AssetTypes.Clear();

			StartCoroutine(CreateFolderData((folderData, hasNext) =>
			{
				m_FolderData = new List<FolderData> { folderData };

				// Send new data to existing folderLists
				foreach (var list in m_ProjectFolderLists)
				{
					list.folderData = GetFolderData();
				}

				// Send new data to existing filterUIs
				foreach (var filterUI in m_FilterUIs)
				{
					filterUI.filterList = GetFilterList();
				}
			}, m_AssetTypes));
		}

		IEnumerator CreateFolderData(Action<FolderData, bool> callback, HashSet<string> assetTypes, bool hasNext = true, HierarchyProperty hp = null)
		{
			if (hp == null)
			{
				hp = new HierarchyProperty(HierarchyType.Assets);
				hp.SetSearchFilter("t:object", 0);
			}
			var name = hp.name;
			var guid = hp.guid;
			var depth = hp.depth;
			var folderList = new List<FolderData>();
			var assetList = new List<AssetData>();
			if (hasNext)
			{
				hasNext = hp.Next(null);
				while (hasNext && hp.depth > depth)
				{
					if (hp.isFolder)
					{
						yield return StartCoroutine(CreateFolderData((data, next) =>
						{
							folderList.Add(data);
							hasNext = next;
						}, assetTypes, hasNext, hp));
					}
					else if (hp.isMainRepresentation) // Ignore sub-assets (mixer children, terrain splats, etc.)
					{
						assetList.Add(CreateAssetData(hp, assetTypes));
					}

					if (hasNext)
						hasNext = hp.Next(null);

					// Spend a minimum amount of time in this function, and if we have extra time in the frame, use it
					if (Time.realtimeSinceStartup - m_ProjectFolderLoadYieldTime > kMaxFrameTime
						&& Time.realtimeSinceStartup - m_ProjectFolderLoadStartTime > kMinProjectFolderLoadTime)
					{
						m_ProjectFolderLoadYieldTime = Time.realtimeSinceStartup;
						yield return null;
						m_ProjectFolderLoadStartTime = Time.realtimeSinceStartup;
					}
				}

				if (hasNext)
					hp.Previous(null);
			}

			callback(new FolderData(name, folderList.Count > 0 ? folderList : null, assetList, guid), hasNext);
		}

		static AssetData CreateAssetData(HierarchyProperty hp, HashSet<string> assetTypes = null)
		{
			var type = string.Empty;
			if (assetTypes != null)
			{
				type = AssetDatabase.GetMainAssetTypeAtPath(AssetDatabase.GUIDToAssetPath(hp.guid)).Name;
				switch (type)
				{
					case "MonoScript":
						type = "Script";
						break;
					case "SceneAsset":
						type = "Scene";
						break;
					case "AudioMixerController":
						type = "AudioMixer";
						break;
				}

				assetTypes.Add(type);
			}

			return new AssetData(hp.name, hp.guid, type);
		}
	}
}
#endif
