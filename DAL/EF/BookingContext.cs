using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DAL.Entities;


namespace DAL.EF
{
    public partial class BookingContext : IdentityDbContext<User>
    {

        #region Constructor
        public BookingContext(DbContextOptions<BookingContext>
        options)
        : base(options)
        { }
        #endregion
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<BookGenre> BookGenre { get; set; }
        public virtual DbSet<Genre> Genre { get; set; }
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<BookAuthor> BookAuthor { get; set; }
        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<Publisher> Publisher { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<BookOrder> BookOrder { get; set; }
        public virtual DbSet<Address> Address { get; set; }

        protected override void OnModelCreating(ModelBuilder
        modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(a => a.User).WithMany(b => b.Orders).HasForeignKey(d => d.UserId);
                entity.HasMany(a => a.BookOrders).WithOne(a => a.Order).HasForeignKey(a => a.IdOrder);
               
            });
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasMany(a => a.BookAuthors).WithOne(a => a.Author).HasForeignKey(a => a.IdAuthor);
            });
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasMany(a => a.Addresses).WithOne(a => a.City).HasForeignKey(a => a.IdCity);
            });
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasMany(a => a.Orders).WithOne(a => a.Address).HasForeignKey(a => a.AddressId);
            });
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasMany(a => a.BookOrders).WithOne(a => a.Book).HasForeignKey(a => a.IdBook);
                entity.HasMany(a => a.Comments).WithOne(a => a.Book).HasForeignKey(a => a.IdBook);
                entity.HasMany(a => a.BookAuthors).WithOne(a => a.Book).HasForeignKey(a => a.IdBook);
                entity.HasMany(a => a.BookGenres).WithOne(a => a.Book).HasForeignKey(a => a.IdBook);
            });
                 modelBuilder.Entity<Publisher>(entity =>
             {
                  entity.HasMany(a => a.Books).WithOne(a => a.Publisher).HasForeignKey(a => a.IdPublisher);
                });
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(a => a.Addresses).WithOne(b => b.User).HasForeignKey(c => c.IdUser);
                entity.HasMany(a => a.Orders).WithOne(b => b.User).HasForeignKey(c => c.UserId);
            });

            modelBuilder.Entity<BookOrder>(entity =>
            {
                entity.HasOne(a => a.Order).WithMany(a => a.BookOrders).HasForeignKey(a => a.IdOrder);
                entity.HasOne(a => a.Book).WithMany(a => a.BookOrders).HasForeignKey(a => a.IdBook);
            });
            modelBuilder.Entity<BookAuthor>(entity =>
            {
                entity.HasOne(a => a.Author).WithMany(a => a.BookAuthors).HasForeignKey(a => a.IdAuthor);
                entity.HasOne(a => a.Book).WithMany(a => a.BookAuthors).HasForeignKey(a => a.IdBook);
            });
            modelBuilder.Entity<BookGenre>(entity =>
            {
                entity.HasOne(a => a.Genre).WithMany(a => a.BookGenres).HasForeignKey(a => a.IdGenre);
                entity.HasOne(a => a.Book).WithMany(a => a.BookGenres).HasForeignKey(a => a.IdBook);
            });
        }

        //public DbSet<DAL.Entities.RegisterViewModel> RegisterViewModel { get; set; }
    }


}
