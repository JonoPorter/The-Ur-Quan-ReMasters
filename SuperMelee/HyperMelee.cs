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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Physics2D;
using AdvanceMath; using AdvanceSystem;
using AdvanceMath.Geometry2D;
using System.Drawing;
namespace ReMasters.SuperMelee
{
    /// <summary>
    /// Only used to generate the thumbnails.
    /// </summary>
    public class HyperMelee 
    {
        static HyperMelee()
        {
            GenerateDots();
        }
        static DotBox dotbox;
        public static int DotsCount = 60;
        public static int MaxDots = 10000;
        public static int NumberofCircleVertexes = 13;
        //public static List<PointF[]> GetVertexes(Size ViewableAreaSize, float scale, Vector2D cameraPosition, List<ICollidableBody> Collidables)
        public static List<PointF[]> GetVertexes(WindowState wstate, List<ICollidableBody> Collidables)
        {
            
            List<PointF[]> returnvalue = new List<PointF[]>();

            BoundingBox2D screenbox = wstate.ScreenBoundingBox;

            float baseradiusInc = (float)(MathHelper.PI * 2) / ((float)NumberofCircleVertexes);
            float radiusInc = baseradiusInc;
            int numerofCV = NumberofCircleVertexes;
            float sizedCV = 100;
            foreach (Physics2D.ICollidableBody body in Collidables)
            {
                if (body.BoundingBox2D == null)
                {
                    body.CalcBoundingBox2D();
                }
                if (!screenbox.TestIntersection(body.BoundingBox2D))
                {
                    continue;
                }
                foreach (Physics2D.ICollidableBodyPart part in body.CollidableParts)
                {
                    if (part.UseCircleCollision)
                    {

                        if (part.BaseGeometry.BoundingRadius > sizedCV)
                        {
                            numerofCV = NumberofCircleVertexes + (int)MathHelper.Sqrt(part.BaseGeometry.BoundingRadius - sizedCV);
                            if (numerofCV > 100)
                            {
                                numerofCV = 100;
                            }
                            radiusInc = (float)(MathHelper.PI * 2) / ((float)numerofCV);
                        }
                        else
                        {
                            numerofCV = NumberofCircleVertexes;
                            radiusInc = baseradiusInc;
                        }

                        PointF[] points = new PointF[numerofCV];
                        for (int angle = 0; angle != numerofCV; ++angle)
                        {
                            Vector2D vect = Vector2D.FromLengthAndAngle(part.BaseGeometry.BoundingRadius, ((float)angle) * radiusInc + part.GoodPosition.Angular);
                            vect = (vect + wstate.Offset + part.GoodPosition.Linear) * wstate.Scale;
                            points[angle].X = (float)vect.X;
                            points[angle].Y = (float)vect.Y;
                        }
                        returnvalue.Add(points);
                    }
                    else
                    {
                        Vector2D[] vects = OperationHelper.ArrayRefOp<Vector2D, Vector2D, Vector2D>(
                                                part.DisplayVertices,
                                                ref wstate.Offset,
                                                Vector2D.Add);

                        //Vector2D[] vects = Vector2D.Multiply(wstate.Scale, Vector2D.Translate(wstate.Offset, part.DisplayVertices));
                        PointF[] points = new PointF[vects.Length];
                        for (int pos = 0; pos != vects.Length; ++pos)
                        {
                            //Vector2D vect = (vects[pos] + Offset) * scale;
                            points[pos].X = (float)vects[pos].X;
                            points[pos].Y = (float)vects[pos].Y;
                        }
                        returnvalue.Add(points);
                    }
                }
            }
            return returnvalue;
        }
        public static Vector2D dotsBoxSize = new Vector2D(15000, 15000);
        public static void GenerateDots()
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
                        colors[pos] = Color.Wheat.ToArgb();
                        break;
                    case 1:
                        colors[pos] = Color.Green.ToArgb();
                        break;
                    default:
                        colors[pos] = Color.White.ToArgb();
                        break;
                }
            }
            dotbox = new DotBox(MaxDots, dotsBoxSize, dots, colors);

        }
        //public List<PointF[]> GetLines(Size ViewableAreaSize, float scale, Vector2D cameraPosition, List<IRay2DEffect> effects)
        public List<PointF[]> GetLines(WindowState wstate, List<IRay2DEffect> effects)
        {


            List<PointF[]> returnvalue = new List<PointF[]>();


            foreach (IRay2DEffect effect in effects)
            {
                PointF[] points = new PointF[2];
                Vector2D vect1 = (effect.RaySegment.Origin + wstate.Offset) * wstate.Scale;
                points[0].X = (float)vect1.X;
                points[0].Y = (float)vect1.Y;
                Vector2D vect2 = (effect.RaySegment.Origin + effect.RaySegment.Direction * effect.DisplayLength + wstate.Offset) * wstate.Scale;
                points[1].X = (float)vect2.X;
                points[1].Y = (float)vect2.Y;
                returnvalue.Add(points);
            }
            return returnvalue;
        }
        public static PointF[] GetDots(Size ViewableAreaSize, float scale, Vector2D cameraPosition)
        {
            return dotbox.GetDots(ViewableAreaSize, cameraPosition, scale);
        }
        class DotBox
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
            public PointF[] GetDots(Size ViewableAreaSize, Vector2D cameraPosition, float scale)
            {
                Vector2D tmp = new Vector2D(ViewableAreaSize.Width / (2 * scale), ViewableAreaSize.Height / (2 * scale));
                Vector2D Offset = tmp - cameraPosition;
                BoundingBox2D screenbox = new BoundingBox2D(cameraPosition + tmp, cameraPosition - tmp);
                Vector2D startpos = new Vector2D();
                Vector2D endpos = new Vector2D();
                startpos.X = size.X * (float)Math.Floor(screenbox.Lower.X * sizeInv.X);
                startpos.Y = size.Y * (float)Math.Floor(screenbox.Lower.Y * sizeInv.Y);
                endpos.X = size.X * (float)Math.Floor(screenbox.Upper.X * sizeInv.X);
                endpos.Y = size.Y * (float)Math.Floor(screenbox.Upper.Y * sizeInv.Y);
                startpos += Offset;
                endpos += Offset;

                int length = dots.Length;
                List<PointF> returnvalue = new List<PointF>();
                Vector2D pos = new Vector2D();
                for (pos.X = startpos.X; pos.X <= endpos.X; pos.X += size.X)
                {
                    for (pos.Y = startpos.Y; pos.Y <= endpos.Y; pos.Y += size.Y)
                    {
                        Vector2D[] values = OperationHelper.ArrayRefOp<Vector2D, Vector2D, Vector2D>(
                                                dots,
                                                ref pos,
                                                Vector2D.Add);

                        OperationHelper.ArrayRefOp<Vector2D, float , Vector2D>(
                                                values,
                                                ref scale,
                                                values,
                                                Vector2D.Multiply);
                        //Vector2D[] values = Vector2D.Translate(pos,dots);
                        //values = Vector2D.Multiply(scale,values);
                        for (int vpos = 0; vpos < length; ++vpos)
                        {
                            PointF point = new PointF();
                            point.X = (float)values[vpos].X;
                            point.Y = (float)values[vpos].Y;
                            returnvalue.Add(point);
                        }
                        if (returnvalue.Count > maxdots)
                        {
                            return returnvalue.ToArray();
                        }
                    }
                }
                return returnvalue.ToArray();
            }
        }
    }
}
