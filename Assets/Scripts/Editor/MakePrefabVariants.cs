using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MakePrefabVariants : EditorWindow {
    private Label colorAmount;
    private List<Material> selectedMaterials = new();
    private GameObject defaultPrefab;
    private Label prefabName;
    
    [MenuItem("Window/Custom Tools/Make Prefab Variants")]
    public static void ShowExample() {
        MakePrefabVariants wnd = GetWindow<MakePrefabVariants>();
        wnd.titleContent = new GUIContent("Make Prefab Variants");
    }

    public void CreateGUI() {
        VisualElement root = rootVisualElement;
        
        Button getColors = new Button {
            text = "Select Colors"
        };
        getColors.clicked += SelectColors;
        root.Add(getColors);

        colorAmount = new Label("Colors Found: 0");
        root.Add(colorAmount);
        
        Button getPrefab = new Button {
            text = "Select Default Prefab"
        };
        getPrefab.clicked += SelectDefaultPrefab;
        root.Add(getPrefab);
        
        prefabName = new Label("Colors Found: 0");
        root.Add(prefabName);
        
        Button makePrefabs = new Button {
            text = "Create Prefab Variants"
        };
        makePrefabs.clicked += MakePrefabs;
        root.Add(makePrefabs);
    }

    private void SelectColors() {
        selectedMaterials.Clear();
        
        var objects = Selection.objects;
        if (objects.Length == 0) {
            colorAmount.text = "Colors Found: 0";
            return;
        }
        
        foreach (var o in objects) {
            if (o is Material m) selectedMaterials.Add(m);
        }

        colorAmount.text = "Colors Found: " + selectedMaterials.Count;
    }

    private void SelectDefaultPrefab() {
        var objects = Selection.objects;
        if (objects.Length == 0) {
            defaultPrefab = null;
            prefabName.text = "Default Prefab: None";
            return;
        }

        if (objects[0] is GameObject go) {
            var prefabType = PrefabUtility.GetPrefabAssetType(go);
            if (prefabType != PrefabAssetType.NotAPrefab) {
                defaultPrefab = go;
                prefabName.text = "Default Prefab: " + go.name;
            } else {
                defaultPrefab = null;
                prefabName.text = "Default Prefab: None";
            }
        }
    }

    private void MakePrefabs() {
        if (selectedMaterials.Count == 0) return;
        if (defaultPrefab == null) return;
        
        string originalPath = AssetDatabase.GetAssetPath(defaultPrefab);
        string folder = System.IO.Path.GetDirectoryName(originalPath) + "/";
        string defaultPrefabName = defaultPrefab.name;
        defaultPrefabName = defaultPrefabName.Replace("-Default", "");

        foreach (var material in selectedMaterials) {
            string materialName = material.name;
            materialName = materialName.Replace("Solid-", "");

            string variantName = defaultPrefabName + "-" + materialName;
            string newPath = AssetDatabase.GenerateUniqueAssetPath(folder + variantName + ".prefab");
            
            
            GameObject objSource = (GameObject) PrefabUtility.InstantiatePrefab(defaultPrefab);
            GameObject obj = PrefabUtility.SaveAsPrefabAsset(objSource, newPath);
            
            GameObject prefabContents = PrefabUtility.LoadPrefabContents(newPath);
            var renderers = prefabContents.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) {
                r.material = material;
            }
            
            PrefabUtility.SaveAsPrefabAsset(prefabContents, newPath);
            PrefabUtility.UnloadPrefabContents(prefabContents);
        }
        
        Debug.Log("Finished Creating Prefab Variants");
    }
}
