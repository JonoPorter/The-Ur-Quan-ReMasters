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
using Physics2D;
using Physics2D.CollidableBodies;
using Physics2D.Collections;
using System.Collections;
using System.Collections.Generic;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using Color = System.Drawing.Color;
using ReMasters.SuperMelee.Ships;
using System.Security.Permissions;

namespace ReMasters.SuperMelee
{
    public static class Logic
    {
        public static bool TrySolveTimeToWaveIntercept(Vector2D Position1, Vector2D Position2, Vector2D relativeVelocity, float waveSpeed, out float time)
        {
            float relativeSpeedSq = Vector2D.GetMagnitudeSq(relativeVelocity);
            if (relativeSpeedSq > 0)
            {
                float waveSpeedSq = waveSpeed * waveSpeed;
                Vector2D difference = Position2 - Position1;
                float distanceSq = Vector2D.GetMagnitudeSq(difference);
                float relativeSpeed = MathHelper.Sqrt(relativeSpeedSq);
                Vector2D direction = relativeVelocity * (1 / relativeSpeed);
                float distanceAlongDirection = direction * difference;
                float minus, plus;
                if (MathHelper.TrySolveQuadratic(
                    /*A=*/ relativeSpeedSq - waveSpeedSq,
                    /*B=*/ 2 * distanceAlongDirection * relativeSpeed,
                    /*C=*/ distanceSq,
                    out plus, out minus))
                {
                    if (plus < minus)
                    {
                        if (plus > 0)
                        {
                            time = plus;
                        }
                        else
                        {
                            time = minus;
                        }
                    }
                    else
                    {
                        if (minus > 0)
                        {
                            time = minus;
                        }
                        else
                        {
                            time = plus;
                        }
                    }
                    return true;
                }

            }
            time = 0;
            return false;
        }


        public static bool TrySolveInterceptPoint(Vector2D sourcePosition, Vector2D targetPosition, Vector2D relativeVelocity, float waveSpeed, out  Vector2D point)
        {
            float time;
            if (Logic.TrySolveTimeToWaveIntercept(
                sourcePosition,
                targetPosition,
                relativeVelocity,
                waveSpeed,
                out time))
            {
                point = targetPosition + relativeVelocity * time;
                return true;
            }
            else
            {
                point = targetPosition;
                return false;
            }
        }
        public static bool TrySolveInterceptAngle(Vector2D sourcePosition, Vector2D targetPosition, Vector2D relativeVelocity, float waveSpeed, out  float  angle)
        {
            Vector2D IP;
            if (Logic.TrySolveInterceptPoint(
                sourcePosition,
                targetPosition,
                relativeVelocity,
                waveSpeed,
                out IP))
            {
                Vector2D DifferenceL = IP - sourcePosition;
                angle = DifferenceL.Angle;
                return true;
            }
            else
            {
                Vector2D DifferenceL = IP - sourcePosition;
                angle = DifferenceL.Angle;
                return  false;
            }
        }

        public static bool InArc(float centerAngle, float currentAngle, float distance, float radius,float tolerance)
        {
            float anglediff = MathHelper.GetAngleDifference(centerAngle, currentAngle);
            float tanthing = MathHelper.Atan(radius / distance);
            return
                MathHelper.Abs(anglediff) 
                <
                MathHelper.Abs(tanthing) + tolerance;
        }
    }
}