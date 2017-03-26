using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Callouts.SA.Objects
{
    internal class Evidence
    {
        internal string Model { get; set; }
        internal string Description { get; set; }
        internal string PublicName { get; set; }

        internal Evidence(string desc, string model, string pubName)
        {
            Model = model;
            Description = desc;
            PublicName = pubName;
        }
    }
}
