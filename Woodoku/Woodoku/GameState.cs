using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodoku
{
    class GameState
    {
        bool[,] board;
        List<int> activeButtons;
        List<Shape> Shapes;
        int Score;
        int SpaceScore;
        bool real;
        public GameState(bool[,] b, List<int> bs, List<Shape> s, int score, int spaceScore, bool boo)
        {
            board = b;
            activeButtons = bs;
            Shapes = s;
            Score = score;
            SpaceScore = spaceScore;
            real = boo;
        }
        public bool[,] getBoard()
        {
            return board;
        }
        public List<int> getActiveButtons()
        {
            return activeButtons;
        }
        public List<Shape> getShapes()
        {
            return Shapes;
        }
        public int getScore()
        {
            return Score;
        }
        public int getSpaceScore()
        {
            return SpaceScore;
        }
        public void setScore(int num)
        {
            Score = num;
        }
        public void setSpaceScore(int num)
        {
            SpaceScore = num;
        }
        public void setActiveButtons(List<int> list)
        {
            activeButtons = list;
        }
        public bool getBool()
        {
            return real;
        }
    }
}
