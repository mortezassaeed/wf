using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities;

public class WorkflowDbContext : DbContext
{



    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options)
            : base(options)
    {
    }

    public DbSet<Process> Processes { get; set; }
    public DbSet<ProcessAllowedDataType> ProcessAllowedDataTypes { get; set; }
    public DbSet<ProcessStep> ProcessSteps { get; set; }
    public DbSet<ProcessStepAction> ProcessStepActions { get; set; }
    //public DbSet<ProcessStepTransition> ProcessStepTransitions { get; set; }
    public DbSet<ProcessInstance> ProcessInstances { get; set; }
    public DbSet<ProcessInstanceStep> ProcessInstanceSteps { get; set; }
    public DbSet<ProcessInstanceHistory> ProcessInstanceHistories { get; set; }

    // TPT Base and Derived Types
    public DbSet<ProcessInstanceDataBase> ProcessInstanceDataBase { get; set; }
    public DbSet<LeaveRequestData> LeaveRequestData { get; set; }
    public DbSet<PurchaseRequestData> PurchaseRequestData { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureProcessEntities(modelBuilder);
        ConfigureProcessInstanceData(modelBuilder);
    }

    private void ConfigureProcessEntities(ModelBuilder modelBuilder)
    {
        // Process
        modelBuilder.Entity<Process>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<ProcessAllowedDataType>(entity =>
        {
            entity.HasIndex(e => new { e.ProcessId, e.DataType }).IsUnique();

            entity.HasOne(e => e.Process)
                .WithMany(p => p.ProcessAllowedDataTypes)
                .HasForeignKey(e => e.ProcessId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ProcessStep
        modelBuilder.Entity<ProcessStep>(entity =>
        {
            entity.HasIndex(e => new { e.ProcessId, e.Code }).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.Process)
                .WithMany(p => p.ProcessSteps)
                .HasForeignKey(e => e.ProcessId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProcessStepAction
        modelBuilder.Entity<ProcessStepAction>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.FromStep)
                .WithMany()
                .HasForeignKey(e => e.FromStepId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ToStep)
                .WithMany()
                .HasForeignKey(e => e.ToStepId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Process)
                .WithMany(m => m.ProcessStepActions)
                .HasForeignKey(e => e.ProcessId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProcessStepTransition
        //modelBuilder.Entity<ProcessStepTransition>(entity =>
        //{
        //    entity.HasQueryFilter(e => !e.IsDeleted);

        //    entity.HasOne(e => e.FromStep)
        //        .WithMany(ps => ps.TransitionsFrom)
        //        .HasForeignKey(e => e.FromStepId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    entity.HasOne(e => e.ToStep)
        //        .WithMany(ps => ps.TransitionsTo)
        //        .HasForeignKey(e => e.ToStepId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    //entity.HasOne(e => e.ProcessStepAction)
        //    //    .WithMany(psa => psa.Transitions)
        //    //    .HasForeignKey(e => e.ProcessStepActionId)
        //    //    .OnDelete(DeleteBehavior.Restrict);
        //});

        // ProcessInstance
        modelBuilder.Entity<ProcessInstance>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.Process)
                .WithMany(p => p.ProcessInstances)
                .HasForeignKey(e => e.ProcessId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProcessInstanceStep
        modelBuilder.Entity<ProcessInstanceStep>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.ProcessInstance)
                .WithMany(pi => pi.ProcessInstanceSteps)
                .HasForeignKey(e => e.ProcessInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ProcessStep)
                .WithMany()
                .HasForeignKey(e => e.ProcessStepId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProcessInstanceHistory
        modelBuilder.Entity<ProcessInstanceHistory>(entity =>
        {
            entity.HasOne(e => e.ProcessInstance)
                .WithMany(pi => pi.ProcessInstanceHistories)
                .HasForeignKey(e => e.ProcessInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ProcessStep)
                .WithMany()
                .HasForeignKey(e => e.ProcessStepId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureProcessInstanceData(ModelBuilder modelBuilder)
    {
        // TPT Strategy - Base table
        modelBuilder.Entity<ProcessInstanceDataBase>()
            .ToTable("ProcessInstanceData");


       modelBuilder.Entity<ProcessInstanceDataBase>()
                .HasOne(d => d.ProcessInstance)
                .WithOne(pi => pi.Data)
                .HasForeignKey<ProcessInstanceDataBase>(d => d.ProcessInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

        // TPT - Each derived type maps to its own table
        modelBuilder.Entity<LeaveRequestData>().ToTable("LeaveRequestData");
        modelBuilder.Entity<PurchaseRequestData>().ToTable("PurchaseRequestData");
       


    }
}

