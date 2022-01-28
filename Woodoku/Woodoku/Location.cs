using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodoku
{
    class Location
    {
        int x;
        int y;
        public Location(int i, int j)
        {
            x = i;
            y = j;
        }
        public Location()
        {
            x = -1;
            y = -1;
        }

        public int getX()
        {
            return x;
        }
        public int getY()
        {
            return y;
        }
    }
}
