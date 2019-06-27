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
using System.Drawing;
using Physics2D;
using AdvanceMath; using AdvanceSystem;
namespace ReMasters.SuperMelee.Ships
{
    
    public class TimeWarp
    {
        public const float MaxAllowedSpeed = 1000;
        public const float MaxAllowedSpeedSq = MaxAllowedSpeed*MaxAllowedSpeed;
        public const float TimeRatio = 50;
        public const float AngularAcceleration = 25;
        public const float VelocityTimeRatio = 50;
        //public const float TimeRatio = 1;
        public const float DistanceRatio = .4f;
        public static readonly Coefficients Coefficients = new Coefficients(1, .2f, .2f);
        public static float ScaleTurning(float TurnRate)
        {
            return (float)((MathHelper.TWO_PI / 16) / (TurnRate + 1.0) / TimeWarp.VelocityTimeRatio) * 1000;
        }
        public static float ScaleTurningOLD(float TurnRate)
        {
            return (float)((MathHelper.TWO_PI / 16) / (TurnRate + 1.0) / TimeWarp.VelocityTimeRatio) * 1000;
        }
        public static float ScaleVelocity(float velocity)
        {
            return ((velocity * DistanceRatio) / VelocityTimeRatio) * 1000;
        }
        public static float ScaleAcceleration(float acceleration, float raw_hotspot_rate)
        {
            return ((10000 * DistanceRatio) / VelocityTimeRatio) * acceleration;
        }
        /*public static float ScaleAcceleration(float acceleration, float raw_hotspot_rate)
        {
            return ((20000 * DistanceRatio) / VelocityTimeRatio) * acceleration;
        }*/
        public static float ScaleRange(float range)
        {
            return range * 40;
        }
        public static float RangeToTime(float range, float velocity)
        {
            return ScaleRange(range) / ScaleVelocity(velocity);
        }
        public static float RateToTime(float rate)
        {
            return (float)(0.05 * rate + 0.05);
        }
        public static float RechargeRateToPerSeconds(float rechargerate,float amount)
        {
            return amount / (RateToTime(rechargerate));
        }


        public static int[] DefaultExposionColors = new int[] { Color.Red.ToArgb(), Color.Red.ToArgb(), Color.Red.ToArgb(), Color.Yellow.ToArgb() };
        public static int DefaultExplosionPrimaryColor = Color.Orange.ToArgb();

    }
}
