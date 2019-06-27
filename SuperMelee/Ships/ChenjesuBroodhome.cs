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
using Color = System.Drawing.Color;
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;
namespace ReMasters.SuperMelee.Ships
{
    public class ChenjesuBroodhomeShipLoader : ShipLoader
    {
        public ChenjesuBroodhomeShipLoader() : base("Chenjesu Broodhome") { }
        protected override IShip CreateHardCodedShip()
        {
            return ChenjesuBroodhome.Create(new PhysicsState(), null);
        }
    }

    [Serializable]
    public class ChenjesuBroodhome : Ship
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
                new Bounded<float>(TimeWarp.ScaleAcceleration(3, 4)),
                new Bounded<float>(TimeWarp.ScaleVelocity(27)));
            DefaultState = new ShipState(new Bounded<float>(36),
                new Bounded<float>(30),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(4, 1)));

            DefaultActions.Add( ChenjesuBroodhomePrimary.Create());
            DefaultActions.Add(ChenjesuBroodhomeSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("ChenjesuBroodhomeDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();




            Vector2D[] mainhullvertecies = new Vector2D[]
            {
                new Vector2D(10,10),
                new Vector2D(0,40),
                new Vector2D(-10,47),

                new Vector2D(-50,20),
                new Vector2D(-60,5),
                new Vector2D(-60,-5),
                new Vector2D(-50,-20),

                new Vector2D(-10,-47),
                new Vector2D(0,-40),
                new Vector2D(10,-10)
            };


            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(0, 0)), mainhullvertecies));
            coes.Add(DefaultCoefficients);
            //goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-10, 0)), new Polygon2D(8, 40)));
            //coes.Add(DefaultCoefficients);
            DefaultShape = new RigidBodyTemplate(20, 728.5412f, goes.ToArray(), coes.ToArray());
            
            DefaultShape.BalanceBody();
            //DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            ChenjesuBroodhome returnvalue = new ChenjesuBroodhome(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Chenjesu Broodhome", new CreateShipDelegate(Create));
        }*/
        protected ChenjesuBroodhome(PhysicsState state, FactionInfo factionInfo)
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
        { }
    }
    [Serializable]
    public class ChenjesuBroodhomePrimary : GunAction
    {
        public static RigidBodyTemplate DefaultShape;
        public static RigidBodyTemplate SubDefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static LifeSpan SubDefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static ShipState SubDefaultState;
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static EffectCollection SubDefaultEffectCollection = new EffectCollection();

        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static TargetingInfo DefaultEffectsWho;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
            DefaultEffectsWho = new TargetingInfo(TargetingTypes.Enemy|TargetingTypes.Debris);
            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(64)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultLifeTime = new LifeSpan(20);
            SubDefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(9,64));
            DefaultState = new ShipState(new Bounded<float>(6), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            SubDefaultState = new ShipState(new Bounded<float>(2), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));

            DefaultCost = new Costs(new ShipStateChange(0, 5, 0, 0), new ShipStateChange(), new ShipStateChange());

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(DefaultEffectsWho, EffectTypes.None, new EffectSounds(), new ShipStateChange(-6, 0, 0, 0)));

            SubDefaultEffectCollection.Effects.Add(new ShipStateEffect(DefaultEffectsWho, EffectTypes.None, new EffectSounds(), new ShipStateChange(-2, 0, 0, 0)));


            DefaultActionSounds = new ActionSounds("Crystal", null, "GlassBreak");
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;

            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();

            goes.Add(new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(6, 15)));
            coes.Add(coe);

            DefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(.4f, 15), goes.ToArray(), coes.ToArray());
            //DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);

            goes.Clear();
            coes.Clear();


            goes.Add(new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(6, 9)));
            coes.Add(coe);


            SubDefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(.01f, 9), goes.ToArray(), coes.ToArray());
            //SubDefaultShape.BalanceBody();
            //Sub//DefaultShape.CalcInertiaMultiplier(.1f);
        }
         static ISolidWeapon CreateWeapon()
        {
            ISolidWeapon solidweapon = new Controlable(
                DefaultLifeTime, 
                new PhysicsState(), 
                DefaultBodyFlags, 
                DefaultShape, 
                 new ShipMovementInfo(DefaultMovementInfo), 
            new ShipState(DefaultState), 
            new ControlableSounds(), 
            new WeaponsLogic(TargetingInfo.All,
            new EffectCollection(DefaultEffectCollection)));
            return solidweapon;
        }
         static ISolidWeapon CreateSubWeapon()
        {
            ISolidWeapon solidweapon = new Controlable(
                SubDefaultLifeTime, 
                new PhysicsState(), 
                DefaultBodyFlags, 
                SubDefaultShape, 
                 new ShipMovementInfo(DefaultMovementInfo), 
            new ShipState(SubDefaultState), 
            new ControlableSounds(), 
            new WeaponsLogic(TargetingInfo.All,
            new EffectCollection(SubDefaultEffectCollection)));
            return solidweapon;
        }

        ISolidWeapon subWeapon;
        public static ChenjesuBroodhomePrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ChenjesuBroodhomePrimary();
        }

        ChenjesuBroodhomePrimary()
            : base(new Bounded<float>(DefaultDelay),
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            false,
            DefaultActionSounds,
            0,
            0,
            CreateWeapon())
        {
            this.subWeapon = CreateSubWeapon();
            this.aIInfo = new RemoteControlGunActionAIInfo();
        }
        public ChenjesuBroodhomePrimary(ChenjesuBroodhomePrimary copy) : base(copy)
        {
            this.subWeapon = (ISolidWeapon)copy.subWeapon.Clone();
        }
        protected override void OnWeaponCreation(ActionResult actionResult, float dt,int index)
        {
            CurrentWeapon.Current.Velocity.Angular = 5;
        }
        protected override bool OnRunning(ActionResult actionResult, float dt)
        {
            return !(CurrentWeapon == null || CurrentWeapon.IsExpired);
        }
        protected override bool OnDeActivated(ActionResult actionResult, float dt)
        {
            if (CurrentWeapon == null || CurrentWeapon.IsExpired)
            {
                return false;
            }
            else
            {
                float extradistance = .1f * source.MovementInfo.MaxLinearVelocity;
                CurrentWeapon.Kill(actionResult);
                float inc = MathHelper.TWO_PI /6;
                CurrentWeapon.Current.Velocity.Linear = Vector2D.Zero;
                for (int pos = 0; pos < 6; ++pos)
                {
                    float angle = inc * pos;
                    ISolidWeapon newWeapon = FireGun(subWeapon, CurrentWeapon, angle, angle, extradistance);
                    newWeapon.OnCreation(actionResult, CurrentWeapon.WeaponInfo);
                }
                CurrentWeapon = null;
                return true;
            }
        } 
        public override object Clone()
        {
            return new ChenjesuBroodhomePrimary(this);
        }
    }
    [Serializable]
    public class ChenjesuBroodhomeSecondary : GunAction
    {
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static Costs DefaultCost;
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultCost = new Costs(new ShipStateChange(0, 30, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));


            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
            DefaultActionSounds = new ActionSounds("Crystal2", null, null);
        }

        static ISolidWeapon CreateWeapon()
        {
            ISolidWeapon w =  ChenjesuBroodhomeDogi.Create();
            w.ControlHandler = new DefaultControlHandler();
            w.AddControler(null, new SmartMissileControler(TargetingInfo.FromRequireAll(TargetingTypes.Enemy | TargetingTypes.Ship)));
            return w;
        }
        public static ChenjesuBroodhomeSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ChenjesuBroodhomeSecondary();
        }

        public ChenjesuBroodhomeSecondary()
            : base(new Bounded<float>(DefaultDelay),
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            true,
            DefaultActionSounds,
            MathHelper.PI,
            MathHelper.PI,
            CreateWeapon())
        { }
    }
    [Serializable]
    public class ChenjesuBroodhomeDogi : Controlable
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static TargetingInfo DefaultTargetingTypes;
        public static TargetingInfo DefaultEffectsWho;
        public static ControlableSounds DefaultControlableSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultEffectsWho = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(0)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(20, 0)),
                new Bounded<float>(TimeWarp.ScaleVelocity(33)));
            DefaultState = new ShipState(new Bounded<float>(3),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0));
            DefaultLifeTime = new LifeSpan(60);
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Ship);
            DefaultEffectCollection.Effects.Add(
                new ShipStateEffect(DefaultEffectsWho, 
                EffectTypes.None,
                new EffectSounds("DogiBark", null, null),
                new ShipStateChange(0, -8, 0, 0)));
            DefaultEffectCollection.AttachmentFlags |= EffectAttachmentFlags.ClonedAttachment;
            DefaultControlableSounds = new ControlableSounds(null, "DogiDie");
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;

            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();

            goes.Add(new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(6, 20)));
            coes.Add(coe);

            DefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(1, 20), goes.ToArray(), coes.ToArray());

        }
        public static ChenjesuBroodhomeDogi Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ChenjesuBroodhomeDogi();
        }
        ChenjesuBroodhomeDogi()
            : base(
                (LifeSpan)DefaultLifeTime.Clone(), 
                new PhysicsState(), 
                DefaultBodyFlags, DefaultShape, 
                 new ShipMovementInfo(DefaultMovementInfo), 
            new ShipState(DefaultState), 
            DefaultControlableSounds, 
            new WeaponsLogic(DefaultTargetingTypes,
            new EffectCollection(DefaultEffectCollection)))
        {
            this.AddControler(null, new ProximityTargetingControler(600, DefaultTargetingTypes));
            this.controlHandler = new TargetingControlHandler();
        }
        public ChenjesuBroodhomeDogi(ChenjesuBroodhomeDogi copy) : base(copy) { }
        public override void OnCollision(GameResult gameResult, IControlable collider)
        {
            Vector2D dir = this.current.Position.Linear - collider.Current.Position.Linear;
            this.current.Velocity.Linear = Vector2D.SetMagnitude(dir, this.movementInfo.MaxLinearVelocity);
            base.OnCollision(gameResult, collider);
        }
        public override object Clone()
        {
            return new ChenjesuBroodhomeDogi(this);
        }
    }
}
#endif
