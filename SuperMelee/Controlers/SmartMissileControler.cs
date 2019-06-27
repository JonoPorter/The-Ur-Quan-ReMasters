using System;
using System.Collections.Generic;
using Physics2D;
using AdvanceMath;
using AdvanceSystem;

namespace ReMasters.SuperMelee.Controlers
{
    [Serializable]
    public class SmartMissileControler : MissileControler
    {
        bool overideDesired = false;
        float desiredAngle;
        List<IControlable> obstacles = new List<IControlable>();
        public SmartMissileControler(TargetingInfo targetingInfo)
            : base(targetingInfo)
        {
        }
        public SmartMissileControler(SmartMissileControler copy) : base(copy) { }
        
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
            overideDesired = false;
            foreach (IControlable obstacle in obstacles)
            {

                if (obstacle.IsThreatTo(this.host))
                {
                    Vector2D dir2 = obstacle.Current.Position.Linear - host.Current.Position.Linear;
                    desiredAngle = (dir2 ^ (host.DirectionVector ^ dir2)).Angle;
                    overideDesired = true;
                    break;
                }

                /*if (obstacle.IsThreatTo(this.host))
                {
                    original.ThrustPercent = 1;
                    original[InputAction.MoveForward] = true;

                    Vector2D dir = Vector2D.Normalize(obstacle.Current.Position.Linear - host.Current.Position.Linear);
                    if (host.DirectionVector * dir > 0)
                    {
                        original.TorquePercent = 1;
                        if (host.DirectionVector.LeftHandNormal * dir > 0)
                        {
                            original[InputAction.RotateLeft] = true;
                        }
                        else
                        {
                            original[InputAction.RotateRight] = true;
                        }
                    }
                    return original;
                }*/
            }
            return base.GetControlInput(dt, original);
        }
        public override void OnCreation(GameResult gameResult, IControlable host)
        {
            base.OnCreation(gameResult,host);
            CollidableBubble bubble = new CollidableBubble(1000, host, obstacles);
            gameResult.AddCollidableArea(bubble);
        }
        public override object Clone()
        {
            return new SmartMissileControler(this);
        }
    }

}
