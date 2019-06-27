#region LGPL License
/*
 * The Ur-Quan ReMasters is a recreation of The Ur-Quan Masters in C#.
 * For the latest info, see http://sourceforge.net/projects/sc2-remake/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using Physics2D;
using AdvanceMath; using AdvanceSystem;
using SdlDotNet;


namespace ReMasters.SuperMelee
{
    /// <summary>
    /// My super compicated AI interface
    /// </summary>
    public interface IControler : ITimed, ICloneable
    {
        bool IsFireAndForget { get;}
        ControlInput GetControlInput(float dt, ControlInput original);
        IControlable Host { get;}
        void OnCreation(GameResult gameResult, IControlable host);
    }
    public interface IPlayerControler : IControler
    {
        void SetKeyboardState(KeyboardState keys);
    }
    public interface IControlHandler : ITimed, ICloneable
    {
        void HandleControlInput(float dt);
        IControlable Host { get;}
        void OnCreation(IControlable host);
    }
    /// <summary>
    /// What a object that can be controlled must have
    /// </summary>
    public interface IControlable : ICollidableBody, IEffectable, IHasTarget, ICleanable
    {
        event EventHandler Killed;

        ContFlags UQMFlags { get;set;}
        bool IsTargetable { get;set;}
        bool IsInvisible { get;set;}
        Vector2D DirectionVector { get;}
        float DirectionAngle { get;}
        FactionInfo FactionInfo { get;set;}
        ControlInput CurrentControlInput { get;set;}
        TargetRetriever TargetRetriever { get;}
        IControler[] Controlers { get;}


        bool IsFireAndForget { get;}
        IControlHandler ControlHandler { get;set;}
        ShipState ShipState { get;set;}
        ShipMovementInfo MovementInfo { get;set;}
        ControlableType ControlableType { get;set;}
        ControlableSounds ControlableSounds { get;set;}

        void Kill(GameResult gameResult);
        void OnCreation(GameResult gameResult, FactionInfo factionInfo);
        void RunControlInput(GameResult gameResult, float dt);
        void UpdateControlable(GameResult gameResult, float dt);
        void AddControler(GameResult gameResult, IControler controler);
        bool IsThreatTo(IControlable other);


    }
    [Serializable]
    public class SpawningLocation
    {
        bool facingTarget;
        float maxDistanceToTarget;
        float minDistanceToTarget;

        public SpawningLocation()
        {
            maxDistanceToTarget = -1;
            minDistanceToTarget = -1;
            facingTarget = false;
        }
        public SpawningLocation(bool facingTarget,
        float maxDistanceToTarget,
        float minDistanceToTarget) 
        {
            this.maxDistanceToTarget = maxDistanceToTarget;
            this.minDistanceToTarget = minDistanceToTarget;
            this.facingTarget = facingTarget;
        }

        public bool FacingTarget
        {
            get { return facingTarget; }
            set { facingTarget = value; }
        }
        public float MinDistanceToTarget
        {
            get { return minDistanceToTarget; }
            set { minDistanceToTarget = value; }
        }
        public float MaxDistanceToTarget
        {
            get { return maxDistanceToTarget; }
            set { maxDistanceToTarget = value; }
        }

    }

    /// <summary>
    /// O a ship can do do actions!
    /// </summary>
    public interface IShip : IControlable
    {
        string Name { get;set;}
        ShipSounds ShipSounds { get;set;}
        ActionList Actions { get;}
        IShip[] SubShips { get;}
        IShip HyperShip { get;}
        IShipAIInfo AIInfo { get;set;}
        void OnHyperShipCreation(GameResult gameResult, IShip hyperShip);
        void UpdateActions(GameResult gameResult, float dt);
        SpawningLocation SpawnInfo { get;set;}
    }
    public interface IHasSource
    {
        IShip Source { get;}
    }
    public interface IHasTarget
    {
        IControlable Target { get;set;}
    }

    public interface IActionChild : IHasSource, IHasTarget
    {
        IAction ActionSource { get;}
    }

    public class AIStateInfo
    {
        IControlable target;
        IControlable[] obstacles;
        bool[] threats;
        Vector2D vectorToTarget;
        public AIStateInfo(IControlable target, Vector2D vectorToTarget, 
            IControlable[] obstacles,
            bool[] threats)
        {
            this.target = target;
            this.obstacles = obstacles;
            this.vectorToTarget = vectorToTarget;
            this.threats = threats;
        }
        public IControlable Target { get { return target; } }
        public IControlable[] Obstacles
        {
            get { return obstacles; }
        }
        public bool[] Threats
        {
            get { return threats; }
        }
        public Vector2D VectorToTarget
        {
            get { return vectorToTarget; }
        }
    }



    public interface IAIInfo : ICloneable 
    {
        void Update(AIStateInfo aIState);
    }
    public interface IShipAIInfo : IAIInfo
    {
        float SensorRadius { get;}
        bool ShouldSetAngle { get;}
        float DesiredAngle { get;}
        void OnSourceCreation(GameResult gameResult, IShip source);
    }
    public interface IActionAIInfo : IAIInfo
    {
        float EffectiveRange { get;}
        bool ShouldAim { get;}
        bool ShouldActivate { get;}
        float AimAngle { get;}
        bool IsThreatTo(IControlable other);
        void OnSourceCreation(GameResult gameResult, IShip source, IAction action);
    }


    /// <summary>
    /// A Action 
    /// </summary>
    public interface IAction : IUpdatable, IHasTarget, IHasSource, ICloneable
    {
        TargetingInfo TargetableTypes { get;}
        Bounded<float> Delay { get;}
        Costs Costs { get; set;}
        bool Ready { get;}
        bool NeedsTarget { get;}
        bool IsActive { get;}
        IActionAIInfo AIInfo { get;set;}
        ActionSounds ActionSounds { get;set;}
        void OnAction(GameResult gameResult, float dt);
        void OnAfterAction(GameResult gameResult, float dt);
        void OnSourceCreation(GameResult gameResult, IShip source);
    }
    /// <summary>
    /// A weapons can effect things!
    /// </summary>
    public interface IWeapon : ICloneable
    {
        IWeaponsLogic WeaponInfo { get;}
        void OnCollision(GameResult gameResult, IControlable collider);
        void OnCreation(GameResult gameResult, IWeaponsLogic weaponInfo);
        void OnCreation(GameResult gameResult, IShip source, IAction actionSource);
    }
    public interface ISolidWeapon : IWeapon, IControlable 
    {
        
    }
    public interface IRayWeapon : IWeapon, IRay2DEffectGroup { }
    public interface IDirectedRayWeapon : IRayWeapon
    {
        float[] Directions { get;}
        float[] Distances { get;}
    }
    /// <summary>
    /// A object that can be effected by effects!
    /// </summary>
    public interface IEffectable
    {
        EffectCollection AttachedEffectCollection { get;}
        void AttachEffects(EffectAttachmentResult attachmentResult, EffectCollection effectCollection);
        void UpdateEffects(GameResult gameResult,float dt);
    }
    /// <summary>
    /// Like damage thing.
    /// </summary>
    public interface IEffect : ICloneable, IEffecter, IUpdatable
    {
        IWeaponsLogic WeaponInfo { get;}
        EffectTypes HarmfulEffectTypes { get;}
        void ApplyEffect(GameResult gameResult, float dt);
        void OnTargetAttachment(EffectAttachmentResult attachmentResult, IControlable attachie);
        void OnCreation(IWeaponsLogic weaponInfo);
        GeneralChange RemoveEffectTypes(EffectTypes types);
        bool Exhausted { get;set;}
        bool IsHarmful { get;}
        bool IsTargetAttached { get;}
    }
    /// <summary>
    /// Shield and limp node.
    /// </summary>
    public interface IProlongedEffect : IEffect, ITimed
    {
        void FilterEffects(float dt, List<IEffect> effects);
        bool BlockEffect(IEffect effect);
        void ApplyAttributeChanges();
        void RemoveAttributeChanges();
        void OnExpired();
        void OnTargetDeath(GameResult gameResult);
        void AttachedUpdate(float dt);
    }
    public interface IEffecter
    {
        TargetingInfo EffectsWho { get;}
        bool CanEffect(IControlable controlable);
    }
    public interface IWeaponsLogic : IEffecter, IActionChild, ICloneable, IUpdatable
    {
        EffectCollection EffectCollection { get;}
        FactionInfo FactionInfo { get; set; }
        IControlable LastBody { get; }
        IWeaponsLogic ParentWeapon { get; }
        IWeapon Host { get; }
        ISolidWeapon SolidHost { get; }
        
        void OnCollision(GameResult gameResult, IControlable collider);
        void OnCreation(GameResult gameResult, IWeapon host, IWeaponsLogic parentWeapon);
        void OnCreation(GameResult gameResult, ISolidWeapon solidHost, IWeaponsLogic parentWeapon);
        void OnCreation(GameResult gameResult, ISolidWeapon solidHost, IShip source, IAction actionSource);
        void OnCreation(GameResult gameResult, IWeapon host, IShip source, IAction actionSource);
        bool IsThreatTo(IControlable other);
    }
}
