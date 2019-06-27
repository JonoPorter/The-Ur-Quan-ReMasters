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
using Physics2D.CollidableBodies;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using System.Security.Permissions;

using Color = System.Drawing.Color;
namespace ReMasters.SuperMelee
{
    [Serializable]
    public class Ship : Controlable, IShip
    {
        #region feilds
        protected string name;
        protected ActionList actions;
        protected IShip[] subShips;
        protected IShip hyperShip;
        protected bool created = false;
        protected ShipSounds shipSounds;
        protected IShipAIInfo aIInfo;
        private SpawningLocation spawnInfo;


        #endregion
        #region constructors
        public Ship(
            LifeSpan lifeTime, 
            MassInertia massInfo, 
            PhysicsState physicsState, 
            BodyFlags flags,
            ICollidableBodyPart[] collidableParts, 
            ShipMovementInfo movementInfo, 
            ShipState shipState, 
            ControlableSounds controlableSounds, 
            ShipSounds shipSounds, 
            ActionList actions, 
            IShip[] subShips)
            : base(
            lifeTime, 
            massInfo, 
            physicsState, 
            flags, 
            collidableParts, 
            movementInfo, 
            shipState, 
            controlableSounds)
        {
            this.UQMFlags |= ContFlags.CanDoGravityWhip;
            this.shipSounds = shipSounds;
            this.subShips = subShips;
            this.actions = actions;
            this.controlableType = ControlableType.Ship;
        }
        public Ship(
            LifeSpan lifeTime, 
            PhysicsState physicsState, 
            BodyFlags flags,
            RigidBodyTemplate template, 
            ShipMovementInfo movementInfo, 
            ShipState shipState, 
            ControlableSounds controlableSounds, 
            ShipSounds shipSounds, 
            ActionList actions, 
            IShip[] subShips)
            : base(
            lifeTime, 
            physicsState, 
            flags,
            template, 
            movementInfo, 
            shipState, 
            controlableSounds)
        {
            this.UQMFlags |= ContFlags.CanDoGravityWhip;
            this.shipSounds = shipSounds;
            this.subShips = subShips;
            this.actions = actions;
            this.controlableType = ControlableType.Ship;
        }


        public Ship(
            LifeSpan lifeTime,
            MassInertia massInfo,
            PhysicsState physicsState,
            BodyFlags flags,
            ICollidableBodyPart[] collidableParts,
            ShipMovementInfo movementInfo,
            ShipState shipState,
            ControlableSounds controlableSounds,
            ShipSounds shipSounds,
            ActionList actions,
            IWeaponsLogic weaponsLogic,
            IShip[] subShips)
            : base(
            lifeTime,
            massInfo,
            physicsState,
            flags,
            collidableParts,
            movementInfo,
            shipState,
            controlableSounds, weaponsLogic)
        {
            this.UQMFlags |= ContFlags.CanDoGravityWhip;
            this.shipSounds = shipSounds;
            this.subShips = subShips;
            this.actions = actions;
            this.controlableType = ControlableType.Ship;
        }


        public Ship(
            LifeSpan lifeTime,
            PhysicsState physicsState,
            BodyFlags flags,
            RigidBodyTemplate template,
            ShipMovementInfo movementInfo,
            ShipState shipState,
            ControlableSounds controlableSounds,
            ShipSounds shipSounds,
             IWeaponsLogic weaponsLogic,
            ActionList actions,
            IShip[] subShips)
            : base(
            lifeTime,
            physicsState,
            flags,
            template,
            movementInfo,
            shipState,
            controlableSounds, weaponsLogic)
        {
            this.UQMFlags |= ContFlags.CanDoGravityWhip;
            this.shipSounds = shipSounds;
            this.subShips = subShips;
            this.actions = actions;
            this.controlableType = ControlableType.Ship;
        }





