using Microsoft.EntityFrameworkCore;

#nullable disable

namespace NationalReserve.Model
{
    public partial class National_ReserveContext : DbContext
    {
        public National_ReserveContext()
        {
        }

        public National_ReserveContext(DbContextOptions<National_ReserveContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Animal>(entity =>
            {
                entity.HasKey(e => e.IdAnimal)
                    .HasName("PK__Animal__5251620255E24BD1");

                entity.ToTable("Animal");

                entity.Property(e => e.IdAnimal).HasColumnName("ID_Animal");

                entity.Property(e => e.DateRegistration)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_Registration");

                entity.Property(e => e.HasFamily).HasColumnName("Has_Family");

                entity.Property(e => e.IdType).HasColumnName("ID_Type");

                entity.Property(e => e.IdZone).HasColumnName("ID_Zone");

                entity.Property(e => e.IsSick).HasColumnName("Is_Sick");

                entity.Property(e => e.LastCheck)
                    .HasColumnType("datetime")
                    .HasColumnName("Last_Check");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

            });

            modelBuilder.Entity<AnimalFeed>(entity =>
            {
                entity.HasKey(e => e.IdFeed)
                    .HasName("PK__Animal_F__644963B88EA3B86E");

                entity.ToTable("Animal_Feed");

                entity.Property(e => e.IdFeed).HasColumnName("ID_Feed");

                entity.Property(e => e.IdAnimal).HasColumnName("ID_Animal");

                entity.Property(e => e.IdSupply).HasColumnName("ID_Supply");

            });

            modelBuilder.Entity<AnimalType>(entity =>
            {
                entity.ToTable("Animal_Type");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Checkpoint>(entity =>
            {
                entity.HasKey(e => e.IdCheckpoint)
                    .HasName("PK__Checkpoi__7FE294726F81E652");

                entity.ToTable("Checkpoint");

                entity.Property(e => e.IdCheckpoint).HasColumnName("ID_Checkpoint");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CheckpointPass>(entity =>
            {
                entity.HasKey(e => e.IdCheckpointPass)
                    .HasName("PK__Checkpoi__46AFC884D03304FC");

                entity.ToTable("Checkpoint_Pass");

                entity.Property(e => e.IdCheckpointPass).HasColumnName("ID_Checkpoint_Pass");

                entity.Property(e => e.IdCheckpoint).HasColumnName("ID_Checkpoint");

                entity.Property(e => e.IdHuman).HasColumnName("ID_Human");

                entity.Property(e => e.PassTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Pass_Time");

                entity.Property(e => e.PassType)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("Pass_Type");
            });

            modelBuilder.Entity<Human>(entity =>
            {
                entity.HasKey(e => e.IdHuman)
                    .HasName("PK__Human__AF0F0AC79D84243F");

                entity.ToTable("Human");

                entity.Property(e => e.IdHuman).HasColumnName("ID_Human");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.IdRole).HasColumnName("ID_Role");

                entity.Property(e => e.IsStaff).HasColumnName("Is_Staff");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.HasKey(e => e.IdMaterial)
                    .HasName("PK__Material__A7F521BBBD476FE7");

                entity.ToTable("Material");

                entity.Property(e => e.IdMaterial).HasColumnName("ID_Material");

                entity.Property(e => e.CostPerOne)
                    .HasColumnType("decimal(7, 2)")
                    .HasColumnName("Cost_Per_one");

                entity.Property(e => e.IdType).HasColumnName("ID_Type");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MaterialType>(entity =>
            {
                entity.ToTable("Material_Type");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PaymentType>(entity =>
            {
                entity.ToTable("Payment_Type");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PlantList>(entity =>
            {
                entity.HasKey(e => e.IdPlant)
                    .HasName("PK__Plant_Li__31C4A3B6BC8E5A74");

                entity.ToTable("Plant_List");

                entity.Property(e => e.IdPlant).HasColumnName("ID_Plant");

                entity.Property(e => e.DateGarden)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_Garden")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DaysToCheck).HasColumnName("Days_To_Check");

                entity.Property(e => e.IdHuman).HasColumnName("ID_Human");

                entity.Property(e => e.IdSupply).HasColumnName("ID_Supply");

                entity.Property(e => e.IdZone).HasColumnName("ID_Zone");

                entity.Property(e => e.LastCheck)
                    .HasColumnType("datetime")
                    .HasColumnName("Last_Check")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SecurityList>(entity =>
            {
                entity.HasKey(e => e.IdSecurity)
                    .HasName("PK__Security__3DA8107DEF10A5DF");

                entity.ToTable("Security_List");

                entity.Property(e => e.IdSecurity).HasColumnName("ID_Security");

                entity.Property(e => e.IdCheckpoint).HasColumnName("ID_Checkpoint");

                entity.Property(e => e.IdHuman).HasColumnName("ID_Human");

                entity.Property(e => e.TimeEnd)
                    .HasColumnType("datetime")
                    .HasColumnName("Time_End")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TimeStart)
                    .HasColumnType("datetime")
                    .HasColumnName("Time_Start")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Sponsorship>(entity =>
            {
                entity.HasKey(e => e.IdPayment)
                    .HasName("PK__Sponsors__C2118ADE016C2C3E");

                entity.ToTable("Sponsorship");

                entity.Property(e => e.IdPayment).HasColumnName("ID_Payment");

                entity.Property(e => e.Amount).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.IdHuman).HasColumnName("ID_Human");

                entity.Property(e => e.IdType).HasColumnName("ID_Type");

                entity.Property(e => e.PaymentDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Payment_Date")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<StaffDocument>(entity =>
            {
                entity.ToTable("Staff_Documents");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.NumberPass).HasColumnName("Number_Pass");

                entity.Property(e => e.SerialPass).HasColumnName("Serial_Pass");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.IdSupplier)
                    .HasName("PK__Supplier__408B7094368EC8A9");

                entity.ToTable("Supplier");

                entity.Property(e => e.IdSupplier).HasColumnName("ID_Supplier");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Supply>(entity =>
            {
                entity.HasKey(e => e.IdSupply)
                    .HasName("PK__Supply__84956EB8BDF05906");

                entity.ToTable("Supply");

                entity.Property(e => e.IdSupply).HasColumnName("ID_Supply");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdMaterial).HasColumnName("ID_Material");

                entity.Property(e => e.IdSupplier).HasColumnName("ID_Supplier");
            });

            modelBuilder.Entity<Zone>(entity =>
            {
                entity.HasKey(e => e.IdZone)
                    .HasName("PK__Zone__8134933FEC910B68");

                entity.ToTable("Zone");

                entity.Property(e => e.IdZone).HasColumnName("ID_Zone");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.IdCheckpoint).HasColumnName("ID_Checkpoint");

                entity.Property(e => e.IsForStaff).HasColumnName("Is_For_Staff");

                entity.Property(e => e.IsForWatch).HasColumnName("Is_For_Watch");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Square).HasColumnType("decimal(6, 2)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
