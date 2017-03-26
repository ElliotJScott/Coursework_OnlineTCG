using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CourseworkClient.Gui
{

    class NumericalTextField : TextField
    {
        public NumericalTextField(Rectangle r, int maxLen, string cap = "", bool hide = false) : base(r, maxLen, cap, hide)
        {
        }
        public override void Update()
        {
            if (Clicked() && selected == false) selected = true;
            if (DeClicked() && selected == true) selected = false;
            if (selected)
            {
                string x = Primary.game.keypresshandler.NewTypedString(text, maxLength);
                string l = "";
                foreach (char c in x)
                {
                    if (c >= '0' && c <= '9') l += c;
                }
                text = l;
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
    }
}
