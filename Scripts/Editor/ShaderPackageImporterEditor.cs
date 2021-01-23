using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;


namespace JBooth.ShaderPackager
{
   [CustomEditor(typeof(ShaderPackageImporter))]
   [CanEditMultipleObjects]
   public class ShaderPackageImporterEditor : ScriptedImporterEditor
   {

      SerializedProperty m_entryProperties;

      // override extraDataType to return the type that will be used in the Editor.
      protected override System.Type extraDataType => typeof(ShaderPackage);

      // override InitializeExtraDataInstance to set up the data.
      protected override void InitializeExtraDataInstance(Object extraTarget, int targetIndex)
      {
         var stack = (ShaderPackage)extraTarget;

         string fileContent = File.ReadAllText(((AssetImporter)targets[targetIndex]).assetPath);
         EditorJsonUtility.FromJsonOverwrite(fileContent, stack);
      }

      protected override void Apply()
      {
         base.Apply();
         // After the Importer is applied, rewrite the file with the custom value.
         for (int i = 0; i < targets.Length; i++)
         {
            string path = ((AssetImporter)targets[i]).assetPath;
            File.WriteAllText(path, EditorJsonUtility.ToJson((ShaderPackage)extraDataTargets[i]));
         }
      }

      public override void OnEnable()
      {
         base.OnEnable();
         // In OnEnable, retrieve the importerUserSerializedObject property and store it.
         m_entryProperties = extraDataSerializedObject.FindProperty("entries");
      }

      public override void OnInspectorGUI()
      {
         extraDataSerializedObject.Update();

         EditorGUILayout.PropertyField(m_entryProperties);

         Debug.Log("assembly : " + typeof(ShaderPackage).AssemblyQualifiedName);
         Debug.Log("namespace : " + typeof(ShaderPackage).Namespace);

         if ((typeof(ShaderPackage).Namespace == "JBooth.ShaderPackager") ||
            ShaderPackageImporter.k_FileExtension == ".shaderpack")
         {
            EditorGUILayout.HelpBox("Warning: You must change the namespace and extension!", MessageType.Error);
         }


         if (GUILayout.Button("Pack"))
         {
            var sp = extraDataSerializedObject.targetObject as ShaderPackage;
            foreach (var e in sp.entries)
            {
               if (e.shader == null)
               {
                  Debug.LogError("Shader is null, cannot pack");
                  break;
               }
               e.shaderSrc = File.ReadAllText(AssetDatabase.GetAssetPath(e.shader));
            }
         }

         extraDataSerializedObject.ApplyModifiedProperties();

         ApplyRevertGUI();
      }


      [MenuItem("Assets/Create/Shader Package", priority = 300)]
      static void CreateMenuItemMinimal()
      {
         string directoryPath = "Assets";
         foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
         {
            directoryPath = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(directoryPath) && File.Exists(directoryPath))
            {
               directoryPath = Path.GetDirectoryName(directoryPath);
               break;
            }
         }
         directoryPath = directoryPath.Replace("\\", "/");
         if (directoryPath.Length > 0 && directoryPath[directoryPath.Length - 1] != '/')
            directoryPath += "/";
         if (string.IsNullOrEmpty(directoryPath))
            directoryPath = "Assets/";

         var fileName = string.Format("New ShaderPackage{0}", ShaderPackageImporter.k_FileExtension);
         directoryPath = AssetDatabase.GenerateUniqueAssetPath(directoryPath + fileName);
         var content = ScriptableObject.CreateInstance<ShaderPackage>();
         File.WriteAllText(directoryPath, EditorJsonUtility.ToJson(content));
         AssetDatabase.Refresh();
      }
   }

}