using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CourseworkClient
{
    public abstract class GuiItem
    {
        public static MouseState oldState = new MouseState();
        public List<GuiItem> subItems = new List<GuiItem>();
        public Rectangle boundingBox;
        public Texture2D texture;
        public bool Clicked(Vector2 mp)
        {
            MouseState newState = Mouse.GetState();
            bool b = new Rectangle((int)mp.X, (int)mp.Y, 1, 1).Intersects(boundingBox) && newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released;
            return b;
        }
        public bool DeClicked(Vector2 mp)
        {
            MouseState newState = Mouse.GetState();
            bool b = (!(new Rectangle((int)mp.X, (int)mp.Y, 1, 1).Intersects(boundingBox))) && newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released;
            return b;
        }
        public static void UpdateOldState()
        {
            oldState = Mouse.GetState();
        }
        public abstract void Update();
        public abstract void Draw(SpriteBatch sb);
    }
}
