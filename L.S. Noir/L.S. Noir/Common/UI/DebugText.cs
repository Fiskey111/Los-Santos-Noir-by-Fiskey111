using System;
using System.Collections.Generic;
using System.Drawing;
using LSNoir.Common.Process;
using Rage.Native;
using RAGENativeUI.Elements;
using Rectangle = RAGENativeUI.Elements.Rectangle;

namespace LSNoir.Common.UI
{
    public class DebugText
    {
        public static int MaxDebugLines = 25;
        
        public static void AddText(string text)
        {
            _debugList.Add(text);
            if (_debugList.Count > MaxDebugLines) _debugList.RemoveAt(0);
        }

        public static void Initialize()
        {
            var p = new ProcessHost();
            p.StartProcess(Process);
            p.Start();
        }

        private static List<string> _debugList = new List<string>();

        private static DateTime _lastTime = DateTime.Now;
        private static void Process()
        {
            if (_lastTime.CompareTo(DateTime.Now) >= 0)
            {
                _lastTime = _lastTime.AddSeconds(10);
                _debugList.RemoveAt(0);
            }

            var point = new Point(0, 0);
            
            for (var i = 0; i < _debugList.Count; i++)
            {
                var t = new Text(_debugList[i], point, 0.25f, Color.Red);
                t.Draw();
                point = new Point(point.X, point.Y + 10);
            }
            Rectangle.Draw(new Point(0, 0), new Size(0, 10 * _debugList.Count), Color.FromArgb(125, Color.White));
        }
    }

    internal static class InternalNatives
    {
        internal static void GetActiveScreenResolution(out int x, out int y) =>
            NativeFunction.Natives.x873C9F3104101DD3(out x, out y);
    }
}