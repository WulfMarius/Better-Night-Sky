﻿using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class CreateAssetBundle : MonoBehaviour
{

    [MenuItem("Assets/Build Asset Bundles")]
    static void ExportResource()
    {
        string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        foreach (string eachAssetBundleName in assetBundleNames)
        {
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = eachAssetBundleName;
            build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(eachAssetBundleName);

            Debug.Log("Building Asset Bundle '" + eachAssetBundleName + "' with assets");
            foreach (string eachAssetPath in build.assetNames)
            {
                Debug.Log("  " + eachAssetPath);
            }

            BuildPipeline.BuildAssetBundles("Assets/AssetBundles", new AssetBundleBuild[] { build }, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows);
        }
    }
}
