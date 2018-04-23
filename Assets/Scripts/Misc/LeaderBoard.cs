using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LeaderBoard
{
    public static LeaderBoard EasyBoard = new LeaderBoard();
    public static LeaderBoard MediumBoard = new LeaderBoard();
    public static LeaderBoard HardBoard = new LeaderBoard();

    public class LeaderBoardItem
    {
        public string Name;
        public int Score;
    }

    public LeaderBoard.LeaderBoardItem[] Table;

    public LeaderBoard()
    {
        Table = new LeaderBoard.LeaderBoardItem[10];
        for (int i = 0; i < 10; i++)
            Table[i] = new LeaderBoard.LeaderBoardItem() { Name = "<unknown>", Score = 0 };
    }

    public void SetPosition(int position, string name, int score)
    {
        var item = Table[position];
        item.Name = name;
        item.Score = score;
    }

}
