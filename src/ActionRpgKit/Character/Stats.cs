using System;
using System.Collections.Generic;
using ActionRpgKit.Character.Attribute;
using System.Runtime.Serialization;

namespace ActionRpgKit.Character.Stats 
{
    #region Abstracts

    /// <summary>
    /// Basic stats each Character has to have.</summary>
    [Serializable]
    public abstract class BaseStats
    {
        /// <summary>
        /// The physical constitution of the Character.</summary>
        public BaseAttribute Body;

        /// <summary>
        /// The mental capabilities of the Character.</summary>
        public BaseAttribute Mind;

        /// <summary>
        /// The soul holds the key to magic powers.</summary>
        public BaseAttribute Soul;

        /// <summary>
        /// Gathered experience points.</summary>
        public BaseAttribute Experience;

        /// <summary>
        /// Depends on the gathered experience.</summary>
        public BaseAttribute Level;

        /// <summary>
        /// The life energy.</summary>
        public BaseAttribute Life;

        /// <summary>
        /// Regeneration rate in points per second.</summary>
        public BaseAttribute LifeRegenerationRate;

        /// <summary>
        /// The magic energy.</summary>
        public BaseAttribute Magic;

        /// <summary>
        /// Regeneration rate in points per second.</summary>
        public BaseAttribute MagicRegenerationRate;

        /// <summary>
        /// Range of the alterness in squared distance.</summary>
        public BaseAttribute AlertnessRange;

        /// <summary>
        /// Basic range of the attack in squared distance.</summary>
        public BaseAttribute AttackRange;

        /// <summary>
        /// The base damage.</summary>
        public BaseAttribute Damage;

        /// <summary>
        /// Speed corresponding to the Unity nav mesh agent's speed.</summary>
        public BaseAttribute MovementSpeed;

        /// <summary>
        /// The level of alertness.</summary>
        public BaseAttribute Alertness;

        /// <summary>
        /// The level of alertness.</summary>
        public BaseAttribute ChasePersistency;

        /// <summary>
        /// A dictionary for ease of access to the attributes by name.</summary>
        public Dictionary<string, BaseAttribute> Dict = new Dictionary<string, BaseAttribute>();

        /// <summary>
        /// Assign all Attributes to the dictionary.</summary>
        protected void AssignAttributesToDict()
        {
            Dict.Add("Body", Body);
            Dict.Add("Mind", Mind);
            Dict.Add("Soul", Soul);
            Dict.Add("Experience", Experience);
            Dict.Add("Level", Level);
            Dict.Add("Life", Life);
            Dict.Add("LifeRegenerationRate", LifeRegenerationRate);
            Dict.Add("MagicRegenerationRate", MagicRegenerationRate);
            Dict.Add("Magic", Magic);
            Dict.Add("AlertnessRange", AlertnessRange);
            Dict.Add("AttackRange", AttackRange);
            Dict.Add("Damage", Damage);
            Dict.Add("MovementSpeed", MovementSpeed);
            Dict.Add("Alertness", Alertness);
            Dict.Add("ChasePersistency", ChasePersistency);
        }

        /// <summary>
        /// Set the values to the given stats values.</summary>
        /// <param name="stats">The stats to copy the values from.</param>
        public void Set(BaseStats stats)
        {
            Body.Value = stats.Body.BaseValue;
            Mind.Value = stats.Mind.BaseValue;
            Soul.Value = stats.Soul.BaseValue;
            Experience.Value = stats.Experience.BaseValue;
            Life.Value = stats.Life.BaseValue;
            Magic.Value = stats.Magic.BaseValue;
            AlertnessRange.Value = stats.AlertnessRange.BaseValue;
            AttackRange.Value = stats.AttackRange.Value;
            Damage.Value = stats.Damage.BaseValue;
            MovementSpeed.Value = stats.MovementSpeed.BaseValue;
            Alertness.Value = stats.Alertness.BaseValue;
            ChasePersistency.Value = stats.ChasePersistency.BaseValue;
        }

