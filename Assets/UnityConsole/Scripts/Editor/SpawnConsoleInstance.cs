using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class SpawnConsoleInstance : Editor
{
#pragma warning disable 
    private static readonly string newInputPrefabName = "DEVELOPER CONSOLE (NewInputSystem)";
    private static readonly string legacyInputPrefabName = "DEVELOPER CONSOLE";

    private const string spawnedConsoleName = "Console";

    [MenuItem("Tools/Unity Console/Spawn Console")]
    public static void SpawnConsolePrefab()
    {
        string prefabName = String.Empty;
        //Choose Prefab name based on current input system
#if ENABLE_INPUT_SYSTEM
        prefabName = newInputPrefabName;
#elif ENABLE_LEGACY_INPUT_MANAGER
        prefabName = legacyInputPrefabName;
#endif

        //Attempt to load the prefab
        Object prefab = Resources.Load(prefabName, typeof(GameObject));
        if (prefab == null)
        {
            return;
        }

        GameObject consoleInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (consoleInstance != null)
        {
            consoleInstance.name = spawnedConsoleName;
        }
    }


}
