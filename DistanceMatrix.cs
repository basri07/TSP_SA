using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_SA
{
    public class DistanceMatrix
    {
        //belki nesneler klonlanabilir bir aşamada
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public DistanceMatrix()
        {

        }
        public DistanceMatrix(int i, int j, double Distance)
        {
            this.i = i;
            this.j = j;
            this.Distance = Distance;

        }
        public int i { get; set; }
        public int j { get; set; }
        public double Distance { get; set; }
    }
}
