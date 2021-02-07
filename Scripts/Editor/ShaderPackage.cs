//////////////////////////////////////////////////////
// Shader Packager
// Copyright (c)2021 Jason Booth
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JBooth.ShaderPackager
{
   public class ShaderPackage : ScriptableObject
   {

      public enum SRPTarget
      {
         Standard,
         URP,
         HDRP
      }

      public enum UnityVersion
      {
         Min = 0,
         Unity2018_4 = 20184,
         Unity2019_1 = 20191,
         Unity2019_2 = 20192,
         Unity2019_3 = 20193,
         Unity2019_4 = 20194,
         Unity2020_1 = 20201,
         Unity2020_2 = 20202,
         Unity2020_3 = 20203,
         Unity2021_1 = 20211,
         Unity2021_2 = 20212,
         Unity2021_3 = 20213,
         Unity2022_1 = 20221,
         Unity2022_2 = 20222,
         Unity2022_3 = 20223,
         Max = 30000
      }

      [System.Serializable]
      public class Entry
      {
         public SRPTarget srpTarget = SRPTarget.Standard;
         public UnityVersion UnityVersionMin = UnityVersion.Min;
         public UnityVersion UnityVersionMax = UnityVersion.Max;
         public Shader shader;
         public string shaderSrc;
      }

      public List<Entry> entries = new List<Entry>();

      public string GetShaderSrc()
      {
         UnityVersion curVersion = UnityVersion.Min;
#if UNITY_2018_4_OR_NEWER
         curVersion = UnityVersion.Unity2018_4;
#endif
#if UNITY_2019_1_OR_NEWER
         curVersion = UnityVersion.Unity2019_1;
#endif
#if UNITY_2019_2_OR_NEWER
         curVersion = UnityVersion.Unity2019_2;
#endif
#if UNITY_2019_3_OR_NEWER
         curVersion = UnityVersion.Unity2019_3;
#endif
#if UNITY_2019_4_OR_NEWER
         curVersion = UnityVersion.Unity2019_4;
#endif
#if UNITY_2020_1_OR_NEWER
      curVersion = UnityVersion.Unity2020_1;
#endif
#if UNITY_2020_2_OR_NEWER
      curVersion = UnityVersion.Unity2020_2;
#endif
#if UNITY_2020_3_OR_NEWER
      curVersion = UnityVersion.Unity2020_3;
#endif
#if UNITY_2021_1_OR_NEWER
      curVersion = UnityVersion.Unity2021_1;
#endif
#if UNITY_2021_2_OR_NEWER
      curVersion = UnityVersion.Unity2021_2;
#endif
#if UNITY_2021_3_OR_NEWER
      curVersion = UnityVersion.Unity2021_3;
#endif
#if UNITY_2022_1_OR_NEWER
      curVersion = UnityVersion.Unity2022_1;
#endif
#if UNITY_2022_2_OR_NEWER
      curVersion = UnityVersion.Unity2022_2;
#endif
#if UNITY_2022_3_OR_NEWER
      curVersion = UnityVersion.Unity2022_3;
#endif

         SRPTarget target = SRPTarget.Standard;
#if USING_HDRP
      target = SRPTarget.HDRP;
#endif
#if USING_URP
      target = SRPTarget.URP;
#endif
         string s = null;
         foreach (var e in entries)
         {
            if (target != e.srpTarget)
               continue;
            // default init state..
            if (e.UnityVersionMax == UnityVersion.Min && e.UnityVersionMin == UnityVersion.Min)
            {
               e.UnityVersionMax = UnityVersion.Max;
            }
            if (curVersion >= e.UnityVersionMin && curVersion <= UnityVersion.Max)
            {
               if (s != null)
               {
                  Debug.LogWarning("Found multiple possible entries for unity version of shader");
               }
               s = e.shaderSrc;
            }
         }
         return s;
      }
   }
}
