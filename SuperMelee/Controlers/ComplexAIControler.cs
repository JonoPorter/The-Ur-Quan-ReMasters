using System;
using System.Collections.Generic;
using Physics2D;
using AdvanceMath;
using AdvanceSystem;

namespace ReMasters.SuperMelee.Controlers
{
    [Serializable]
    public class ComplexAIControler : MissileControler
    {
        List<IControlable> obstacles = new List<IControlable>();
        IShip shipHost;
        float desiredAngle;
        bool overideDesired = false;
        public ComplexAIControler()
            : base(new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None))
        {
        }
        public ComplexAIControler(ComplexAIControler copy) : base(copy) { }
        protected override bool CheckTarget()
        {
            return base.CheckTarget() && !overideDesired;
        }
        static Random rand = new Random();
        protected override float GetDesiredAngle()
        {
            if (overideDesired)
            {
                return desiredAngle;
            }
            return base.GetDesiredAngle();
        }
        public override ControlInput GetControlInput(float dt, ControlInput original)
        {

            if (rand.Next(0, 99) == 1)
            {
                base.target = null;
                base.CheckTarget();
            }
            overideDesired = false;


            IControlable[] obstacles = this.obstacles.ToArray();
            bool[] threats = new bool[obstacles.Length];
            for (int pos = 0; pos < obstacles.Length; ++pos)
            {
                threats[pos] = obstacles[pos].IsThreatTo(this.host);
            }
            AIStateInfo stateInfo = null;
            int actionsCount = Math.Min(3, shipHost.Actions.Count);
            if (target != null)
            {
                stateInfo = new AIStateInfo(
                    target,
                    target.Current.Position.Linear - host.Current.Position.Linear,
                    obstacles,
                    threats
                    );
                for (int pos = 0; pos < actionsCount; ++pos)
                {
                    IActionAIInfo actionAIInfo = shipHost.Actions[pos].AIInfo;
                    if (actionAIInfo != null)
                    {
                        actionAIInfo.Update(stateInfo);
                        if (!overideDesired && actionAIInfo.ShouldAim)
                        {
                            desiredAngle = actionAIInfo.AimAngle;
                            overideDesired = true;
                        }
                        if (actionAIInfo.ShouldActivate)
                        {
                            original[InputAction.Action] = true;
                            original.ActiveActions[pos] = true;
                        }
                    }
                }
            }
            if (!overideDesired)
            {
                for (int pos = 0; pos < obstacles.Length; ++pos)
                {
                    if (threats[pos])
                    {
                        IControlable obstacle = obstacles[pos];
                        Vector2D dir2 = obstacle.Current.Position.Linear - host.Current.Position.Linear;
                        if ((obstacle.ControlableType & ControlableType.Ship) == ControlableType.Ship)
                        {
                            desiredAngle = (-host.DirectionVector).Angle;
                            overideDesired = true;
                        }
                        else
                        {
                            desiredAngle = (dir2 ^ (host.DirectionVector ^ dir2)).Angle;
                            overideDesired = true;
                        }
                        break;
                    }
                }
            }
            if (target != null)
            {
                IShipAIInfo shipAIInfo = shipHost.AIInfo;
                if (shipAIInfo != null)
                {
                    shipAIInfo.Update(stateInfo);
                    if (shipAIInfo.ShouldSetAngle)
                    {
                        overideDesired = true;
                        desiredAngle = shipAIInfo.DesiredAngle;
                    }
                }
            }
            else if (!overideDesired)
            {
                if (host.Current.Velocity.Linear.Magnitude > host.MovementInfo.MaxLinearVelocity.Value * .05f)
                {
                    desiredAngle = (-host.Current.Velocity.Linear).Angle;
                    overideDesired = true;
                    original = base.GetControlInput(dt, original);
                    if (MathHelper.Abs(MathHelper.GetAngleDifference(desiredAngle, host.DirectionAngle)) < .1f)
                    {
                        original.ThrustPercent = 1;
                    }
                    else
                    {
                        original.ThrustPercent = 0;
                    }
                    return original;
                }
            }
            return base.GetControlInput(dt, original);
        }
        public override void OnCreation(GameResult gameResult, IControlable host)
        {
            base.OnCreation(gameResult, host);
            this.shipHost = (IShip)host;

            float SensorRadius = 1000;
            IShipAIInfo shipAIInfo = shipHost.AIInfo;
            if (shipAIInfo != null)
            {
                SensorRadius = shipAIInfo.SensorRadius;
            }
            CollidableBubble bubble = new CollidableBubble(SensorRadius, host, obstacles);
            gameResult.AddCollidableArea(bubble);
            this.lifeTime = new LifeSpan();
        }
        public override object Clone()
        {
            return new ComplexAIControler(this);
        }
    }
}