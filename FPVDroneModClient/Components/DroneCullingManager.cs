#if !UNITY_EDITOR
using FPVDroneModClient.Helpers;
using Koenigz.PerfectCulling;
using Koenigz.PerfectCulling.EFT;
using System.Collections.Generic;
using UnityEngine;

namespace FPVDroneModClient.Components
{
    public class DroneCullingManager : MonoBehaviour
    {
        public static DroneCullingManager Instance;
        private readonly Dictionary<PerfectCullingBakeGroup, bool> _defaultBakeGroupStates = [];
        private readonly Dictionary<DisablerCullingObjectBase, bool> _defaultCullingStates = [];
        private readonly Dictionary<PerfectCullingCrossSceneGroup, bool> _defaultSceneGroupStates = [];

        private void Start()
        {
            Instance = this;

            DisablerCullingObjectBase[] cullingObjects = FindObjectsOfType<DisablerCullingObjectBase>();

            foreach (DisablerCullingObjectBase cullingObject in cullingObjects)
            {
                _defaultCullingStates.Add(cullingObject, cullingObject.enabled);
            }

            PerfectCullingAdaptiveGrid adaptiveGrid = FindObjectOfType<PerfectCullingAdaptiveGrid>();
            if (adaptiveGrid != null && adaptiveGrid.RuntimeGroupMapping.Count > 0)
            {
                foreach (PerfectCullingCrossSceneGroup sceneGroup in adaptiveGrid.RuntimeGroupMapping)
                {
                    foreach (PerfectCullingBakeGroup bakeGroup in sceneGroup.bakeGroups)
                    {
                        _defaultBakeGroupStates.Add(bakeGroup, bakeGroup.IsEnabled);
                    }

                    _defaultSceneGroupStates.Add(sceneGroup, sceneGroup.enabled);
                }
            }

            DebugLogger.LogInfo("started culling manager");
        }

        private void OnDestroy()
        {
            DebugLogger.LogInfo("stopped culling manager");
        }

        public void SetCullingState(bool state)
        {
            if (state) // if culling enabled
            {
                foreach (KeyValuePair<PerfectCullingCrossSceneGroup, bool> sceneGroupState in _defaultSceneGroupStates)
                {
                    sceneGroupState.Key.enabled = sceneGroupState.Value;
                }

                foreach (KeyValuePair<PerfectCullingBakeGroup, bool> bakeGroupState in _defaultBakeGroupStates)
                {
                    bakeGroupState.Key.IsEnabled = bakeGroupState.Value;
                }

                foreach (KeyValuePair<DisablerCullingObjectBase, bool> cullingObjectState in _defaultCullingStates)
                {
                    cullingObjectState.Key.enabled = cullingObjectState.Value;
                }
            }
            else
            {
                foreach (KeyValuePair<PerfectCullingCrossSceneGroup, bool> sceneGroupState in _defaultSceneGroupStates)
                {
                    sceneGroupState.Key.enabled = false;
                }

                foreach (KeyValuePair<PerfectCullingBakeGroup, bool> bakeGroupState in _defaultBakeGroupStates)
                {
                    bakeGroupState.Key.IsEnabled = true;
                }

                foreach (KeyValuePair<DisablerCullingObjectBase, bool> cullingObjectState in _defaultCullingStates)
                {
                    cullingObjectState.Key.enabled = true;
                }
            }
        }
    }
}
#endif
