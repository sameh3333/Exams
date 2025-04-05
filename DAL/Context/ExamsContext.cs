using System;
using System.Collections.Generic;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Domin;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Exams.Models;
using Microsoft.AspNetCore.Identity;
namespace DAL.Context;

public partial class ExamsContext : IdentityDbContext<ApplicationUser>
{
    public ExamsContext()
    {
    }
    public ExamsContext(DbContextOptions<ExamsContext> options) : base(options) { }

    

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<TbChoice> TbChoices { get; set; }

    public virtual DbSet<TbExam> TbExams { get; set; }

    public virtual DbSet<TbQuestion> TbQuestions { get; set; }

    public virtual DbSet<TbResult> TbResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    { 
        if (!optionsBuilder.IsConfigured)
        {

        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        // ✅ حل مشكلة عدم وجود مفتاح رئيسي في Identity
        modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
        modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.UserId, r.RoleId });
        modelBuilder.Entity<IdentityUserToken<string>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });





        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Log");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });



        modelBuilder.Entity<TbChoice>(entity =>
        {
            entity.ToTable("TbChoice");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ChoiceText)
                .HasMaxLength(300);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Question)
                .WithMany(q => q.TbChoices) // تأكد أن العلاقة مع TbQuestion مضبوطة
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<TbExam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TbExam__3214EC078A31B919");

            entity.ToTable("TbExam");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentState).HasDefaultValue(1);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TbQuesti__3214EC07870B536D");

            entity.ToTable("TbQuestion");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentState).HasDefaultValue(1);
            entity.Property(e => e.QuestionText).HasMaxLength(500);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Exam).WithMany(p => p.TbQuestions)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("FK_TbQuestion_TbExam");
        });

        modelBuilder.Entity<TbResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TbResult__3214EC0788447D4F");

            entity.ToTable("TbResult");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentState).HasDefaultValue(1);
            entity.Property(e => e.StudentName).HasMaxLength(200);
            entity.Property(e => e.TakenDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Exam).WithMany(p => p.TbResults)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("FK_TbResult_TbExam");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
