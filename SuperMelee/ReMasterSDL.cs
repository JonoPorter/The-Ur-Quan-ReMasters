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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Physics2D;
using AdvanceMath; 
using AdvanceMath.Geometry2D;
using ReMasters.SuperMelee.Ships;
using System.Media;
using SdlDotNet;
using Tao.OpenGl;
using Tao.Sdl;
using AdvanceSystem;
namespace ReMasters.SuperMelee
{



    /// <summary>
    /// This a converted nehe example.
    /// </summary>
    public class ReMasterSDL
    {
        #region Fields
        BaseDisplayDemo demo;

        public static PhysicsTimer physicsTimer;

        //private MusicDictionary music = new MusicDictionary();
        //Width of screen
        int width = 640;
        //Height of screen
        int height = 640;
        // Bits per pixel of screen
        int bpp = 16;
        // Surface to render on
        Surface screen;

        float timeScale = 1;
        float extraDT;
        float targetDT;
        bool allowSmallerThenTarget = false;
        KeyboardState state = new KeyboardState();
        Thread intergrateThread;
        DateTime lastTime = DateTime.Now;
        /// <summary>
        /// Width of window
        /// </summary>
        protected int Width
        {
            get
            {
                return width;
            }
        }

        /// <summary>
        /// Height of window
        /// </summary>
        protected int Height
        {
            get
            {
                return height;
            }
        }

        /// <summary>
        /// Bits per pixel of surface
        /// </summary>
        protected int BitsPerPixel
        {
            get
            {
                return this.bpp;
            }
        }

        #endregion Fields
        #region Constructors

        /// <summary>
        /// Basic constructor
        /// </summary>
        public ReMasterSDL(BaseDisplayDemo demo)
        {
            Initialize(demo);
        }

        #endregion Constructors
        /// <summary>
        /// Initializes methods common to all NeHe lessons
        /// </summary>
        protected void Initialize(BaseDisplayDemo demo)
        {
#if Release
            try
            {
#endif
                //Mixer.Open(22050, AudioFormat.Default, 2, 2048);



                // Sets keyboard events
                // Sets the ticker to update OpenGL Context
                Events.Tick += new TickEventHandler(this.Tick);
                // Sets the resize window event
                Events.VideoResize += new VideoResizeEventHandler(Events_VideoResize);
                Events.Quit += new QuitEventHandler(Events_Quit);
                // Set the Frames per second.
                Events.Fps = 60;
                targetDT = (float)1 / (float)Events.Fps;
                // Creates SDL.NET Surface to hold an OpenGL scene
                //screen = Video.SetVideoModeWindowOpenGL(width, height, true);
                //Environment.
                //screen = Video.SetVideoModeOpenGL(width, height, 16);
                screen = Video.SetVideoModeWindowOpenGL(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2, true);

                //Video.WindowIcon();
                // Video.
                //Video.SetVideoModeWindowOpenGL(Video.Screen.Width,Video.Screen.Height,true);
                //Tao.Sdl.Sdl.SDL_WM_IconifyWindow();


                // Sets Window icon and title
                this.WindowAttributes();

                MeleeMusic.PlayDefault();
                this.demo = demo;
                physicsTimer = new PhysicsTimer(Update2, 1 / (float)Events.Fps, 3);
                physicsTimer.Start(false);

#if Release
            }
            catch (Exception ex)
            {
                ErrorBox.DisplayError(ex);
                throw;
            }
#endif
        }
        void Events_Quit(object sender, QuitEventArgs e)
        {
            MeleeMusic.Close();
            MeleeSound.Close();
            Events.QuitApplication();
            Application.Exit();
        }
        void Events_MusicFinished(object sender, MusicFinishedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }
        void Events_VideoResize(object sender, VideoResizeEventArgs e)
        {

            //width = screen.Size.Width;
            //height = screen.Size.Height;
            width = e.Width;
            height = e.Height;
            screen = Video.SetVideoModeWindowOpenGL(width, height, true);
            this.InitGL();
            //Reshape();
        }
        /// <summary>
        /// Sets Window icon and caption
        /// </summary>
        protected void WindowAttributes()
        {
            
            Video.WindowIcon();
            Video.WindowCaption = "The Ur-Quan Remasters";
        }
        /// <summary>
        /// Resizes window
        /// </summary>
        protected virtual void Reshape()
        {
            this.Reshape(1.0F);
        }
        /// <summary>
        /// Resizes window
        /// </summary>
        /// <param name="distance"></param>
        protected virtual void Reshape(float distance)
        {
            // Reset The current Viewport
            Gl.glViewport(0, 0, width, height);
            // Select The Projection Matrix
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            // Reset The Projection Matrix
            Gl.glLoadIdentity();
            // Calculate The Aspect Ratio Of The Window
            //Glu.gluPerspective(45.0F, (width / (float)height), 0.1F, distance);

            Glu.gluPerspective(45.0, (width / (float)height), 0, distance);
            // Select The Modelview Matrix
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            // Reset The Modelview Matrix
            Gl.glLoadIdentity();
        }

