using System;
using System.Collections.Generic;
using Physics2D;
using AdvanceMath;
using AdvanceSystem;

namespace ReMasters.SuperMelee.Controlers
{
    public class SimpleAIControler : MissileControler
    {
        List<IControlable> obstacles = new List<IControlable>();
        public SimpleAIControler()
            : base(new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None))
        {
        }
        public SimpleAIControler(SimpleAIControler copy) : base(copy) { }
        public override ControlInput GetControlInput(float dt, ControlInput original)
        {
            foreach (IControlable obstacle in obstacles)
            {
                if (obstacle.IsThreatTo(this.host))
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
                }
            }
            if (obstacles.Contains(target))
            {
                original[InputAction.Action] = true;
                original.ActiveActions[0] = true;
            }
            return base.GetControlInput(dt, original);
        }
        public override void OnCreation(GameResult gameResult, IControlable host)
        {
            base.OnCreation(gameResult, host);
            CollidableBubble bubble = new CollidableBubble(1000, host, obstacles);
            gameResult.AddCollidableArea(bubble);
            this.lifeTime = new LifeSpan();
        }
        public override object Clone()
        {
            return new SimpleAIControler(this);
        }
    }
}
