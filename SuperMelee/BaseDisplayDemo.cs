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
//using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
/*using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;*/
//using Microsoft.DirectX.DirectInput;
using Physics2D;
using Physics2D.CollidableBodies;
using AdvanceMath; using AdvanceSystem;
using AdvanceMath.Geometry2D;
using ReMasters.SuperMelee.Ships;
using ReMasters.SuperMelee.Controlers;
using System.Xml.Serialization;

using SdlDotNet;
using Tao.OpenGl;
using System.Drawing.Drawing2D;
using Physics2D;
namespace ReMasters.SuperMelee
{




    [Serializable]
    public class WindowState
    {
        public Size ViewableAreaSize;
        public float Scale;
        public Vector2D CameraPosition;
        public Vector2D Offset;
        public BoundingBox2D ScreenBoundingBox;
        public WindowState(Size ViewableAreaSize, float Scale, Vector2D CameraPosition)
        {
            this.ViewableAreaSize = ViewableAreaSize;
            this.Scale = Scale;
            this.CameraPosition = CameraPosition;
            Vector2D tmp = new Vector2D(ViewableAreaSize.Width / (2 * Scale), ViewableAreaSize.Height / (2 * Scale));
            this.Offset = tmp - CameraPosition;
            Vector2D OtherBound = -tmp - CameraPosition;
            this.ScreenBoundingBox = new BoundingBox2D(CameraPosition + tmp, CameraPosition - tmp);
        }
        public void DrawPoint(Vector2D point, int color)
        {
            Color c = Color.FromArgb(color);
            float r = ((float)(int)c.R) / ((float)(int)Byte.MaxValue);
            float g = ((float)(int)c.G) / ((float)(int)Byte.MaxValue);
            float b = ((float)(int)c.B) / ((float)(int)Byte.MaxValue);
            Gl.glColor3f(r, g, b);
            Gl.glVertex2f((float)point.X, (float)point.Y);
        }
    }
    public delegate IShip GetNewShip(FactionInfo faction);
    [Serializable]
    public class SuperMeleeData
    {
        static string[] SuicideShipNames = new string[] { "ShofixtiScout" };
        public static int[] teamcolors = new int[] { Color.Gray.ToArgb(), Color.Red.ToArgb(), Color.Blue.ToArgb(), Color.Green.ToArgb(), Color.Purple.ToArgb(), Color.Green.ToArgb(), Color.Orange.ToArgb() };
        static Random rand = new Random();
        static int numberofCircleVertexes = 15;
        static Color backgroundColor = Color.Black;
        static Color defaultColor1 = Color.Blue;
        static Color defaultColor2 = Color.Gray;
        static float MinAsteroidCount = 5;//50;
        static int DeathWait = 4000;
        public static int AfterChoiceWait = 4500;
        static int VictorySongWait = 1000;
        [NonSerialized]
        public GetNewShip GetNewShip;
        public Vector2D CamerasLastPosition = Vector2D.Zero;
        public float LastScale = .5f;
        public static float ArenaSize = 8000;
        public NumberBinder<float> ArenaXSize = NumberBinder<float>.GetBinder(-ArenaSize, ArenaSize, true);
        public NumberBinder<float> ArenaYSize = NumberBinder<float>.GetBinder(-ArenaSize, ArenaSize, true);
        public DotBox dotbox;
        public SuperMeleeWorld world;
        public List<StatusBox> statusBoxs = new List<StatusBox>();
        public List<IPlayerControler> controlers = new List<IPlayerControler>();

