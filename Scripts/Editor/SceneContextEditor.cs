using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace EasyDI
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SceneContext))]
    public class SceneContextEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            var myScript = target as SceneContext;
            if (myScript.MonoInstaller.FindAll(_ => _ != null).Count() == 0)
            {
                myScript.IsAutoSearchInstallerInThisGameObject = EditorGUILayout.ToggleLeft(nameof(myScript.IsAutoSearchInstallerInThisGameObject), myScript.IsAutoSearchInstallerInThisGameObject);
            }
            //else

            //SerializedObject serializedObject = new UnityEditor.SerializedObject(myScript);
            var list = serializedObject.FindProperty(nameof(myScript.MonoInstaller));
            EditorGUILayout.PropertyField(list);
            //myScript.MonoInstaller = (List<MonoInstaller>)list.exposedReferenceValue;
            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}
