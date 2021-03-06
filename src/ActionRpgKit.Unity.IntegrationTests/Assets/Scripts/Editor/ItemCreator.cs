﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using ActionRpgKit.Item;
using System;

/// <summary>
/// Editor Menu for Items.
/// Re-implement accordingly for each Item type.</summary>
public class ItemCreator : EditorWindow
{
    /// <summary>
    /// The relative path to the ScriptableObjects Items.</summary>
    private static string RelativePath = "Assets/Data/Items/";

    /// <summary>
    /// The absolute path to the ScriptableObjects Items.</summary>
    private static string AbsolutePath = Application.dataPath + "/Data/Items/";

    // General Items
    string _name;
    string _description;

    // Weapons
    float _damage;
    float _range;
    float _speed;

    /// <summary>
    /// The itemType.</summary>
    int _itemType;

    /// <summary>
    /// Possible types of items.</summary>
    string[] _itemTypes = new string[] { "UsableItem", "WeaponItem" };

    [MenuItem("ActionRpgKit/Create New Item")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = GetWindow(typeof(ItemCreator));
        window.Show();
    }

    void OnGUI()
    {
        _itemType = EditorGUILayout.Popup(_itemType, _itemTypes);
        _name = EditorGUILayout.TextField("Name:", _name);
        _description = EditorGUILayout.TextArea(_description, GUILayout.Height(60));

        // 1 = Weapon
        if (_itemType == 1)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Damage:");
            _damage = EditorGUILayout.FloatField(_damage);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Range:");
            _range = EditorGUILayout.FloatField(_range);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Speed:");
            _speed = EditorGUILayout.FloatField(_speed);
            EditorGUILayout.EndHorizontal();
        }

        // Create the Item
        if (GUILayout.Button(string.Format("Create {0}", _itemTypes[_itemType]), GUILayout.Height(30)))
        {
            if (_name.Length > 0)
            {
                CreateNewItem();
            }
        }
    }

    void CreateNewItem()
    {
        // 0 = UsableItem
        if (_itemType == 0)
        {
            var item = CreateUsableItem();
            AssetDatabase.CreateAsset(item, Path.Combine(RelativePath,
                                      string.Format("{0}_{1}.asset", item.Item.Id, _name)));
        }
        // 1 = WeaponItem
        else if (_itemType == 1)
        {
            var item = CreateWeaponItem();
            AssetDatabase.CreateAsset(item, Path.Combine(RelativePath,
                                      string.Format("{0}_{1}.asset", item.Item.Id, _name)));
        }
        else
        {
            return;
        }
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Create a UsableItemData.</summary>
    /// <returns>A ScriptableItem</returns>
    UsableItemData CreateUsableItem()
    {
        var item = new UsableItem();
        item.Name = _name;
        item.Description = _description;
        item.Id = GetId();
        var scriptableItem = ScriptableObject.CreateInstance<UsableItemData>();
        scriptableItem.Item = item;
        return scriptableItem;
    }

    /// <summary>
    /// Create a WeaponItemData.</summary>
    /// <returns>A ScriptableItem</returns>
    WeaponItemData CreateWeaponItem()
    {
        var item = new WeaponItem();
        item.Name = _name;
        item.Description = _description;
        item.Damage = _damage;
        item.Range = _range;
        item.Speed = _speed;
        item.Id = GetId();
        var scriptableItem = ScriptableObject.CreateInstance<WeaponItemData>();
        scriptableItem.Item = item;
        return scriptableItem;
    }

    /// <summary>
    /// Set the Id to the nmber of already existing Items in the Data folder.</summary>
    int GetId()
    {
        if (!Directory.Exists(AbsolutePath))
        {
            Directory.CreateDirectory(AbsolutePath);
        }
        var path = new DirectoryInfo(AbsolutePath);
        var files = path.GetFiles("*.asset", SearchOption.AllDirectories);
        int biggestId = -1;
        for (int i = 0; i < files.Length; i++)
        {
            int fileId = Int32.Parse(files[i].Name.Split('_')[0]);
            if (fileId > biggestId)
            {
                biggestId = fileId;
            }
        }
        return biggestId + 1;
    }
}
#endif
