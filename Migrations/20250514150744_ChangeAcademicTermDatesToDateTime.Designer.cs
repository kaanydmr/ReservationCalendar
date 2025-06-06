﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Project.Data;

#nullable disable

namespace Project.Migrations
{
    [DbContext(typeof(ReservationDbContext))]
    [Migration("20250514150744_ChangeAcademicTermDatesToDateTime")]
    partial class ChangeAcademicTermDatesToDateTime
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Project.Models.AcademicTerm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("Id")
                        .HasName("PK__Academic__3214EC07D7F482A1");

                    b.HasIndex("CreatedById");

                    b.ToTable("AcademicTerms");
                });

            modelBuilder.Entity("Project.Models.Classroom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Building")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("Floor")
                        .HasColumnType("int");

                    b.Property<bool>("HasComputers")
                        .HasColumnType("bit");

                    b.Property<bool>("HasProjector")
                        .HasColumnType("bit");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("Id")
                        .HasName("PK__Classroo__3214EC07F6AADBE3");

                    b.ToTable("Classrooms");
                });

            modelBuilder.Entity("Project.Models.ClassroomFeedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ClassroomId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("InstructorId")
                        .HasColumnType("int");

                    b.Property<byte>("Rating")
                        .HasColumnType("tinyint");

                    b.HasKey("Id")
                        .HasName("PK__Classroo__3214EC0734D017D3");

                    b.HasIndex("ClassroomId");

                    b.HasIndex("InstructorId");

                    b.ToTable("ClassroomFeedback", (string)null);
                });

            modelBuilder.Entity("Project.Models.ClassroomReservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AcademicTermId")
                        .HasColumnType("int");

                    b.Property<int?>("ApprovedRejectedById")
                        .HasColumnType("int");

                    b.Property<int>("ClassroomId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<byte>("DayOfWeek")
                        .HasColumnType("tinyint");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time");

                    b.Property<int>("InstructorId")
                        .HasColumnType("int");

                    b.Property<bool>("IsHoliday")
                        .HasColumnType("bit");

                    b.Property<string>("RejectionReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("Id")
                        .HasName("PK__Classroo__3214EC07F30A4887");

                    b.HasIndex("AcademicTermId");

                    b.HasIndex("ApprovedRejectedById");

                    b.HasIndex("ClassroomId");

                    b.HasIndex("InstructorId");

                    b.ToTable("ClassroomReservations");
                });

            modelBuilder.Entity("Project.Models.ContactMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__ContactM__3214EC07703ABA20");

                    b.HasIndex("UserId");

                    b.ToTable("ContactMessages");
                });

            modelBuilder.Entity("Project.Models.Holiday", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id")
                        .HasName("PK__Holidays__3214EC07D46BC282");

                    b.HasIndex(new[] { "Date" }, "UQ__Holidays__77387D072F52EC92")
                        .IsUnique();

                    b.ToTable("Holidays");
                });

            modelBuilder.Entity("Project.Models.SystemLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ActionType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsError")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__SystemLo__3214EC077948A9F8");

                    b.HasIndex("UserId");

                    b.ToTable("SystemLogs");
                });

            modelBuilder.Entity("Project.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("Id")
                        .HasName("PK__Users__3214EC07032C5894");

                    b.HasIndex(new[] { "Email" }, "UQ__Users__A9D105343CC79A8B")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Project.Models.AcademicTerm", b =>
                {
                    b.HasOne("Project.Models.User", "CreatedBy")
                        .WithMany("AcademicTerms")
                        .HasForeignKey("CreatedById")
                        .IsRequired()
                        .HasConstraintName("FK__AcademicT__Creat__403A8C7D");

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("Project.Models.ClassroomFeedback", b =>
                {
                    b.HasOne("Project.Models.Classroom", "Classroom")
                        .WithMany("ClassroomFeedbacks")
                        .HasForeignKey("ClassroomId")
                        .IsRequired()
                        .HasConstraintName("FK__Classroom__Class__534D60F1");

                    b.HasOne("Project.Models.User", "Instructor")
                        .WithMany("ClassroomFeedbacks")
                        .HasForeignKey("InstructorId")
                        .IsRequired()
                        .HasConstraintName("FK__Classroom__Instr__5441852A");

                    b.Navigation("Classroom");

                    b.Navigation("Instructor");
                });

            modelBuilder.Entity("Project.Models.ClassroomReservation", b =>
                {
                    b.HasOne("Project.Models.AcademicTerm", "AcademicTerm")
                        .WithMany("ClassroomReservations")
                        .HasForeignKey("AcademicTermId")
                        .IsRequired()
                        .HasConstraintName("FK__Classroom__Acade__4E88ABD4");

                    b.HasOne("Project.Models.User", "ApprovedRejectedBy")
                        .WithMany("ClassroomReservationApprovedRejectedBies")
                        .HasForeignKey("ApprovedRejectedById")
                        .HasConstraintName("FK__Classroom__Appro__4F7CD00D");

                    b.HasOne("Project.Models.Classroom", "Classroom")
                        .WithMany("ClassroomReservations")
                        .HasForeignKey("ClassroomId")
                        .IsRequired()
                        .HasConstraintName("FK__Classroom__Class__4CA06362");

                    b.HasOne("Project.Models.User", "Instructor")
                        .WithMany("ClassroomReservationInstructors")
                        .HasForeignKey("InstructorId")
                        .IsRequired()
                        .HasConstraintName("FK__Classroom__Instr__4D94879B");

                    b.Navigation("AcademicTerm");

                    b.Navigation("ApprovedRejectedBy");

                    b.Navigation("Classroom");

                    b.Navigation("Instructor");
                });

            modelBuilder.Entity("Project.Models.ContactMessage", b =>
                {
                    b.HasOne("Project.Models.User", "User")
                        .WithMany("ContactMessages")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK__ContactMe__UserI__59063A47");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project.Models.SystemLog", b =>
                {
                    b.HasOne("Project.Models.User", "User")
                        .WithMany("SystemLogs")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__SystemLog__UserI__5DCAEF64");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project.Models.AcademicTerm", b =>
                {
                    b.Navigation("ClassroomReservations");
                });

            modelBuilder.Entity("Project.Models.Classroom", b =>
                {
                    b.Navigation("ClassroomFeedbacks");

                    b.Navigation("ClassroomReservations");
                });

            modelBuilder.Entity("Project.Models.User", b =>
                {
                    b.Navigation("AcademicTerms");

                    b.Navigation("ClassroomFeedbacks");

                    b.Navigation("ClassroomReservationApprovedRejectedBies");

                    b.Navigation("ClassroomReservationInstructors");

                    b.Navigation("ContactMessages");

                    b.Navigation("SystemLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
