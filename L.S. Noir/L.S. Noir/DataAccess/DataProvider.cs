namespace LSNoir.DataAccess
{
    class DataProvider
    {
        private static IDataProvider provider;

        public static IDataProvider Instance => provider ?? (provider = new XmlDataProvider());
    }
}
