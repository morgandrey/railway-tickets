using Microsoft.EntityFrameworkCore;
using RailwayWebApp.Models;

namespace RailwayWebApp.Data
{
    public sealed class RailwaysDBContext : DbContext
    {
        public RailwaysDBContext(DbContextOptions options)
            : base(options) {
            Database.EnsureCreated();
        }

        public DbSet<Discount> Discount { get; set; }
        public DbSet<Passenger> Passenger { get; set; }
        public DbSet<PassportType> PassportType { get; set; }
        public DbSet<Sale> Sale { get; set; }
        public DbSet<Seat> Seat { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Train> Train { get; set; }
        public DbSet<TrainArrivalTown> TrainArrivalTown { get; set; }
        public DbSet<TrainDepartureTown> TrainDepartureTown { get; set; }
        public DbSet<TrainWagon> TrainWagon { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Wagon> Wagon { get; set; }
        public DbSet<WagonType> WagonType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Discount>(entity =>
            {
                entity.HasKey(e => e.IdDiscount)
                    .HasName("Discounts_pk")
                    .IsClustered(false);

                entity.ToTable("Discount", "rw");

                entity.Property(e => e.IdDiscount).HasColumnName("id_discount");

                entity.Property(e => e.DiscountMultiply).HasColumnName("discount_multiply");

                entity.Property(e => e.DiscountName)
                    .IsRequired()
                    .HasColumnName("discount_name")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Passenger>(entity =>
            {
                entity.HasKey(e => e.IdPassenger)
                    .HasName("Passenger_Auth_pk")
                    .IsClustered(false);

                entity.ToTable("Passenger", "rw");

                entity.Property(e => e.IdPassenger).HasColumnName("id_passenger");

                entity.Property(e => e.IdPassengerPassportType).HasColumnName("id_passenger_passport_type");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.PassengerBirthday)
                    .HasColumnName("passenger_birthday")
                    .HasColumnType("date");

                entity.Property(e => e.PassengerFullName)
                    .IsRequired()
                    .HasColumnName("passenger_full_name")
                    .HasMaxLength(100);

                entity.Property(e => e.PassengerPassport).HasColumnName("passenger_passport");

                entity.HasOne(d => d.PassportTypeNavigation)
                    .WithMany(p => p.Passengers)
                    .HasForeignKey(d => d.IdPassengerPassportType)
                    .HasConstraintName("Passenger_Passport_Type_id_passport_type_fk");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Passengers)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("Passenger_User_id_user_fk");
            });

            modelBuilder.Entity<PassportType>(entity =>
            {
                entity.HasKey(e => e.IdPassportType)
                    .HasName("Passport_Type_pk")
                    .IsClustered(false);

                entity.ToTable("Passport_Type", "rw");

                entity.Property(e => e.IdPassportType).HasColumnName("id_passport_type");

                entity.Property(e => e.Passport)
                    .IsRequired()
                    .HasColumnName("passport_type")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.IdSale)
                    .HasName("Sale_pk")
                    .IsClustered(false);

                entity.ToTable("Sale", "rw");

                entity.Property(e => e.IdSale).HasColumnName("id_sale");

                entity.Property(e => e.IdDiscount).HasColumnName("id_discount");

                entity.Property(e => e.IdPassenger).HasColumnName("id_passenger");

                entity.Property(e => e.IdTicket).HasColumnName("id_ticket");

                entity.Property(e => e.SaleDate)
                    .HasColumnName("sale_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TotalPrice).HasColumnName("total_price");

                entity.HasOne(d => d.DiscountNavigation)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.IdDiscount)
                    .HasConstraintName("Sale_Discount_id_discount_fk");

                entity.HasOne(d => d.PassengerNavigation)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.IdPassenger)
                    .HasConstraintName("Sale_Passengers_id_passenger_fk");

                entity.HasOne(d => d.TicketNavigation)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.IdTicket)
                    .HasConstraintName("Sale_Ticket_id_ticket_fk");
            });

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(e => e.IdSeat)
                    .HasName("Seat_pk")
                    .IsClustered(false);

                entity.ToTable("Seat", "rw");

                entity.Property(e => e.IdSeat).HasColumnName("id_seat");

                entity.Property(e => e.IdWagon).HasColumnName("id_wagon");

                entity.Property(e => e.Seat1).HasColumnName("seat");

                entity.Property(e => e.SeatAvailability).HasColumnName("seat_availability");

