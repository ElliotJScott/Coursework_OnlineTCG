using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace CourseworkClient
{
    class TextField : GuiItem
    {
        public bool selected = false;
        public string text = "";
        readonly int maxLength;
        bool drawBar = false;
        readonly bool hideText;
        readonly string caption;
        int drawBarTimer = 0;
        const int drawBarTimePeriod = 40;

        /// <summary>
        /// Creates a new TextField
        /// </summary>
        /// <param name="r">The bounding box of the card</param>
        /// <param name="maxLen">The maximum length of string that the field can contain</param>
        /// <param name="cap">The caption that the textfield should have. "" means no caption</param>
        /// <param name="hide">Whether the text should be hidden (like a password field)</param>
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
            if (Clicked() && selected == false) selected = true;
            if (DeClicked() && selected == true) selected = false;
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
            sb.DrawString(Primary.game.mainFont, (!hideText ? text : new string('●', text.Length)) + (drawBar ? "|" : ""), new Vector2(boundingBox.X + 2, boundingBox.Y + 4), Color.Black);
            if (caption != null && caption != "")
            {
                sb.Draw(Primary.game.textFieldInfoTab, new Rectangle(boundingBox.X, boundingBox.Y - Primary.game.textFieldInfoTab.Height, Primary.game.textFieldInfoTab.Width, Primary.game.textFieldInfoTab.Height), Color.White);
                sb.DrawString(Primary.game.mainFont, caption, new Vector2(boundingBox.X, boundingBox.Y - Primary.game.textFieldInfoTab.Height), Color.Black);
            }
        }
        public override string ToString() => "TextField: " + selected + "|" + text + "|" + maxLength + "|" + hideText + "|" + drawBarTimer;

    }
}
