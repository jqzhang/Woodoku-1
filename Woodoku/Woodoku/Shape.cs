using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodoku
{
    public class Shape
    {

        string name;
        bool[,] shape = new bool[5,5];
        Bitmap image;
        int earliestRound;
        int latestRound;
        public Shape(string n, bool[,] array, Bitmap i, int e, int l)
        {

            name = n;
            shape = array;
            image = i;
            earliestRound = e;
            latestRound = l;
        }
        public Shape()
        {
            name = "";
            shape = new bool[0,0];
            image = new Bitmap(1,1);
            earliestRound = 0;
            latestRound = 1000;
        }
        public bool [,] getShape()
        {
            return shape;
        }
        public Bitmap getBitmap()
        {
            return image;
        }
        public string getName()
        {
            return name;
        }
        public int getEarliestRound()
        {
            return earliestRound;
        }
        public int getLatestRound()
        {
            return latestRound;
        }
    }
}