        public void OnShipsDeath(object sender, EventArgs e)
        {
            IShip oldShip = sender as IShip;
            if (oldShip != null)
            {
                GameResult gameResult = new GameResult();
                Asteroid.CreateDebris(gameResult, oldShip);
                world.HandleActionEvents(gameResult);
                if (oldShip.ControlableType == ControlableType.Ship)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(DelayedAddShip), oldShip);
                }
            }
        }
        object syncroot = new object();


        void DelayedAddShip(object oship)
        {
            IShip oldShip = (IShip)oship;
            Thread.Sleep(VictorySongWait);
            ShipSounds Victor = null;
            if(Array.IndexOf<string>(SuicideShipNames, oldShip.GetType().Name ) != -1)
            {
                Victor = oldShip.ShipSounds;
            }
            foreach (IShip ship in GetShips())
            {
                if (!ship.IsExpired)
                {
                    if ((ship.FactionInfo.FactionCollection.GetDiplomacy(ship.FactionInfo, oldShip.FactionInfo) & FactionDiplomacy.Enemy) == FactionDiplomacy.Enemy)
                    {
                        Victor  = ship.ShipSounds;
                        break;
                    }
                }
            }
            if (Victor != null)
            {
                Victor.Victory.Play();
            }
            Thread.Sleep(DeathWait - VictorySongWait);
            lock (world)
            {
                ReMasterSDL.physicsTimer.Enabled = false;

                lock (syncroot)
                {
                    GameResult gameResult = new GameResult();
                    IShip newShip = GetNewShip(oldShip.FactionInfo);
                    //IShip newShip2 = (IShip)newShip.Clone();
                    newShip.AddControler(null,oldShip.Controlers[0]);
                    if (oldShip.Controlers.Length > 1)
                    {
                        newShip.AddControler(null,oldShip.Controlers[1]);
                    }
                    AddShip(gameResult,newShip, oldShip.FactionInfo);
                    world.HandleActionEvents(gameResult);
                }
            }
            BindWorld(CamerasLastPosition);

            MeleeMusic.PlayDefault();
            Thread.Sleep(AfterChoiceWait);
            ReMasterSDL.physicsTimer.Enabled = true;

        }






        void AddAIClone(GameResult gameResult, IShip newShip2, FactionInfo factionInfo)
        {
            newShip2.Current.Position.Linear += Vector2D.FromLengthAndAngle(900, (float)rand.NextDouble());
            newShip2.SetAllPositions();
            newShip2.AddControler(null, new ComplexAIControler());
            newShip2.OnCreation(gameResult,factionInfo);
            SetTarget(newShip2);
        }
        public void SetTarget(IShip ship)
        {
            ship.TargetRetriever.OnSourceCreation(ship);
            ship.Target = ship.TargetRetriever.Next(new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None));
            if (ship.Target != null)
            {
                ship.Target.Target = ship;
            }
            SetSpawnLocation(ship);

        }
        public void SetSpawnLocation(IShip ship)
        {
            if (ship.SpawnInfo == null || ship.Target == null)
            {
                return;
            }
            Vector2D diff = ship.Target.Current.Position.Linear - ship.Current.Position.Linear;
            float mag = diff.Magnitude;
            bool setPosition = false;
            if (ship.SpawnInfo.FacingTarget)
            {
                ship.Current.Position.Angular = (-diff).Angle;
                setPosition = true;
            }
            if (ship.SpawnInfo.MinDistanceToTarget > 0 && mag < ship.SpawnInfo.MinDistanceToTarget)
            {
                diff.Magnitude =  ship.SpawnInfo.MinDistanceToTarget;
                ship.Current.Position.Linear = ship.Target.Current.Position.Linear + diff;
                setPosition = true;
            }
            else if (ship.SpawnInfo.MaxDistanceToTarget > 0 && mag > ship.SpawnInfo.MaxDistanceToTarget)
            {
                diff.Magnitude = ship.SpawnInfo.MaxDistanceToTarget;
                ship.Current.Position.Linear = ship.Target.Current.Position.Linear + diff;
                setPosition = true;
            }
            if (setPosition)
            {
                ship.SetAllPositions();

            }
        }
        public void AddShip(GameResult gameResult, IShip ship, FactionInfo factionInfo)
        {
            if (ship == null)
            {
                return;
            }
            statusBoxs.Add(new StatusBox(ship));
            ship.OnCreation(gameResult,factionInfo);
            ship.Killed += new EventHandler(OnShipsDeath);
            if (ship.SubShips != null)
            {
                foreach (IShip subShip in ship.SubShips)
                {
                    if (!subShip.IsExpired)
                    {
                        subShip.Killed += new EventHandler(OnShipsDeath);
                    }
                }
            }
            SetTarget(ship);
        }
        public void BindWorld(Vector2D position)
        {
            Bounded<float> arenaXSize = new Bounded<float>(ArenaXSize.Lower + position.X, 0, ArenaXSize.Upper + position.X, true);
            Bounded<float> arenaYSize = new Bounded<float>(ArenaYSize.Lower + position.Y, 0, ArenaYSize.Upper + position.Y, true);
            foreach (ICollidableBody collidable in world.Collidables)
            {
                Vector2D pos = collidable.Current.Position.Linear;
                pos.X = arenaXSize.Binder.Bind(pos.X);
                pos.Y = arenaYSize.Binder.Bind(pos.Y);
                if (collidable.Current.Position.Linear != pos)
                {
                    collidable.Current.Position.Linear = pos;
                    collidable.CalcBoundingBox2D();
                    collidable.SaveGood();
                }

            }
        }
        void BindWorldScreen(Vector2D position)
        {
            position.X = ArenaXSize.Bind(position.X);
            position.Y = ArenaYSize.Bind(position.Y);
            Bounded<float> arenaXSize = new Bounded<float>(ArenaXSize.Lower + position.X, 0, ArenaXSize.Upper + position.X, true);
            Bounded<float> arenaYSize = new Bounded<float>(ArenaYSize.Lower + position.Y, 0, ArenaYSize.Upper + position.Y, true);
            foreach (ICollidableBody collidable in world.Collidables)
            {
                Vector2D pos = collidable.Current.Position.Linear;
                pos.X = arenaXSize.Binder.Bind(pos.X);
                pos.Y = arenaYSize.Binder.Bind(pos.Y);
                if (collidable.Current.Position.Linear != pos)
                {
                    collidable.Current.Position.Linear = pos;
                    collidable.CalcBoundingBox2D();
                    collidable.SaveGood();
                }
            }
        }
        public void Update(float dt)
        {
            BindWorld(CamerasLastPosition);
            world.Update(dt);
        }
        public void GenerateAsteroids(Vector2D center, int count)
        {
            if (count < MinAsteroidCount)
            {
                PhysicsState state = new PhysicsState();
                Vector2D pos = new Vector2D(
                    (float)Math.Sign(rand.NextDouble() - .5f) * ArenaXSize.Upper + center.X + (float)Math.Sign(rand.NextDouble() - .5f) * 1400,
                    (float)Math.Sign(rand.NextDouble() - .5f) * ArenaYSize.Upper + center.Y + (float)Math.Sign(rand.NextDouble() - .5f) * 1400);
                state.Position = new ALVector2D((float)(rand.NextDouble() * MathHelper.PI * 2), pos);
                state.Velocity = new ALVector2D((float)((rand.NextDouble() - .5f) * 4 * MathHelper.PI), new Vector2D((float)(rand.NextDouble() - .5f) * 1400, (float)(rand.NextDouble() - .5f) * 1400));
                GameResult gameResult = new GameResult();
                Asteroid.CreateAsteroid(gameResult,state);
                world.HandleActionEvents(gameResult);
            }
        }

        public static void SetColor(int argb)
        {
            Color c = Color.FromArgb(argb);
            float r = ((float)(int)c.R) / ((float)(int)Byte.MaxValue);
            float g = ((float)(int)c.G) / ((float)(int)Byte.MaxValue);
            float b = ((float)(int)c.B) / ((float)(int)Byte.MaxValue);
            Gl.glColor3f(r, g, b);
        }
        public static void SetColor(int argb,float a)
        {
            Color c = Color.FromArgb(argb);
            float r = ((float)(int)c.R) / ((float)(int)Byte.MaxValue);
            float g = ((float)(int)c.G) / ((float)(int)Byte.MaxValue);
            float b = ((float)(int)c.B) / ((float)(int)Byte.MaxValue);
            Gl.glColor4f(r, g, b, a);
        }
        void DrawVertexes(WindowState state)
        {
            List<IControlable> BODIES = new List<IControlable>(world.Collidables);
            float baseradiusInc = (float)(MathHelper.PI * 2) / ((float)numberofCircleVertexes);
            float radiusInc = baseradiusInc;
            int numerofCV = numberofCircleVertexes;
            float sizedCV = 100;

            int color = defaultColor1.ToArgb();
            int color2 = defaultColor2.ToArgb();
            Matrix3x3 matrix = Matrix3x3.FromScale(new Vector2D(state.Scale, state.Scale)) * Matrix3x3.FromTranslate2D(state.Offset);
            foreach (IControlable body in BODIES)
            {
                if (body.BoundingBox2D == null || !body.IgnoreInfo.IsCollidable)
                {
                    body.CalcBoundingBox2D();
                }
                if (!state.ScreenBoundingBox.TestIntersection(body.BoundingBox2D) || body.IsExpired || body.IsInvisible)
                {
                    continue;
                }
                ControlableWave wave = body as ControlableWave;

                if (body.ControlHandler != null &&
                     body.Target != null && !body.Target.IsExpired && !body.Target.IsInvisible &&
                    body.Controlers != null && body.Controlers.Length > 0)
                {

                    Vector2D direction = Vector2D.Normalize ( body.Target.Current.Position.Linear - body.Current.Position.Linear);
                    Vector2D[] vertexes = new Vector2D[3];
                    vertexes[0] = body.Current.Position.Linear + direction * (body.BoundingRadius + 60);
                    vertexes[1] = body.Current.Position.Linear + Vector2D.Rotate(MathHelper.PI / 24, direction) * (body.BoundingRadius + 20);
                    vertexes[2] = body.Current.Position.Linear + Vector2D.Rotate(-MathHelper.PI / 24, direction) * (body.BoundingRadius + 20);

                    //Vector2D.Transform(matrix, ref vertexes);
                    OperationHelper.ArrayRefOp<Matrix3x3, Vector2D, Vector2D>(
                        ref matrix,
                        vertexes,
                        vertexes,
                        Vector2D.Multiply);


                    Gl.glBegin(Gl.GL_POLYGON);
                    SetColor(teamcolors[body.FactionInfo.FactionNumber]);
                    Gl.glVertex3f(vertexes[0].X, vertexes[0].Y, 0);
                    SetColor(teamcolors[0]);
                    Gl.glVertex3f(vertexes[1].X, vertexes[1].Y, 0);
                    Gl.glVertex3f(vertexes[2].X, vertexes[2].Y, 0);
                    Gl.glEnd();
                }
                foreach (Physics2D.ICollidableBodyPart part in body.CollidableParts)
                {
                    //Gl.glLoadIdentity();   
                    Gl.glBegin(Gl.GL_POLYGON);
                    //IColoredPart colored = part as IColoredPart;
                    if (part.UseCircleCollision)
                    {

                        if (part.BaseGeometry.BoundingRadius > sizedCV)
                        {
                            numerofCV = numberofCircleVertexes + (int)MathHelper.Sqrt(part.BaseGeometry.BoundingRadius - sizedCV);
                            if (numerofCV > 100)
                            {
                                numerofCV = 100;
                            }
                            radiusInc = (float)(MathHelper.PI * 2) / ((float)numerofCV);
                        }
                        else
                        {
                            numerofCV = numberofCircleVertexes;
                            radiusInc = baseradiusInc;
                        }

                        for (int angle = 0; angle != numerofCV; ++angle)
                        {
                            Vector2D vect = matrix * (Vector2D.FromLengthAndAngle(part.BaseGeometry.BoundingRadius, ((float)angle) * radiusInc + part.GoodPosition.Angular) + part.GoodPosition.Linear);

                            int vectColor = color2;
                            if (angle == 0)
                            {
                                vectColor = color;
                                if (body != null)
                                {
                                    vectColor = teamcolors[body.FactionInfo.FactionNumber];

                                }
                            }
                            if (wave != null)
                            {
                                if (wave.Colors.Length > angle)
                                {
                                    vectColor = (wave.Colors[angle]);
                                }
                                else
                                {
                                    if (wave.PrimaryColor != 0)
                                    {
                                        vectColor = (wave.PrimaryColor);
                                    }
                                }
                            }
                            SetColor(vectColor);

                            Gl.glVertex3f((float)vect.X, (float)vect.Y, 0);
                        }

                        //soundInfo.Add(points);
                    }
                    else
                    {
                        Vector2D[] vects = part.DisplayVertices;
                        if (vects != null)
                        {
                            vects = OperationHelper.ArrayRefOp<Matrix3x3, Vector2D, Vector2D>(
                                ref matrix,
                                part.DisplayVertices,
                                Vector2D.Multiply);

                            //vects = Vector2D.Transform(matrix, part.DisplayVertices);
                            for (int pos = 0; pos != vects.Length; ++pos)
                            {
                                int vectColor = color2;
                                if (pos == 0)
                                {
                                    vectColor = color;
                                    if (body != null)
                                    {
                                        vectColor = teamcolors[body.FactionInfo.FactionNumber];
                                    }
                                }
                                SetColor(vectColor);
                                Gl.glVertex3f((float)vects[pos].X, (float)vects[pos].Y, 0);
                            }
                        }
                    }
                    Gl.glEnd();
                }
            }
        }
        void DrawLines(WindowState state)
        {
           

            List<IRay2DEffect> effects = world.Ray2DEffects;
            if (effects != null && effects.Count>0)
            {
                Gl.glLineWidth(1);
                Gl.glBegin(Gl.GL_LINES);
                foreach (IRay2DEffect effect in effects)
                {
                    if (effect != null && effect.RaySegment != null)
                    {
  
                        Vector2D vect1 = (effect.RaySegment.Origin + state.Offset) * state.Scale;


                        Color c = Color.White;
                        float r = ((float)(int)c.R) / ((float)(int)Byte.MaxValue);
                        float g = ((float)(int)c.G) / ((float)(int)Byte.MaxValue);
                        float b = ((float)(int)c.B) / ((float)(int)Byte.MaxValue);

                        Gl.glColor3f(r, g, b);
                        Gl.glVertex3f((float)vect1.X, (float)vect1.Y, 0);

                        Vector2D vect2 = (effect.RaySegment.Origin + effect.RaySegment.Direction * effect.DisplayLength + state.Offset) * state.Scale;
                        c = Color.LightCyan;
                        r = ((float)(int)c.R) / ((float)(int)Byte.MaxValue);
                        g = ((float)(int)c.G) / ((float)(int)Byte.MaxValue);
                        b = ((float)(int)c.B) / ((float)(int)Byte.MaxValue);
                        Gl.glColor3f(r, g, b);
                        Gl.glVertex3f((float)vect2.X, (float)vect2.Y, 0);
                    }
                }
                Gl.glEnd();

            }


        }

        float interpol = .15f;
        SmoothTransition scaleTransition = new SmoothTransition(.5f,11.5f,.6f);
        SmoothTransition xTransition = new SmoothTransition(0, 800000, 3000);
        SmoothTransition yTransition = new SmoothTransition(0, 800000, 3000);
        TimeDifference timed = new TimeDifference(); 
        public bool DrawGraphics(Size ViewableAreaSize)
        {
            bool returnvalue = false;
            List<Vector2D> positions = new List<Vector2D>();
            Vector2D cameraPosition = Vector2D.Zero;
            int count = 0;
            float scale = LastScale;
            int asteroidCount = 0;
            List<IControlable> Controllables = world.Collidables;

            foreach (IControlable controlable in Controllables)
            {
                if ((controlable.ControlableType & ControlableType.Ship) == ControlableType.Ship)
                {
                    Vector2D position = controlable.Good.Position.Linear;
                    positions.Add(position);
                    cameraPosition += position;
                    count++;
                }
                if ((controlable.ControlableType & ControlableType.Debris) == ControlableType.Debris)
                {
                    asteroidCount++;
                }
            }
            float dt = (float)timed.GetElapsed().TotalSeconds;

            if (count > 1)
            {
                cameraPosition *= (1 / (float)count);
                Vector2D diff = positions[1] - positions[0];
                bool didshift = false;
                if (MathHelper.Abs(diff.X) > ArenaXSize.Upper)
                {
                    didshift = true;
                    cameraPosition.X += ArenaXSize.Upper;
                }
                if (MathHelper.Abs(diff.Y) > ArenaYSize.Upper)
                {
                    didshift = true;
                    cameraPosition.Y += ArenaYSize.Upper;
                }
                if (didshift)
                {
                    BindWorldScreen(cameraPosition);
                    positions.Clear();
                    count = 0;
                    cameraPosition = Vector2D.Zero;
                    foreach (IControlable controlable in Controllables)
                    {
                        if (controlable.ControlableType == ControlableType.Ship)
                        {
                            Vector2D position = controlable.Good.Position.Linear;
                            positions.Add(position);
                            cameraPosition += position;
                            count++;
                        }
                    }
                    cameraPosition *= ((1 / (float)count) );

                }

                Vector2D maxDist = new Vector2D(ViewableAreaSize.Width, ViewableAreaSize.Height)*.4f;// *.25f;
                scale = 1;
                foreach (Vector2D position in positions)
                {
                    Vector2D dist = position - cameraPosition;
                    if (dist.X != 0)
                    {
                        scale = MathHelper.Min(scale, MathHelper.Abs(maxDist.X / (dist.X)));
                    }
                    if (dist.Y != 0)
                    {
                        scale = MathHelper.Min(scale, MathHelper.Abs(maxDist.Y / (dist.Y)));
                    }
                }
                scale *= .75f;

                scale = ((float)((int)(MathHelper.Ceiling( MathHelper.Sqrt(scale) * 30)))) / 30;
                scale *= scale;

                scale = scaleTransition.CalcValue(scale, dt);
                cameraPosition.X = xTransition.CalcValue(cameraPosition.X, dt);
                cameraPosition.Y = yTransition.CalcValue(cameraPosition.Y, dt);
               // cameraPosition = (CamerasLastPosition * interpol2 + cameraPosition * interpol);
               // scale = (LastScale * interpol2 + scale * interpol);


                CamerasLastPosition = cameraPosition;
                LastScale = scale;
            }
            else if (count > 0)
            {
                cameraPosition.X = xTransition.CalcValue(cameraPosition.X, dt);
                cameraPosition.Y = yTransition.CalcValue(cameraPosition.Y, dt);
                scale = scaleTransition.CalcValue(LastScale, dt);
                /*cameraPosition = positions[0] * .02f + CamerasLastPosition * .98f;
                CamerasLastPosition = cameraPosition;
                xTransition.CurrentValue = cameraPosition.X;
                yTransition.CurrentValue = cameraPosition.Y;*/
                //scaleTransition.CurrentValue = scale;
            }
            else
            {
                cameraPosition = CamerasLastPosition;
            }
            GenerateAsteroids(cameraPosition, asteroidCount);
            WindowState state = new WindowState(ViewableAreaSize, scale, cameraPosition);

            dotbox.DrawDots(state);
            for (int pos = statusBoxs.Count - 1; pos > -1; --pos)
            {
                if (statusBoxs[pos].IsExpired)
                {
                    statusBoxs.RemoveAt(pos);
                }
                else
                {
                    statusBoxs[pos].Draw(state);
                }
            }
            DrawLines(state);
            DrawVertexes(state);

            return returnvalue;
        }
        public List<IShip> GetShips()
        {
            List<IShip> ships = new List<IShip>();
            List<IControlable> Controllables = new List<IControlable>(world.Collidables);
            foreach (IControlable controlable in Controllables)
            {
                if ((controlable.ControlableType & ControlableType.Ship) == ControlableType.Ship)
                {
                    IShip ship = controlable as IShip;
                    if (ship != null)
                    {
                        ships.Add(ship);
                    }
                }
            }
            return ships;
        }
    }


    public class BaseDisplayDemo 
    {
        static int DotsCount = 30;
        static int MaxDots = 1000;
        static Vector2D dotsBoxSize = new Vector2D(5000, 5000);
        static Random rand = new Random();

        PlayerControlerConfigInfo player1config;
        PlayerControlerConfigInfo player2config;

        SuperMeleeData data = new SuperMeleeData();

       // ShipLoaderSelector shipSelector;
        FactionInfo AsteroidFaction = FactionInfo.DefaultFactionCollection.CreateNewOrGetFaction("Debris");

        FactionInfo player1 = FactionInfo.DefaultFactionCollection.CreateNewOrGetFaction("Player 1");
        FactionInfo player2 = FactionInfo.DefaultFactionCollection.CreateNewOrGetFaction("Player 2");
        GUI.ShipSelection player1ShipSelection;
        GUI.ShipSelection player2ShipSelection;
        MemoryStream stream;
        BinaryFormatter formater = new BinaryFormatter();
        bool player1IsAI;
        bool player2IsAI;
            int player1WingmanCount;
        int player2WingmanCount;
        public BaseDisplayDemo(
            GUI.ShipSelection player1ShipSelection,
             bool player1IsAI,
            int player1WingmanCount,
            GUI.ShipSelection player2ShipSelection,
            bool player2IsAI,
            int player2WingmanCount
            )
        {
            this.player1ShipSelection = player1ShipSelection;
            this.player1IsAI = player1IsAI;
            this.player1WingmanCount = player1WingmanCount;
            this.player2ShipSelection = player2ShipSelection;
            this.player2IsAI = player2IsAI;
            this.player2WingmanCount = player2WingmanCount;

            Console.WriteLine("Generating Stars");
            GenerateDots();
            Console.WriteLine("Creating World");
            this.data.world = new SuperMeleeWorld();
            Console.WriteLine("Initializing  controls");
            InitControlerConfigs();
            Console.WriteLine("Initializing  Ships");
            //InitShipSelector();
            this.data.GetNewShip = new GetNewShip(GetNewShip);
        }
        public static List<ShipLoader> GetShipLoaders()
        {
            List<ShipLoader> loaders = new List<ShipLoader>();

#if !Release
            loaders.Add(new SuperMelee.Ships.MmrnmhrmXFormShipLoader());
            loaders.Add(new SuperMelee.Ships.UtwigJuggerShipLoader());
            loaders.Add(new SuperMelee.Ships.EarthlingCruiserShipLoader());
            loaders.Add(new SuperMelee.Ships.ShofixtiScoutShipLoader());
            loaders.Add(new SuperMelee.Ships.DruugeMaulerShipLoader());
            loaders.Add(new SuperMelee.Ships.ChmmrAvatarShipLoader());
            loaders.Add(new SuperMelee.Ships.SpathiEluderShipLoader());
            loaders.Add(new SuperMelee.Ships.OrzNemesisShipLoader());
            loaders.Add(new SuperMelee.Ships.MelnormeTraderShipLoader());
            loaders.Add(new SuperMelee.Ships.KzerZaDreadnoughtShipLoader());
            loaders.Add(new SuperMelee.Ships.SupoxBladeShipLoader());
            loaders.Add(new SuperMelee.Ships.MyconPodshipShipLoader());
            loaders.Add(new SuperMelee.Ships.ArilouSkiffShipLoader());
            loaders.Add(new SuperMelee.Ships.AndrosynthGuardianShipLoader());
            loaders.Add(new SuperMelee.Ships.IlwrathAvengerShipLoader());
            loaders.Add(new SuperMelee.Ships.SlylandroProbeShipLoader());
            loaders.Add(new SuperMelee.Ships.SyreenPenetratorShipLoader());
            loaders.Add(new SuperMelee.Ships.KohrAhMarauderShipLoader());
            loaders.Add(new SuperMelee.Ships.UmgahDroneShipLoader());
            loaders.Add(new SuperMelee.Ships.YehatTerminatorShipLoader());
            loaders.Add(new SuperMelee.Ships.VuxIntruderShipLoader());
            loaders.Add(new SuperMelee.Ships.ThraddashTorchShipLoader());
            loaders.Add(new SuperMelee.Ships.ZoqFotPikStingerShipLoader());
            loaders.Add(new SuperMelee.Ships.ChenjesuBroodhomeShipLoader());
            loaders.Add(new SuperMelee.Ships.PkunkFuryShipLoader());
            loaders.Add(new SuperMelee.Ships.EarthlingCarrierShipLoader());


#endif
#if Release
            loaders.AddRange(ConfigHandler.GetShips()); 
#endif
            return loaders;
        }
        

        public void InitControlerConfigs()
        {
            player1config = new PlayerControlerConfigInfo();
            player1config.MoveForward = Key.UpArrow;
            player1config.RotateRight = Key.RightArrow;
            player1config.RotateLeft = Key.LeftArrow;
            player1config.PrimaryAction = Key.Return;
            player1config.SecondaryAction = Key.RightShift;
            player1config.TargetSelection = Key.Slash;
            DoControls(SuperMeleePaths.ConfigDir+"Player1KeyConfig.xml", ref player1config);

            player2config = new PlayerControlerConfigInfo();
            player2config.MoveForward = Key.E;
            player2config.RotateRight = Key.F;
            player2config.RotateLeft = Key.S;
            player2config.PrimaryAction = Key.A;
            player2config.SecondaryAction = Key.W;
            player2config.TargetSelection = Key.Z;
            DoControls(SuperMeleePaths.ConfigDir+"Player2KeyConfig.xml", ref player2config);
        }
        private static void DoControls(string FileName, ref PlayerControlerConfigInfo configinfo)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(configinfo.GetType());
            bool existstest = File.Exists(FileName);
            using (FileStream stream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (existstest)
                {
                    configinfo = (PlayerControlerConfigInfo)ser.Deserialize(stream);
                }
                else
                {
                    ser.Serialize(stream, configinfo);
                }
            }
        }
        public SuperMeleeWorld World
        {
            get
            {
                return data.world;
            }
        }
        public bool Update(float dt)
        {
            data.Update(dt);
            return false;
        }

        public IShip GetNewShip(FactionInfo info)
        {

            IShip ship = null;
            if (info.FactionNumber == 1)
            {
                lock (player1ShipSelection)
                {
                    if (player1ShipSelection.ShipsLeft == 0)
                    {
                        MessageBox.Show("Player 1 Lost!");
                        Environment.Exit(0);
                    }
                    ship = player1ShipSelection.GetNextShip();
                }
            }
            if (info.FactionNumber == 2)
            {
                lock (player2ShipSelection)
                {
                    if (player2ShipSelection.ShipsLeft == 0)
                    {
                        MessageBox.Show("Player 2 Lost!");
                        Environment.Exit(0);
                    }
                    ship = player2ShipSelection.GetNextShip();
                }
            }
            if (ship != null)
            {
                
                ship.Current.Position = new ALVector2D(
                    (float)(rand.NextDouble() * MathHelper.TWO_PI),
                    (float)(rand.NextDouble() * -SuperMeleeData.ArenaSize),
                    (float)(rand.NextDouble() * -SuperMeleeData.ArenaSize));
                ship.SetAllPositions();

            }
            return ship;

            /*
            lock (shipSelector)
            {
                shipSelector.Text = info.FactionName + " Selection";
                shipSelector.ShowDialog();
                if (shipSelector.CurrentLoader != null)//shipSelector.lvShips.SelectedItems.Count > 0)
                {
                    IShip rv = shipSelector.CurrentLoader.Ship;
                    rv.Current = state;
                    rv.SetAllPositions();
                    return rv;

                }
                return null;
            }*/
        }
        public virtual void UpdateKeyBoard(KeyboardState keys,float dt)
        {
            
            if (keys[Key.F5])
            {
                lock (data)
                {
                    long size = 50000;
                    if (stream != null)
                    {
                        size = stream.Length;
                        stream.Close();
                        stream = null;
                    }
                    stream = new MemoryStream((int)size);
                    formater.Serialize(stream, data);
                }
            }
            else if (keys[Key.F6] && stream != null)
            {
                lock (data)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    data = (SuperMeleeData)formater.Deserialize(stream);
                    this.data.GetNewShip = new GetNewShip(GetNewShip);
                }
            }
            else
            {
                foreach (IPlayerControler controler in data.controlers)
                {
                    controler.SetKeyboardState(keys);
                }
            }
        }
        void AddAIClone(GameResult gameResult, FactionInfo faction,IShip target)
        {
            IShip ship = GetNewShip(faction);
            ship.Current.Position.Linear += Vector2D.FromLengthAndAngle(900 * rand.Next(1, 3), (float)rand.NextDouble());
            ship.SetAllPositions();
            ship.AddControler(null,new ComplexAIControler());
            data.AddShip(gameResult, ship, faction);
            //ship.Target = target;
        }
        public virtual void AddObjects()
        {
            GameResult gameResult = new GameResult();
            AsteroidFaction.FactionType = FactionType.Debris;
            SuperMeleeData.AfterChoiceWait = 0;

            IShip ship = GetNewShip(player1);
            if (player1IsAI)
            {
                ship.AddControler(null,new ComplexAIControler());
            }
            else
            {
                SuperMeleeData.AfterChoiceWait = 2500;
                PlayerControler c1 = new PlayerControler(this.player1config);
                data.controlers.Add(c1);
                ship.AddControler(null,c1);
            }
            data.AddShip(gameResult, ship, player1);

            IShip ship2 = GetNewShip(player2);
            if (player2IsAI)
            {
                ship2.AddControler(null, new ComplexAIControler());
            }
            else
            {
                SuperMeleeData.AfterChoiceWait = 2500;
                PlayerControler c2 = new PlayerControler(this.player2config);
                data.controlers.Add(c2);
                ship2.AddControler(null,c2);
            }
            data.AddShip(gameResult, ship2, player2);
            if (ship2 == null)
            {
                return;
            }
            data.SetTarget(ship2);
            data.SetTarget(ship);
            //ship2.Target = ship;
            //ship.Target = ship2;
            for (int pos = 0; pos < player1WingmanCount; ++pos)
            {
                AddAIClone(gameResult, player1, ship2);
            }
            for (int pos = 0; pos < player2WingmanCount; ++pos)
            {
                AddAIClone(gameResult, player2, ship);
            }


            FactionInfo.DefaultFactionCollection.SetDiplomacy(player1, player2, FactionDiplomacy.Enemy);
            PhysicsState tmp = new PhysicsState();
            tmp.Position = new ALVector2D(0, new Vector2D(1500, 1500));
            data.world.AddIGravitySource(Planet.Create(gameResult,tmp));
            data.world.CalcGravity = true;
            World.HandleActionEvents(gameResult);
        }
        private void GenerateDots()
        {
            Random rand = new Random();
            Vector2D[] dots = new Vector2D[DotsCount];
            int[] colors = new int[DotsCount];
            for (int pos = 0; pos != DotsCount; ++pos)
            {
                dots[pos].X = (float)rand.NextDouble() * dotsBoxSize.X;
                dots[pos].Y = (float)rand.NextDouble() * dotsBoxSize.Y;
                switch (rand.Next(3))
                {
                    case 0:
                        colors[pos] = Color.Blue.ToArgb();
                        break;
                    case 1:
                        colors[pos] = Color.Green.ToArgb();
                        break;
                    default:
                        colors[pos] = Color.White.ToArgb();
                        break;
                }
            }
            data.dotbox = new DotBox(MaxDots, dotsBoxSize, dots, colors);
        }

        int player1count = -1;
        int player2count = -1;
        public void DrawGraphics(Size s)
        {
            if (player1count == -1)
            {
                player1count = this.player1ShipSelection.ShipsLeft;
                player2count = this.player2ShipSelection.ShipsLeft;
            }
            float width = 6;
            float screenWidth = (float)Video.Screen.Width;
            float margin = .3f;
            float offset = screenWidth * margin;
            float length = screenWidth * (1 - (2 * margin));
            Gl.glTranslatef(0, width * 1.5f, 0);

            SuperMeleeData.SetColor(SuperMeleeData.teamcolors[1]);
            DrawFilledBox(length * ((float)this.player1ShipSelection.ShipsLeft / (float)player1count), width);
            DrawBox(length, width);

            Gl.glTranslatef(0, -width * 1.5f, 0);
            SuperMeleeData.SetColor(SuperMeleeData.teamcolors[2]);
            DrawFilledBox(length * ((float)this.player2ShipSelection.ShipsLeft / (float)player2count), width);
            DrawBox(length, width);




            data.DrawGraphics(s);
        }
        void DrawFilledBox(float length, float width)
        {
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex2f(0, 0);
            Gl.glVertex2f(length, 0);
            Gl.glVertex2f(length, width);
            Gl.glVertex2f(0, width);
            Gl.glEnd();
        }
        void DrawBox(float length, float width)
        {
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex2f(0, 0);
            Gl.glVertex2f(length, 0);
            Gl.glVertex2f(length, width);
            Gl.glVertex2f(0, width);
            Gl.glEnd();
        }
    }
    [Serializable]
    public class DotBox
    {
        Vector2D size;
        Vector2D sizeInv;
        Vector2D[] dots;
        int[] colors;
        int maxdots;
        Random rand = new Random();
        public DotBox(int maxdots, Vector2D size, Vector2D[] dots, int[] colors)
        {
            this.size = size;
            this.dots = dots;
            this.colors = colors;
            this.maxdots = maxdots;
            this.sizeInv.X = 1 / size.X;
            this.sizeInv.Y = 1 / size.Y;
        }
        public void DrawDots(WindowState state)
        {
            Vector2D Offset = state.Offset;
            BoundingBox2D screenbox = state.ScreenBoundingBox;
            Vector2D startpos = new Vector2D();
            Vector2D endpos = new Vector2D();
            startpos.X = size.X * (float)Math.Floor(screenbox.Lower.X * sizeInv.X);
            startpos.Y = size.Y * (float)Math.Floor(screenbox.Lower.Y * sizeInv.Y);
            endpos.X = size.X * (float)Math.Floor(screenbox.Upper.X * sizeInv.X);
            endpos.Y = size.Y * (float)Math.Floor(screenbox.Upper.Y * sizeInv.Y);
            startpos += Offset;
            endpos += Offset;

            int length = dots.Length;
            Vector2D pos = new Vector2D();
            int count = 0;
            Gl.glBegin(Gl.GL_POINTS);
            for (pos.X = startpos.X; pos.X <= endpos.X; pos.X += size.X)
            {
                for (pos.Y = startpos.Y; pos.Y <= endpos.Y; pos.Y += size.Y)
                {
                    Matrix3x3 matrix = Matrix3x3.FromScale(new Vector2D(state.Scale, state.Scale)) * Matrix3x3.FromTranslate2D(pos);
                    
                   // Vector2D[] values = Vector2D.Transform(matrix, dots);

                    Vector2D[] values = OperationHelper.ArrayRefOp<Matrix3x3, Vector2D, Vector2D>(
                                 ref matrix,
                                 dots,
                                 Vector2D.Multiply);


                    for (int vpos = 0; vpos < length; ++vpos)
                    {
                        state.DrawPoint(values[vpos], colors[vpos]);
                        count++;
                    }
                    if (count > maxdots)
                    {
                        return;
                    }
                }
            }
            Gl.glEnd();
        }
    }
}
