﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BankingApplication.Api.Entities;

namespace BankingApplication.Api.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>().Property(t => t.UserFrom).IsOptional();
            modelBuilder.Entity<Transaction>().Property(t => t.UserTo).IsOptional();

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Transaction> Transactions { get; set; }

    }
}