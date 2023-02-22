using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_SA
{
    public class Nodes
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public Nodes ()
        {
            
        }
        public Nodes(int Node, int Coordx, int Coordy)
        {
            this.Node = Node;
            this.Coordx = Coordx;
            this.Coordy = Coordy;

        }
        public int Node { get; set; }
        public int Coordx { get; set; }
        public int Coordy { get; set; }
    }
}

