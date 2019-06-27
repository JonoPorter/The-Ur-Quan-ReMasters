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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
/*using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;*/
namespace ReMasters.SuperMelee
{
   /* public partial class ReMasterForm : Form
    {
        BaseDisplayDemo demo;
        Thread demoThread;
        private Microsoft.DirectX.Direct3D.Device device;
        private Microsoft.DirectX.DirectInput.Device keyboard;
        bool runThread = true;
        float timeScale = .8;
        float extraDT = 0;
        float targetDT = .020;
        bool allowSmallerThenTarget = true;
        public ReMasterForm(BaseDisplayDemo demo)
        {
            demo.CalcGraphics(this.Size);
            InitializeComponent();
            #region keyboard


            keyboard = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);

            keyboard.SetCooperativeLevel(this, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);

            keyboard.Acquire();
            #endregion
            #region device
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;

            device = new Microsoft.DirectX.Direct3D.Device(0, Microsoft.DirectX.Direct3D.DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque , true);
            #endregion
            this.demo = demo;
            this.BackColor = demo.BackgroundColor;
            this.demoThread = new Thread(new ThreadStart(DemoProcess));
            this.demoThread.Start();
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
                if (trueDT < targetDT)
                {
                    extraDT += dt;
                    return false;
                }
            }
            extraDT = trueDT - intergrations * timestep;
            bool soundInfo = false;
            for (int pos = 0; pos < intergrations; ++pos)
            {
                demo.UpdateKeyBoard(keyboard.GetCurrentKeyboardState(), timestep * timeScale);
                soundInfo = demo.Update(timestep * timeScale) || soundInfo;
                soundInfo = demo.CalcGraphics(this.Size) || soundInfo;
                new Thread(new ThreadStart(Invalidate)).Start();
            }
            return soundInfo;
        }
        private void DemoProcess()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            try
            {
                demo.InitObjects();
                demo.AddObjects();
                demo.CalcGraphics(this.Size);
                this.Invalidate();
                Thread.Sleep(2000);
                demo.World.Enabled = true;
                DateTime lastRan = DateTime.Now;
                while (this.Enabled && !this.Disposing && runThread)
                {
                    DateTime now = DateTime.Now;
                    float dt = now.Subtract(lastRan).TotalSeconds;
                    lastRan = now;
                    if (runThread)
                    {
                        if (dt > 1)
                        {
                            dt = 1;
                        }
                        if (this.demo.World.Enabled)
                        {
                            try
                            {
                                demo.UpdateKeyBoard(keyboard.GetCurrentKeyboardState(), dt);
                            }
                            catch { return; }
                            if (Integrate(dt ))
                            {
                                lastRan = DateTime.Now;
                            }
                        }
                        else
                        {
                            demo.UpdateKeyBoard(keyboard.GetCurrentKeyboardState(), dt);
                            demo.CalcGraphics(this.Size);
                            new Thread(new ThreadStart(Invalidate)).Start();
                            
                        }
                    }
                    int sleeptime = (int)((targetDT -  dt) * 700);
                    if (sleeptime > 0)
                    {
                        Thread.Sleep(sleeptime);
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
            catch (System.Threading.ThreadInterruptedException) { return; }
            catch(Exception ex) {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
                while ((ex = ex.InnerException) != null)
                {
                    MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
                }
            }
        }
        private void DemoProcessOLD()
        {
            try
            {
                demo.InitObjects();
                demo.AddObjects();
                demo.CalcGraphics(this.Size);
                Thread.Sleep(2000);
                demo.World.Enabled = true;
                DateTime lastRan = DateTime.Now;
                while (this.Enabled && !this.Disposing && runThread)
                {


                    float dt = DateTime.Now.Subtract(lastRan).TotalSeconds;
                    float dtMW = (dt - targetDT);
                    if (dtMW < 0)
                    {
                        lastRan = DateTime.Now;
                        Thread.Sleep(TimeSpan.FromSeconds(-dtMW*.7));
                        dt += DateTime.Now.Subtract(lastRan).TotalSeconds;;
                    }
                    lastRan = DateTime.Now;

                    if (runThread)
                    {
                        try
                        {
                            demo.UpdateKeyBoard(keyboard.GetCurrentKeyboardState(), dt);
                        }
                        catch { return; }
                        if (this.demo.World.Enabled)
                        {
                            if (demo.Update(dt))
                            {
                                lastRan = DateTime.Now;
                            }
                        }
                        demo.CalcGraphics(this.Size);
                        this.Invalidate();
                    }
                }
            }
            catch (System.Threading.ThreadInterruptedException) { }
        }
        DateTime lastRun = DateTime.Now;
        private void ReMasterForm_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                Microsoft.DirectX.Direct3D.CustomVertex.TransformedColored[] dots = demo.Dots;
                List<Microsoft.DirectX.Direct3D.CustomVertex.TransformedColored[]> vertexes = demo.Vertexes;
                List<Microsoft.DirectX.Direct3D.CustomVertex.TransformedColored[]> lines = demo.Lines;
                device.Clear(ClearFlags.Target, demo.BackgroundColor, 1.0f, 0);
                device.BeginScene();
                device.VertexFormat = Microsoft.DirectX.Direct3D.CustomVertex.TransformedColored.Format;
                if (dots != null && dots.Length > 0)
                {
                    device.DrawUserPrimitives(PrimitiveType.PointList, dots.Length, dots);
                }
                foreach (CustomVertex.TransformedColored[] points in lines)
                {
                    device.DrawUserPrimitives(PrimitiveType.LineList, points.Length, points);
                }
                foreach (Microsoft.DirectX.Direct3D.CustomVertex.TransformedColored[] points in vertexes)
                {
                    device.DrawUserPrimitives(PrimitiveType.TriangleFan, points.Length - 2, points);
                }

                device.EndScene();
                device.Present();
            }
            catch { }
        }
        private void StopDemoThread()
        {
            if (runThread)
            {
                keyboard.Dispose();
                device.Dispose();
            }
            runThread = false;
            if (demoThread.ThreadState == ThreadState.WaitSleepJoin)
            {
                this.demoThread.Interrupt();
                this.demoThread.Join(500);
            }
            if (demoThread.ThreadState == ThreadState.Running)
            {
                this.demoThread.Abort();
            }
        }
        private void ReMasterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopDemoThread();

        }
        private void ReMasterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopDemoThread();
        }
        private void ReMasterForm_Load(object sender, EventArgs e)
        {

        }
        private void timer1_Tick(object sender, EventArgs e)
        {

            //if (Integrate(.023))
            //{
                //lastRan = DateTime.Now;
           // }
        }
    }*/
}