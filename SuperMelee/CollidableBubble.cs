#region GPL License
/*
 * The Ur-Quan ReMasters is a recreation of The Ur-Quan Masters in C#.
 * For the latest info, see http://sourceforge.net/projects/sc2-remake/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a other of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 
 */
#endregion

using System;
using Physics2D;
using System.Collections;
using System.Collections.Generic;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using Color = System.Drawing.Color;
namespace ReMasters.SuperMelee
{
    [Serializable]
    public class CollidableBubble : BaseCollidable, ICollidableArea
    {
        ICollidableBody host;
        float range;
        List<IControlable> collidables;
        public CollidableBubble(float range, IControlable host, List<IControlable> collidables)
            : base(new LifeSpan(host.LifeTime))
        {
            this.host = host;
            this.range = range;
            this.collidables = collidables;
        }
        #region ICollidableArea Members
        public override void CalcBoundingBox2D()
        {
            Vector2D position1 = host.Current.Position.Linear;
            Vector2D position2 = position1;
            position1.X += range;
            position1.Y += range;
            position2.X -= range;
            position2.Y -= range;
            this.boundingBox2D = new BoundingBox2D(position1, position2);
        }
        class CDist : IComparable<CDist>
        {
            public IControlable Controlable;
            public float DistanceSq;
            #region IComparable<CDist> Members
            public int CompareTo(CDist other)
            {
                return DistanceSq.CompareTo(other.DistanceSq);
            }
            #endregion
        }
        public void HandlePossibleIntersections(float dt, List<ICollidableBody> pairs) 
        {
            pairs.Remove(host);
            collidables.Clear();
            List<CDist> tmp = new List<CDist>();
            foreach (ICollidableBody col in pairs)
            {
                IControlable c = col as IControlable;
                if (c != null && c.IsTargetable)
                {
                    float truerange = range + c.BoundingRadius + host.BoundingRadius;
                    float distanceSq = (host.Current.Position.Linear - c.Current.Position.Linear).MagnitudeSq;
                    if ((truerange * truerange) >= distanceSq)
                    {
                        CDist d = new CDist();
                        d.Controlable = c;
                        d.DistanceSq = distanceSq;
                        tmp.Add(d);
                    }
                }
            }
            tmp.Sort();
            foreach (CDist d in tmp)
            {
                collidables.Add(d.Controlable);
            }
        }
        #endregion
    }
}
