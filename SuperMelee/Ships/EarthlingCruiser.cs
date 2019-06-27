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
#if !Release
using System;
using System.Collections.Generic;
using Physics2D;
using Physics2D.CollidableBodies;
using AdvanceSystem;
using AdvanceMath.Geometry2D;
using AdvanceMath; 
using Color = System.Drawing.Color;
using ReMasters.SuperMelee.Weapons;
using ReMasters.SuperMelee.Controlers;
using ReMasters.SuperMelee.Effects;
using ReMasters.SuperMelee.Actions;

namespace ReMasters.SuperMelee.Ships
{
    public class EarthlingCruiserShipLoader : OrganizedShipLoader
    {
        public EarthlingCruiserShipLoader() : base("Earthling Cruiser") { }
        protected override IShip CreateHardCodedShip(RigidBodyTemplate defaultShape, ActionList DefaultActions)
        {
            IShip returnvalue = new Ship(
                new LifeSpan(), 
                new PhysicsState(), 
                BodyFlags.None, defaultShape, 
                new ShipMovementInfo(
                    new Bounded<float>(TimeWarp.AngularAcceleration),
                    new Bounded<float>(TimeWarp.ScaleTurning(1)),
                    new Bounded<float>(TimeWarp.ScaleAcceleration(3, 4)),
                    new Bounded<float>(TimeWarp.ScaleVelocity(24))), 
                new ShipState(new Bounded<float>(18),
                    new Bounded<float>(18),
                    new Bounded<float>(0),
                    new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(8, 1))), 
                new ControlableSounds(null, "ShipDies"), 
                new ShipSounds("EarthlingCruiserDitty"), 
                DefaultActions, 
                null); 
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        protected override RigidBodyTemplate GetShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();

            Vector2D[] pods = Polygon2D.FromRectangle(20, 70);
            Vector2D[] mainhullp = Polygon2D.FromRectangle(30, 150);
            Vector2D[] subhullp = Polygon2D.FromRectangle(40, 20);

            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI + .7f, new Vector2D(-35, 22)), subhullp));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI + .7f, new Vector2D(35, 22)), subhullp));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI - .7f, new Vector2D(-35, -22)), subhullp));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI - .7f, new Vector2D(35, -22)), subhullp));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(40, 40)), pods));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(40, -40)), pods));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-40, 40)), pods));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-40, -40)), pods));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(20, 0)), mainhullp));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(120, 0)), Polygon2D.FromNumberofSidesAndRadius(10, 35)));
            coes.Add(DefaultCoefficients);


            RigidBodyTemplate DefaultShape = new RigidBodyTemplate(18, 3923.7657051329197f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();
            return DefaultShape;
        }
        protected override IEnumerable<IAction> GetActions()
        {
            yield return GetPrimary();
            yield return GetSecondary();
        }
        IAction GetPrimary()
        {
            return new GunAction(
                new Bounded<float>(TimeWarp.RateToTime(10)),
                new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None),
                new Costs(new ShipStateChange(0, 9, 0, 0), null, null),
                false, // true if it needs a target to be launched; otherwise false.
                new ActionSounds("MissileLaunch1", null, null),
                0,
                0,
                CreatePrimaryWeapon());
        }
        RigidBodyTemplate GetPrimaryWeaponShape()
        {
            Coefficients coe = TimeWarp.Coefficients;

            Vector2D[] bulletverticies = new Vector2D[]
            {
                new Vector2D(30,5),
                new Vector2D(-30,5),
                new Vector2D(-30,-5),
                new Vector2D(30,-5)
            };
            IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, bulletverticies);
            RigidBodyTemplate DefaultShape = new RigidBodyTemplate(MassInertia.FromRectangle(1, 10, 2), new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
            DefaultShape.BalanceBody();
            return DefaultShape;
        }
        ISolidWeapon CreatePrimaryWeapon()
        {

            EffectCollection DefaultEffectCollection = new EffectCollection();
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-4, 0, 0, 0)));

            ISolidWeapon w = new SpawningSolidWeapon(
                new LifeSpan(TimeWarp.RangeToTime(60, 80)),
                new PhysicsState(),
                BodyFlags.None,
                GetPrimaryWeaponShape(),
                new ShipMovementInfo(
                    new Bounded<float>(TimeWarp.AngularAcceleration),
                    new Bounded<float>(TimeWarp.ScaleTurning(3)),
                    new Bounded<float>(TimeWarp.ScaleAcceleration(80, 0)),
                    new Bounded<float>(TimeWarp.ScaleVelocity(80))),
                new ShipState(
                    new Bounded<float>(1),
                    new Bounded<float>(0),
                    new Bounded<float>(0),
                    new Bounded<float>(0)),
                new ControlableSounds(),
                new WeaponsLogic(
                    new TargetingInfo(TargetingTypes.All, TargetingTypes.None, TargetingTypes.None),
                    new EffectCollection(DefaultEffectCollection)),
                CreatePrimarySpawn(),
                true);

            w.ControlHandler = new DefaultControlHandler();
            w.AddControler(null,new MissileControler(TargetingInfo.FromRequireAll(TargetingTypes.Enemy | TargetingTypes.Ship)));
            return w;
        }
        ISolidWeapon CreatePrimarySpawn()
        {
            return new ControlableWave(
                new LifeSpan(.3f),
                .2f,
                new PhysicsState(),
                30,
                400,
                TimeWarp.DefaultExposionColors,
                TimeWarp.DefaultExplosionPrimaryColor,
                new ShipMovementInfo(),
                new ShipState(new Bounded<float>(40), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0)),
                new ControlableSounds(),
                new WeaponsLogic(TargetingInfo.None, new EffectCollection()));
        }
        float SecondaryRadius = 500;
        int SecondaryMaxNumberofTargets = 10;
        float SecondaryImpulse = 200;
        IAction GetSecondary()
        {
            return new RayPointDefence(
                new Bounded<float>(TimeWarp.RateToTime(9)),
                new TargetingInfo(TargetingTypes.Other, TargetingTypes.None, TargetingTypes.Ally),
                new Costs(new ShipStateChange(0, 4, 0, 0), null, null),
                new ActionSounds("Laser1", null, null),
                SecondaryRadius,
                SecondaryMaxNumberofTargets,
                CreateSecondaryWeapon());
        }
        TargetedRayWeapon CreateSecondaryWeapon()
        {
            TargetingInfo DefaultEffectsWho = new TargetingInfo(TargetingTypes.Enemy | TargetingTypes.Other | TargetingTypes.Debris, TargetingTypes.None, TargetingTypes.Ally);
            EffectCollection DefaultEffectCollection = new EffectCollection();
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(DefaultEffectsWho, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            return new TargetedRayWeapon(
                new LifeSpan(TimeWarp.RateToTime(0)),
                new WeaponsLogic(
                    DefaultEffectsWho,
                    DefaultEffectCollection),
                SecondaryImpulse,
                SecondaryRadius);
        }
    }
}
#endif
