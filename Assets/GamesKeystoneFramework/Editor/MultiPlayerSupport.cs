using System;
using System.Collections.Generic;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Netcode.Transports.UTP;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GamesKeystoneFramework.Editor
{
    public class MultiPlayerSupport : EditorWindow
    {
        private bool _setUp;

        private bool _useRigidBody;

        private GameObject _multiPlayerObjectGroup;
        
        private GameObject[] _settingObject;
        private SerializedObject _settingSerializedObject;
        private SerializedProperty _settingProperty;
        
        [MenuItem("Window/GamesKeystoneFramework/MultiplayerSupport")]
        public static void ShowWindow()
        {
            GetWindow<MultiPlayerSupport>("MultiPlayerSupport").Show();
        }

        private void OnEnable()
        {
            _multiPlayerObjectGroup = GameObject.Find("MultiPlayerObjectGroup");
            _setUp = _multiPlayerObjectGroup != null;
            _settingObject = new GameObject[10];
            
            //リストのSerializedObject化
            //_settingSerializedObject = new SerializedObject();
        }

        private void OnGUI()
        {
            if (!_setUp)
            {
                if (GUILayout.Button("Setup Multiplayer"))
                {
                    //マルチプレイで共有するオブジェクトの親を作成
                    _setUp = true;
                    GameObject multiPlayerObjectGroup = new GameObject("MultiPlayerObjectGroup");
                    _multiPlayerObjectGroup = multiPlayerObjectGroup;
                    Undo.RegisterCreatedObjectUndo(multiPlayerObjectGroup, "MultiPlayerObjectGroup");
                    Selection.activeGameObject = multiPlayerObjectGroup;

                    //マルチプレイマネージャーのセットアップ
                    GameObject MultiPlayManager = new GameObject("MultiPlayManager");
                    var manager = MultiPlayManager.AddComponent<MultiPlayManager>();
                    MultiPlayManager.AddComponent<MultiPlayHostSystem>();
                    MultiPlayManager.AddComponent<MultiPlayHostSystem>();
                    MultiPlayManager.AddComponent<NetworkManager>();
                    MultiPlayManager.AddComponent<UnityTransport>();
                    SerializedObject managerObj = new SerializedObject(manager);
                    SerializedProperty managerProp = managerObj.FindProperty("MultiPlayObjectGroup");
                    if (managerProp != null)
                    {
                        managerProp.objectReferenceValue = multiPlayerObjectGroup;
                        managerObj.ApplyModifiedProperties();
                        EditorUtility.SetDirty(manager);
                    }
                    else
                    {
                        Debug.Log("Property :MultiPlayerObjectGroup is not found");
                    }
                }
            }
            else
            {
                GUILayout.Label("Setup Object");
                
                
                for (int i = 0; i < _settingObject.Length; i++)
                {
                    _settingObject[i] =
                        (GameObject)EditorGUILayout.ObjectField(_settingObject[i], typeof(GameObject), true);
                }

                EditorGUILayout.ToggleLeft("Use RigidBody", _useRigidBody);
                if (GUILayout.Button("Setting"))
                {
                    foreach (var obj in _settingObject)
                    {
                        obj.transform.SetParent(_multiPlayerObjectGroup.transform);
                        obj.AddComponent<NetworkObject>();
                        obj.AddComponent<NetworkTransform>();
                        obj.AddComponent<MultiPlayObject>();
                        if (_useRigidBody)
                        {
                            obj.AddComponent<Rigidbody>();
                            obj.AddComponent<NetworkRigidbody>();
                        }
                    }
                    
                    _settingObject = null;
                }
            }
        }
    }
}