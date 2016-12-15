using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    class NumericalTextField : TextField
    {
        public NumericalTextField(Rectangle r, int maxLen, string cap = "", bool hide = false) : base(r, maxLen, cap, hide)
        {
        }
    }
}
