using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace CourseworkClient
{
    class TextField : GuiItem
    {
        public bool selected = false;
        string text = "";
        readonly int maxLength;
        bool drawBar = false;
        readonly bool hideText;
        readonly string caption;
        int drawBarTimer = 0;
        const int drawBarTimePeriod = 40;

        public TextField(Rectangle r, int maxLen, string cap = "", bool hide = false)
        {
            hideText = hide;
            caption = cap;
            boundingBox = r;
            maxLength = maxLen;
            texture = Primary.game.textFieldTexture;
        }
        public override void Update()
        {
            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            if (Clicked(mousePos) && selected == false) selected = true;
            if (DeClicked(mousePos) && selected == true) selected = false;
            if (selected)
            {
                text = Primary.game.keypresshandler.NewTypedString(text, maxLength);
                if (drawBarTimer++ == drawBarTimePeriod)
                {
                    drawBar = !drawBar;
                    drawBarTimer = 0;
                }
            }
            else
            {
                drawBar = false;
                drawBarTimer = 0;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, boundingBox, Color.White);
            sb.DrawString(Primary.game.mainFont, (!hideText ? text : new string('●', text.Length)) + (drawBar ? "|" : ""), new Vector2(boundingBox.X + 5, boundingBox.Y), Color.Black);
            sb.DrawString(Primary.game.mainFont, ToString(), new Vector2(boundingBox.X + boundingBox.Width, boundingBox.Y), Color.Black);
        }
        public override string ToString() => "TextField: " + selected + "|" + text + "|" + maxLength + "|" + hideText + "|" + drawBarTimer;

    }
}
