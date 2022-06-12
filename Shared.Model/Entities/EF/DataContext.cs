using Microsoft.EntityFrameworkCore;
using Shared.Model.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Entities.EF
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
    : base(options)
        {
        }
        public DbSet<tblUser> tblUser { get; set; }
        public DbSet<tblParkingSpace> tblParkingSpaces { get; set; }
        public DbSet<tblParkingArea> tblParkingAreas { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*            if (!optionsBuilder.IsConfigured)
                        {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                            optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=123qweasd;database=studentmanagement", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.26-mysql"));
                        }*/
        }

    }
}