        /// <summary>
        /// Initializes the OpenGL system
        /// </summary>
        protected virtual void InitGL()
        {
            // Enable Smooth Shading
            Gl.glShadeModel(Gl.GL_SMOOTH);
            // Black Background
            Gl.glClearColor(0.0F, 0.0F, 0.0F, 0.5f);
            // Depth Buffer Setup
            Gl.glClearDepth(1.0F);
            // Enables Depth Testing
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            // The Type Of Depth Testing To Do
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            // Really Nice Perspective Calculations
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
            Gl.glViewport(0, 0, width, height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0, width, 0, height, -100, 100);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
        }


        /// <summary>
        /// Renders the scene
        /// </summary>
        protected virtual void DrawGLScene()
        {

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            //Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[0]);
            Gl.glLoadIdentity();

            demo.DrawGraphics(screen.Size);
        }

        bool first = true;

        private void StartThread()
        {
            Thread.Sleep(4000);
            physicsTimer.Enabled = true;
        }

        private void Tick(object sender, TickEventArgs e)
        {
#if Release
		try
            {  
#endif
            if (first)
            {
                new Thread(StartThread).Start();
                physicsTimer.Enabled = false;
                first = false;
                demo.AddObjects();
                lastTime = DateTime.Now;
            }
            else
            {
                physicsTimer.EndUpdate();
            }
            MeleeSound.PlaySounds();

            DrawGLScene();

            physicsTimer.BeginUpdate();

            Video.GLSwapBuffers();

#if Release
		            }
            catch (Exception ex)
            {
                ErrorBox.DisplayError(ex);
            }  
#endif
        }

        private void Integrate(object dt)
        {
            FileIOPermission permission = new FileIOPermission(PermissionState.Unrestricted);
            permission.AllFiles = FileIOPermissionAccess.AllAccess;
            permission.Deny();
#if Release
		 try
            {  
#endif
            Integrate((float)dt);
#if Release
		}
            catch (ThreadAbortException) { Console.WriteLine("ThreadAbortException in Integrate"); }
            catch (ThreadInterruptedException) { Console.WriteLine("ThreadInterruptedException in Integrate"); }
            catch (Exception ex)
            {
                ErrorBox.DisplayError(ex);
            }  
#endif
        }
        private bool Integrate(float dt)
        {
            float trueDT = dt + extraDT;
            int intergrations = (int)(trueDT / targetDT);
            float timestep = targetDT;
            if (allowSmallerThenTarget)
            {
                if (intergrations > 0)
                {
                    timestep = trueDT / intergrations;
                }
                else
                {
                    intergrations = 1;
                    timestep = trueDT;
                }
            }
            else
            {
                if (trueDT < targetDT || !(intergrations > 0))
                {
                    extraDT += dt;
                    return false;
                }
            }
            //intergrations = 1;
            extraDT = trueDT - intergrations * timestep;
            bool returnvalue = false;
            for (int pos = 0; pos < intergrations; ++pos)
            {
                state.Update();
                demo.UpdateKeyBoard(state, timestep * timeScale);
                returnvalue = demo.Update(timestep * timeScale) || returnvalue;
            }
            return returnvalue;
        }
        void Update2(float dt)
        {
            state.Update();
            demo.UpdateKeyBoard(state, dt);
            demo.Update(dt);
        }

