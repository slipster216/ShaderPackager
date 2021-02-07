//////////////////////////////////////////////////////
// Shader Packager
// Copyright (c)2021 Jason Booth
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;

namespace JBooth.ShaderPackager
{
   [ScriptedImporter(0, ShaderPackageImporter.k_FileExtension)]
   public class ShaderPackageImporter : ScriptedImporter
   {
      public const string k_FileExtension = ".shaderpack";

      public override void OnImportAsset(AssetImportContext ctx)
      {

         string fileContent = File.ReadAllText(ctx.assetPath);
         var package = ObjectFactory.CreateInstance<ShaderPackage>();
         if (!string.IsNullOrEmpty(fileContent))
         {
            EditorJsonUtility.FromJsonOverwrite(fileContent, package);
         }


         if (package.entries == null)
         {
            package.entries = new List<ShaderPackage.Entry>();
         }

         string shaderSrc = package.GetShaderSrc();
         if (shaderSrc == null)
         {
            Debug.LogError("No Shader for this platform and SRP provided");
            // maybe make an error shader here?
            return;
         }

         Shader shader = ShaderUtil.CreateShaderAsset(ctx, shaderSrc, true);

         ctx.AddObjectToAsset("MainAsset", shader);
         ctx.SetMainObject(shader);
      }


   }
}
