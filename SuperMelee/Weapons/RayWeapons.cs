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
using Physics2D.CollidableAreas;
using Physics2D.CollisionDetection;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
namespace ReMasters.SuperMelee.Weapons
{


    [Serializable]
    public abstract class RayWeapon : BaseCollidableWeapon, IRayWeapon
    {
        protected List<IRay2DEffect> ray2DEffects = new List<IRay2DEffect>();
        public RayWeapon(
            LifeSpan lifeTime,
            IWeaponsLogic weaponInfo)
            : base(lifeTime, weaponInfo)
        {
            this.lifeTime = lifeTime;
        }
        public RayWeapon(RayWeapon copy)
            : base(copy)
        { }
        public List<IRay2DEffect> Ray2DEffects
        {
            get { return ray2DEffects; }
        }
        protected void RemoveUnEffectable(List<ICollidableBody> collidables)
        {
            for (int pos = collidables.Count - 1; pos > -1; --pos)
            {
                IControlable controlable = collidables[pos] as IControlable;
                if (controlable != null)
                {
                    if (!weaponInfo.CanEffect(controlable))
                    {
                        collidables.RemoveAt(pos);
                    }
                }
            }
        }
        public virtual void HandlePossibleIntersections(float dt, List<ICollidableBody> collidables)
        {
            if (collidables != null)
            {
                PhysicsState before = null;
                RemoveUnEffectable(collidables);
                int length = ray2DEffects.Count;
                for (int pos = 0; pos < length; ++pos)
                {
                    RayICollidableBodyPair pair = GetIntersection(ray2DEffects[pos].RaySegment, collidables);
                    if (pair != null && pair.BestIntersectInfo.Intersects)
                    {
                        if (pair.ICollidableBody.CollisionState.GenerateRayEvents)
                        {
                            before = new PhysicsState(pair.ICollidableBody.Current);
                        }
                        ray2DEffects[pos].ApplyEffect(dt, pair);
                        if (pair.ICollidableBody.CollisionState.GenerateRayEvents)
                        {
                            pair.ICollidableBody.CollisionState.InterferenceInfos.Add(
                                new InterferenceInfo(new RayCollidableInterferenceInfo(this,
                                pair.RaySegment2D,
                                pair.BestIntersectInfo,
                                before,
                                new PhysicsState(pair.ICollidableBody.Current),
                                pair.ICollidableBody)));
                        }
                    }
                }
            }
        }
        protected RayICollidableBodyPair GetIntersection(RaySegment2D raySegment, List<ICollidableBody> collidables)
        {
            int collidablesCount = collidables.Count;
            if (collidablesCount == 0)
            {
                return null;
            }
            float Smallestdistance = raySegment.Length;
            RayICollidableBodyPair returnvalue = null;
            RayICollidableBodyPair pair;
            for (int pos = 0; pos < collidablesCount; ++pos)
            {
                pair = new RayICollidableBodyPair(raySegment, collidables[pos]);
                if (pair.TestIntersection())
                {
                    if (pair.BestIntersectInfo != null && Smallestdistance > pair.BestIntersectInfo.DistanceFromOrigin)
                    {
                        Smallestdistance = pair.BestIntersectInfo.DistanceFromOrigin;
                        returnvalue = pair;
                    }
                }
            }
            return returnvalue;
        }
        public override void Update(float dt)
        {
            this.weaponInfo.Update(dt);
            base.Update(dt);
        }
        public sealed override void OnCreation(GameResult gameResult, IShip source, IAction actionSource)
        {
            base.OnCreation(gameResult, source, actionSource);
            gameResult.AddCollidableArea(this);
        }
        public sealed override void OnCreation(GameResult gameResult, IWeaponsLogic weaponInfo)
        {
            base.OnCreation(gameResult, weaponInfo);
            gameResult.AddCollidableArea(this);
        }
    }
    [Serializable]
    public class TargetedRayWeapon : RayWeapon
    {
        protected float impulse;
        protected float distance;
        public TargetedRayWeapon(
            LifeSpan lifeTime, 
            IWeaponsLogic weaponInfo, 
            float impulse, 
            float distance)
            : base(lifeTime, weaponInfo)
        {
            this.impulse = impulse;
            this.distance = distance;
        }
        public TargetedRayWeapon(TargetedRayWeapon copy): base(copy)
        {
            this.impulse = copy.impulse;
            this.distance = copy.distance;
        }

