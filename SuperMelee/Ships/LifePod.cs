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
using Physics2D;
using Physics2D.CollidableBodies;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using ReMasters.SuperMelee;
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;

namespace ReMasters.SuperMelee.Ships
{
    [Serializable]
    public class LifePod : Controlable
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags =  BodyFlags.IgnoreGravity;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static TargetingInfo DefaultTargetingTypes;
        static LifePod()
        {
            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleAcceleration(4,0)),
                new Bounded<float>(TimeWarp.ScaleVelocity(4)));
            DefaultState = new ShipState(new Bounded<float>(1),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0));
            DefaultLifeTime = new LifeSpan(30);
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Ship);
            DefaultEffectCollection.Effects.Add(new MedKitEffect(DefaultTargetingTypes, EffectTypes.None, new EffectSounds(null, "GetCrew", null), 1));
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            IGeometry2D hull = new Polygon2D(ALVector2D.Zero, Polygon2D.FromSquare(10));
            DefaultShape = new RigidBodyTemplate(MassInertia.FromSquare(.01f, 10), new IGeometry2D[] { hull }, new Coefficients[] { coe });
            DefaultShape.BalanceBody();
        }
        public LifePod()
            : base(
                (LifeSpan)DefaultLifeTime.Clone(), 
                new PhysicsState(), 
                DefaultBodyFlags, DefaultShape, 
                 new ShipMovementInfo(DefaultMovementInfo), 
            new ShipState(DefaultState), 
            new ControlableSounds(), 
            new WeaponsLogic(DefaultTargetingTypes,
            new EffectCollection(DefaultEffectCollection)))
        {
            this.AddControler(null,new ProximityTargetingControler(600, DefaultTargetingTypes));
            this.controlHandler = new TargetingControlHandler();
            this.IgnoreInfo.JoinGroupToIgnore(FactionInfo.DefaultFactionCollection.CreateNewOrGetFaction("Debris").FactionNumber);
        }
        protected LifePod(LifePod copy) : base(copy) { }
        public override bool IsThreatTo(IControlable other)
        {
            return false;
        }
        public override object Clone()
        {
            return new LifePod(this);
        }
    }
}