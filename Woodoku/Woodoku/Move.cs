using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodoku
{
    class Move
    {
        Location location;
        int number;
        public Move(Location l, int num)
        {
            location = l;
            number = num;
        }
        public Move()
        {
            location = new Location();
            number = 0;
        }
        public Location getLocation()
        {
            return location;
        }
        public int getSelectedShape()
        {
            return number;
        }
    }
}
