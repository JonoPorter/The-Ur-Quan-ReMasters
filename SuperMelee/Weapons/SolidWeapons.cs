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
using Physics2D.CollisionDetection;
using Physics2D.CollidableBodies;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
namespace ReMasters.SuperMelee.Weapons
{

    [Serializable]
    public class SpawningSolidWeapon : Controlable
    {
        protected ISolidWeapon spawn;
        protected bool diesOnSpawn;
        bool spawned;
        public SpawningSolidWeapon(
            LifeSpan lifeTime,
            MassInertia massInfo,
            PhysicsState physicsState,
            BodyFlags flags, ICollidableBodyPart[] collidableParts,
            ShipMovementInfo movementInfo,
            ShipState shipState,
            ControlableSounds controlableSounds,
            IWeaponsLogic weaponInfo,
            ISolidWeapon spawn,
            bool diesOnSpawn)
            : base(
            lifeTime,
            massInfo,
            physicsState,
            flags, collidableParts,
            movementInfo,
            shipState,
            controlableSounds,
            weaponInfo)
        {
            this.spawn = spawn;
            this.diesOnSpawn = diesOnSpawn;
        }
        public SpawningSolidWeapon(
            LifeSpan lifeTime,
            PhysicsState physicsState,
            BodyFlags flags,
            RigidBodyTemplate template,
            ShipMovementInfo movementInfo,
            ShipState shipState,
            ControlableSounds controlableSounds,
            IWeaponsLogic weaponInfo,
            ISolidWeapon spawn,
            bool diesOnSpawn)
            : base(
            lifeTime,
            physicsState,
            flags, template,
            movementInfo,
            shipState,
            controlableSounds,
            weaponInfo)
        {
            this.spawn = spawn;
            this.diesOnSpawn = diesOnSpawn;
        }
        public SpawningSolidWeapon(SpawningSolidWeapon copy)
            : base(copy)
        {
            this.spawn = (ISolidWeapon)copy.spawn.Clone();
            this.diesOnSpawn = copy.diesOnSpawn;
        }
        protected void SpawnWeapon(GameResult gameResult, IControlable collider)
        {
            if (spawned)
            {
                return;
            }
            spawned = true;
            PhysicsState physicsState = new PhysicsState(this.Good);
            physicsState.ForceAccumulator = ALVector2D.Zero;
            physicsState.Acceleration = ALVector2D.Zero;
            if (collider != null)
            {
                physicsState.Velocity.Linear = collider.Good.Velocity.Linear;
            }
            else
            {
                physicsState.Velocity.Linear = this.current.Velocity.Linear;
            }

            ISolidWeapon newWeapon = (ISolidWeapon)spawn.Clone();
            newWeapon.Current.Set(physicsState);

            newWeapon.SetAllPositions();
            newWeapon.OnCreation(gameResult, this.weaponInfo);
            if (diesOnSpawn)
            {
                this.Kill(gameResult);
            }
        }
        public override void Kill(GameResult gameResult)
        {
            if (!diesOnSpawn)
            {
                SpawnWeapon(gameResult, null);
            }
            base.Kill(gameResult);
        }
        protected override void OnEffectAttachment(GameResult gameResult)
        {
            SpawnWeapon(gameResult, null);
        }
        public override void OnCollision(GameResult gameResult, IControlable collider)
        {
            base.OnCollision(gameResult,collider);
            if (diesOnSpawn )
            {
                SpawnWeapon(gameResult, collider);
            }
        }
        public override object Clone()
        {
            return new SpawningSolidWeapon(this);
        }
    }
}
