using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GestionTransporte.Models.DbEntities;

namespace GestionTransporte.DAL;

public partial class GestionTransporteDbContext : DbContext
{
    public GestionTransporteDbContext()
    {
    }

    public GestionTransporteDbContext(DbContextOptions<GestionTransporteDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TipoIdentificacion> TipoIdentificacion { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TipoIdentificacion>(entity =>
        {
            entity.HasKey(e => e.IdTipoIdentificacion);

            entity.ToTable("Tipo_Identificacion");

            entity.Property(e => e.NombreTipoIdentificacion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
} 