using FPVDroneModClient.Config;
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
        private readonly Dictionary<PerfectCullingBakeGroup, bool> _bakeGroupStates = [];
        private readonly Dictionary<DisablerCullingObjectBase, bool> _cullingObjectStates = [];
        private readonly Dictionary<PerfectCullingCrossSceneGroup, bool> _sceneGroupStates = [];

        private void Start()
        {
            Instance = this;

            DisablerCullingObjectBase[] cullingObjects = FindObjectsOfType<DisablerCullingObjectBase>();

            foreach (DisablerCullingObjectBase cullingObject in cullingObjects)
            {
                _cullingObjectStates.Add(cullingObject, cullingObject.enabled);
            }

            PerfectCullingAdaptiveGrid adaptiveGrid = FindObjectOfType<PerfectCullingAdaptiveGrid>();
            if (adaptiveGrid != null && adaptiveGrid.RuntimeGroupMapping.Count > 0)
            {
                foreach (PerfectCullingCrossSceneGroup sceneGroup in adaptiveGrid.RuntimeGroupMapping)
                {
                    foreach (PerfectCullingBakeGroup bakeGroup in sceneGroup.bakeGroups)
                    {
                        _bakeGroupStates.Add(bakeGroup, bakeGroup.IsEnabled);
                    }

                    _sceneGroupStates.Add(sceneGroup, sceneGroup.enabled);
                }
            }

            DebugLogger.LogInfo("started culling manager");
        }

        private void UpdateCullingRestoreStates()
        {
            foreach (PerfectCullingCrossSceneGroup group in _sceneGroupStates.Keys.ToList())
            {
                _sceneGroupStates[group] = group.enabled;
            }

            foreach (PerfectCullingBakeGroup bakeGroup in _bakeGroupStates.Keys.ToList())
            {
                _bakeGroupStates[bakeGroup] = bakeGroup.IsEnabled;
            }

            foreach (DisablerCullingObjectBase cullingObject in _cullingObjectStates.Keys.ToList())
            {
                _cullingObjectStates[cullingObject] = cullingObject.enabled;
            }
        }

        private void OnDestroy()
        {
            DebugLogger.LogInfo("stopped culling manager");
        }

        public void EnableCulling()
        {
            SetCullingState(true);
        }

        public void DisableCulling()
        {
            UpdateCullingRestoreStates();
            if (GeneralConfig.DisableCulling.Value)
            {
                SetCullingState(false);
            }
        }

        private void SetCullingState(bool state)
        {
            if (state) // if culling enabled
            {
                foreach (KeyValuePair<PerfectCullingCrossSceneGroup, bool> sceneGroupState in _sceneGroupStates)
                {
                    sceneGroupState.Key.enabled = sceneGroupState.Value;
                }

                foreach (KeyValuePair<PerfectCullingBakeGroup, bool> bakeGroupState in _bakeGroupStates)
                {
                    bakeGroupState.Key.IsEnabled = bakeGroupState.Value;
                }

                foreach (KeyValuePair<DisablerCullingObjectBase, bool> cullingObjectState in _cullingObjectStates)
                {
                    cullingObjectState.Key.enabled = cullingObjectState.Value;
                }
            }
            else
            {
                foreach (KeyValuePair<PerfectCullingCrossSceneGroup, bool> sceneGroupState in _sceneGroupStates)
                {
                    sceneGroupState.Key.enabled = false;
                }

                foreach (KeyValuePair<PerfectCullingBakeGroup, bool> bakeGroupState in _bakeGroupStates)
                {
                    bakeGroupState.Key.IsEnabled = true;
                }

                foreach (KeyValuePair<DisablerCullingObjectBase, bool> cullingObjectState in _cullingObjectStates)
                {
                    cullingObjectState.Key.enabled = true;
                }
            }
        }
    }
}

