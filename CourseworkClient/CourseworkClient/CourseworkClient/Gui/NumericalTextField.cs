using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    class NumericalTextField : TextField
    {
        public NumericalTextField(Rectangle r, int maxLen, string cap = "", bool hide = false) : base(r, maxLen, cap, hide)
        {
        }
    }
}
