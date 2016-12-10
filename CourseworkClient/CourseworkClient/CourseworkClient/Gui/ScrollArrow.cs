using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
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
        public ScrollArrow(Rectangle r, Orientation o)
        {
            boundingBox = r;
            orientation = o;
        }
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(usable ? Primary.game.greenArrowTexture : Primary.game.grayArrowTexture, boundingBox, null, Color.White, ((float)orientation) * ((float)Math.PI / 2f), new Vector2(boundingBox.X + (0.5f * boundingBox.Width), boundingBox.Y + (0.5f * boundingBox.Height)), SpriteEffects.None, 0);
        }

        public override void Update() { }
        
        
    }
}