        //		private void Resize (object sender, VideoResizeEventArgs e)
        //		{
        //			screen = Video.SetVideoModeWindowOpenGL(e.Width, e.Height, true);
        //			if (screen.Width != e.Width || screen.Height != e.Height)
        //			{
        //				//this.InitGL();
        //				this.Reshape();
        //			}
        //		}


        /// <summary>
        /// Starts lesson
        /// </summary>
        public void Run()
        {
#if Release
            try
            { 
#endif
            Reshape();
            InitGL();
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Events.Run();
#if Release
		 }
            catch (Exception ex)
            {
                ErrorBox.DisplayError(ex);
            }  
#endif

        }
    }

    //Tao.Sdl.Sdl.
    //SdlDotNet.Key.
    /*public class UQMKeys
    {
        class KeyPair
        {
            public string Name;
            public int Value;
            public KeyPair(string Name, int Value)
            {
                this.Name = Name;
                this.Value = Value;
            }
        }
        static List<KeyPair> keys = new List<KeyPair>();
        static UQMKeys()
        {
            keys.Add(new KeyPair("Backspace", Sdl.SDLK_BACKSPACE));
            keys.Add(new KeyPair("Tab", Sdl.SDLK_TAB));
            keys.Add(new KeyPair("Clear", Sdl.SDLK_CLEAR));
            keys.Add(new KeyPair("Return", Sdl.SDLK_RETURN));
            keys.Add(new KeyPair("Pause", Sdl.SDLK_PAUSE));
            keys.Add(new KeyPair("Escape", Sdl.SDLK_ESCAPE));
            keys.Add(new KeyPair("Space", Sdl.SDLK_SPACE));
            keys.Add(new KeyPair("!", Sdl.SDLK_EXCLAIM));
            keys.Add(new KeyPair("\"", Sdl.SDLK_QUOTEDBL));
            keys.Add(new KeyPair("Hash", Sdl.SDLK_HASH));
            keys.Add(new KeyPair("$", Sdl.SDLK_DOLLAR));
            keys.Add(new KeyPair("&", Sdl.SDLK_AMPERSAND));
            keys.Add(new KeyPair("'", Sdl.SDLK_QUOTE));
            keys.Add(new KeyPair("(", Sdl.SDLK_LEFTPAREN));
            keys.Add(new KeyPair(")", Sdl.SDLK_RIGHTPAREN));
            keys.Add(new KeyPair("*", Sdl.SDLK_ASTERISK));
            keys.Add(new KeyPair("+", Sdl.SDLK_PLUS));
            keys.Add(new KeyPair(",", Sdl.SDLK_COMMA));
            keys.Add(new KeyPair("-", Sdl.SDLK_MINUS));
            keys.Add(new KeyPair(".", Sdl.SDLK_PERIOD));
            keys.Add(new KeyPair("/", Sdl.SDLK_SLASH));
            keys.Add(new KeyPair("0", Sdl.SDLK_0));
            keys.Add(new KeyPair("1", Sdl.SDLK_1));
            keys.Add(new KeyPair("2", Sdl.SDLK_2));
            keys.Add(new KeyPair("3", Sdl.SDLK_3));
            keys.Add(new KeyPair("4", Sdl.SDLK_4));
            keys.Add(new KeyPair("5", Sdl.SDLK_5));
            keys.Add(new KeyPair("6", Sdl.SDLK_6));
            keys.Add(new KeyPair("7", Sdl.SDLK_7));
            keys.Add(new KeyPair("8", Sdl.SDLK_8));
            keys.Add(new KeyPair("9", Sdl.SDLK_9));
            keys.Add(new KeyPair(":", Sdl.SDLK_COLON));
            keys.Add(new KeyPair(";", Sdl.SDLK_SEMICOLON));
            keys.Add(new KeyPair("<", Sdl.SDLK_LESS));
            keys.Add(new KeyPair("=", Sdl.SDLK_EQUALS));
            keys.Add(new KeyPair(">", Sdl.SDLK_GREATER));
            keys.Add(new KeyPair("?", Sdl.SDLK_QUESTION));
            keys.Add(new KeyPair("@", Sdl.SDLK_AT));
            keys.Add(new KeyPair("[", Sdl.SDLK_LEFTBRACKET));
            keys.Add(new KeyPair("\\", Sdl.SDLK_BACKSLASH));
            keys.Add(new KeyPair("]", Sdl.SDLK_RIGHTBRACKET));
            keys.Add(new KeyPair("^", Sdl.SDLK_CARET));
            keys.Add(new KeyPair("_", Sdl.SDLK_UNDERSCORE));
            keys.Add(new KeyPair("`", Sdl.SDLK_BACKQUOTE));
            keys.Add(new KeyPair("a", Sdl.SDLK_a));
            keys.Add(new KeyPair("b", Sdl.SDLK_b));
            keys.Add(new KeyPair("c", Sdl.SDLK_c));
            keys.Add(new KeyPair("d", Sdl.SDLK_d));
            keys.Add(new KeyPair("e", Sdl.SDLK_e));
            keys.Add(new KeyPair("f", Sdl.SDLK_f));
            keys.Add(new KeyPair("g", Sdl.SDLK_g));
            keys.Add(new KeyPair("h", Sdl.SDLK_h));
            keys.Add(new KeyPair("i", Sdl.SDLK_i));
            keys.Add(new KeyPair("j", Sdl.SDLK_j));
            keys.Add(new KeyPair("k", Sdl.SDLK_k));
            keys.Add(new KeyPair("l", Sdl.SDLK_l));
            keys.Add(new KeyPair("m", Sdl.SDLK_m));
            keys.Add(new KeyPair("n", Sdl.SDLK_n));
            keys.Add(new KeyPair("o", Sdl.SDLK_o));
            keys.Add(new KeyPair("p", Sdl.SDLK_p));
            keys.Add(new KeyPair("q", Sdl.SDLK_q));
            keys.Add(new KeyPair("r", Sdl.SDLK_r));
            keys.Add(new KeyPair("s", Sdl.SDLK_s));
            keys.Add(new KeyPair("t", Sdl.SDLK_t));
            keys.Add(new KeyPair("u", Sdl.SDLK_u));
            keys.Add(new KeyPair("v", Sdl.SDLK_v));
            keys.Add(new KeyPair("w", Sdl.SDLK_w));
            keys.Add(new KeyPair("x", Sdl.SDLK_x));
            keys.Add(new KeyPair("y", Sdl.SDLK_y));
            keys.Add(new KeyPair("z", Sdl.SDLK_z));
            keys.Add(new KeyPair("Delete", Sdl.SDLK_DELETE));
            keys.Add(new KeyPair("Keypad-0", Sdl.SDLK_KP0));
            keys.Add(new KeyPair("Keypad-1", Sdl.SDLK_KP1));
            keys.Add(new KeyPair("Keypad-2", Sdl.SDLK_KP2));
            keys.Add(new KeyPair("Keypad-3", Sdl.SDLK_KP3));
            keys.Add(new KeyPair("Keypad-4", Sdl.SDLK_KP4));
            keys.Add(new KeyPair("Keypad-5", Sdl.SDLK_KP5));
            keys.Add(new KeyPair("Keypad-6", Sdl.SDLK_KP6));
            keys.Add(new KeyPair("Keypad-7", Sdl.SDLK_KP7));
            keys.Add(new KeyPair("Keypad-8", Sdl.SDLK_KP8));
            keys.Add(new KeyPair("Keypad-9", Sdl.SDLK_KP9));
            keys.Add(new KeyPair("Keypad-.", Sdl.SDLK_KP_PERIOD));
            keys.Add(new KeyPair("Keypad-/", Sdl.SDLK_KP_DIVIDE));
            keys.Add(new KeyPair("Keypad-*", Sdl.SDLK_KP_MULTIPLY));
            keys.Add(new KeyPair("Keypad--", Sdl.SDLK_KP_MINUS));
            keys.Add(new KeyPair("Keypad-+", Sdl.SDLK_KP_PLUS));
            keys.Add(new KeyPair("Keypad-Enter", Sdl.SDLK_KP_ENTER));
            keys.Add(new KeyPair("Keypad-=", Sdl.SDLK_KP_EQUALS));
            keys.Add(new KeyPair("Up", Sdl.SDLK_UP));
            keys.Add(new KeyPair("Down", Sdl.SDLK_DOWN));
            keys.Add(new KeyPair("Right", Sdl.SDLK_RIGHT));
            keys.Add(new KeyPair("Left", Sdl.SDLK_LEFT));
            keys.Add(new KeyPair("Insert", Sdl.SDLK_INSERT));
            keys.Add(new KeyPair("Home", Sdl.SDLK_HOME));
            keys.Add(new KeyPair("End", Sdl.SDLK_END));
            keys.Add(new KeyPair("PageUp", Sdl.SDLK_PAGEUP));
            keys.Add(new KeyPair("PageDown", Sdl.SDLK_PAGEDOWN));
            keys.Add(new KeyPair("F1", Sdl.SDLK_F1));
            keys.Add(new KeyPair("F2", Sdl.SDLK_F2));
            keys.Add(new KeyPair("F3", Sdl.SDLK_F3));
            keys.Add(new KeyPair("F4", Sdl.SDLK_F4));
            keys.Add(new KeyPair("F5", Sdl.SDLK_F5));
            keys.Add(new KeyPair("F6", Sdl.SDLK_F6));
            keys.Add(new KeyPair("F7", Sdl.SDLK_F7));
            keys.Add(new KeyPair("F8", Sdl.SDLK_F8));
            keys.Add(new KeyPair("F9", Sdl.SDLK_F9));
            keys.Add(new KeyPair("F10", Sdl.SDLK_F10));
            keys.Add(new KeyPair("F11", Sdl.SDLK_F11));
            keys.Add(new KeyPair("F12", Sdl.SDLK_F12));
            keys.Add(new KeyPair("F13", Sdl.SDLK_F13));
            keys.Add(new KeyPair("F14", Sdl.SDLK_F14));
            keys.Add(new KeyPair("F15", Sdl.SDLK_F15));
            keys.Add(new KeyPair("RightShift", Sdl.SDLK_RSHIFT));
            keys.Add(new KeyPair("LeftShift", Sdl.SDLK_LSHIFT));
            keys.Add(new KeyPair("RightControl", Sdl.SDLK_RCTRL));
            keys.Add(new KeyPair("LeftControl", Sdl.SDLK_LCTRL));
            keys.Add(new KeyPair("RightAlt", Sdl.SDLK_RALT));
            keys.Add(new KeyPair("LeftAlt", Sdl.SDLK_LALT));
            keys.Add(new KeyPair("RightMeta", Sdl.SDLK_RMETA));
            keys.Add(new KeyPair("LeftMeta", Sdl.SDLK_LMETA));
            keys.Add(new KeyPair("RightSuper", Sdl.SDLK_RSUPER));
            keys.Add(new KeyPair("LeftSuper", Sdl.SDLK_LSUPER));
            keys.Add(new KeyPair("AltGr", Sdl.SDLK_MODE));
            keys.Add(new KeyPair("Compose", Sdl.SDLK_COMPOSE));
            keys.Add(new KeyPair("Help", Sdl.SDLK_HELP));
            keys.Add(new KeyPair("Print", Sdl.SDLK_PRINT));
            keys.Add(new KeyPair("SysReq", Sdl.SDLK_SYSREQ));
            keys.Add(new KeyPair("Break", Sdl.SDLK_BREAK));
            keys.Add(new KeyPair("Menu", Sdl.SDLK_MENU));
            keys.Add(new KeyPair("Power", Sdl.SDLK_POWER));
            keys.Add(new KeyPair("Euro", Sdl.SDLK_EURO));
            keys.Add(new KeyPair("Undo", Sdl.SDLK_UNDO));
            keys.Add(new KeyPair("Unknown", 0));
        }
        public static int GetValue(string name)
        {
            foreach (KeyPair pair in keys)
            {
                if (pair.Name == name)
                {
                    return pair.Value;
                }
            }
            return 0;
        }
        public static string GetName(int value)
        {
            foreach (KeyPair pair in keys)
            {
                if (pair.Value == value)
                {
                    return pair.Name;
                }
            }
            return "Unknown";
        }
    }*/

}
