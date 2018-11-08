using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using duelfighteronline.Models;

namespace duelfighteronline.Context
{
    public class CharacterClassContext : DbContext
    {
        
        public CharacterClassContext() : base("CharacterDatabase")
        { }
        public DbSet<CharacterInfo> CharacterInfo { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            Database.SetInitializer<CharacterClassContext>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }



    }
}