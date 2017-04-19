using System;
using System.Diagnostics;
using System.IO;
using LSNoir.Extensions;
using Rage;

namespace LSNoir.Startup
{
    internal class FileChecker
    {
        internal string Path { get; set; }
        internal FileType Description { get; set; }
        internal bool IsRunning{ get; set; }
        internal bool IsSuccessful { get; set; }

        internal FileChecker(string filePath, FileType fileType)
        {
            Path = filePath;
            Description = fileType;
        }

        internal void StartFileCheck()
        {
            try
            {
                IsRunning = true;
                $"Checking for file {Description}".AddLog(true);

                if (File.Exists(this.Path))
                {
                    var lastAccessTime = System.IO.File.GetLastAccessTime(this.Path);

                    // <0 = accessed before now ; 0 = accessed now ; >0 = accessed after the current time
                    if (DateTime.Compare(lastAccessTime, new DateTime(2017, 4, 8)) >= 0)
                    {
                        $"{Description} already viewed, check returns true".AddLog(true);
                        IsRunning = false;
                        IsSuccessful = true;
                    }
                    else
                    {
                        "~r~L.S. Noir Error".DisplayNotification(
                            $"Please view the {Description} before using this plugin." +
                            $"\nPress ~y~Y~w~ to open the file now.", 0);

                        var opened = false;
                        var count = 0;
                        while (count <= 300)
                        {
                            if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Y))
                            {
                                opened = true;
                                break;
                            }
                            GameFiber.Sleep(10);
                            count++;
                        }

                        if (!opened) return;

                        GameFiber.StartNew(delegate
                        {
                            "Opening file".AddLog(true);

                            Process.Start("notepad.exe", Path);

                            while (IsFileinUse(new FileInfo(Path)))
                                GameFiber.Yield();

                            "File closed by user".AddLog(true);
                        });
                    }
                }
                else
                {
                    IsRunning = false;
                    $"File {Description} not found".AddLog(true);
                }
            }
            catch (Exception ex)
            {
                IsRunning = false;
                $"Error getting file: {ex}".AddLog(true);
            }
        }

        internal enum FileType { ini, readme, license }

        protected virtual bool IsFileinUse(FileInfo file)
        {
            try
            {
                file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            return false;
        }
    }
}
