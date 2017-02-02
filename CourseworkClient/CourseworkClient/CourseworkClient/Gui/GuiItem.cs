using CourseworkClient.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CourseworkClient
{
    /// <summary>
    /// All non form child classes of IFormItem inherit from this
    /// </summary>
    public abstract class GuiItem : IFormItem
    {
        public static MouseState oldState = new MouseState();
        public List<GuiItem> subItems = new List<GuiItem>();
        public Rectangle boundingBox;
        public Texture2D texture;
        /// <summary>
        /// Gets whether the item was clicked
        /// </summary>
        /// <returns>Returns whether the item was clicked</returns>
        public bool Clicked()
        {
            MouseState newState = Mouse.GetState();
            bool b = new Rectangle(newState.X, newState.Y, 1, 1).Intersects(boundingBox) && newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released;
            return b;
        }
        /// <summary>
        /// Gets whether the user clicked not on this item
        /// </summary>
        /// <returns>Returns whether the user clicked not on this item</returns>
        public bool DeClicked()
        {
            MouseState newState = Mouse.GetState();
            bool b = (!(new Rectangle(newState.X, newState.Y, 1, 1).Intersects(boundingBox))) && newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released;
            return b;
        }
        /// <summary>
        /// Updates what the state of the mouse was in the current frame once all logic for the current frame has been executed
        /// </summary>
        public static void UpdateOldState()
        {
            oldState = Mouse.GetState();
        }

        public abstract void Draw(SpriteBatch sb);
        public abstract void Update();
    }
}
