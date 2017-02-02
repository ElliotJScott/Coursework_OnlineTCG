using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    /// <summary>
    /// Enumeration of rotation for various things
    /// </summary>
    enum Orientation
    {
        Up,
        Right,
        Down,
        Left
    }
    class ScrollArrow : GuiItem
    {
        Orientation orientation;
        public bool usable;
        /// <summary>
        /// Creates a new ScrollArrow
        /// </summary>
        /// <param name="r">The bounding box of the arrow</param>
        /// <param name="o">The orientation that the arrow should be drawn at</param>
        public ScrollArrow(Rectangle r, Orientation o)
        {
            boundingBox = r;
            orientation = o;
            //Console.WriteLine("new ScrollArrow with rectangle ({0}, {1}, {2}, {3})", r.X, r.Y, r.Width, r.Height);
        }
        public override void Draw(SpriteBatch sb)
        {
            //Vector2 rotato = new Vector2(boundingBox.X + (0.5f * boundingBox.Width), boundingBox.Y + (0.5f * boundingBox.Height)); Rotations are dumb because they're done relative to the graphic not the frame
            Vector2 rotato = new Vector2(boundingBox.Width / 2, boundingBox.Height / 2);
            float rotation = ((float)orientation) * ((float)Math.PI / 2f);
            sb.Draw(usable ? Primary.game.greenArrowTexture : Primary.game.grayArrowTexture, new Rectangle (boundingBox.X + (int)rotato.X, boundingBox.Y + (int)rotato.Y, boundingBox.Width, boundingBox.Height), null, Color.White, rotation, rotato, SpriteEffects.None, 1);
        }

        public override void Update() { }
        
        
    }
}
