﻿// <auto-generated />
using System;
using EmergencyManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EmergencyManagementAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.5");

            modelBuilder.Entity("EmergencyManagementAPI.Models.Abrigo", b =>
                {
                    b.Property<int>("IdAbrigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id_abrigo");

                    b.Property<int>("CapacidadeTotal")
                        .HasColumnType("INTEGER")
                        .HasColumnName("capacidade_total");

                    b.Property<string>("Endereco")
                        .HasMaxLength(150)
                        .HasColumnType("TEXT")
                        .HasColumnName("endereco");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("nome");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT")
                        .HasColumnName("status");

                    b.Property<string>("Telefone")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT")
                        .HasColumnName("telefone");

                    b.Property<int>("VagasDisponiveis")
                        .HasColumnType("INTEGER")
                        .HasColumnName("vagas_disponiveis");

                    b.HasKey("IdAbrigo");

                    b.ToTable("abrigos");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.Alerta", b =>
                {
                    b.Property<int>("IdAlerta")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id_alerta");

                    b.Property<DateTime>("DataEmissao")
                        .HasColumnType("TEXT")
                        .HasColumnName("data_emissao");

                    b.Property<string>("Fonte")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("fonte");

                    b.Property<int>("IdOcorrencia")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id_ocorrencia");

                    b.Property<decimal?>("Latitude")
                        .HasColumnType("TEXT")
                        .HasColumnName("latitude");

                    b.Property<decimal?>("Longitude")
                        .HasColumnType("TEXT")
                        .HasColumnName("longitude");

                    b.Property<string>("Mensagem")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT")
                        .HasColumnName("mensagem");

                    b.Property<string>("NivelUrgencia")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT")
                        .HasColumnName("nivel_urgencia");

                    b.Property<decimal?>("RaioAfetado")
                        .HasColumnType("TEXT")
                        .HasColumnName("raio_afetado");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("titulo");

                    b.HasKey("IdAlerta");

                    b.HasIndex("IdOcorrencia");

                    b.ToTable("alertas");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.CheckinAbrigo", b =>
                {
                    b.Property<int>("IdCheckin")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id_checkin");

                    b.Property<DateTime>("DataEntrada")
                        .HasColumnType("TEXT")
                        .HasColumnName("data_entrada");

                    b.Property<DateTime?>("DataSaida")
                        .HasColumnType("TEXT")
                        .HasColumnName("data_saida");

                    b.Property<int>("IdAbrigo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id_abrigo");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id_usuario");

                    b.HasKey("IdCheckin");

                    b.HasIndex("IdAbrigo");

                    b.HasIndex("IdUsuario");

                    b.ToTable("checkins_abrigos");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.Ocorrencia", b =>
                {
                    b.Property<int>("IdOcorrencia")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id_ocorrencia");

                    b.Property<DateTime>("DataOcorrencia")
                        .HasColumnType("TEXT")
                        .HasColumnName("data_ocorrencia");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT")
                        .HasColumnName("descricao");

                    b.Property<int>("IdTipo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id_tipo");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id_usuario");

                    b.Property<decimal?>("Latitude")
                        .HasColumnType("TEXT")
                        .HasColumnName("latitude");

                    b.Property<decimal?>("Longitude")
                        .HasColumnType("TEXT")
                        .HasColumnName("longitude");

                    b.Property<string>("NivelRisco")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT")
                        .HasColumnName("nivel_risco");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT")
                        .HasColumnName("status");

                    b.HasKey("IdOcorrencia");

                    b.HasIndex("IdTipo");

                    b.HasIndex("IdUsuario");

                    b.ToTable("ocorrencias");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.TipoOcorrencia", b =>
                {
                    b.Property<int>("IdTipo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id_tipo");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT")
                        .HasColumnName("nome");

                    b.HasKey("IdTipo");

                    b.ToTable("tipos_ocorrencia");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.Usuario", b =>
                {
                    b.Property<int>("IdUsuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id_usuario");

                    b.Property<DateTime>("DataCadastro")
                        .HasColumnType("TEXT")
                        .HasColumnName("data_cadastro");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("email");

                    b.Property<string>("Endereco")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT")
                        .HasColumnName("endereco");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("nome");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("senha");

                    b.Property<string>("Telefone")
                        .HasMaxLength(15)
                        .HasColumnType("TEXT")
                        .HasColumnName("telefone");

                    b.Property<string>("TipoUsuario")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT")
                        .HasColumnName("tipo_usuario");

                    b.HasKey("IdUsuario");

                    b.ToTable("usuarios");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.Alerta", b =>
                {
                    b.HasOne("EmergencyManagementAPI.Models.Ocorrencia", "Ocorrencia")
                        .WithMany("Alertas")
                        .HasForeignKey("IdOcorrencia")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ocorrencia");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.CheckinAbrigo", b =>
                {
                    b.HasOne("EmergencyManagementAPI.Models.Abrigo", "Abrigo")
                        .WithMany("CheckinsAbrigos")
                        .HasForeignKey("IdAbrigo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmergencyManagementAPI.Models.Usuario", "Usuario")
                        .WithMany("CheckinsAbrigos")
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Abrigo");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.Ocorrencia", b =>
                {
                    b.HasOne("EmergencyManagementAPI.Models.TipoOcorrencia", "TipoOcorrencia")
                        .WithMany("Ocorrencias")
                        .HasForeignKey("IdTipo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmergencyManagementAPI.Models.Usuario", "Usuario")
                        .WithMany("Ocorrencias")
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TipoOcorrencia");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.Abrigo", b =>
                {
                    b.Navigation("CheckinsAbrigos");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.Ocorrencia", b =>
                {
                    b.Navigation("Alertas");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.TipoOcorrencia", b =>
                {
                    b.Navigation("Ocorrencias");
                });

            modelBuilder.Entity("EmergencyManagementAPI.Models.Usuario", b =>
                {
                    b.Navigation("CheckinsAbrigos");

                    b.Navigation("Ocorrencias");
                });
#pragma warning restore 612, 618
        }
    }
}
