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
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;
namespace ReMasters.SuperMelee.Ships
{
    public class VuxIntruderShipLoader : ShipLoader
    {
        public VuxIntruderShipLoader() : base("Vux Intruder") { }
        protected override IShip CreateHardCodedShip()
        {
            return VuxIntruder.Create(new PhysicsState(), null);
        }
    }




    [Serializable]
    public class VuxIntruder : Ship
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime = new LifeSpan();
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static ActionList DefaultActions = new ActionList();
        public static ControlableSounds DefaultControlableSounds;
        public static ShipSounds DefaultShipSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(6)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(7, 4)),
                new Bounded<float>(TimeWarp.ScaleVelocity(21)));

            DefaultState = new ShipState(new Bounded<float>(20),
                new Bounded<float>(40),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(8, 1)));
            DefaultActions.Add(VuxIntruderPrimary.Create());
            DefaultActions.Add(VuxIntruderSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("VuxIntruderDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();

            Vector2D[] pods = Polygon2D.FromRectangle(10, 70);


            Vector2D[] leftWingvertecies = new Vector2D[]
                {
                    new Vector2D(25,20),
                    new Vector2D(-30,20),
                    new Vector2D(-40,-50),
                    new Vector2D(-5,-50)
                };
            Vector2D[] RightWingvertecies = new Vector2D[]
                {
                    new Vector2D(-5,50),
                    new Vector2D(-40,50),
                    new Vector2D(-30,-20),
                    new Vector2D(25,-20)
                };
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-10, -30)), leftWingvertecies));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-10, 30)), RightWingvertecies));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-35, 75)), pods));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-35, -75)), pods));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(20, 0)), Polygon2D.FromRectangle(20, 120)));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(90, 0)), Polygon2D.FromNumberofSidesAndRadius(5, 25)));
            coes.Add(DefaultCoefficients);


            DefaultShape = new RigidBodyTemplate(18, 3923.7657051329197f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();

            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }
            VuxIntruder returnvalue = new VuxIntruder(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Vux Intruder", new CreateShipDelegate(Create));
        }*/
        protected VuxIntruder(PhysicsState state, FactionInfo factionInfo)
            : base(
            (LifeSpan)DefaultLifeTime.Clone(), 
            state, 
            DefaultBodyFlags, DefaultShape, 
            new ShipMovementInfo(DefaultMovementInfo), 
            new ShipState(DefaultState), 
            DefaultControlableSounds, 
            DefaultShipSounds, 
            new ActionList(DefaultActions), 
            null)
        {
            this.SpawnInfo = new SpawningLocation(true, 600, -1);
        }
        VuxIntruder(VuxIntruder copy) : base(copy) { }
        public override void OnCreation(GameResult gameResult, FactionInfo factionInfo)
        {
            base.OnCreation(gameResult, factionInfo);
            this.ignoreInfo.AddGroupToIgnore(factionInfo.FactionNumber);
        }
        public override object Clone()
        {
            return new VuxIntruder(this);
        }
    }
    [Serializable]
    public class VuxIntruderPrimary : RayAction
    {
        public static LifeSpan DefaultLifeTime;
        public static Costs DefaultCost;
        public static float DefaultRange = TimeWarp.ScaleRange(9);
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultTargetingTypes = TargetingInfo.None;
            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultEffectCollection.Effects.Add(
                new ShipStateEffect(
                    DefaultTargetingTypes,
                    EffectTypes.None,
                    new EffectSounds(),
                    new ShipStateChange(-.15f, 0, 0, 0)));
            DefaultLifeTime = new LifeSpan(TimeWarp.RateToTime(0));
            DefaultActionSounds = new ActionSounds("Laser8", null, null);
            DefaultEffectCollection.AttachmentFlags |= EffectAttachmentFlags.ClonedAttachment;
        }
        static DirectedRayWeapon CreateWeapon()
        {
            return new DirectedRayWeapon(
                (LifeSpan)DefaultLifeTime.Clone(),
                new WeaponsLogic(DefaultTargetingTypes,
                new EffectCollection(DefaultEffectCollection)),
                new float[] { 100, 100, 100, 100 },
                new float[] { -.125f, -.15f, -.1f, -.175f },
                new float[] { 0, 0, 0, 0 },
                new float[] { DefaultRange, DefaultRange, DefaultRange, DefaultRange });
        }
        public static VuxIntruderPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new VuxIntruderPrimary();
        }

        VuxIntruderPrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           CreateWeapon())
        { }
    }
    [Serializable]
    public class VuxIntruderSecondary : GunAction
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(0)),
                new Bounded<float>(8000),
                new Bounded<float>(TimeWarp.ScaleVelocity(25)));
            DefaultState = new ShipState(new Bounded<float>(1), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultCost = new Costs(new ShipStateChange(0, 2, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(7));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(35, 25));
            DefaultEffectCollection.Effects.Add(
                new VuxSlowDownEffect(
                    DefaultTargetingTypes,
                    EffectTypes.None,
                    new EffectSounds("Bite", null, null),
                    new LifeSpan(),
                    new ShipMovementInfoChange(0, -.125f, -.125f, 0)));
            DefaultActionSounds = new ActionSounds("OrganicGun2", null, null);
            DefaultEffectCollection.AttachmentFlags |= EffectAttachmentFlags.WeaponExpires;
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;

            DefaultShape = new RigidBodyTemplate(
                MassInertia.FromSolidCylinder(.01f, 10),
                new IGeometry2D[] { new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(8, 10)) },
                new Coefficients[] { coe });
            //DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        static ISolidWeapon CreateWeapon()
        {
            ISolidWeapon w = new Controlable(
                DefaultLifeTime,
                new PhysicsState(),
                DefaultBodyFlags,
                DefaultShape,
                 new ShipMovementInfo(DefaultMovementInfo),
            new ShipState(DefaultState),
            new ControlableSounds(),
            new WeaponsLogic(TargetingInfo.All,
            new EffectCollection(DefaultEffectCollection)));
            w.ControlHandler = new DefaultControlHandler();
            w.AddControler(null, new MissileControler(TargetingInfo.FromRequireAll(TargetingTypes.Enemy | TargetingTypes.Ship)));
            return w;
        }
        public static VuxIntruderSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new VuxIntruderSecondary();
        }
        VuxIntruderSecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           MathHelper.PI,
           MathHelper.PI,
           CreateWeapon(), 
            MathHelper.PI,
            MathHelper.PI,
            0)
        {
            this.aIInfo = new SpecificRangeActionAIInfo(DefaultCost.ActivationCost * 2, VuxIntruderPrimary.DefaultRange, DefaultMovementInfo.MaxLinearVelocity.Value * DefaultLifeTime.TimeLeft * 2);
        }
        public VuxIntruderSecondary(VuxIntruderSecondary copy) : base(copy) { }
        public override void OnSourceCreation(GameResult gameResult, IShip source)
        {
            base.OnSourceCreation(gameResult, source);
            this.Weapon.IgnoreInfo.JoinGroupToIgnore(source.FactionInfo.FactionNumber);
        }
        public override object Clone()
        {
            return new VuxIntruderSecondary(this);
        }
    }
    [Serializable]
    public class VuxSlowDownEffect : MovementInfoEffect
    {
        public VuxSlowDownEffect(TargetingInfo effectsWho,
            EffectTypes effectTypes,
            EffectSounds effectSounds,
            LifeSpan lifeTime,
            ShipMovementInfoChange percents)
            : base(effectsWho, effectTypes, effectSounds, lifeTime, percents)
        {
            this.isHarmful = percents.MaxAngularAcceleration < 1 || percents.MaxAngularVelocity < 1 || percents.MaxLinearAcceleration < 1 || percents.MaxLinearVelocity < 1;

        }
        public VuxSlowDownEffect(MovementInfoEffect copy)
            : base(copy)
        { }
        public override void OnTargetAttachment(EffectAttachmentResult attachmentResult, IControlable attachie)
        {
            this.smic.MaxAngularAcceleration *= attachie.MovementInfo.MaxAngularAcceleration.Value;
            this.smic.MaxAngularVelocity *= attachie.MovementInfo.MaxAngularVelocity.Value;
            this.smic.MaxLinearAcceleration *= attachie.MovementInfo.MaxLinearAcceleration.Value;
            this.smic.MaxLinearVelocity *= attachie.MovementInfo.MaxLinearVelocity.Value;
            base.OnTargetAttachment(attachmentResult, attachie);
        }
        public override object Clone()
        {
            return new VuxSlowDownEffect(this);
        }
    }

}
#endif
