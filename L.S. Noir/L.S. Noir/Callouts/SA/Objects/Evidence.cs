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
