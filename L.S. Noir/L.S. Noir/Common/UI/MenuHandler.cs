using RAGENativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Common.UI
{
    internal class MenuHandler
    {
        private static MenuPool _menuPool = new MenuPool();

        internal static void DrawMenus()
        {
            _menuPool?.ProcessMenus();
        }
        internal static void AddMenu(UIMenu menu)
        {
            if (_menuPool?.Contains(menu) == true) return;
            _menuPool?.Add(menu);
        }
        internal static void Remove(UIMenu menu)
        {
            if (_menuPool?.Contains(menu) != true) return;
            _menuPool?.Remove(menu);
        }

    }
}
