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
using Physics2D;
using Physics2D.CollidableBodies;
using Physics2D.Collections;
using System.Collections;
using System.Collections.Generic;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using Color = System.Drawing.Color;
using ReMasters.SuperMelee.Ships;
using System.Security.Permissions;

namespace ReMasters.SuperMelee
{
    [Serializable]
    public class ControllerList<T> : TimedList<T>
        where T : IControler
    {
        public ControllerList()
            : base()
        { }
        public ControllerList(int capacity)
            : base(capacity)
        { }
        public ControllerList(IEnumerable<T> collection)
            : base(collection)
        { }
        public ControlInput GetControlInput(float dt, ControlInput original)
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                original = base[pos].GetControlInput(dt, original);
            }
            return original;
        }
        public void OnCreation(GameResult gameResult, IControlable host)
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base[pos].OnCreation(gameResult, host);
            }
        }
    }
    [Flags, Serializable]
    public enum ContFlags : int
    {
        None = 0,
        Invisible = 1 << 0,
        NoMaxSpeed = 1 << 1,
        CanDoGravityWhip = 1 << 2,
    }
    [Serializable]
    public class Controlable : RigidBody, IControlable, ICleanable, ISolidWeapon
    {
        #region events
        public event EventHandler Killed;
        #endregion
        #region feilds
        protected TargetRetriever targetRetriever;
        protected ContFlags contFlags;
        protected bool canTarget = true;
        private bool created = false;
        protected ShipState shipState;
        protected ShipMovementInfo movementInfo;
        [NonSerialized]
        protected Vector2D direction = Vector2D.XAxis;
        protected EffectCollection attachedEffectCollection = new EffectCollection();
        protected IControlable target;
        protected FactionInfo factionInfo;
        protected ControllerList<IControler> controlers = new ControllerList<IControler>();
        protected IControlHandler controlHandler;
        protected ControlInput currentControlInput = new ControlInput(0);
        protected ControlableType controlableType = ControlableType.Debris;
        protected ControlableSounds controlableSounds;
        protected bool died = false;
        protected IWeaponsLogic weaponInfo;
        bool isWeaponCreated = false;
        #endregion
        #region constructors
        public Controlable(
            LifeSpan lifeTime,
            MassInertia massInfo,
            PhysicsState physicsState,
            BodyFlags flags,
            ICollidableBodyPart[] collidableParts,
            ShipMovementInfo movementInfo,
            ShipState shipState,
            ControlableSounds controlableSounds)
            : base(lifeTime,
            massInfo,
            physicsState,
            flags,
            collidableParts)
        {

            this.shipState = shipState;
            this.movementInfo = movementInfo;
            this.controlableSounds = controlableSounds;
            this.attachedEffectCollection = new EffectCollection();
            this.CollisionState.GenerateRayEvents = true;
        }
        public Controlable(
                LifeSpan lifeTime,
                PhysicsState physicsState,
                BodyFlags flags,
                RigidBodyTemplate template,
                ShipMovementInfo movementInfo,
                ShipState shipState,
                ControlableSounds controlableSounds)
            : base(
                lifeTime,
                physicsState,
                flags,
                template)
        {
            this.shipState = shipState;
            this.movementInfo = movementInfo;
            this.controlableSounds = controlableSounds;
            this.attachedEffectCollection = new EffectCollection();
            this.CollisionState.GenerateRayEvents = true;
        }


        public Controlable(
            LifeSpan lifeTime,
            MassInertia massInfo,
            PhysicsState physicsState,
            BodyFlags flags,
            ICollidableBodyPart[] collidableParts,
            ShipMovementInfo movementInfo,
            ShipState shipState,
            ControlableSounds controlableSounds,
            IWeaponsLogic weaponInfo)
            : this(
            lifeTime,
            massInfo,
            physicsState,
            flags, collidableParts,
            movementInfo,
            shipState,
            controlableSounds)
        {
            this.CollisionState.GenerateCollisionEvents = true;
            this.CollisionState.GenerateContactEvents = true;
            this.weaponInfo = weaponInfo;

            this.controlableType = ControlableType.Weapon;
        }
        public Controlable(
            LifeSpan lifeTime,
            PhysicsState physicsState,
            BodyFlags flags,
            RigidBodyTemplate template,
            ShipMovementInfo movementInfo,
            ShipState shipState,
            ControlableSounds controlableSounds,
            IWeaponsLogic weaponInfo)
            : this(
            lifeTime,
            physicsState,
            flags, template,
            movementInfo,
            shipState,
            controlableSounds)
        {
            this.weaponInfo = weaponInfo;

            this.CollisionState.GenerateCollisionEvents = true;
            this.CollisionState.GenerateContactEvents = true;
            this.controlableType = ControlableType.Weapon;
        }


        protected Controlable(Controlable copy)
            : base(copy)
        {
            this.controlableSounds = copy.controlableSounds;
            this.Killed = Functions.Clone<EventHandler>(copy.Killed);
            this.shipState = new ShipState(copy.shipState);
            this.movementInfo = new ShipMovementInfo(copy.movementInfo);
            foreach (IControler controler in copy.controlers)
            {
                AddControler(null, (IControler)controler.Clone());
            }
            this.target = copy.target;
            this.ControlHandler = Functions.Clone<IControlHandler>(copy.ControlHandler);
            this.currentControlInput = copy.currentControlInput;
            this.direction = copy.direction;
            this.controlableType = copy.controlableType;
            this.attachedEffectCollection = new EffectCollection(copy.attachedEffectCollection);
            this.contFlags = copy.contFlags;
            this.CollisionState.GenerateRayEvents = true;
            if (copy.weaponInfo != null)
            {
                this.weaponInfo = (IWeaponsLogic)copy.weaponInfo.Clone();
            }
        }
        #endregion
        #region properties
        public TargetRetriever TargetRetriever
        {
            get
            {
                return targetRetriever;
            }
        }
        public ContFlags UQMFlags
        {
            get
            {
                return contFlags;
            }
            set
            {
                this.contFlags = value;
            }
        }
        public bool IsInvisible
        {
            get
            {
                return (contFlags & ContFlags.Invisible) == ContFlags.Invisible || IsExpired;
            }
            set
            {
                if (IsInvisible ^ value)
                {
                    contFlags ^= ContFlags.Invisible;
                }
            }
        }
        public bool IsTargetable
        {
            get
            {
                return canTarget && !IsInvisible;
            }
            set
            {
                canTarget = value;
            }
        }
        public ControlableSounds ControlableSounds
        {
            get { return controlableSounds; }
            set { controlableSounds = value; }
        }
        public virtual IControlable Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
        public ShipState ShipState
        {
            get
            {
                return shipState;
            }
            set
            {
                shipState = value;
            }
        }
        public ShipMovementInfo MovementInfo
        {
            get
            {
                return movementInfo;
            }
            set
            {
                movementInfo = value;
            }
        }
        public Vector2D DirectionVector
        {
            get { return direction; }
        }
        public virtual float DirectionAngle
        {
            get
            {
                return current.Position.Angular;
            }
        }
        public IControlHandler ControlHandler
        {
            get
            {
                return controlHandler;
            }
            set
            {
                controlHandler = value;
                if (created && controlHandler != null)
                {
                    controlHandler.OnCreation(this);
                }
            }
        }
        public FactionInfo FactionInfo
        {
            get
            {
                return factionInfo;
            }
            set
            {
                factionInfo = value;
            }
        }
        public ControlInput CurrentControlInput
        {
            get
            {
                return currentControlInput;
            }
            set
            {
                currentControlInput = value;
            }
        }
        public ControlableType ControlableType
        {
            get
            {
                return controlableType;
            }
            set
            {
                controlableType = value;
            }
        }
        public EffectCollection AttachedEffectCollection
        {
            get { return attachedEffectCollection; }
        }
        public bool IsFireAndForget
        {
            get
            {
                foreach (IControler controler in controlers)
                {
                    if (controler.IsFireAndForget)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public IControler[] Controlers
        {
            get
            {
                return controlers.ToArray();
            }
        }
        public IWeaponsLogic WeaponInfo
        {
            get { return weaponInfo; }
        }

        #endregion
        #region methods


        public override void UpdatePosition(float dt)
        {
            base.UpdatePosition(dt);
            direction = matrix.NormalMatrix * Vector2D.XAxis;
        }
        protected virtual ControlInput GetInput(float dt)
        {
            if (controlers.Count > 0)
            {
                return controlers.GetControlInput(dt, new ControlInput(0));
            }
            return null;
        }
        public override void CalcAccelerations(Vector2D accelerationDueToGravity)
        {
            if ((this.Flags & BodyFlags.IgnoreGravity) != BodyFlags.IgnoreGravity &&
                (UQMFlags & ContFlags.CanDoGravityWhip) == ContFlags.CanDoGravityWhip)
            {
                if (accelerationDueToGravity == Vector2D.Zero)
                {
                    UQMFlags &= ~ContFlags.NoMaxSpeed;
                }
                else
                {
                    UQMFlags |= ContFlags.NoMaxSpeed;
                }
            }
            base.CalcAccelerations(accelerationDueToGravity);
        }
        public override void Update(float dt)
        {
            if (weaponInfo != null)
            {
                weaponInfo.Update(dt);
            }

            float speedSq = this.current.Velocity.Linear.MagnitudeSq;
            if (speedSq > TimeWarp.MaxAllowedSpeedSq)
            {
                this.current.Velocity.Linear *= (TimeWarp.MaxAllowedSpeed / MathHelper.Sqrt(speedSq));
            }

            this.attachedEffectCollection.AttachedUpdate(dt);
            controlers.Update(dt);
            if (controlHandler != null)
            {
                controlHandler.Update(dt);
            }
            base.Update(dt);
        }
        public override bool RemoveExpired()
        {
            controlers.RemoveExpired();
            if (controlHandler != null)
            {
                if (controlHandler.IsExpired)
                {
                    controlHandler = null;
                }
            }
            return base.RemoveExpired();
        }
        public override object Clone()
        {
            return new Controlable(this);
        }
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
            direction = matrix.NormalMatrix * Vector2D.XAxis;

        }
        public virtual bool IsThreatTo(IControlable other)
        {
            return weaponInfo != null && weaponInfo.IsThreatTo(other);
        }


        public void AttachEffects(EffectAttachmentResult attachmentResult, EffectCollection effectCollection)
        {
            attachedEffectCollection.AttachEffects(attachmentResult, this, effectCollection);
        }
        protected virtual void OnEffectAttachment(GameResult gameResult)
        { }
        public void AddControler(GameResult gameResult, IControler controler)
        {
            controlers.Add(controler);
            if (created)
            {
                controler.OnCreation(gameResult, this);
            }
        }
        public void UpdateEffects(GameResult gameResult, float dt)
        {
            attachedEffectCollection.FilterEffects(dt);
            attachedEffectCollection.UpdateEffects(gameResult, dt);
            attachedEffectCollection.RemoveExpired();
        }
        public virtual void UpdateControlable(GameResult gameResult, float dt)
        {
            this.shipState.Update(dt);
            if (shipState.Health.IsEmpty)
            {
                Kill(gameResult);
                return;
            }
            currentControlInput = GetInput(dt);
            RunControlInput(gameResult, dt);
        }
        public virtual void RunControlInput(GameResult gameResult, float dt)
        {
            if (weaponInfo != null)
            {
                if (weaponInfo.EffectCollection.AllExhausted)
                {
                    Kill(gameResult);
                }
            }
            if (this.controlHandler != null)
            {
                this.controlHandler.HandleControlInput(dt);
            }
        }
        protected void OnDeath(GameResult gameResult)
        {
            if (died) { return; }
            died = true;
            this.IsExpired = true;
            this.controlableSounds.Death.Play();
            attachedEffectCollection.OnTargetDeath(gameResult);
            if (Killed != null)
            {
                Killed(this, new EventArgs());
            }
        }
        public virtual void Kill(GameResult gameResult)
        {
            OnDeath(gameResult);
        }
        public virtual void OnCreation(GameResult gameResult, FactionInfo factionInfo)
        {
            this.targetRetriever = new TargetRetriever();
            this.targetRetriever.OnSourceCreation(this);
            this.factionInfo = factionInfo;
            this.SetAllPositions();
            this.controlableSounds.Created.Play();
            controlers.OnCreation(gameResult, this);
            if (controlHandler != null)
            {
                controlHandler.OnCreation(this);
            }
            attachedEffectCollection.OnCreation(gameResult, this);
            created = true;
            gameResult.AddControlable(this);

        }

        public virtual void OnCollision(GameResult gameResult, IControlable collider)
        {
            weaponInfo.OnCollision(gameResult, collider);
        }
        public void OnCreation(GameResult gameResult, IShip source, IAction actionSource)
        {
            if (isWeaponCreated) { return; }
            isWeaponCreated = true;
            this.weaponInfo.OnCreation(gameResult, this, source, actionSource);
            this.OnCreation(gameResult, source.FactionInfo);
        }
        public void OnCreation(GameResult gameResult, IWeaponsLogic weaponInfo)
        {
            if (isWeaponCreated) { return; }
            isWeaponCreated = true;
            this.weaponInfo.OnCreation(gameResult, this, weaponInfo);
            this.OnCreation(gameResult, weaponInfo.Source.FactionInfo);
        }
        #endregion

    }
    [Serializable]
    public class ControlableWave : ImpulseWave, IControlable, ICleanable, ISolidWeapon
    {
        #region events
        public event EventHandler Killed;
        #endregion
        #region feilds
        protected TargetRetriever targetRetriever;
        protected ContFlags contFlags;
        protected bool canTarget = true;
        private bool created = false;
        protected ShipState shipState;
        protected ShipMovementInfo movementInfo;
        [NonSerialized]
        protected Vector2D direction = Vector2D.XAxis;
        protected EffectCollection attachedEffectCollection;
        protected ControllerList<IControler> controlers = new ControllerList<IControler>();
        protected IControlable target;
        protected IControlHandler controlHandler;
        protected FactionInfo factionInfo;
        protected ControlInput currentControlInput = new ControlInput(0);
        protected ControlableType controlableType = ControlableType.Debris;
        protected ControlableSounds controlableSounds;
        protected bool died = false;
        int[] colors;

        public int[] Colors
        {
            get { return colors; }
        }
        int primaryColor;

        protected IWeaponsLogic weaponInfo;


        public int PrimaryColor
        {
            get { return primaryColor; }
        }
        #endregion
        #region constructors
        public ControlableWave(
            LifeSpan lifeTime,
            float mass,
            PhysicsState physicsState, float initialRadius,
            float expansionRate,
            int[] colors,
            int primaryColor,
            ShipMovementInfo movementInfo,
            ShipState shipState,
            FactionInfo factionInfo,
            ControlableSounds controlableSounds)
            : base(
            lifeTime,
            mass,
            physicsState,
            initialRadius,
            expansionRate)
        {
            this.shipState = shipState;
            this.movementInfo = movementInfo;
            this.factionInfo = factionInfo;
            this.controlableSounds = controlableSounds;
            this.attachedEffectCollection = new EffectCollection();
            this.CollisionState.GenerateRayEvents = true;
            this.colors = colors;
            this.primaryColor = primaryColor;
        }

        public ControlableWave(
            LifeSpan lifeTime,
            float mass,
            PhysicsState physicsState,
            float initialRadius,
            float expansionRate,
            int[] colors,
            int primaryColor,
            ShipMovementInfo movementInfo,
            ShipState shipState,
            ControlableSounds controlableSounds,
            IWeaponsLogic weaponInfo)
                    : this(
            lifeTime,
            mass,
            physicsState, initialRadius,
            expansionRate,
            colors,
            primaryColor,
            movementInfo,
            shipState, null, controlableSounds)
        {
            this.weaponInfo = weaponInfo;
            this.CollisionState.GenerateCollisionEvents = true;
            this.CollisionState.GenerateContactEvents = true;
            this.controlableType = ControlableType.Weapon;
        }

        protected ControlableWave(ControlableWave copy)
            : base(copy)
        {
            this.Killed = Functions.Clone<EventHandler>(copy.Killed);
            this.shipState = new ShipState(copy.shipState);
            this.movementInfo = new ShipMovementInfo(copy.movementInfo);
            foreach (IControler controler in copy.controlers)
            {
                AddControler(null, (IControler)controler.Clone());
            }
            this.target = copy.target;
            this.ControlHandler = Functions.Clone<IControlHandler>(copy.ControlHandler);

            this.currentControlInput = copy.currentControlInput;
            this.direction = copy.direction;
            this.controlableType = copy.controlableType;
            this.controlableSounds = copy.controlableSounds;
            this.attachedEffectCollection = new EffectCollection(copy.attachedEffectCollection);
            this.contFlags = copy.contFlags;
            this.CollisionState.GenerateRayEvents = true;


            this.colors = copy.colors;
            this.primaryColor = copy.primaryColor;
            if (copy.weaponInfo != null)
            {
                this.weaponInfo = (IWeaponsLogic)copy.weaponInfo.Clone();
            }

        }
        #endregion
        #region properties
        public TargetRetriever TargetRetriever
        {
            get
            {
                return targetRetriever;
            }
        }

        public ContFlags UQMFlags
        {
            get
            {
                return contFlags;
            }
            set
            {
                this.contFlags = value;
            }
        }
        public bool IsInvisible
        {
            get
            {
                return (contFlags & ContFlags.Invisible) == ContFlags.Invisible || IsExpired;
            }
            set
            {
                if (IsInvisible ^ value)
                {
                    contFlags ^= ContFlags.Invisible;
                }
            }
        }
        public bool IsTargetable
        {
            get
            {
                return canTarget && !IsInvisible;
            }
            set
            {
                canTarget = value;
            }
        }
        public ControlableSounds ControlableSounds
        {
            get { return controlableSounds; }
            set { controlableSounds = value; }
        }
        public virtual IControlable Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
        public ShipState ShipState
        {
            get
            {
                return shipState;
            }
            set
            {
                shipState = value;
            }
        }
        public ShipMovementInfo MovementInfo
        {
            get
            {
                return movementInfo;
            }
            set
            {
                movementInfo = value;
            }
        }
        public virtual float DirectionAngle
        {
            get
            {
                return current.Position.Angular;
            }
        }
        public Vector2D DirectionVector
        {
            get { return direction; }
        }
        public IControlHandler ControlHandler
        {
            get
            {
                return controlHandler;
            }
            set
            {
                controlHandler = value;
                if (created && controlHandler != null)
                {
                    controlHandler.OnCreation(this);
                }
            }
        }
        public FactionInfo FactionInfo
        {
            get
            {
                return factionInfo;
            }
            set
            {
                factionInfo = value;
            }
        }
        public ControlInput CurrentControlInput
        {
            get
            {
                return currentControlInput;
            }
            set
            {
                currentControlInput = value;
            }
        }
        public ControlableType ControlableType
        {
            get
            {
                return controlableType;
            }
            set
            {
                controlableType = value;
            }
        }
        public EffectCollection AttachedEffectCollection
        {
            get { return attachedEffectCollection; }
        }
        public bool IsFireAndForget
        {
            get
            {
                foreach (IControler controler in controlers)
                {
                    if (controler.IsFireAndForget)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public IControler[] Controlers
        {
            get
            {
                return controlers.ToArray();
            }
        }
        public IWeaponsLogic WeaponInfo
        {
            get { return weaponInfo; }
        }

        #endregion
        #region methods

        public override object Clone()
        {
            return new ControlableWave(this);
        }
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
            direction = matrix.NormalMatrix * Vector2D.XAxis;

        }
        public virtual bool IsThreatTo(IControlable other)
        {
            return weaponInfo != null && weaponInfo.IsThreatTo(other);
        }
        public override void CalcAccelerations(Vector2D accelerationDueToGravity)
        {
            if ((this.Flags & BodyFlags.IgnoreGravity) != BodyFlags.IgnoreGravity &&
                (UQMFlags & ContFlags.CanDoGravityWhip) == ContFlags.CanDoGravityWhip)
            {
                if (accelerationDueToGravity == Vector2D.Zero)
                {
                    UQMFlags &= ~ContFlags.NoMaxSpeed;
                }
                else
                {
                    UQMFlags |= ContFlags.NoMaxSpeed;
                }
            }
            base.CalcAccelerations(accelerationDueToGravity);
        }
        public override void Update(float dt)
        {
            if (weaponInfo != null)
            {
                weaponInfo.Update(dt);
            }


            float speedSq = this.current.Velocity.Linear.MagnitudeSq;
            if (speedSq > TimeWarp.MaxAllowedSpeedSq)
            {
                this.current.Velocity.Linear *= (TimeWarp.MaxAllowedSpeed / MathHelper.Sqrt(speedSq));
            }

            this.attachedEffectCollection.AttachedUpdate(dt);
            controlers.Update(dt);
            if (controlHandler != null)
            {
                controlHandler.Update(dt);
            }
            base.Update(dt);
        }
        public override bool RemoveExpired()
        {
            controlers.RemoveExpired();
            if (controlHandler != null)
            {
                if (controlHandler.IsExpired)
                {
                    controlHandler = null;
                }
            }
            return base.RemoveExpired();
        }
        public override void UpdatePosition(float dt)
        {
            base.UpdatePosition(dt);
            direction = matrix.NormalMatrix * Vector2D.XAxis;

        }
        protected virtual ControlInput GetInput(float dt)
        {
            if (controlers.Count > 0)
            {
                return controlers.GetControlInput(dt, new ControlInput(0));
            }
            return null;
        }

        public void AttachEffects(EffectAttachmentResult attachmentResult, EffectCollection effectCollection)
        {
            attachedEffectCollection.AttachEffects(attachmentResult, this, effectCollection);
        }
        protected virtual void OnEffectAttachment(GameResult gameResult)
        { }
        public void AddControler(GameResult gameResult, IControler controler)
        {
            controlers.Add(controler);
            if (created)
            {
                controler.OnCreation(gameResult, this);
            }
        }
        public void UpdateEffects(GameResult gameResult, float dt)
        {
            attachedEffectCollection.FilterEffects(dt);
            attachedEffectCollection.UpdateEffects(gameResult, dt);
            attachedEffectCollection.RemoveExpired();
        }
        public virtual void UpdateControlable(GameResult gameResult, float dt)
        {
            this.shipState.Update(dt);
            if (shipState.Health.IsEmpty)
            {
                Kill(gameResult);
                return;
            }
            currentControlInput = GetInput(dt);
            RunControlInput(gameResult, dt);
        }
        public virtual void RunControlInput(GameResult gameResult, float dt)
        {
            if (weaponInfo != null)
            {
                if (weaponInfo.EffectCollection.AllExhausted)
                {
                    Kill(gameResult);
                }
            }
            if (this.controlHandler != null)
            {
                this.controlHandler.HandleControlInput(dt);
            }
        }
        protected void OnDeath(GameResult gameResult)
        {
            if (died) { return; }
            died = true;
            this.IsExpired = true;
            this.controlableSounds.Death.Play();
            attachedEffectCollection.OnTargetDeath(gameResult);
            if (Killed != null)
            {
                Killed(this, new EventArgs());
            }
        }
        public virtual void Kill(GameResult gameResult)
        {
            OnDeath(gameResult);
        }
        public virtual void OnCreation(GameResult gameResult, FactionInfo factionInfo)
        {
            this.targetRetriever = new TargetRetriever();
            this.targetRetriever.OnSourceCreation(this);
            this.factionInfo = factionInfo;
            this.SetAllPositions();
            this.controlableSounds.Created.Play();
            controlers.OnCreation(gameResult, this);
            if (controlHandler != null)
            {
                controlHandler.OnCreation(this);
            }
            attachedEffectCollection.OnCreation(gameResult, this);
            created = true;
            gameResult.AddControlable(this);
        }


        public virtual void OnCollision(GameResult gameResult, IControlable collider)
        {
            weaponInfo.OnCollision(gameResult, collider);
        }
        public void OnCreation(GameResult gameResult, IShip source, IAction actionSource)
        {
            this.weaponInfo.OnCreation(gameResult, this, source, actionSource);
            this.OnCreation(gameResult, source.FactionInfo);
        }
        public void OnCreation(GameResult gameResult, IWeaponsLogic weaponInfo)
        {
            this.weaponInfo.OnCreation(gameResult, this, weaponInfo);
            this.OnCreation(gameResult, weaponInfo.Source.FactionInfo);
        }

        #endregion
    }
}
