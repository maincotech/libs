using Maincotech.EF;
using Maincotech.Localization.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Maincotech.Localization
{
    public class LocalizationDbContext : BoundedDbContext
    {
        public LocalizationDbContext(DbContextOptions<LocalizationDbContext> options) : base(options)
        {
        }

        protected override IEnumerable<IMappingConfiguration> GetConfigurations()
        {
            var configurations = new List<IMappingConfiguration>
            {
                new TermsMappingConfiguration()
            };
            return configurations;
        }

        public override void EnsureCreated()
        {
            base.EnsureCreated(); //Make sure database is created.

            //Make sure table is created.
            this.EnsureTables();
        }

        public override void EnsureDeleted()
        {
            //delete tables
            this.DropTables();
            base.EnsureDeleted();
        }
    }
}