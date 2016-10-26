﻿using System;

namespace ActionRpgKit.Character
{

    #region Interfaces

    /// <summary>
    /// Determine the state of a Character.</summary>
    public interface IState
    {
        /// <summary>
        /// Called when entering the State.</summary>
        void EnterState(BaseCharacter character);

        /// <summary>
        /// Called right before changing to the next State.</summary>
        void ExitState(BaseCharacter character);

        /// <summary>
        /// Called to perform the interal calculation of the IState.</summary>
        void UpdateState(BaseCharacter character);
    }

    #endregion

    #region Implementations

    /// <summary>
    /// The initial State of a Character.</summary>
    [Serializable]
    public class IdleState : IState
    {
        public void EnterState(BaseCharacter character) { }

        public void ExitState(BaseCharacter character) { }

        public void UpdateState(BaseCharacter character)
        {
            if (character.Enemies.Count > 0)
            {
                character.ChangeState(character._alertState);
            }
        }
    }

    [Serializable]
    public class AlertState : IState
    {
        public void EnterState(BaseCharacter character) { }

        public void ExitState(BaseCharacter character) { }

        public void UpdateState(BaseCharacter character)
        {
            if (character.Enemies.Count == 0)
            {
                character.ChangeState(character._idleState);
                return;
            }

            character.ChangeState(character._chaseState);
        }
    }

    [Serializable]
    public class ChaseState : IState
    {
        public void EnterState(BaseCharacter character) { }

        public void ExitState(BaseCharacter character) { }

        public void UpdateState(BaseCharacter character)
        {
            if (character.Enemies.Count == 0)
            {
                character.ChangeState(character._alertState);
                return;
            }

            if (character.EnemyInAttackRange(character.Enemies[0]))
            {
                character.ChangeState(character._attackState);
            }
        }
    }

    [Serializable]
    public class AttackState : IState
    {
        public void EnterState(BaseCharacter character) { }

        public void ExitState(BaseCharacter character) { }

        public void UpdateState(BaseCharacter character)
        {

            if (character.Enemies.Count == 0)
            {
                character.ChangeState(character._alertState);
                return;
            }

            // If not in Attack Range any more, chase the enemy
            if (!character.EnemyInAttackRange(character.Enemies[0]))
            {
                character.ChangeState(character._chaseState);
                return;
            }

            // Attack
            if (character.CanAttack())
            {
                character.Attack(character.Enemies[0]);
            }
        }
    }

    [Serializable]
    public class DyingState : IState
    {
        public void EnterState(BaseCharacter character) { }

        public void ExitState(BaseCharacter character) { }

        public void UpdateState(BaseCharacter character) { }
    }

    #endregion
}
