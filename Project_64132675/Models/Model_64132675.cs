using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Project_64132675.Models
{
    public partial class Model_64132675 : DbContext
    {
        public Model_64132675()
            : base("name=Model_64132675")
        {
        }

        public virtual DbSet<BOOKING> BOOKING { get; set; }
        public virtual DbSet<CUSTOMER> CUSTOMER { get; set; }
        public virtual DbSet<EMPLOYEE> EMPLOYEE { get; set; }
        public virtual DbSet<FEATURE> FEATURE { get; set; }
        public virtual DbSet<FLOOR> FLOOR { get; set; }
        public virtual DbSet<PAYMENTSTATUS> PAYMENTSTATUS { get; set; }
        public virtual DbSet<ROOM> ROOM { get; set; }
        public virtual DbSet<ROOMCLASS> ROOMCLASS { get; set; }
        public virtual DbSet<ROOMSTATUS> ROOMSTATUS { get; set; }
        public virtual DbSet<SERVICE> SERVICE { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BOOKING>()
                .Property(e => e.BOOKING_AMOUNT)
                .HasPrecision(10, 2);

            modelBuilder.Entity<BOOKING>()
                .HasMany(e => e.ROOM)
                .WithMany(e => e.BOOKING)
                .Map(m => m.ToTable("BOOKINGROOM").MapLeftKey("BOOKING_ID").MapRightKey("ROOM_ID"));

            modelBuilder.Entity<BOOKING>()
                .HasMany(e => e.SERVICE)
                .WithMany(e => e.BOOKING)
                .Map(m => m.ToTable("BOOKINGSERVICE").MapLeftKey("BOOKING_ID").MapRightKey("SERVICE_ID"));

            modelBuilder.Entity<CUSTOMER>()
                .Property(e => e.GENDER)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CUSTOMER>()
                .Property(e => e.EMAIL)
                .IsUnicode(false);

            modelBuilder.Entity<CUSTOMER>()
                .Property(e => e.PHONE_NUMBER)
                .IsUnicode(false);

            modelBuilder.Entity<CUSTOMER>()
                .Property(e => e.PASSWORD)
                .IsUnicode(false);

            modelBuilder.Entity<EMPLOYEE>()
                .Property(e => e.GENDER)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<EMPLOYEE>()
                .Property(e => e.EMAIL)
                .IsUnicode(false);

            modelBuilder.Entity<EMPLOYEE>()
                .Property(e => e.PHONE_NUMBER)
                .IsUnicode(false);

            modelBuilder.Entity<EMPLOYEE>()
                .Property(e => e.PASSWORD)
                .IsUnicode(false);

            modelBuilder.Entity<ROOMCLASS>()
                .Property(e => e.BASE_PRICE)
                .HasPrecision(10, 2);

            modelBuilder.Entity<ROOMCLASS>()
                .HasMany(e => e.FEATURE)
                .WithMany(e => e.ROOMCLASS)
                .Map(m => m.ToTable("ROOMCLASSFEATURE").MapLeftKey("ROOM_CLASS_ID").MapRightKey("FEATURE_ID"));

            modelBuilder.Entity<SERVICE>()
                .Property(e => e.PRICE)
                .HasPrecision(10, 2);
        }
    }
}
