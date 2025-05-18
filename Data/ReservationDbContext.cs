using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data;

public partial class ReservationDbContext : DbContext
{
    public ReservationDbContext()
    {
    }

    public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcademicTerm> AcademicTerms { get; set; }

    public virtual DbSet<Classroom> Classrooms { get; set; }

    public virtual DbSet<ClassroomFeedback> ClassroomFeedbacks { get; set; }

    public virtual DbSet<ClassroomReservation> ClassroomReservations { get; set; }

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<SystemLog> SystemLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=ReservationDB;User Id=SA;Password=MyStrongPass123;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicTerm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Academic__3214EC07D7F482A1");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.AcademicTerms)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AcademicT__Creat__403A8C7D");
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Classroo__3214EC07F6AADBE3");

            entity.Property(e => e.Building).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<ClassroomFeedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Classroo__3214EC0734D017D3");

            entity.ToTable("ClassroomFeedback");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Classroom).WithMany(p => p.ClassroomFeedbacks)
                .HasForeignKey(d => d.ClassroomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classroom__Class__534D60F1");

            entity.HasOne(d => d.Instructor).WithMany(p => p.ClassroomFeedbacks)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classroom__Instr__5441852A");
        });

        modelBuilder.Entity<ClassroomReservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Classroo__3214EC07F30A4887");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.AcademicTerm).WithMany(p => p.ClassroomReservations)
                .HasForeignKey(d => d.AcademicTermId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classroom__Acade__4E88ABD4");

            entity.HasOne(d => d.ApprovedRejectedBy).WithMany(p => p.ClassroomReservationApprovedRejectedBies)
                .HasForeignKey(d => d.ApprovedRejectedById)
                .HasConstraintName("FK__Classroom__Appro__4F7CD00D");

            entity.HasOne(d => d.Classroom).WithMany(p => p.ClassroomReservations)
                .HasForeignKey(d => d.ClassroomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classroom__Class__4CA06362");

            entity.HasOne(d => d.Instructor).WithMany(p => p.ClassroomReservationInstructors)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classroom__Instr__4D94879B");
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContactM__3214EC07703ABA20");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Subject).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.ContactMessages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContactMe__UserI__59063A47");
        });

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Holidays__3214EC07D46BC282");

            entity.HasIndex(e => e.Date, "UQ__Holidays__77387D072F52EC92").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SystemLo__3214EC077948A9F8");

            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.SystemLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__SystemLog__UserI__5DCAEF64");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07032C5894");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105343CC79A8B").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