        /// <summary>
        /// Pretty representation of the Attribute.</summary>
        /// <returns></returns>
        public override string ToString()
        {
            string repr = string.Format("--- Primary Attributes ------------\n" +
                                         "{0}\n{1}\n{2}\n{3}\n" +
                                         "--- Secondary Attributes ------------\n" +
                                         "{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}\n{12}\n{13}\n",
                                         Body.ToString(),
                                         Mind.ToString(),
                                         Soul.ToString(),
                                         Experience.ToString(),
                                         Level.ToString(),
                                         Life.ToString(),
                                         Magic.ToString(),
                                         MagicRegenerationRate.ToString(),
                                         AlertnessRange.ToString(),
                                         AttackRange.ToString(),
                                         Damage.ToString(),
                                         MovementSpeed.ToString(),
                                         Alertness.ToString(),
                                         ChasePersistency.ToString());
            return repr;
        }
    }

    #endregion

    #region Implementations

    /// <summary>
    /// Interconnected, more dynamic Attributes.</summary>
    [Serializable]
    public class PlayerStats : BaseStats
    {
        public PlayerStats()
        {
            // Primary Attributes
            Body = new PrimaryAttribute("Body", 0, 999, 0);
            Mind = new PrimaryAttribute("Mind", 0, 999, 0);
            Soul = new PrimaryAttribute("Soul", 0, 999, 0);
            Experience = new PrimaryAttribute("Experience", 0, 980100, 0);
            AlertnessRange = new PrimaryAttribute("AlertnessRange", 2, 999, 2);
            AttackRange = new PrimaryAttribute("AttackRange", 1, 999, 1);
            Damage = new PrimaryAttribute("Damage", 0, 999, 0);
            MovementSpeed = new PrimaryAttribute("MovementSpeed", 1, 10, 1);
            Alertness = new PrimaryAttribute("Alertness", 0, 10, 0);
            ChasePersistency = new PrimaryAttribute("ChasePersistency", 0, 100, 0);

            // Secondary Attributes
            Level = new SecondaryAttribute("Level",
                    x => (int)(Math.Sqrt(x[0].Value / 100)) * 1f, 
                    new BaseAttribute[] { Experience }, 0, 99);
            MagicRegenerationRate = new SecondaryAttribute(
                                    "MagicRegenerationRate",
                                    x => 1 + (x[0].Value / 1000),
                                    new BaseAttribute[] { Mind }, 0, 99);
            LifeRegenerationRate = new SecondaryAttribute(
                                    "LifeRegenerationRate",
                                    x => 1 + (x[0].Value / 1000),
                                    new BaseAttribute[] { Body }, 0, 99);

            // Volume Attributes
            Life = new VolumeAttribute("Life", 
                   x => (int)(20 + 5 * x[0].Value + x[1].Value / 3) * 1f, 
                   new BaseAttribute[] { Level, Body }, 0, 999);
            Magic =new VolumeAttribute("Magic", 
                    x => (int)(20 + 5 * x[0].Value + x[1].Value / 3) * 1f, 
                    new BaseAttribute[] { Level, Soul }, 0, 999);

            AssignAttributesToDict();
        }

    }

    /// <summary>
    /// Holds just simple PrimaryAttributes.</summary>
    [Serializable]
    public class EnemyStats : BaseStats
    {
        public EnemyStats()
        {
            Body = new PrimaryAttribute("Body", 0, 999, 0);
            Mind = new PrimaryAttribute("Mind", 0, 999, 0);
            Soul = new PrimaryAttribute("Soul", 0, 999, 0);
            Experience = new PrimaryAttribute("Experience", 0, 980100, 0);
            Level = new PrimaryAttribute("Level", 0, 99, 0);
            Life = new PrimaryAttribute("Life", 0, 999, 0);
            MagicRegenerationRate = new PrimaryAttribute("MagicRegenerationRate", 0, 2, 0);
            LifeRegenerationRate = new PrimaryAttribute("LifeRegenerationRate", 0, 2, 0);
            Magic = new PrimaryAttribute("Magic", 0, 999, 0);
            AlertnessRange = new PrimaryAttribute("AlertnessRange", 2, 999, 2);
            Damage = new PrimaryAttribute("Damage", 0, 999, 0);
            AttackRange = new PrimaryAttribute("AttackRange", 1, 999, 1);
            MovementSpeed = new PrimaryAttribute("MovementSpeed", 1, 10, 1);
            Alertness = new PrimaryAttribute("Alertness", 0, 10, 0);
            ChasePersistency = new PrimaryAttribute("ChasePersistency", 0, 100, 0);
            AssignAttributesToDict();
        }
    }

    #endregion
}
