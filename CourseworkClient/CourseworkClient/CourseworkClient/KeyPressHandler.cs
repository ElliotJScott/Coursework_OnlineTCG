using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace CourseworkClient
{
    public class KeyPressHandler
    {
        KeyboardState oldKeyboardState;
        const int ASCII_A = 'A';
        const int ASCII_Z = 'Z';
        const int timerDelay = 35;
        public List<KeyPressTimer> timers = new List<KeyPressTimer>();
        #region dict
        Dictionary[] dictionary = { new Dictionary(Keys.OemPipe, '\\', '|'),
            new Dictionary(Keys.OemComma, ',', '<'),
            new Dictionary(Keys.OemPeriod, '.', '>'),
            new Dictionary(Keys.OemSemicolon, ';', ':'),
            new Dictionary(Keys.OemQuestion, '/', '?'),
            new Dictionary(Keys.OemTilde, '\'', '@'),
            new Dictionary(Keys.OemQuotes, '#', '~'),
            new Dictionary(Keys.D1, '1', '!'),
            new Dictionary(Keys.D2, '2', '"'),
            new Dictionary(Keys.D3, '3', '£'),
            new Dictionary(Keys.D4, '4', '$'),
            new Dictionary(Keys.D5, '5', '%'),
            new Dictionary(Keys.D6, '6', '^'),
            new Dictionary(Keys.D7, '7', '&'),
            new Dictionary(Keys.D8, '8', '*'),
            new Dictionary(Keys.D9, '9', '('),
            new Dictionary(Keys.D0, '0', ')'),
            new Dictionary(Keys.OemOpenBrackets, '[', '{'),
            new Dictionary(Keys.OemCloseBrackets, ']', '}'),
            new Dictionary(Keys.OemMinus, '-', '_'),
            new Dictionary(Keys.OemPlus, '=', '+'),
            new Dictionary(Keys.Space, ' ', ' ')
        };
        #endregion
        /// <summary>
        /// Checks whether a given key has been pressed
        /// </summary>
        /// <param name="k">The key to check</param>
        /// <returns>Whether the key has been pressed</returns>
        public bool HasKeyBeenPressed(Keys k) => oldKeyboardState.IsKeyUp(k) && Keyboard.GetState().IsKeyDown(k);

        /// <summary>
        /// Gets any changes to a given string based on keys pressed
        /// </summary>
        /// <param name="initstring">The initial string</param>
        /// <param name="maxLength">The maximum length of the string</param>
        /// <returns>The updated string</returns>
        public string NewTypedString(string initstring, int maxLength)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            Keys[] newKeys = currentKeyboardState.GetPressedKeys();
            Keys[] oldKeys = oldKeyboardState.GetPressedKeys();
            bool shift = false;
            if (currentKeyboardState.IsKeyDown(Keys.LeftShift) || currentKeyboardState.IsKeyDown(Keys.RightShift)) shift = true;
            foreach (Keys key in newKeys)
            {
                if (!oldKeys.Contains(key))
                {
                    if (key == Keys.Back)
                    {
                        if (initstring.Length > 0) initstring = initstring.Substring(0, initstring.Length - 1);
                    }
                    else if (initstring.Length < maxLength) initstring += OnKeyDownType(key, shift);
                    timers.Add(new KeyPressTimer(key));
                }
                else
                {
                    int index = GetIndexOfTimer(key);
                    if (index >= 0)
                    {
                        timers[index]++;
                        if ((int)timers[index] > timerDelay && initstring.Length < maxLength)
                        {
                            if (key == Keys.Back)
                            {
                                if (initstring.Length > 0) initstring = initstring.Substring(0, initstring.Length - 1);
                            }
                            else initstring += OnKeyDownType(key, shift);
                        }
                    }
                }
            }
            foreach (Keys key in oldKeys)
            {
                if (!newKeys.Contains(key))
                {
                    int index = GetIndexOfTimer(key);
                    if (index >= 0)
                    {
                        timers.RemoveAt(index);
                    }
                }
            }
            return initstring;

        }
        /// <summary>
        /// Gets the string outputted when a key is pressed
        /// </summary>
        /// <param name="key">The key pressed</param>
        /// <param name="shift">Whether shift is held</param>
        /// <returns>The string produced by the key</returns>
        string OnKeyDownType(Keys key, bool shift)
        {
            string keyname = key.ToString();
            char[] chars = keyname.ToCharArray();
            if (chars.Length == 1 && chars[0] >= ASCII_A && chars[0] <= ASCII_Z) return shift ? chars[0].ToString() : chars[0].ToString().ToLower();
            else
                foreach (Dictionary d in dictionary)
                    if (d.key == key) return shift ? d.shiftCharacter.ToString() : d.character.ToString();
            return "";
        }
        /// <summary>
        /// Gets the index of the timer for a given key
        /// </summary>
        /// <param name="k">The key to get the timer of</param>
        /// <returns>The timer of the key</returns>
        int GetIndexOfTimer(Keys k)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i].key == k) return i;
            }
            return -1;
        }
        /// <summary>
        /// Updates the keyboard state once all logic has been performed
        /// </summary>
        public void UpdateOldState()
        {
            oldKeyboardState = Keyboard.GetState();
        }
    }
    /// <summary>
    /// A mapping of a key to what it produces when it is pressed with and without shift pressed
    /// </summary>
    class Dictionary
    {
        public Keys key;
        public char character;
        public char shiftCharacter;
        /// <summary>
        /// Creates a new mapping of a key
        /// </summary>
        /// <param name="k">The key</param>
        /// <param name="c">The output when shift is not held</param>
        /// <param name="sc">The output when shift is held</param>
        public Dictionary(Keys k, char c, char sc)
        {
            key = k;
            character = c;
            shiftCharacter = sc;
        }
    }
    /// <summary>
    /// A timer for how long a given key has been pressed.
    /// </summary>
    public class KeyPressTimer
    {
        public int timer;
        public Keys key;
        /// <summary>
        /// Creates a new timer for a key
        /// </summary>
        /// <param name="k">The key to create the timer for</param>
        public KeyPressTimer(Keys k)
        {
            key = k;
            timer = 0;
        }
        public static explicit operator int (KeyPressTimer k) => k.timer;
        public static KeyPressTimer operator ++(KeyPressTimer k)
        {
            k.timer++;
            return k;
        }
        public override string ToString()
        {
            return key.ToString() + ": " + timer;
        }
    }

}
