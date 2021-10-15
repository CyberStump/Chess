
namespace Chess.Figures
{
    class Queen : Figure // Королева / Ферзь
    {
        public Queen(char colorSign) : base('Q', colorSign)
        {

        }


        /*private bool CanMove(int i, int j)
        {
            return ( (i == PosX || j == PosY) || (i - PosX == j - PosY));
        }*/
    }
}
