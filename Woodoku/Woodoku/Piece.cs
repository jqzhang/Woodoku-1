using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Woodoku
{
    class Piece : Label
    {
        private int x;
        private int y;
        private int size = 40;

        public Piece(int xValue, int yValue)
        {
            x = xValue;
            y = yValue;
            Location = new System.Drawing.Point(xValue, yValue);
            Size = new System.Drawing.Size(size, size);
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            Enabled = true;
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