        public override void CalcBoundingBox2D()
        {
            IControlable source = weaponInfo.LastBody;
            IControlable target = weaponInfo.Target;
            Vector2D start = source.Current.Position.Linear;
            Vector2D end = target.Current.Position.Linear;
            Vector2D Direction = end - start;
            float Distance = Direction.Magnitude;
            Direction = Direction * (1 / Distance);
            RaySegment2D segment = new RaySegment2D(start, Direction, distance);
            ray2DEffects.Add(new SelectiveImpulseRay(lifeTime, segment, impulse, BodyFlags.IgnoreGravity));
            this.boundingBox2D = segment.BoundingBox2D;
        }
        public override object Clone()
        {
            return new TargetedRayWeapon(this);
        }
    }
    [Serializable]
    public class LightningRayWeapon : TargetedRayWeapon
    {
        protected static Random rand = new Random();
        protected float displace;
        protected float detail;
            public LightningRayWeapon(
            LifeSpan lifeTime, 
            IWeaponsLogic weaponInfo, 
            float impulse, 
            float distance, 
            float displace, 
            float detail)
                : base(lifeTime, weaponInfo, impulse, distance)
        {
            this.displace = displace;
            this.detail = detail;
        }
        public LightningRayWeapon(LightningRayWeapon copy)
            : base(copy)
        {
            this.displace = copy.displace;
            this.detail = copy.detail;
        }
        public override void CalcBoundingBox2D()
        {
            IControlable source = weaponInfo.LastBody;
            IControlable target = weaponInfo.Target;
            Vector2D start = source.Current.Position.Linear;
            Vector2D end = target.Current.Position.Linear;
            Vector2D Direction = end - start;
            float Distance = Direction.Magnitude;
            if (Distance > distance)
            {
                Direction = Direction * (1 / Distance);
                end = Direction * distance + start;
            }
            LinkedList<Vector2D> points = new LinkedList<Vector2D>();
            points.AddFirst(start);
            points.AddLast(end);
            Lightning(points, points.First, points.Last, displace);
            CalcEffects(points);
            Vector2D[] pointsarr = new Vector2D[points.Count];
            points.CopyTo(pointsarr, 0);
            this.boundingBox2D = BoundingBox2D.FromVectors(pointsarr);
        }
        protected void Lightning(LinkedList<Vector2D> points, LinkedListNode<Vector2D> first, LinkedListNode<Vector2D> last, float displace)
        {
            if (displace < detail)
            {
                return;
            }
            else
            {
                Vector2D mid = (first.Value + last.Value) * .5f;
                mid.X += ((float)rand.NextDouble() - .5f) * displace;
                mid.Y += ((float)rand.NextDouble() - .5f) * displace;
                LinkedListNode<Vector2D> midnode = new LinkedListNode<Vector2D>(mid);
                points.AddAfter(first, midnode);
                displace *= .5f;
                Lightning(points, first, midnode, displace);
                Lightning(points, midnode, last, displace);
            }
        }
        protected void CalcEffects(LinkedList<Vector2D> points)
        {
            ray2DEffects = new List<IRay2DEffect>(points.Count - 1);
            Vector2D last = Vector2D.Zero;
            bool good = false;
            foreach (Vector2D point in points)
            {
                if (good)
                {
                    Vector2D Direction = point - last;
                    float Distance = Direction.Magnitude;
                    Direction = Direction * (1 / Distance);
                    RaySegment2D segment = new RaySegment2D(last, Direction, Distance);
                    ray2DEffects.Add(new SelectiveImpulseRay(lifeTime, segment, impulse, BodyFlags.IgnoreGravity));
                }
                good = true;
                last = point;
            }
        }
        public override void HandlePossibleIntersections(float dt, List<ICollidableBody> collidables)
        {
            if (collidables != null)
            {
                PhysicsState before = null;
                RemoveUnEffectable(collidables);
                int length = ray2DEffects.Count;
                for (int pos = 0; pos < length; ++pos)
                {
                    RayICollidableBodyPair pair = GetIntersection(ray2DEffects[pos].RaySegment, collidables);
                    if (pair != null && pair.BestIntersectInfo.Intersects)
                    {
                        if (pair.ICollidableBody.CollisionState.GenerateRayEvents)
                        {
                            before = new PhysicsState(pair.ICollidableBody.Current);
                        }
                        ray2DEffects[pos].ApplyEffect(dt, pair);
                        if (pair.ICollidableBody.CollisionState.GenerateRayEvents)
                        {
                            pair.ICollidableBody.CollisionState.InterferenceInfos.Add(
                                new InterferenceInfo(new RayCollidableInterferenceInfo(this,
                                pair.RaySegment2D,
                                pair.BestIntersectInfo,
                                before,
                                new PhysicsState(pair.ICollidableBody.Current),
                                pair.ICollidableBody)));
                        }
                        pos++;
                        if (pos < length)
                        {
                            ray2DEffects.RemoveRange(pos, length - pos);
                        }
                        break;
                    }
                }
            }
        }
        public override object Clone()
        {
            return new LightningRayWeapon(this);
        }

    }
    [Serializable]
    public class DirectedRayWeapon : RayWeapon, IDirectedRayWeapon
    {
        float[] impulses;
        float[] positions;
        float[] directions;
        float[] distances;
        public DirectedRayWeapon(
            LifeSpan lifeTime, 
            IWeaponsLogic weaponInfo, 
            float[] impulses, 
            float[] positions, 
            float[] directions, 
            float[] distances)
            : base(lifeTime, weaponInfo)
        {
            this.impulses = impulses;
            this.positions = positions;
            this.directions = directions;
            this.distances = distances;
        }
        public DirectedRayWeapon(DirectedRayWeapon copy):base(copy)
        {
            this.impulses = copy.impulses;
            this.positions = copy.positions;
            this.directions = copy.directions;
            this.distances = copy.distances;
        }
        public float[] Directions
        {
            get
            {
                return directions;
            }
        }
        public float[] Distances
        {
            get
            {
                return distances;
            }
        }
        public override void CalcBoundingBox2D()
        {
            ray2DEffects.Clear();
            int length = positions.Length;
            BoundingBox2D box = null;
            IControlable source = weaponInfo.LastBody;
            for (int pos = 0; pos < length; ++pos)
            {
                Vector2D start = Vector2D.Rotate(source.Current.Position.Angular + positions[pos], Vector2D.XAxis) * source.BoundingRadius + source.Current.Position.Linear;
                Vector2D Direction = Vector2D.Rotate(source.Current.Position.Angular + directions[pos], Vector2D.XAxis);
                RaySegment2D segment = new RaySegment2D(start, Direction, distances[pos]);
                if (box == null)
                {
                    box = segment.BoundingBox2D;
                }
                else
                {
                    box = BoundingBox2D.From2BoundingBox2Ds(box, segment.BoundingBox2D);
                }
                ray2DEffects.Add(new SelectiveImpulseRay(lifeTime, segment, impulses[pos], BodyFlags.IgnoreGravity));
            }
            this.boundingBox2D = box;
        }
        public override object Clone()
        {
            return new DirectedRayWeapon(this);
        }
    }
    [Serializable]
    public class MultiStageDirectedRayWeapon : RayWeapon, IDirectedRayWeapon
    {
        static LifeSpan GetLifeTime(float[] startTimes, float[] lifeTimes)
        {
            float total = 0;
            int count = startTimes.Length;
            for (int pos = 0; pos < count; ++pos)
            {
                total = Math.Max(total, lifeTimes[pos] + startTimes[pos]);
            }
            return new LifeSpan(total);
        }
        float[] impulses;
        float[] positions;
        float[] directions;
        float[] distances;
        float[] startTimes;
        float[] lifeTimes;
        float totalTime;
        public MultiStageDirectedRayWeapon(
            IWeaponsLogic weaponInfo,
            float[] impulses,
            float[] positions,
            float[] directions,
            float[] distances,
            float[] startTimes,
            float[] lifeTimes)
            : base(GetLifeTime(startTimes, lifeTimes), weaponInfo)
        {
            this.impulses = impulses;
            this.positions = positions;
            this.directions = directions;
            this.distances = distances;
            this.startTimes = startTimes;
            this.lifeTimes = lifeTimes;
            this.totalTime = this.lifeTime.TimeLeft;
        }
        public MultiStageDirectedRayWeapon(MultiStageDirectedRayWeapon copy)
            : base(copy)
        {
            this.impulses = copy.impulses;
            this.positions = copy.positions;
            this.directions = copy.directions;
            this.distances = copy.distances;
            this.startTimes = copy.startTimes;
            this.lifeTimes = copy.lifeTimes;
            this.totalTime = copy.totalTime;
        }
        public float[] Directions
        {
            get
            {
                return directions;
            }
        }
        public float[] Distances
        {
            get
            {
                return distances;
            }
        }
        public override void CalcBoundingBox2D()
        {
            ray2DEffects.Clear();
            float timepassed = totalTime - lifeTime.TimeLeft;
            int length = positions.Length;
            BoundingBox2D box = null;
            IControlable source = weaponInfo.LastBody;
            for (int pos = 0; pos < length; ++pos)
            {
                if (timepassed >= startTimes[pos] && timepassed <= startTimes[pos] + lifeTimes[pos])
                {
                    Vector2D start = Vector2D.Rotate(source.Current.Position.Angular + positions[pos], Vector2D.XAxis) * source.BoundingRadius + source.Current.Position.Linear;
                    Vector2D Direction = Vector2D.Rotate(source.Current.Position.Angular + directions[pos], Vector2D.XAxis);
                    RaySegment2D segment = new RaySegment2D(start, Direction, distances[pos]);
                    if (box == null)
                    {
                        box = segment.BoundingBox2D;
                    }
                    else
                    {
                        box = BoundingBox2D.From2BoundingBox2Ds(box, segment.BoundingBox2D);
                    }
                    ray2DEffects.Add(new SelectiveImpulseRay(lifeTime, segment, impulses[pos], BodyFlags.IgnoreGravity));
                }
            }
            if (box == null)
            {
                box = new BoundingBox2D(Vector2D.Zero, Vector2D.Zero);
            }
            this.boundingBox2D = box;
        }
        public override object Clone()
        {
            return new MultiStageDirectedRayWeapon(this);
        }
    }
    [Serializable]
    public class SelectiveImpulseRay : ImpulseRay
    {
        BodyFlags ignoreFlags;
        public SelectiveImpulseRay(
                    LifeSpan lifeTime, 
                    RaySegment2D raySegment, 
                    float impulse, 
                    BodyFlags ignoreFlags)
            : base(lifeTime,raySegment, impulse )
        {
            this.ignoreFlags = ignoreFlags;
        }
        public override void ApplyEffect(float dt, RayICollidableBodyPair pair)
        {
            displayLength = pair.BestIntersectInfo.DistanceFromOrigin;
            effectApplied = true;
            if ((pair.ICollidableBody.Flags & ignoreFlags) == BodyFlags.None)
            {
                Vector2D point = raySegment.GetPoint(pair.BestIntersectInfo) - pair.ICollidableBody.Current.Position.Linear;
                float effectImpulse = impulse * dt;
                pair.ICollidableBody.ApplyImpulse(point, effectImpulse * raySegment.Direction);
            }
        }
    }
}
