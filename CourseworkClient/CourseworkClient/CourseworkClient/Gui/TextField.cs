using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace CourseworkClient
{
    class TextField : GuiItem
    {
        bool selected = false;
        string text = "";
        readonly int maxLength;
        bool drawBar = false;
        int drawBarTimer = 0;
        const int drawBarTimePeriod = 40;

        public TextField(Rectangle r, int maxLen)
        {
            boundingBox = r;
            maxLength = maxLen;
        }
        public override void Update()
        {

            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            if (Clicked(mousePos) && selected == false) selected = true;
            else if (DeClicked(mousePos) && selected == true) selected = false;
            if (selected)
            {
                text = Primary.game.keypresshandler.NewTypedString(text, maxLength);
                if (drawBarTimer++ == drawBarTimePeriod)
                {
                    drawBar = !drawBar;
                    drawBarTimer = 0;
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, boundingBox, Color.White);
            sb.DrawString(Primary.game.mainFont, text + (drawBar ? "|":""), new Vector2(boundingBox.X + 5, boundingBox.Y + 5), Color.White);
        }
    }
}
