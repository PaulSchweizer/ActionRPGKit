#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using ActionRpgKit.Character.Skill;
using ActionRpgKit.Item;
using System;

/// <summary>
/// Editor Menu for Skills.
/// Re-implement accordingly for each Skill type.</summary>
public class SkillCreator : EditorWindow
{
    /// <summary>
    /// The relative path to the ScriptableObjects Items.</summary>
    private static string RelativePath = "Assets/Data/Skills/";

    /// <summary>
    /// The absolute path to the ScriptableObjects Items.</summary>
    private static string AbsolutePath = Application.dataPath + "/Data/Skills/";

    // General Skill
    private string _name;
    private string _description;
    private float _preUseTime;
    private float _cooldownTime; 
    private IItem[] _itemSequence;

    // General Magic
    private float _cost; 

    // General Combat and MeleeCombat
    private float _damage;
    private int _maximumTargets;
    private float _range;

    // PassiveMagic
    private float _duration;
    private float _modifierValue;
    private string _modifiedAttributeName;

    // RangedCombat
    private float _projectileSpeed;
    private float _projectileLifetime;

    /// <summary>
    /// The itemType.</summary>
    int _skillType;

    /// <summary>
    /// Possible types of items.</summary>
    string[] _skillTypes = new string[] { "PassiveMagicSkill", "MeleeCombatSkill", "RangedCombatSkill" };

    [MenuItem("ActionRpgKit/Create New Skill")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = GetWindow(typeof(SkillCreator));
        window.Show();
    }

    void OnGUI()
    {
        _skillType = EditorGUILayout.Popup(_skillType, _skillTypes);
        _name = EditorGUILayout.TextField("Name", _name);
        _description = EditorGUILayout.TextArea(_description, GUILayout.Height(60));
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("PreUseTime:");
        _preUseTime = EditorGUILayout.FloatField(_preUseTime);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("CooldownTime:");
        _cooldownTime = EditorGUILayout.FloatField(_cooldownTime);
        EditorGUILayout.EndHorizontal();

        // 0 = PassiveMagicSkill
        if (_skillType == 0)
        { 
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Cost:");
            _cost = EditorGUILayout.FloatField(_cost);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Duration:");
            _duration = EditorGUILayout.FloatField(_duration);
            EditorGUILayout.EndHorizontal();
      
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ModifierValue:");
            _modifierValue = EditorGUILayout.FloatField(_modifierValue);
            EditorGUILayout.EndHorizontal();
            
            _modifiedAttributeName = EditorGUILayout.TextField("ModifiedAttributeName", _modifiedAttributeName);
        }
        
        // 1 = MeleeCombatSkill
        else if (_skillType == 1)
        { 
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Damage:");
            _damage = EditorGUILayout.FloatField(_damage);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MaximumTargets:");
            _maximumTargets = EditorGUILayout.IntField(_maximumTargets);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Range:");
            _range = EditorGUILayout.FloatField(_range);
            EditorGUILayout.EndHorizontal();
        }
        
        // 2 = RangedCombatSkill
        else if (_skillType == 2)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Damage:");
            _damage = EditorGUILayout.FloatField(_damage);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MaximumTargets:");
            _maximumTargets = EditorGUILayout.IntField(_maximumTargets);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Range:");
            _range = EditorGUILayout.FloatField(_range);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ProjectileSpeed:");
            _projectileSpeed = EditorGUILayout.FloatField(_projectileSpeed);
            EditorGUILayout.EndHorizontal();
      
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ProjectileLifetime:");
            _projectileLifetime = EditorGUILayout.FloatField(_projectileLifetime);
            EditorGUILayout.EndHorizontal();
        }

        // Create the Skill
        if (GUILayout.Button(string.Format("Create {0}", _skillTypes[_skillType]), GUILayout.Height(30)))
        {
            if (_name != "")
            {
                CreateNewSkill();
            }
        }
    }

    void CreateNewSkill()
    {
        // 0 = Passive Magic
        if (_skillType == 0)
        {
            var skill = CreatePassiveMagicSkill();
            AssetDatabase.CreateAsset(skill, Path.Combine(RelativePath,
                                      string.Format("{0}_{1}.asset", skill.Skill.Id, _name)));
        }
        // 1 = MeleeCombatSkill
        else if (_skillType == 1)
        {
            var skill = CreateMeleeCombatSkill();
            AssetDatabase.CreateAsset(skill, Path.Combine(RelativePath,
                                      string.Format("{0}_{1}.asset", skill.Skill.Id, _name)));
        }
        // 2 = RangedCombatSkill
        else if (_skillType == 2)
        {
            var skill = CreateRangedCombatSkill();
            AssetDatabase.CreateAsset(skill, Path.Combine(RelativePath,
                                      string.Format("{0}_{1}.asset", skill.Skill.Id, _name)));
        }
        else
        {
            return;
        }
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Create a UPassiveMagicSkill.</summary>
    /// <returns>A ScriptableObject</returns>
    UPassiveMagicSkill CreatePassiveMagicSkill()
    {
        var skill = new PassiveMagicSkill(
                            id: GetId(),
                            name: _name,
                            description: _description,
                            preUseTime: _preUseTime,
                            cooldownTime: _cooldownTime,
                            cost: _cost,
                            itemSequence: new IItem[] { },
                            duration: _duration,
                            modifierValue: _modifierValue,
                            modifiedAttributeName: _modifiedAttributeName
            );
        var scriptableSkill = ScriptableObject.CreateInstance<UPassiveMagicSkill>();
        scriptableSkill.Skill = skill;
        return scriptableSkill;
    }
    
    /// <summary>
    /// Create a UMeleeCombatSkill.</summary>
    /// <returns>A ScriptableObject</returns>
    UMeleeCombatSkill CreateMeleeCombatSkill()
    {
        var skill = new MeleeCombatSkill(
                            id: GetId(),
                            name: _name,
                            description: _description,
                            preUseTime: _preUseTime,
                            cooldownTime: _cooldownTime, 
                            damage: _damage,
                            itemSequence: new IItem[] { },
                            maximumTargets: _maximumTargets,
                            range: _range);
        var scriptableSkill = ScriptableObject.CreateInstance<UMeleeCombatSkill>();
        scriptableSkill.Skill = skill;
        return scriptableSkill;
    }
    
        /// <summary>
    /// Create a URangedCombatSkill.</summary>
    /// <returns>A ScriptableObject</returns>
    URangedCombatSkill CreateRangedCombatSkill()
    {
        var skill = new RangedCombatSkill(
                            id: GetId(),
                            name: _name,
                            description: _description,
                            preUseTime: _preUseTime,
                            cooldownTime: _cooldownTime, 
                            damage: _damage,
                            maximumTargets: _maximumTargets,
                            range: _range,
                            itemSequence: new IItem[] { },
                            projectileSpeed: _projectileSpeed,
                            projectileLifetime: _projectileLifetime);
        var scriptableSkill = ScriptableObject.CreateInstance<URangedCombatSkill>();
        scriptableSkill.Skill = skill;
        return scriptableSkill;
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
            if(fileId > biggestId)
            {
                biggestId = fileId;
            }
        }
        return biggestId + 1;
    }
}
#endif
