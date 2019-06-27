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

namespace ReMasters.SuperMelee
{
    /// <summary>
    /// How a event can return stuff that needs to be added.
    /// such as missltes or sparks.
    /// </summary>
    [Serializable]
    public class GameResult
    {
        private List<IControlable> controlables;
        private List<ICollidableArea> collidableAreas;
        private List<IJoint> joints;




        public GameResult()
        {
            controlables = new List<IControlable>();
            collidableAreas = new List<ICollidableArea>();
            joints = new List<IJoint>();
        }
        protected GameResult(GameResult result)
        {
            this.controlables = result.controlables;
            this.collidableAreas = result.collidableAreas;
            this.joints = result.joints;
        }
        public List<IControlable> Controlables
        {
            get { return controlables; }
        }
        public List<ICollidableArea> CollidableAreas
        {
            get { return collidableAreas; }
        }
        public List<IJoint> Joints
        {
            get { return joints; }
        }

        public void AddControlable(IControlable controlable)
        {
            this.controlables.Add(controlable);
        }
        public void AddCollidableArea(ICollidableArea collidableArea)
        {
            this.collidableAreas.Add(collidableArea);
        }
        public void AddJoint(IJoint joint)
        {
            this.joints.Add(joint);
        }
        public void Clear()
        {
            controlables.Clear();
            collidableAreas.Clear();
            joints.Clear();
        }
        public bool IsEmpty
        {
            get
            {
                return controlables.Count == 0 && collidableAreas.Count == 0 && joints.Count == 0;
            }
        }
    }
    public sealed class ActionResult : GameResult
    {
        public bool ApplyCosts = true;
        public bool ResetDelay = true;
        public bool PlaySound = true;
        public ActionResult(GameResult gameResult)
            : base(gameResult)
        { }
    }


    [Serializable]
    public sealed class EffectAttachmentResult : GameResult
    {
        public bool EffectAttached = false;
        public bool WeaponIsExpired = false;
        public EffectAttachmentResult(GameResult gameResult)
            : base(gameResult)
        { }
    }


}