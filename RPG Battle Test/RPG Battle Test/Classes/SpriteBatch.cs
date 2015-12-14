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
    //A pseudo-spritebatch for drawing renderable objects together and ordering by depth
    public class SpriteBatch
    {
        private List<BatchObject> Batch = null;

        private struct BatchObject
        {
            public Drawable DrawableObj;

            /// <summary>
            /// Depth, 0 being behind and higher numbers being in front
            /// </summary>
            public int Depth;

            public BatchObject(Drawable drawableobj, int depth)
            {
                DrawableObj = drawableobj;
                Depth = depth;
            }
        }

        public SpriteBatch()
        {
            Batch = new List<BatchObject>();
        }

        /// <summary>
        /// Sorts the batch by depth, rendering those with lower values first
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private int SortBatch(BatchObject first, BatchObject second)
        {
            if (first.Depth < second.Depth)
                return -1;
            else if (first.Depth > second.Depth)
                return 1;
            return 0;
        }

        public void Draw(Sprite sprite, Vector2f position, int depth)
        {
            Sprite newsprite = new Sprite(sprite);
            sprite.Position = position;
            BatchObject batchobj = new BatchObject(newsprite, depth);
            Batch.Add(batchobj);
        }

        public void Draw(Sprite sprite, Vector2f position, float rotation, int depth)
        {
            Sprite newsprite = new Sprite(sprite);
            sprite.Position = position;
            sprite.Rotation = rotation;
            BatchObject batchobj = new BatchObject(newsprite, depth);
            Batch.Add(batchobj);
        }

        public void Draw(Sprite sprite, Vector2f position, float rotation, Vector2f originRelative, int depth)
        {
            Sprite newsprite = new Sprite(sprite);
            sprite.Position = position;
            sprite.Rotation = rotation;
            sprite.Origin = Helper.GetSpriteOrigin(sprite, originRelative.X, originRelative.Y);
            BatchObject batchobj = new BatchObject(newsprite, depth);
            Batch.Add(batchobj);
        }

        public void DrawText(string text, Vector2f position, int depth)
        {
            DrawText(new Text(text, new Font(Constants.ContentPath + "arial.ttf")), position, depth);
        }

        public void DrawText(string text, Vector2f position, Vector2f originRelative, int depth)
        {
            DrawText(new Text(text, new Font(Constants.ContentPath + "arial.ttf")), position, originRelative, depth);
        }

        public void DrawText(Text text, Vector2f position, int depth)
        {
            Text newtext = new Text(text);
            text.Position = position;
            BatchObject batchobj = new BatchObject(text, depth);
            Batch.Add(batchobj);
        }

        public void DrawText(Text text, Vector2f position, Vector2f originRelative, int depth)
        {
            Text newtext = new Text(text);
            text.Position = position;
            text.Origin = Helper.GetTextOrigin(text, originRelative.X, originRelative.Y);
            BatchObject batchobj = new BatchObject(text, depth);
            Batch.Add(batchobj);
        }

        //End drawing, and draw everything, sorting by depth
        public void End()
        {
            Batch.Sort(SortBatch);

            for (int i = 0; i < Batch.Count; i++)
            {
                Batch[i].DrawableObj.Draw(GameCore.GameWindow, RenderStates.Default);
            }

            Batch.Clear();
        }
    }
}