        protected Ship(Ship copy)
            : base(copy)
        {
            this.shipSounds = copy.shipSounds;
            this.actions = new ActionList(copy.actions);
            if (copy.subShips != null)
            {
                int length = copy.subShips.Length;
                this.subShips = new IShip[length];
                for (int pos = 0; pos < length; ++pos)
                {
                    this.subShips[pos] = (IShip)copy.subShips[pos].Clone();
                }
            }
            this.aIInfo = Functions.Clone<IShipAIInfo>(copy.aIInfo);
            this.spawnInfo = copy.spawnInfo;
        }
        #endregion
        #region properties
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public SpawningLocation SpawnInfo
        {
            get { return spawnInfo; }
            set { spawnInfo = value; }
        }
        public IShipAIInfo AIInfo
        {
            get { return aIInfo; }
            set { aIInfo = value; }
        }
        public ShipSounds ShipSounds
        {
            get { return shipSounds; }
            set { shipSounds = value; }
        }
        public ActionList Actions
        {
            get
            {
                return actions;
            }
            set
            {
                actions = value;
            }
        }
        public IShip[] SubShips
        {
            get 
            { 
                return subShips; 
            }
        }
        public IShip HyperShip
        {
            get 
            {
                return hyperShip;
            }
        }
        public override IControlable Target
        {
            get
            {
                return base.Target;
            }
            set
            {
                base.Target = value;
                foreach (IAction action in actions)
                {
                    action.Target = value;
                }
            }
        }
        #endregion
        #region methods
        public override void OnCreation(GameResult gameResult, FactionInfo factionInfo)
        {
            if (created)
            {
                return;
            }
            created = true;
            base.OnCreation(gameResult, factionInfo);
            actions.OnCreation(gameResult, this);
            if (subShips != null)
            {
                int length = subShips.Length;
                for (int pos = 0; pos < length; ++pos)
                {
                    subShips[pos].OnCreation(gameResult, factionInfo);
                    subShips[pos].OnHyperShipCreation(gameResult, this);
                }
            }
            if (aIInfo != null)
            {
                aIInfo.OnSourceCreation(gameResult, this);
            }
            if (weaponInfo != null)
            {
                OnCreation(gameResult, this, new ReMasters.SuperMelee.Actions.NullAction());
            }
        }
        public virtual void OnHyperShipCreation(GameResult gameResult, IShip hyperShip)
        {
            this.controlableType = ControlableType.SubShip;
            this.hyperShip = hyperShip;
            this.current.Position += hyperShip.Current.Position;
            this.SetAllPositions();
        }
        public void UpdateActions(GameResult gameResult, float dt)
        {
             actions.UpdateActions(gameResult,dt, this.currentControlInput);
        }
        public override void RunControlInput(GameResult gameResult, float dt)
        {
            UpdateActions(gameResult, dt);
            base.RunControlInput(gameResult, dt);
        }
        public override void Kill(GameResult gameResult)
        {
            base.Kill(gameResult);
            if (subShips != null)
            {
                foreach (IShip subShip in subShips)
                {
                    subShip.Kill(gameResult);
                }
            }
        }
        public override void UpdateControlable(GameResult gameResult, float dt)
        {
            if (hyperShip == null)
            {
                base.UpdateControlable(gameResult, dt);
                return;
            }
            this.currentControlInput = null;
            if (shipState.Health.IsEmpty)
            {
                Kill(gameResult);
            }
        }

        protected override ControlInput GetInput(float dt)
        {
            return controlers.GetControlInput(dt, new ControlInput(actions.Count));
        }
        public override void Update(float dt)
        {
            actions.Update(dt);
            base.Update(dt);
        }
        protected virtual void TransformSwap(Ship other)
        {}
        public void Transform(Ship other)
        {
            this.attachedEffectCollection.RemoveAttributeChanges();
            Functions.Swap<ICollidableBodyPart[]>(ref this.collidableParts, ref other.collidableParts);
            Functions.Swap<int>(ref this.partcount, ref other.partcount);
            Functions.Swap<MassInertia>(ref this.massInfo, ref other.massInfo);
            Functions.Swap<ActionList>(ref this.actions, ref other.actions);
            Functions.Swap<ShipMovementInfo>(ref this.movementInfo, ref other.movementInfo);
            Functions.Swap<float>(ref this.boundingRadius, ref other.boundingRadius);
            Functions.Swap<Bounded<float>>(ref this.shipState.EnergyChangeRate, ref other.shipState.EnergyChangeRate);
            Functions.Swap<Bounded<float>>(ref this.shipState.HealthChangeRate, ref other.shipState.HealthChangeRate);
            Functions.Swap<IControlHandler>(ref this.controlHandler, ref other.controlHandler);
            TransformSwap(other);
            this.SetAllPositions();
            this.CalcBoundingBox2D();
            other.SetAllPositions();
            other.CalcBoundingBox2D();
            this.attachedEffectCollection.ApplyAttributeChanges();
            
        }
        public override bool IsThreatTo(IControlable other)
        {
            for (int pos = 0; pos < actions.Count; ++pos)
            {
                IActionAIInfo actionAIInfo = actions[pos].AIInfo;
                if (actionAIInfo != null&&actionAIInfo.IsThreatTo(other))
                {
                    return true;
                }
            }
            return base.IsThreatTo(other);
        }
        public override object Clone()
        {
            return new Ship(this);
        }
        #endregion
    }
}
