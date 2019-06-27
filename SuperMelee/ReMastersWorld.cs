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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Physics2D;
namespace ReMasters.SuperMelee
{
    [Serializable]
    public class SuperMeleeWorld : World2D<IControlable,ICollidableArea,IJoint>
    {
        public SuperMeleeWorld():base()
        {
            this.Collision += HandleWeaponCollisions;
            this.doSeperateIntersecting = true;
            this.contactBroadPhaseOnlyOnce = true;
            this.seperatingFactor = 1.6f;
            this.collisionStepsCount = 5;
            this.contactStepsCount = 5;
            Physics.MetersPerDistanceUnit = 10000;
            TargetRetriever.SetPossibleTargets(this.collidableBodies);
            this.rayTraceVelocityTolerance = 50;
            
        }
        public override void AddICollidableBody(IControlable collidableBody)
        {
            base.AddICollidableBody(collidableBody);
            //HandleActionEvents(collidableBody.OnCreation());
        }
        protected void HandleWeaponCollisions(object sender, CollisionEventArgs args)
        {
            GameResult gameResult = new GameResult();
            List<ICollidableBody> Handled = new List<ICollidableBody>();
            foreach (InterferenceInfo info in args.InterferenceInfos)
            {
                if (info.InterferenceType == InterferenceType.CollidablePair)
                {
                    if (!Handled.Contains(info.CollidablePairInfo.ICollidableBody2))
                    {
                        Handled.Add(info.CollidablePairInfo.ICollidableBody2);
                        IWeapon weapon = info.CollidablePairInfo.ICollidableBody1 as IWeapon;
                        IControlable victim = info.CollidablePairInfo.ICollidableBody2 as IControlable;
                        if (weapon != null && victim != null)
                        {
                            weapon.OnCollision(gameResult, victim);
                        }
                    }
                }
                else
                {
                    IWeapon weapon = info.RayCollidableInfo.Area as IWeapon;
                    IControlable victim = info.RayCollidableInfo.Collidable as IControlable;
                    if (weapon != null && victim != null)
                    {
                        weapon.OnCollision(gameResult, victim);
                    }
                }
            }
            HandleActionEvents(gameResult);
        }
        protected override void UpdateIUpdatables(float dt)
        {
            base.UpdateIUpdatables(dt);
            GameResult gameResult = new GameResult() ;
            int lenght = this.collidableBodies.Count;
            for (int pos = 0; pos < lenght; ++pos)
            {
                collidableBodies[pos].UpdateControlable(gameResult,dt);
            }
            for (int pos = 0; pos < lenght; ++pos)
            {
                 collidableBodies[pos].UpdateEffects(gameResult,dt);
            }
            HandleActionEvents(gameResult);
        }
        public void HandleActionEvents(GameResult result)
        {
            if (result == null)
            {
                return;
            }
            int count = result.Controlables.Count;

            if (this.inUpdate)
            {
                lock (PendingSyncRoot)
                {
                    this.pendingCollidableBodies.AddRange(result.Controlables);
                    this.pendingCollidableAreas.AddRange(result.CollidableAreas);
                    this.pendingJoints.AddRange(result.Joints);
                }
            }
            else
            {
                lock (this)
                {
                    foreach (IControlable c in result.Controlables)
                    {
                        this.collidableBodies.Add(c);
                        if (c.IgnoreInfo.IsCollidable)
                        {
                            this.detector.AddICollidableBody(c);
                        }
                    }
                    foreach (ICollidableArea area in result.CollidableAreas)
                    {
                        this.collidableAreas.Add(area);
                        if (area.IgnoreInfo.IsCollidable)
                        {
                            this.detector.AddICollidableArea(area);
                        }
                    }
                    this.joints.AddRange(result.Joints);
                }
            }
        }
        public override void OnDeserialization(object sender)
        {
            TargetRetriever.SetPossibleTargets(collidableBodies);
            base.OnDeserialization(sender);
            this.Collision +=  HandleWeaponCollisions;
        }
    }
}
