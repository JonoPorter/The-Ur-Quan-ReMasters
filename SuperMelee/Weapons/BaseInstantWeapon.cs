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
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
namespace ReMasters.SuperMelee.Weapons
{
    [Serializable]
    public class InstantWeapon : BaseTimed, IWeapon
    {
        protected IWeaponsLogic weaponInfo;
        protected InstantWeapon() { }
        public InstantWeapon(IWeaponsLogic weaponInfo)
        {
            this.weaponInfo = weaponInfo;
        }
        public InstantWeapon(LifeSpan lifeTime, IWeaponsLogic weaponInfo)
            : base(lifeTime)
        {
            this.weaponInfo = weaponInfo;
        }
        public InstantWeapon(InstantWeapon copy)
            : base(copy)
        {
            this.weaponInfo = (IWeaponsLogic)copy.weaponInfo.Clone();
        }
        public IWeaponsLogic WeaponInfo
        {
            get { return weaponInfo; }
        }
        public virtual void OnCollision(GameResult gameResult, IControlable collider)
        {
            weaponInfo.OnCollision(gameResult, collider);
        }
        public void OnCreation(GameResult gameResult, IShip source, IAction actionSource)
        {
            this.weaponInfo.OnCreation(gameResult, this, source, actionSource);
        }
        public void OnCreation(GameResult gameResult, IWeaponsLogic weaponInfo)
        {
            this.weaponInfo.OnCreation(gameResult, this, weaponInfo);
        }
        public virtual object Clone()
        {
            return new InstantWeapon(this);
        }
    }

    [Serializable]
    public abstract class BaseCollidableWeapon : BaseCollidable, IWeapon
    {
        protected IWeaponsLogic weaponInfo;
        protected BaseCollidableWeapon() { }
        public BaseCollidableWeapon(IWeaponsLogic weaponInfo)
        {
            this.weaponInfo = weaponInfo;
        }
        public BaseCollidableWeapon(LifeSpan lifeTime, IWeaponsLogic weaponInfo)
            : base(lifeTime)
        {
            this.weaponInfo = weaponInfo;
        }
        public BaseCollidableWeapon(BaseCollidableWeapon copy)
            : base(copy)
        {
            this.weaponInfo = (IWeaponsLogic)copy.weaponInfo.Clone();
        }
        public IWeaponsLogic WeaponInfo
        {
            get { return weaponInfo; }
        }
        public virtual void OnCollision(GameResult gameResult, IControlable collider)
        {
            weaponInfo.OnCollision(gameResult, collider);
        }
        public virtual void OnCreation(GameResult gameResult, IShip source, IAction actionSource)
        {
            this.weaponInfo.OnCreation(gameResult, this, source, actionSource);
        }
        public virtual void OnCreation(GameResult gameResult, IWeaponsLogic weaponInfo)
        {
            this.weaponInfo.OnCreation(gameResult, this, weaponInfo);
        }
        public abstract object Clone();
    }
}