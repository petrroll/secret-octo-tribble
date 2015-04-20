using Robocode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petrrollus.Helpers
{
    public class CyclicArray<T>
    {
        protected T[] array;
        protected int currIndex;

        public int Lenght { get { return array.Length; } }
        public int LastIndex { get { return array.Length - 1; } }

        public CyclicArray(int lenght)
        {
            array = new T[lenght];
            currIndex = LastIndex;
        }

        public void AddNew(T item)
        {
            currIndex = ++currIndex % array.Length;
            array[currIndex] = item;
        }

        public T GetCurrent()
        {
            return GetLast(0);
        }

        public T GetLast()
        {
            return GetLast(1);
        }

        public T GetLast(int nthLast)
        {
            nthLast = (nthLast > LastIndex) ? LastIndex : nthLast;
            int wantedIndex = (nthLast <= currIndex) ? currIndex - nthLast : array.Length + currIndex - nthLast;

            return array[wantedIndex];
        }
    }

    public class CyclicDoubleArray : CyclicArray<double>
    {
        public CyclicDoubleArray(int lenght)
            : base(lenght)
        {

        }

        public double GetDifference(int nthLast)
        {
            return (array[currIndex] - GetLast(nthLast));
        }

        public double GetDifference()
        {
            return GetDifference(1);
        }
    }

    public class CyclicCoordinatesArray : CyclicArray<Coordinates>
    {
        public CyclicCoordinatesArray(int lenght)
            : base(lenght)
        {

        }

        public Coordinates GetDifference(int nthLast)
        {
            return (array[currIndex] - GetLast(nthLast));
        }

        public Coordinates GetDifference()
        {
            return GetDifference(1);
        }
    }

    public static class DebugDrawer
    {
        public static void DebugDrawRect(Coordinates cord, double a, System.Drawing.Brush b, IGraphics graphicDevice)
        {
            DebugDrawRect(cord.X, cord.Y, a, b, graphicDevice);
        }

        public static void DebugDrawRect(double x, double y, double a, System.Drawing.Brush b, IGraphics graphicDevice)
        {
            graphicDevice.FillRectangle(b, new System.Drawing.RectangleF((float)x, (float)y, (float)a, (float)a));
        }

        public static void DebugDrawLine(double x, double y, double angle, double lenght, System.Drawing.Pen p ,IGraphics graphicDevice)
        {
            double incX = lenght * Math.Sin(angle);
            double incY = lenght * Math.Cos(angle);

            graphicDevice.DrawLine(p, (float)x, (float)y, (float)(x + incX), (float)(y + incY));
        }

        public static void DebugDrawLine(Coordinates cord, double angle, double lenght, System.Drawing.Pen p, IGraphics graphicDevice)
        {
            DebugDrawLine(cord.X, cord.Y, angle, lenght, p, graphicDevice);
        }
    }

    public static class MiscHelper
    {
        public static double GetAngleBetweenPositions(Coordinates originPosition, Coordinates otherPosition)
        {
            Coordinates diff = otherPosition - originPosition;

            double xDiff = diff.X;
            double yDiff = diff.Y;

            return Math.Atan2(xDiff, yDiff);
        }
    }

    public struct Coordinates
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public Coordinates(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Distance()
        {
            return Math.Sqrt(X*X + Y*Y);
        }

        public override bool Equals(object obj)
        {
            return obj is Coordinates && (Coordinates)obj == this;
        }

        public override int GetHashCode()
        {
            return (X.ToString() + ":" + Y.ToString()).GetHashCode();
        }

        public static bool operator ==(Coordinates a, Coordinates b)
        {
            return (a.X == b.X && a.Y == b.Y);
        }

        public static bool operator !=(Coordinates a, Coordinates b)
        {
            return !(a == b);
        }

        public static Coordinates operator +(Coordinates a, Coordinates b)
        {
            return new Coordinates(a.X + b.X, a.Y + b.Y);
        }

        public static Coordinates operator -(Coordinates a, Coordinates b)
        {
            return new Coordinates(a.X - b.X, a.Y - b.Y);
        }

        public static Coordinates operator /(Coordinates a, double b)
        {
            return new Coordinates(a.X / b, a.Y / b);
        }

        public static Coordinates operator *(Coordinates a, double b)
        {
            return new Coordinates(a.X * b, a.Y * b);
        }

    }
}
