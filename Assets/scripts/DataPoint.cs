using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Clustering
{
    public class DataPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Cluster { get; set; }

        public DataPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            DataPoint point = (DataPoint)obj;
            return Math.Abs(X - point.X) < 0.00001 && Math.Abs(Y - point.Y) < 0.00001;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0} , {1})", X, Y);
        }

        public static bool operator ==(DataPoint p1, DataPoint p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(DataPoint p1, DataPoint p2)
        {
            return !p1.Equals(p2);
        }
    }
}
