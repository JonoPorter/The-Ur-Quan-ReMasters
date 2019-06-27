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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
/*using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;*/
//using Microsoft.DirectX.DirectInput;
using SdlDotNet;
using Physics2D;
using AdvanceMath; using AdvanceSystem;
using AdvanceMath.Geometry2D;
using ReMasters.SuperMelee.Ships;
using Tao.OpenGl;
namespace ReMasters.SuperMelee
{
    [Serializable]
    public sealed class StatusBox : BaseTimed
    {
        public static float Distance = 5;
        public static float Heigth = 40;
        public static float Width = 120;
        public static int HealthColor = Color.Green.ToArgb();
        public static int EnergyColor = Color.Blue.ToArgb();
        public static int BorderColor = Color.Gray.ToArgb();
        static Vector2D[][] baseLines;
        static Vector2D[][] baseVertexes;
        static StatusBox()
        {
            baseLines = new Vector2D[5][];

            baseLines[0] = new Vector2D[2];
            baseLines[0][0] = new Vector2D(-.5f * (Width + 1), 0);
            baseLines[0][1] = new Vector2D(.5f * (Width + 1), 0);

            baseLines[1] = new Vector2D[2];
            baseLines[1][0] = new Vector2D(-.5f * (Width + 1), -.5f * (Heigth + 1));
            baseLines[1][1] = new Vector2D(.5f * (Width + 1), -.5f * (Heigth + 1));

            baseLines[2] = new Vector2D[2];
            baseLines[2][0] = new Vector2D(-.5f * (Width + 1), -(Heigth + 1));
            baseLines[2][1] = new Vector2D(.5f * (Width + 1), -(Heigth + 1));

            baseLines[3] = new Vector2D[2];
            baseLines[3][0] = new Vector2D(.5f * (Width + 1), 0);
            baseLines[3][1] = new Vector2D(.5f * (Width + 1), -(Heigth + 1));

            baseLines[4] = new Vector2D[2];
            baseLines[4][0] = new Vector2D(-.5f * (Width + 1), 0);
            baseLines[4][1] = new Vector2D(-.5f * (Width + 1), -(Heigth + 1));

            baseVertexes = new Vector2D[2][];
            baseVertexes[0] = new Vector2D[4];
            baseVertexes[0][0] = new Vector2D(-.5f * (Width), 0);
            baseVertexes[0][1] = new Vector2D(.5f * (Width), 0);
            baseVertexes[0][2] = new Vector2D(.5f * (Width), -.5f * (Heigth));
            baseVertexes[0][3] = new Vector2D(-.5f * (Width), -.5f * (Heigth));

            baseVertexes[1] = new Vector2D[4];
            baseVertexes[1][0] = new Vector2D(-.5f * (Width), -.5f * (Heigth + 2));
            baseVertexes[1][1] = new Vector2D(.5f * (Width), -.5f * (Heigth + 2));
            baseVertexes[1][2] = new Vector2D(.5f * (Width), -Heigth);
            baseVertexes[1][3] = new Vector2D(-.5f * (Width), -Heigth);
        }
        IControlable controlable;
        public StatusBox(IControlable controlable)
            : base(new LifeSpan(controlable.LifeTime))
        {
            this.controlable = controlable;
        }
        public void Draw(WindowState state)
        {
            if (controlable.IsInvisible)
            {
                return;
            }
            Vector2D basepoint = controlable.Good.Position.Linear;
            basepoint.Y -= (Distance + controlable.BoundingRadius);


            Gl.glBegin(Gl.GL_QUADS);
            for (int pos = 0; pos < 4; pos++)
            {
                Vector2D tmp = baseVertexes[0][pos];
                if (pos == 2 || pos == 1)
                {
                    tmp.X -= (float)(1 - controlable.ShipState.Health.Percent) * Width;
                }
                Vector2D vect = ((tmp + basepoint) + state.Offset) * state.Scale;
                state.DrawPoint(vect, HealthColor);
            }
            for (int pos = 0; pos < 4; pos++)
            {
                Vector2D tmp = baseVertexes[1][pos];
                if (pos == 2 || pos == 1)
                {
                    tmp.X -= (float)(1 - controlable.ShipState.Energy.Percent) * Width;
                }
                Vector2D vect = ((tmp + basepoint) + state.Offset) * state.Scale;
                state.DrawPoint(vect, EnergyColor);
            }
            Gl.glEnd();
            Gl.glBegin(Gl.GL_LINES);
            for (int pos1 = 0; pos1 < 5; pos1++)
            {
                for (int pos2 = 0; pos2 < 2; pos2++)
                {
                    Vector2D vect = ((baseLines[pos1][pos2] + basepoint) + state.Offset) * state.Scale;
                    state.DrawPoint(vect, BorderColor);
                }
            }
            Gl.glEnd();
        }
    }
}