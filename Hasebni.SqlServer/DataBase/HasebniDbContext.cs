using Hasebni.Model.Main;
using Hasebni.Model.Security;
using Hasebni.Model.Setting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hasebni.SqlServer.DataBase
{
    public class HasebniDbContext: IdentityDbContext<HUser, HRole, int, HUserClaim, HUserRole
                                                     , HUserLogin, HRoleClaim, HUserToken>
    {
        public HasebniDbContext(DbContextOptions<HasebniDbContext> options):base(options)
        {

        }
        //  public override  
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Item>()
            .HasOne(g => g.Group)
            .WithMany(i => i.Items)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Purchase>()
           .HasOne(i => i.Item)
           .WithMany(p => p.Purchases)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subscriber>()
           .HasOne(p => p.Purchase)
           .WithMany(s => s.Subscribers)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subscriber>()
          .HasOne(m => m.Member)
          .WithMany(s => s.Subscribers)
          .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Member>()
          .HasOne(p => p.Profile)
          .WithMany(m => m.Members)
          .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Member>()
        .HasOne(p => p.Group)
        .WithMany(s => s.Members)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
        .HasOne(m => m.FromMember)
        .WithMany(m => m.FromNotifications)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
       .HasOne(m => m.ToMember)
       .WithMany(m => m.ToNotifications)
       .OnDelete(DeleteBehavior.NoAction);

        }


        public DbSet<Group> Groups { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Device> Devices { get; set; }


    }
}
