using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CourseworkClient
{
    abstract class GuiItem
    {
        MouseState oldState = new MouseState();
        public Rectangle boundingBox;
        public Texture2D texture;
        public bool Clicked(Vector2 mp)
        {
            MouseState newState = Mouse.GetState();
            bool b = new Rectangle((int)mp.X, (int)mp.Y, 1, 1).Intersects(boundingBox) && newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released;
            oldState = newState;
            return b;
        }
        public bool DeClicked(Vector2 mp)
        {
            MouseState newState = Mouse.GetState();
            bool b = !(new Rectangle((int)mp.X, (int)mp.Y, 1, 1).Intersects(boundingBox)) && newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released;
            oldState = newState;
            return b;
        }
        public abstract void Update();
        public abstract void Draw(SpriteBatch sb);
    }
}
