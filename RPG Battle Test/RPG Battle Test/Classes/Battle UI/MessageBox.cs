using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;

namespace RPG_Battle_Test
{
    //A generic message box with a border
    public class MessageBox
    {
        protected List<Sprite> Parts = new List<Sprite>();

        //Get or set the position of the entire message box
        //The position is the very middle
        public virtual Vector2f Position 
        {
            get
            {
                return Middle.Position;
            }
            set
            {
                Sprite M = Middle;
                M.Position = value;

                int width = (int)(M.Texture.Size.X * (Math.Abs(M.Scale.X) / 2));
                int height = (int)(M.Texture.Size.Y * (Math.Abs(M.Scale.Y) / 2));

                //Have all the parts come together by using the middle as a reference point
                //The + 1 for R and B is required due to the origin being in the middle (likely truncation due to int division)
                //The order is: TL, TR, L, R, BL, BR, T, B
                Parts[0].Position = new Vector2f(M.Position.X - width, M.Position.Y - height);
                Parts[1].Position = new Vector2f(M.Position.X + width, M.Position.Y - height);
                Parts[2].Position = new Vector2f(M.Position.X - width, M.Position.Y);
                Parts[3].Position = new Vector2f(M.Position.X + width + 1, M.Position.Y);
                Parts[4].Position = new Vector2f(M.Position.X - width, M.Position.Y + height);
                Parts[5].Position = new Vector2f(M.Position.X + width, M.Position.Y + height);
                Parts[6].Position = new Vector2f(M.Position.X, M.Position.Y - height);
                Parts[7].Position = new Vector2f(M.Position.X, M.Position.Y + height + 1);
            }
        }

        protected Sprite Middle => Parts[Parts.Count - 1];
        public Vector2f Size => Middle.Scale;

        //3 Sprites compose the message box, which are flipped accordingly
        //Size doesn't count the additional size added by the corners
        public MessageBox(uint sizeX, uint sizeY, Vector2f position)
        {
            //Top left
            Sprite TL = Helper.CreateSprite(new Texture(Constants.ContentPath + "MessageBox\\MsgBoxTL.png"), false);
            Sprite TR = new Sprite(TL);
            TR.Scale = new Vector2f(-1, 1);

            //Bottom left
            Sprite BL = new Sprite(TL);
            BL.Scale = new Vector2f(1, -1);
            Sprite BR = new Sprite(TL);
            BR.Scale = new Vector2f(-1, -1);

            //Left
            Sprite L = Helper.CreateSprite(new Texture(Constants.ContentPath + "MessageBox\\MsgBoxL.png"), false);
            Sprite R = new Sprite(L);
            L.Scale = new Vector2f(1, sizeY);
            R.Scale = new Vector2f(-1, sizeY);

            //Top
            Sprite T = Helper.CreateSprite(new Texture(Constants.ContentPath + "MessageBox\\MsgBoxT.png"), false);
            Sprite B = new Sprite(T);
            T.Scale = new Vector2f(sizeX, 1);
            B.Scale = new Vector2f(sizeX, -1);

            //Middle
            Sprite M = Helper.CreateSprite(new Texture(Constants.ContentPath + "MessageBox\\MsgBoxM.png"), false);
            M.Scale = new Vector2f(sizeX, sizeY);

            Parts.Add(TL);
            Parts.Add(TR);
            Parts.Add(L);
            Parts.Add(R);
            Parts.Add(BL);
            Parts.Add(BR);
            Parts.Add(T);
            Parts.Add(B);
            Parts.Add(M);

            Position = position;
        }

        public void Resize(uint newx, uint newy)
        {
            Sprite M = Middle;

            //If the box is already this size, don't bother resizing
            if (M.Scale.X == newx && M.Scale.Y == newy)
            {
                Debug.Log("Didn't resize the MessageBox because it is already this size!");
                return;
            }

            M.Scale = new Vector2f(newx, newy);

            Parts[2].Scale = new Vector2f(1, newy);
            Parts[3].Scale = new Vector2f(-1, newy);
            Parts[6].Scale = new Vector2f(newx, 1);
            Parts[7].Scale = new Vector2f(newx, -1);

            //Set position to itself to have the parts come together
            Position = Position;
        }

        public void Resize(Vector2f diff)
        {
            Resize((uint)(Size.X + diff.X), (uint)(Size.Y + diff.Y));
        }

        public virtual void Draw()
        {
            for (int i = 0; i < Parts.Count; i++)
            {
                float layer = Constants.BASE_UI_LAYER + (.01f + (i * .01f));
                GameCore.spriteSorter.Add(Parts[i], layer);
                //Parts[i].Draw(GameCore.GameWindow, RenderStates.Default);
            }
        }
    }
}
