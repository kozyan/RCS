using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DatabaseContext : DbContext
{

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Spending> Spendings { get; set; }
    public DbSet<Income> Incomes { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=DBRCS.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /// 车辆
        modelBuilder.Entity<Vehicle>().ToTable("Vehicles");

        /// 支出
        modelBuilder.Entity<Spending>().ToTable("Spendings");
        
        /// 收入
        modelBuilder.Entity<Income>().ToTable("Incomes");

        modelBuilder.Entity<Income>().OwnsOne(v => v.Loan, i => {
            i.Property(p => p.Amount).HasColumnName("LoanAmount");
            i.Property(p => p.Date).HasColumnName("LoanDate");
        });
        modelBuilder.Entity<Income>().OwnsOne(v => v.GiftPack, i => {
            i.Property(p => p.Amount).HasColumnName("PackAmount");
            i.Property(p => p.Date).HasColumnName("PackDate");
        });
        modelBuilder.Entity<Income>().OwnsOne(v => v.Gift, i => {
            i.Property(p => p.Amount).HasColumnName("GiftAmount");
            i.Property(p => p.Date).HasColumnName("GiftDate");
        });
        modelBuilder.Entity<Income>().OwnsOne(v => v.Planting, i => {
            i.Property(p => p.Amount).HasColumnName("PlantingAmount");
            i.Property(p => p.Date).HasColumnName("PlantingDate");
        });

        modelBuilder.Ignore<Loan>()
                    .Ignore<GiftPack>()
                    .Ignore<Gift>()
                    .Ignore<Planting>();
    }

}

public enum EntityState {
    Normal = 0,
    New = 1,
    Deleted = 2,
    Modified = 3
}

public class Entity {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get;set;}
    public EntityState State {get;set;} = EntityState.Normal; //默认为普通状态(通常为检索出来时的状态)
}

/// 车辆
public class Vehicle : Entity {
    public string License {get;set;}

    public IList<Spending> Spendings { get; set; } = new List<Spending>();
    public IList<Income> Incomes { get; set; } = new List<Income>();

}

/// 支出
public class Spending : Entity {
    public Vehicle Vehicle {get;set;}
    public Decimal Total {get;set;}
    public DateTime? SpendDate {get;set;}
}

/// 收入
public class Income : Entity {
    public Vehicle Vehicle {get;set;} 
    public Decimal Total {get;set;} ///总收入 = 个贷 + 礼包 + 礼品 + 中植
    public DateTime? IncomeDate {get;set;}

    public Loan Loan {get;set;} = null;
    public GiftPack GiftPack { get; set; } = null;
    public Gift Gift { get; set; } = null;
    public Planting Planting {get;set;} = null;
}

/// 个贷
public class Loan {
    public decimal Amount {get;set;}
    public DateTime Date {get;set;}
}

/// 礼包
public class GiftPack {
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}

/// 礼品
public class Gift {
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}

/// 中植
public class Planting {
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}