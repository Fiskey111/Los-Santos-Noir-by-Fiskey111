using Rage;
using Rage.Native;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Common.UI
{
    internal class OnscreenTextbox
    {
        public static string DisplayBox(int maxLength, string windowTitle, string defaultText)
        {
            var value = GetBox(maxLength, windowTitle, defaultText);
            return value ?? string.Empty;
        }

        private static string GetBox(int maxLength, string windowTitle, string defaultText)
        {
            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(true, "", "", defaultText, "", "", "", maxLength + 1);
            var scaleform = new Scaleform();
            scaleform.Load("TEXT_INPUT_BOX");
            scaleform.CallFunction("SET_TEXT_BOX", "", windowTitle, defaultText);
            scaleform.CallFunction("SET_MULTI_LINE");

            var update = 0;
            while (update == 0)
            {
                scaleform.Render2D();
                try
                {
                    update = NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>();
                }
                catch (NullReferenceException)
                {
                }
                GameFiber.Yield();
            }
            string result;
            try
            {
                result = NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
            }
            catch (NullReferenceException)
            {
                result = null;
            }
            return result;
        }
    }
}