                entity.HasOne(d => d.WagonNavigation)
                    .WithMany(p => p.Seats)
                    .HasForeignKey(d => d.IdWagon)
                    .HasConstraintName("Seat_Wagon_id_wagon_fk");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.IdTicket)
                    .HasName("Ticket_pk")
                    .IsClustered(false);

                entity.ToTable("Ticket", "rw");

                entity.Property(e => e.IdTicket).HasColumnName("id_ticket");

                entity.Property(e => e.IdSeat).HasColumnName("id_seat");

                entity.Property(e => e.IdTrainArrivalTown).HasColumnName("id_train_arrival_town");

                entity.Property(e => e.IdTrainDepartureTown).HasColumnName("id_train_departure_town");

                entity.Property(e => e.TicketDate)
                    .HasColumnName("ticket_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.TicketTravelTime).HasColumnName("ticket_travel_time");

                entity.HasOne(d => d.SeatNavigation)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.IdSeat)
                    .HasConstraintName("Ticket_Seat_id_seat_fk");

                entity.HasOne(d => d.TrainArrivalTownNavigation)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.IdTrainArrivalTown)
                    .HasConstraintName("Ticket_Train_Arrival_Town_id_train_arrival_town_fk");

                entity.HasOne(d => d.TrainDepartureTownNavigation)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.IdTrainDepartureTown)
                    .HasConstraintName("Ticket_Train_Departure_Town_id_train_departure_town_fk");
            });

            modelBuilder.Entity<Train>(entity =>
            {
                entity.HasKey(e => e.IdTrain)
                    .HasName("Trains_pk")
                    .IsClustered(false);

                entity.ToTable("Train", "rw");

                entity.Property(e => e.IdTrain).HasColumnName("id_train");

                entity.Property(e => e.TrainName)
                    .IsRequired()
                    .HasColumnName("train_name")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<TrainArrivalTown>(entity =>
            {
                entity.HasKey(e => e.IdTrainArrivalTown)
                    .HasName("Train_Arrival_Town_pk")
                    .IsClustered(false);

                entity.ToTable("Train_Arrival_Town", "rw");

                entity.Property(e => e.IdTrainArrivalTown).HasColumnName("id_train_arrival_town");

                entity.Property(e => e.TownName)
                    .IsRequired()
                    .HasColumnName("town_name")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<TrainDepartureTown>(entity =>
            {
                entity.HasKey(e => e.IdTrainDepartureTown)
                    .HasName("Train_Departure_Town_pk")
                    .IsClustered(false);

                entity.ToTable("Train_Departure_Town", "rw");

                entity.Property(e => e.IdTrainDepartureTown).HasColumnName("id_train_departure_town");

                entity.Property(e => e.TownName)
                    .IsRequired()
                    .HasColumnName("town_name")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<TrainWagon>(entity =>
            {
                entity.HasKey(e => e.IdTrainWagon)
                    .HasName("Train_Wagon_pk")
                    .IsClustered(false);

                entity.ToTable("Train_Wagon", "rw");

                entity.Property(e => e.IdTrainWagon).HasColumnName("id_train_wagon");

                entity.Property(e => e.IdTrain).HasColumnName("id_train");

                entity.Property(e => e.TrainTravelCount).HasColumnName("train_travel_count");

                entity.HasOne(d => d.TrainNavigation)
                    .WithMany(p => p.TrainWagon)
                    .HasForeignKey(d => d.IdTrain)
                    .HasConstraintName("Train_Wagon_Train_id_train_fk");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser)
                    .HasName("User_pk")
                    .IsClustered(false);


                entity.ToTable("User", "rw");

                entity.HasIndex(e => e.UserLogin)
                    .HasName("User_user_login_uindex")
                    .IsUnique();

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.UserHash)
                    .IsRequired()
                    .HasColumnName("user_hash")
                    .HasMaxLength(100);

                entity.Property(e => e.UserLogin)
                    .IsRequired()
                    .HasColumnName("user_login")
                    .HasMaxLength(50);

                entity.Property(e => e.UserSalt)
                    .IsRequired()
                    .HasColumnName("user_salt")
                    .HasMaxLength(100);

                entity.Property(e => e.UserType).HasColumnName("user_type");

            });

            modelBuilder.Entity<Wagon>(entity =>
            {
                entity.HasKey(e => e.IdWagon)
                    .HasName("Wagon_pk")
                    .IsClustered(false);

                entity.ToTable("Wagon", "rw");

                entity.Property(e => e.IdWagon).HasColumnName("id_wagon");

                entity.Property(e => e.IdTrainWagon).HasColumnName("id_train_wagon");

                entity.Property(e => e.IdWagonType).HasColumnName("id_wagon_type");

                entity.Property(e => e.WagonNumber).HasColumnName("wagon_number");

                entity.HasOne(d => d.TrainWagonNavigation)
                    .WithMany(p => p.Wagon)
                    .HasForeignKey(d => d.IdTrainWagon)
                    .HasConstraintName("Wagon_Train_Wagon_id_train_wagon_fk");

                entity.HasOne(d => d.WagonTypeNavigation)
                    .WithMany(p => p.Wagons)
                    .HasForeignKey(d => d.IdWagonType)
                    .HasConstraintName("Wagon_Wagon_Type_id_wagon_type_fk");
            });

            modelBuilder.Entity<WagonType>(entity =>
            {
                entity.HasKey(e => e.IdWagonType)
                    .HasName("Train_Type_pk")
                    .IsClustered(false);

                entity.ToTable("Wagon_Type", "rw");

                entity.Property(e => e.IdWagonType).HasColumnName("id_wagon_type");

                entity.Property(e => e.WagonPrice).HasColumnName("wagon_price");

                entity.Property(e => e.WagonType1)
                    .IsRequired()
                    .HasColumnName("wagon_type")
                    .HasMaxLength(100);
            });
        }
    }
}
