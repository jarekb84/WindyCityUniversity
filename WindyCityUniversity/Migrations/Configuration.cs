using System.Data.Entity.Migrations;
using WindyCityUniversity.DAL;

namespace WindyCityUniversity.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SchoolContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}