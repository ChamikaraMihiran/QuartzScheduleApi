﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace QuartzScheduleApi.Model
{
    public partial class QuartzDBContext : DbContext
    {
        public QuartzDBContext()
        {
        }

        public QuartzDBContext(DbContextOptions<QuartzDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<QrtzBlobTriggers> QrtzBlobTriggers { get; set; }
        public virtual DbSet<QrtzCalendars> QrtzCalendars { get; set; }
        public virtual DbSet<QrtzCronTriggers> QrtzCronTriggers { get; set; }
        public virtual DbSet<QrtzExecutions> QrtzExecutions { get; set; }
        public virtual DbSet<QrtzFiredTriggers> QrtzFiredTriggers { get; set; }
        public virtual DbSet<QrtzJobDetails> QrtzJobDetails { get; set; }
        public virtual DbSet<QrtzJobSettings> QrtzJobSettings { get; set; }
        public virtual DbSet<QrtzLocks> QrtzLocks { get; set; }
        public virtual DbSet<QrtzLogRecords> QrtzLogRecords { get; set; }
        public virtual DbSet<QrtzPausedTriggerGrps> QrtzPausedTriggerGrps { get; set; }
        public virtual DbSet<QrtzSchedulerState> QrtzSchedulerState { get; set; }
        public virtual DbSet<QrtzSimpleTriggers> QrtzSimpleTriggers { get; set; }
        public virtual DbSet<QrtzSimpropTriggers> QrtzSimpropTriggers { get; set; }
        public virtual DbSet<QrtzTriggers> QrtzTriggers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=UNICORNCMI;Database=QuartzDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QrtzBlobTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.ToTable("QRTZ_BLOB_TRIGGERS");

                entity.Property(e => e.SchedName)
                    .HasColumnName("SCHED_NAME")
                    .HasMaxLength(120);

                entity.Property(e => e.TriggerName)
                    .HasColumnName("TRIGGER_NAME")
                    .HasMaxLength(150);

                entity.Property(e => e.TriggerGroup)
                    .HasColumnName("TRIGGER_GROUP")
                    .HasMaxLength(150);

                entity.Property(e => e.BlobData).HasColumnName("BLOB_DATA");
            });

            modelBuilder.Entity<QrtzCalendars>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.CalendarName });

                entity.ToTable("QRTZ_CALENDARS");

                entity.Property(e => e.SchedName)
                    .HasColumnName("SCHED_NAME")
                    .HasMaxLength(120);

                entity.Property(e => e.CalendarName)
                    .HasColumnName("CALENDAR_NAME")
                    .HasMaxLength(200);

                entity.Property(e => e.Calendar)
                    .IsRequired()
                    .HasColumnName("CALENDAR");
            });

            modelBuilder.Entity<QrtzCronTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.ToTable("QRTZ_CRON_TRIGGERS");

                entity.HasIndex(e => e.CronId)
                    .HasName("UNIQUE_QRTZ_CRON_TRIGGERS")
                    .IsUnique();

                entity.Property(e => e.SchedName)
                    .HasColumnName("SCHED_NAME")
                    .HasMaxLength(120);

                entity.Property(e => e.TriggerName)
                    .HasColumnName("TRIGGER_NAME")
                    .HasMaxLength(150);

                entity.Property(e => e.TriggerGroup)
                    .HasColumnName("TRIGGER_GROUP")
                    .HasMaxLength(150);

                entity.Property(e => e.CronExpression)
                    .IsRequired()
                    .HasColumnName("CRON_EXPRESSION")
                    .HasMaxLength(120);

                entity.Property(e => e.CronId)
                    .HasColumnName("CRON_ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TimeZoneId)
                    .HasColumnName("TIME_ZONE_ID")
                    .HasMaxLength(80);

                entity.HasOne(d => d.QrtzTriggers)
                    .WithOne(p => p.QrtzCronTriggers)
                    .HasForeignKey<QrtzCronTriggers>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                    .HasConstraintName("FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QrtzExecutions>(entity =>
            {
                entity.HasKey(e => e.ExecutionId);

                entity.ToTable("QRTZ_EXECUTIONS");

                entity.Property(e => e.ExecutionId).HasColumnName("EXECUTION_ID");

                entity.Property(e => e.EndTime).HasColumnName("END_TIME");

                entity.Property(e => e.JobId).HasColumnName("JOB_ID");

                entity.Property(e => e.LogId).HasColumnName("LOG_ID");

                entity.Property(e => e.NextFireTime).HasColumnName("NEXT_FIRE_TIME");

                entity.Property(e => e.PrevFireTime).HasColumnName("PREV_FIRE_TIME");

                entity.Property(e => e.StartTime).HasColumnName("START_TIME");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasColumnName("STATE")
                    .HasMaxLength(16);

                entity.Property(e => e.TriggerId).HasColumnName("TRIGGER_ID");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.QrtzExecutions)
                    .HasPrincipalKey(p => p.JobId)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QRTZ_EXECUTIONS_QRTZ_JOB_DETAILS");

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.QrtzExecutions)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_QRTZ_EXECUTIONS_QRTZ_LOG_RECORDS");

                entity.HasOne(d => d.Trigger)
                    .WithMany(p => p.QrtzExecutions)
                    .HasPrincipalKey(p => p.TriggerId)
                    .HasForeignKey(d => d.TriggerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QRTZ_EXECUTIONS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QrtzFiredTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.EntryId });

                entity.ToTable("QRTZ_FIRED_TRIGGERS");

                entity.HasIndex(e => new { e.SchedName, e.InstanceName })
                    .HasName("IDX_QRTZ_FT_TRIG_INST_NAME");

                entity.HasIndex(e => new { e.SchedName, e.JobGroup })
                    .HasName("IDX_QRTZ_FT_JG");

                entity.HasIndex(e => new { e.SchedName, e.TriggerGroup })
                    .HasName("IDX_QRTZ_FT_TG");

                entity.HasIndex(e => new { e.SchedName, e.InstanceName, e.RequestsRecovery })
                    .HasName("IDX_QRTZ_FT_INST_JOB_REQ_RCVRY");

                entity.HasIndex(e => new { e.SchedName, e.JobName, e.JobGroup })
                    .HasName("IDX_QRTZ_FT_J_G");

                entity.HasIndex(e => new { e.SchedName, e.TriggerName, e.TriggerGroup })
                    .HasName("IDX_QRTZ_FT_T_G");

                entity.Property(e => e.SchedName)
                    .HasColumnName("SCHED_NAME")
                    .HasMaxLength(120);

                entity.Property(e => e.EntryId)
                    .HasColumnName("ENTRY_ID")
                    .HasMaxLength(140);

                entity.Property(e => e.FiredTime).HasColumnName("FIRED_TIME");

                entity.Property(e => e.InstanceName)
                    .IsRequired()
                    .HasColumnName("INSTANCE_NAME")
                    .HasMaxLength(200);

                entity.Property(e => e.IsNonconcurrent).HasColumnName("IS_NONCONCURRENT");

                entity.Property(e => e.JobGroup)
                    .HasColumnName("JOB_GROUP")
                    .HasMaxLength(150);

                entity.Property(e => e.JobName)
                    .HasColumnName("JOB_NAME")
                    .HasMaxLength(150);

                entity.Property(e => e.Priority).HasColumnName("PRIORITY");

                entity.Property(e => e.RequestsRecovery).HasColumnName("REQUESTS_RECOVERY");

                entity.Property(e => e.SchedTime).HasColumnName("SCHED_TIME");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasColumnName("STATE")
                    .HasMaxLength(16);

                entity.Property(e => e.TriggerGroup)
                    .IsRequired()
                    .HasColumnName("TRIGGER_GROUP")
                    .HasMaxLength(150);

                entity.Property(e => e.TriggerName)
                    .IsRequired()
                    .HasColumnName("TRIGGER_NAME")
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<QrtzJobDetails>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.JobName, e.JobGroup });

                entity.ToTable("QRTZ_JOB_DETAILS");

                entity.HasIndex(e => e.JobId)
                    .HasName("UNIQUE_QRTZ_JOB_DETAILS")
                    .IsUnique();

                entity.Property(e => e.SchedName)
                    .HasColumnName("SCHED_NAME")
                    .HasMaxLength(120);

                entity.Property(e => e.JobName)
                    .HasColumnName("JOB_NAME")
                    .HasMaxLength(150);

                entity.Property(e => e.JobGroup)
                    .HasColumnName("JOB_GROUP")
                    .HasMaxLength(150);

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(250);

                entity.Property(e => e.IsDurable).HasColumnName("IS_DURABLE");

                entity.Property(e => e.IsNonconcurrent).HasColumnName("IS_NONCONCURRENT");

                entity.Property(e => e.IsUpdateData).HasColumnName("IS_UPDATE_DATA");

                entity.Property(e => e.JobClassName)
                    .IsRequired()
                    .HasColumnName("JOB_CLASS_NAME")
                    .HasMaxLength(250);

                entity.Property(e => e.JobData).HasColumnName("JOB_DATA");

                entity.Property(e => e.JobId)
                    .HasColumnName("JOB_ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.RequestsRecovery).HasColumnName("REQUESTS_RECOVERY");

                entity.Property(e => e.TriggerId).HasColumnName("TRIGGER_ID");

                entity.HasOne(d => d.Trigger)
                    .WithMany(p => p.QrtzJobDetails)
                    .HasPrincipalKey(p => p.TriggerId)
                    .HasForeignKey(d => d.TriggerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_QRTZ_JOB_DETAILS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QrtzJobSettings>(entity =>
            {
                entity.HasKey(e => e.SettingId);

                entity.ToTable("QRTZ_JOB_SETTINGS");

                entity.Property(e => e.SettingId).HasColumnName("SETTING_ID");

                entity.Property(e => e.JobId).HasColumnName("JOB_ID");

                entity.Property(e => e.Key)
                    .HasColumnName("KEY")
                    .HasMaxLength(200);

                entity.Property(e => e.Path)
                    .HasColumnName("PATH")
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .HasColumnName("VALUE")
                    .HasMaxLength(250);

                entity.Property(e => e.ValueType)
                    .HasColumnName("VALUE_TYPE")
                    .HasMaxLength(150);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.QrtzJobSettings)
                    .HasPrincipalKey(p => p.JobId)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_QRTZ_JOB_SETTINGS_QRTZ_JOB_DETAILS");
            });

            modelBuilder.Entity<QrtzLocks>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.LockName });

                entity.ToTable("QRTZ_LOCKS");

                entity.Property(e => e.SchedName)
                    .HasColumnName("SCHED_NAME")
                    .HasMaxLength(120);

                entity.Property(e => e.LockName)
                    .HasColumnName("LOCK_NAME")
                    .HasMaxLength(40);
            });

            modelBuilder.Entity<QrtzLogRecords>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("QRTZ_LOG_RECORDS");

                entity.Property(e => e.LogId).HasColumnName("LOG_ID");

                entity.Property(e => e.ExecutionTime).HasColumnName("EXECUTION_TIME");

                entity.Property(e => e.Location)
                    .HasColumnName("LOCATION")
                    .HasMaxLength(250);

                entity.Property(e => e.Message)
                    .HasColumnName("MESSAGE")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<QrtzPausedTriggerGrps>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerGroup });

                entity.ToTable("QRTZ_PAUSED_TRIGGER_GRPS");

                entity.Property(e => e.SchedName)
                    .HasColumnName("SCHED_NAME")
                    .HasMaxLength(120);

                entity.Property(e => e.TriggerGroup)
                    .HasColumnName("TRIGGER_GROUP")
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<QrtzSchedulerState>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.InstanceName });

                entity.ToTable("QRTZ_SCHEDULER_STATE");

                entity.Property(e => e.SchedName)
                    .HasColumnName("SCHED_NAME")
                    .HasMaxLength(120);

                entity.Property(e => e.InstanceName)
                    .HasColumnName("INSTANCE_NAME")
                    .HasMaxLength(200);

                entity.Property(e => e.CheckinInterval).HasColumnName("CHECKIN_INTERVAL");

                entity.Property(e => e.LastCheckinTime).HasColumnName("LAST_CHECKIN_TIME");
            });

            modelBuilder.Entity<QrtzSimpleTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.ToTable("QRTZ_SIMPLE_TRIGGERS");

                entity.Property(e => e.SchedName)
                    .HasColumnName("SCHED_NAME")
                    .HasMaxLength(120);

                entity.Property(e => e.TriggerName)
                    .HasColumnName("TRIGGER_NAME")
                    .HasMaxLength(150);

                entity.Property(e => e.TriggerGroup)
                    .HasColumnName("TRIGGER_GROUP")
                    .HasMaxLength(150);

                entity.Property(e => e.RepeatCount).HasColumnName("REPEAT_COUNT");

                entity.Property(e => e.RepeatInterval).HasColumnName("REPEAT_INTERVAL");

                entity.Property(e => e.TimesTriggered).HasColumnName("TIMES_TRIGGERED");

                entity.HasOne(d => d.QrtzTriggers)
                    .WithOne(p => p.QrtzSimpleTriggers)
                    .HasForeignKey<QrtzSimpleTriggers>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                    .HasConstraintName("FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QrtzSimpropTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.ToTable("QRTZ_SIMPROP_TRIGGERS");

                entity.Property(e => e.SchedName)
                    .HasColumnName("SCHED_NAME")
                    .HasMaxLength(120);

                entity.Property(e => e.TriggerName)
                    .HasColumnName("TRIGGER_NAME")
                    .HasMaxLength(150);

                entity.Property(e => e.TriggerGroup)
                    .HasColumnName("TRIGGER_GROUP")
                    .HasMaxLength(150);

                entity.Property(e => e.BoolProp1).HasColumnName("BOOL_PROP_1");

                entity.Property(e => e.BoolProp2).HasColumnName("BOOL_PROP_2");

                entity.Property(e => e.DecProp1)
                    .HasColumnName("DEC_PROP_1")
                    .HasColumnType("numeric(13, 4)");

                entity.Property(e => e.DecProp2)
                    .HasColumnName("DEC_PROP_2")
                    .HasColumnType("numeric(13, 4)");

                entity.Property(e => e.IntProp1).HasColumnName("INT_PROP_1");

                entity.Property(e => e.IntProp2).HasColumnName("INT_PROP_2");

                entity.Property(e => e.LongProp1).HasColumnName("LONG_PROP_1");

                entity.Property(e => e.LongProp2).HasColumnName("LONG_PROP_2");

                entity.Property(e => e.StrProp1)
                    .HasColumnName("STR_PROP_1")
                    .HasMaxLength(512);

                entity.Property(e => e.StrProp2)
                    .HasColumnName("STR_PROP_2")
                    .HasMaxLength(512);

                entity.Property(e => e.StrProp3)
                    .HasColumnName("STR_PROP_3")
                    .HasMaxLength(512);

                entity.Property(e => e.TimeZoneId)
                    .HasColumnName("TIME_ZONE_ID")
                    .HasMaxLength(80);

                entity.HasOne(d => d.QrtzTriggers)
                    .WithOne(p => p.QrtzSimpropTriggers)
                    .HasForeignKey<QrtzSimpropTriggers>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                    .HasConstraintName("FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QrtzTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.ToTable("QRTZ_TRIGGERS");

                entity.HasIndex(e => e.TriggerId)
                    .HasName("UNIQUE_QRTZ_TRIGGERS")
                    .IsUnique();

                entity.HasIndex(e => new { e.SchedName, e.CalendarName })
                    .HasName("IDX_QRTZ_T_C");

                entity.HasIndex(e => new { e.SchedName, e.JobGroup })
                    .HasName("IDX_QRTZ_T_JG");

                entity.HasIndex(e => new { e.SchedName, e.NextFireTime })
                    .HasName("IDX_QRTZ_T_NEXT_FIRE_TIME");

                entity.HasIndex(e => new { e.SchedName, e.TriggerGroup })
                    .HasName("IDX_QRTZ_T_G");

                entity.HasIndex(e => new { e.SchedName, e.TriggerState })
                    .HasName("IDX_QRTZ_T_STATE");

                entity.HasIndex(e => new { e.SchedName, e.JobName, e.JobGroup })
                    .HasName("IDX_QRTZ_T_J");

                entity.HasIndex(e => new { e.SchedName, e.MisfireInstr, e.NextFireTime })
                    .HasName("IDX_QRTZ_T_NFT_MISFIRE");

                entity.HasIndex(e => new { e.SchedName, e.TriggerGroup, e.TriggerState })
                    .HasName("IDX_QRTZ_T_N_G_STATE");

                entity.HasIndex(e => new { e.SchedName, e.TriggerState, e.NextFireTime })
                    .HasName("IDX_QRTZ_T_NFT_ST");

                entity.HasIndex(e => new { e.SchedName, e.MisfireInstr, e.NextFireTime, e.TriggerState })
                    .HasName("IDX_QRTZ_T_NFT_ST_MISFIRE");

                entity.HasIndex(e => new { e.SchedName, e.TriggerName, e.TriggerGroup, e.TriggerState })
                    .HasName("IDX_QRTZ_T_N_STATE");

                entity.HasIndex(e => new { e.SchedName, e.MisfireInstr, e.NextFireTime, e.TriggerGroup, e.TriggerState })
                    .HasName("IDX_QRTZ_T_NFT_ST_MISFIRE_GRP");

                entity.Property(e => e.SchedName)
                    .HasColumnName("SCHED_NAME")
                    .HasMaxLength(120);

                entity.Property(e => e.TriggerName)
                    .HasColumnName("TRIGGER_NAME")
                    .HasMaxLength(150);

                entity.Property(e => e.TriggerGroup)
                    .HasColumnName("TRIGGER_GROUP")
                    .HasMaxLength(150);

                entity.Property(e => e.CalendarName)
                    .HasColumnName("CALENDAR_NAME")
                    .HasMaxLength(200);

                entity.Property(e => e.CronId).HasColumnName("CRON_ID");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(250);

                entity.Property(e => e.EndTime).HasColumnName("END_TIME");

                entity.Property(e => e.JobData).HasColumnName("JOB_DATA");

                entity.Property(e => e.JobGroup)
                    .IsRequired()
                    .HasColumnName("JOB_GROUP")
                    .HasMaxLength(150);

                entity.Property(e => e.JobName)
                    .IsRequired()
                    .HasColumnName("JOB_NAME")
                    .HasMaxLength(150);

                entity.Property(e => e.MisfireInstr).HasColumnName("MISFIRE_INSTR");

                entity.Property(e => e.NextFireTime).HasColumnName("NEXT_FIRE_TIME");

                entity.Property(e => e.PrevFireTime).HasColumnName("PREV_FIRE_TIME");

                entity.Property(e => e.Priority).HasColumnName("PRIORITY");

                entity.Property(e => e.StartTime).HasColumnName("START_TIME");

                entity.Property(e => e.TriggerId)
                    .HasColumnName("TRIGGER_ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TriggerState)
                    .IsRequired()
                    .HasColumnName("TRIGGER_STATE")
                    .HasMaxLength(16);

                entity.Property(e => e.TriggerType)
                    .IsRequired()
                    .HasColumnName("TRIGGER_TYPE")
                    .HasMaxLength(8);

                entity.HasOne(d => d.Cron)
                    .WithMany(p => p.QrtzTriggersNavigation)
                    .HasPrincipalKey(p => p.CronId)
                    .HasForeignKey(d => d.CronId)
                    .HasConstraintName("FK_QRTZ_TRIGGERS_QRTZ_CRON_TRIGGERS");

                entity.HasOne(d => d.QrtzJobDetailsNavigation)
                    .WithMany(p => p.QrtzTriggers)
                    .HasForeignKey(d => new { d.SchedName, d.JobName, d.JobGroup })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS");
            });
        }
    }
}
