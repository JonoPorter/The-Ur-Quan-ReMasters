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
namespace ReMasters.SuperMelee.Actions
{





    [Serializable]
    public abstract class BaseAction : IAction
    {
        #region feilds
        protected IShip source;
        protected IControlable target;
        protected Bounded<float> delay;
        protected TargetingInfo targetableTypes;
        protected Costs costs;
        protected bool needsTarget;
        protected bool activated;
        protected ActionSounds actionSounds;
        protected IActionAIInfo aIInfo;

        #endregion
        #region constructors
        public BaseAction(
            Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            bool needsTarget,
            ActionSounds actionSounds)
        {
            this.source = null;
            this.target = null;
            this.delay = delay;
            this.targetableTypes = targetableTypes;
            this.costs = costs;
            this.needsTarget = needsTarget;
            this.actionSounds = actionSounds;
        }
        protected BaseAction(BaseAction copy)
        {
            this.source = copy.source;
            this.target = copy.target;
            this.delay = new Bounded<float>(copy.delay);
            this.targetableTypes = copy.targetableTypes;
            this.costs = copy.costs;
            this.needsTarget = copy.needsTarget;
            this.actionSounds = copy.actionSounds;
            this.aIInfo = Functions.Clone<IActionAIInfo>(copy.aIInfo);
        }
        #endregion
        #region properties

        public IActionAIInfo AIInfo
        {
            get { return aIInfo; }
            set { aIInfo = value; }
        }
        public ActionSounds ActionSounds
        {
            get { return actionSounds; }
            set { actionSounds = value; }
        }
        public TargetingInfo TargetableTypes
        {
            get
            {
                return targetableTypes;
            }
        }
        public Bounded<float> Delay
        {
            get
            {
                return delay;
            }
        }
        public bool IsActive
        {
            get
            {
                return activated;
            }
        }
        public bool Ready
        {
            get
            {
                return delay.IsFull;
            }
        }
        public IControlable Target
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
        public IShip Source
        {
            get
            {
                return source;
            }
        }
        public Costs Costs
        {
            get
            {
                return costs;
            }
            set
            {
                costs = value;
            }
        }
        public bool NeedsTarget
        {
            get
            {
                return needsTarget;
            }
        }
        #endregion
        #region methods
        protected bool AcquireTarget()
        {
            if (needsTarget)
            {
                if (target == null || !target.IsTargetable || !this.targetableTypes.MeetsRequirements( FactionInfo.GetTargetingType(   source,target)) )
                {
                    target = source.TargetRetriever.Next(this.targetableTypes);
                    if (target == null || !target.IsTargetable)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        protected abstract bool OnActivated(ActionResult actionResult, float dt);
        protected virtual bool OnRunning(ActionResult actionResult, float dt) { return true; }
        protected virtual bool OnDeActivated(ActionResult actionResult, float dt) { return true; }
        public void OnAction(GameResult gameResult, float dt)
        {
            ShipStateChange cost;
            ActionResult actionResult = new ActionResult(gameResult);
            if (activated && source.ShipState.MeetsCost(cost = (-costs.RunningCost * dt)) && AcquireTarget())
            {
                if (OnRunning(actionResult, dt))
                {
                    if (actionResult.ApplyCosts)
                    {
                        source.ShipState.Change(cost);
                    }
                    if (actionResult.PlaySound)
                    {
                        actionSounds.Running.Play(dt);
                    }
                }
                else
                {
                    activated = false;
                }
                return;
            }
            else if (Ready && source.ShipState.MeetsCost(cost = (-costs.ActivationCost)) && AcquireTarget())
            {
                if (OnActivated(actionResult, dt))
                {
                    if (actionResult.ResetDelay)
                    {
                        delay.Empty();
                    }
                    if (actionResult.ApplyCosts)
                    {
                        source.ShipState.Change(cost);
                    }
                    if (actionResult.PlaySound)
                    {
                        actionSounds.Activated.Play();
                    }
                    if (costs.RunningCost != null)
                    {
                        activated = true;
                    }
                }
                return;
            }
            else if (activated && costs.DeActivationCost != null && source.ShipState.MeetsCost(cost = (-costs.DeActivationCost)) && AcquireTarget())
            {
                if (OnDeActivated(actionResult, dt))
                {
                    if (actionResult.ApplyCosts)
                    {
                        actionSounds.DeActivated.Play();
                    }
                    if (actionResult.PlaySound)
                    {
                        source.ShipState.Change(cost);
                    }
                }
                activated = false;
                return;
            }
            activated = false;
        }
        public void OnAfterAction(GameResult gameResult, float dt)
        {
            ShipStateChange cost;
            if (activated && costs.DeActivationCost != null && 
                source.ShipState.MeetsCost(cost = (-costs.DeActivationCost)) &&
                AcquireTarget())
            {
                ActionResult actionResult = new ActionResult(gameResult);
                activated = false;
                if (OnDeActivated(actionResult, dt))
                {
                    if (actionResult.PlaySound)
                    {
                        actionSounds.DeActivated.Play();
                    }
                    if (actionResult.ApplyCosts)
                    {
                        source.ShipState.Change(cost);
                    }
                }
            }
            else
            {
                activated = false;
            }
        }
        public virtual void OnSourceCreation(GameResult gameResult, IShip source)
        {
            this.source = source;
            if (aIInfo != null)
            {
                aIInfo.OnSourceCreation(gameResult,source, this);
            }
        }


        public virtual void Update(float dt)
        {
            delay.Value += dt;
        }
        public abstract object Clone();
        public bool CanTarget(IControlable other)
        {
            return targetableTypes.IsAll ||
                targetableTypes.MeetsRequirements(FactionInfo.GetTargetingType(source, other));
        }
        #endregion
    }
}