using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class TMSContext : DbContext
    {
        public TMSContext()
        {
        }

        public TMSContext(DbContextOptions<TMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BangGiaDacBiet> BangGiaDacBiets { get; set; }
        public virtual DbSet<BangGium> BangGia { get; set; }
        public virtual DbSet<BangQuyDoi> BangQuyDois { get; set; }
        public virtual DbSet<CungDuong> CungDuongs { get; set; }
        public virtual DbSet<DiaDiem> DiaDiems { get; set; }
        public virtual DbSet<DoiTac> DoiTacs { get; set; }
        public virtual DbSet<DonViTinh> DonViTinhs { get; set; }
        public virtual DbSet<DonViVanTai> DonViVanTais { get; set; }
        public virtual DbSet<HopDongVaPhuLuc> HopDongVaPhuLucs { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<LoaiDiaDiem> LoaiDiaDiems { get; set; }
        public virtual DbSet<LoaiHangHoa> LoaiHangHoas { get; set; }
        public virtual DbSet<LoaiPhuPhi> LoaiPhuPhis { get; set; }
        public virtual DbSet<LoaiPhuongTien> LoaiPhuongTiens { get; set; }
        public virtual DbSet<LoaiRomooc> LoaiRomoocs { get; set; }
        public virtual DbSet<LoaiThungHang> LoaiThungHangs { get; set; }
        public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public virtual DbSet<NhaPhanPhoi> NhaPhanPhois { get; set; }
        public virtual DbSet<PhuPhi> PhuPhis { get; set; }
        public virtual DbSet<PhuongThucVanChuyen> PhuongThucVanChuyens { get; set; }
        public virtual DbSet<QuanHuyen> QuanHuyens { get; set; }
        public virtual DbSet<QuocGium> QuocGia { get; set; }
        public virtual DbSet<Romooc> Romoocs { get; set; }
        public virtual DbSet<TaiXe> TaiXes { get; set; }
        public virtual DbSet<TinhThanh> TinhThanhs { get; set; }
        public virtual DbSet<VanDon> VanDons { get; set; }
        public virtual DbSet<XaPhuong> XaPhuongs { get; set; }
        public virtual DbSet<XeVanChuyen> XeVanChuyens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-2FO88N0;Database=TMS;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<BangGiaDacBiet>(entity =>
            {
                entity.HasKey(e => e.MaBangGiaDb)
                    .HasName("PK_ThongTin_BangGiaDacBiet");

                entity.ToTable("BangGiaDacBiet");

                entity.Property(e => e.MaBangGiaDb)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaBangGiaDB");

                entity.Property(e => e.Km).HasColumnName("KM");

                entity.Property(e => e.TenBangGiaDb)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("TenBangGiaDB");
            });

            modelBuilder.Entity<BangGium>(entity =>
            {
                entity.HasKey(e => e.MaBangGia)
                    .HasName("PK_ThongTin_BangGia");

                entity.Property(e => e.MaBangGia)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GiaUsd)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("GiaUSD");

                entity.Property(e => e.GiaVnd)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("GiaVND");

                entity.Property(e => e.MaCungDuong)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaDvt)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaDVT");

                entity.Property(e => e.MaLoaiHangHoa)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaLoaiPhuongTien)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaPtvc)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaPTVC");

                entity.HasOne(d => d.MaCungDuongNavigation)
                    .WithMany(p => p.BangGia)
                    .HasForeignKey(d => d.MaCungDuong)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTin_BangGia_ThongTin_CungDuong");
            });

            modelBuilder.Entity<BangQuyDoi>(entity =>
            {
                entity.HasKey(e => e.MaQuyDoi)
                    .HasName("PK_ThongTin_BangQuyDoi");

                entity.ToTable("BangQuyDoi");

                entity.Property(e => e.MaQuyDoi).ValueGeneratedNever();

                entity.Property(e => e.CongThucQuyDoi)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LoaiHangCanQuyDoi)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LoaiHangCanQuyDOi");

                entity.Property(e => e.MaDtv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaDTV");
            });

            modelBuilder.Entity<CungDuong>(entity =>
            {
                entity.HasKey(e => e.MaCungDuong)
                    .HasName("PK_ThongTin_CungDuong");

                entity.ToTable("CungDuong");

                entity.Property(e => e.MaCungDuong)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DiemCuoi)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DiemDau)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DiemLayRong)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.GhiChu)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MaHopDong)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenCungDuong)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DiaDiem>(entity =>
            {
                entity.HasKey(e => e.MaDiaDiem)
                    .HasName("PK_DanhMucDiaDiem");

                entity.ToTable("DiaDiem");

                entity.Property(e => e.DiaChiDayDu)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.MaGps)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaGPS");

                entity.Property(e => e.MaLoaiDiaDiem)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SoNha)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TenDiaDiem)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.MaHuyenNavigation)
                    .WithMany(p => p.DiaDiems)
                    .HasForeignKey(d => d.MaHuyen)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiaChi_DanhMucDiaDiem_DiaChi_QuanHuyen");

                entity.HasOne(d => d.MaLoaiDiaDiemNavigation)
                    .WithMany(p => p.DiaDiems)
                    .HasForeignKey(d => d.MaLoaiDiaDiem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DanhMuc_DiaDiem_DanhMuc_LoaiDiaDiem");

                entity.HasOne(d => d.MaPhuongNavigation)
                    .WithMany(p => p.DiaDiems)
                    .HasForeignKey(d => d.MaPhuong)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiaChi_DanhMucDiaDiem_DiaChi_XaPhuong");

                entity.HasOne(d => d.MaQuocGiaNavigation)
                    .WithMany(p => p.DiaDiems)
                    .HasForeignKey(d => d.MaQuocGia)
                    .HasConstraintName("FK_DiaChi_DanhMucDiaDiem_DiaChi_QuocGia");

                entity.HasOne(d => d.MaTinhNavigation)
                    .WithMany(p => p.DiaDiems)
                    .HasForeignKey(d => d.MaTinh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiaChi_DanhMucDiaDiem_DiaChi_TinhThanh");
            });

            modelBuilder.Entity<DoiTac>(entity =>
            {
                entity.HasKey(e => e.MaDoiTac)
                    .HasName("PK_ThongTin_DoiTac");

                entity.ToTable("DoiTac");

                entity.Property(e => e.MaDoiTac)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaKh)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaKH");

                entity.Property(e => e.NhomDc)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("NhomDC");

                entity.Property(e => e.TenDoiTac)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.DoiTacs)
                    .HasForeignKey(d => d.MaKh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTin_DoiTac_ThongTin_KhachHang");
            });

            modelBuilder.Entity<DonViTinh>(entity =>
            {
                entity.HasKey(e => e.MaDvt)
                    .HasName("PK_PhanLoai_DVT");

                entity.ToTable("DonViTinh");

                entity.Property(e => e.MaDvt)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaDVT");

                entity.Property(e => e.MoTa)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenDvt)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("TenDVT");
            });

            modelBuilder.Entity<DonViVanTai>(entity =>
            {
                entity.HasKey(e => e.MaDonViVanTai)
                    .HasName("PK_PhanLoai_DonViVanTai");

                entity.ToTable("DonViVanTai");

                entity.Property(e => e.MaDonViVanTai)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenDonViVanTai)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<HopDongVaPhuLuc>(entity =>
            {
                entity.HasKey(e => e.MaHopDong)
                    .HasName("PK_ThongTin_HopDongVaPhuLuc");

                entity.ToTable("HopDongVaPhuLuc");

                entity.Property(e => e.MaHopDong)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.GhiChu).HasMaxLength(50);

                entity.Property(e => e.MaKh)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaKH");

                entity.Property(e => e.MaPtvc)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaPTVC");

                entity.Property(e => e.ParentId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PhanLoaiHopDong)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenHienThi)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ThoiGianBatDau).HasColumnType("date");

                entity.Property(e => e.TrangThai)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.HopDongVaPhuLucs)
                    .HasForeignKey(d => d.MaKh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTin_HopDongVaPhuLuc_ThongTin_KhachHang");
            });

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.MaKh)
                    .HasName("PK_ThongTinNguoiBan");

                entity.ToTable("KhachHang");

                entity.Property(e => e.MaKh)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaKH");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaBangGia)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaSoThue)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sdt)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SDT");

                entity.Property(e => e.TenKh)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("TenKH");

                entity.HasOne(d => d.MaBangGiaNavigation)
                    .WithMany(p => p.KhachHangs)
                    .HasForeignKey(d => d.MaBangGia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KhachHang_BangGia");
            });

            modelBuilder.Entity<LoaiDiaDiem>(entity =>
            {
                entity.HasKey(e => e.MaLoaiDiaDiem)
                    .HasName("PK_DanhMuc_LoaiDiaDiem");

                entity.ToTable("LoaiDiaDiem");

                entity.Property(e => e.MaLoaiDiaDiem)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenPhanLoaiDiaDiem)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<LoaiHangHoa>(entity =>
            {
                entity.HasKey(e => e.MaLoaiHangHoa)
                    .HasName("PK_PhanLoai_HangHoa");

                entity.ToTable("LoaiHangHoa");

                entity.Property(e => e.MaLoaiHangHoa)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenLoaiHangHoa)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<LoaiPhuPhi>(entity =>
            {
                entity.HasKey(e => e.MaLoaiPhuPhi)
                    .HasName("PK_PhanLoai_PhuPhi");

                entity.ToTable("LoaiPhuPhi");

                entity.Property(e => e.MaLoaiPhuPhi)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenLoaiPhuPhi)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<LoaiPhuongTien>(entity =>
            {
                entity.HasKey(e => e.MaLoaiPhuongTien)
                    .HasName("PK_PhanLoai_PhuongTien");

                entity.ToTable("LoaiPhuongTien");

                entity.Property(e => e.MaLoaiPhuongTien)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenLoaiPhuongTien)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<LoaiRomooc>(entity =>
            {
                entity.HasKey(e => e.MaLoaiRomooc)
                    .HasName("PK_PhanLoai_Romooc");

                entity.ToTable("LoaiRomooc");

                entity.Property(e => e.MaLoaiRomooc)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenLoaiRomooc)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<LoaiThungHang>(entity =>
            {
                entity.HasKey(e => e.MaLoaiThungHang)
                    .HasName("PK_PhanLoai_Container");

                entity.ToTable("LoaiThungHang");

                entity.Property(e => e.MaLoaiThungHang)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenLoaiThungHang)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<NhaCungCap>(entity =>
            {
                entity.HasKey(e => e.MaNhaCungCap)
                    .HasName("PK_ThongTin_NhaThau");

                entity.ToTable("NhaCungCap");

                entity.Property(e => e.MaNhaCungCap)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LoaiDichVu)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LoaiNhaCungCap)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MaHopDong)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaSoThue)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sdt)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("SDT");

                entity.Property(e => e.TenNhaCungCap)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.MaHopDongNavigation)
                    .WithMany(p => p.NhaCungCaps)
                    .HasForeignKey(d => d.MaHopDong)
                    .HasConstraintName("FK_NhaCungCap_HopDongVaPhuLuc");
            });

            modelBuilder.Entity<NhaPhanPhoi>(entity =>
            {
                entity.HasKey(e => e.MaNpp)
                    .HasName("PK_ThongTin_NhaPhanPhoi");

                entity.ToTable("NhaPhanPhoi");

                entity.Property(e => e.MaNpp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaNPP");

                entity.Property(e => e.MaDoiTac)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenNpp)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("TenNPP");

                entity.HasOne(d => d.MaDoiTacNavigation)
                    .WithMany(p => p.NhaPhanPhois)
                    .HasForeignKey(d => d.MaDoiTac)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTin_NhaPhanPhoi_ThongTin_DoiTac");
            });

            modelBuilder.Entity<PhuPhi>(entity =>
            {
                entity.HasKey(e => e.MaPhuPhi)
                    .HasName("PK_ThongTin_PhuPhi");

                entity.ToTable("PhuPhi");

                entity.Property(e => e.MaPhuPhi)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GiaUsd)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("GiaUSD");

                entity.Property(e => e.GiaVnd)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("GiaVND");

                entity.Property(e => e.MaCungDuong)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaLoaiHangHoa)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaLoaiPhuPhi)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaPtvc)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaPTVC");

                entity.Property(e => e.NgayHieuLuc).HasColumnType("date");

                entity.HasOne(d => d.MaCungDuongNavigation)
                    .WithMany(p => p.PhuPhis)
                    .HasForeignKey(d => d.MaCungDuong)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhuPhi_CungDuong");
            });

            modelBuilder.Entity<PhuongThucVanChuyen>(entity =>
            {
                entity.HasKey(e => e.MaPtvc)
                    .HasName("PK_HinhThucVanChuyen");

                entity.ToTable("PhuongThucVanChuyen");

                entity.Property(e => e.MaPtvc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaPTVC");

                entity.Property(e => e.TenPtvc)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("TenPTVC");
            });

            modelBuilder.Entity<QuanHuyen>(entity =>
            {
                entity.HasKey(e => e.MaHuyen)
                    .HasName("PK_DiaChi_QuanHuyen");

                entity.ToTable("QuanHuyen");

                entity.Property(e => e.MaHuyen).ValueGeneratedNever();

                entity.Property(e => e.PhanLoai)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenHuyen)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<QuocGium>(entity =>
            {
                entity.HasKey(e => e.MaQuocGia)
                    .HasName("PK_DiaChi_QuocGia");

                entity.Property(e => e.MaQuocGia).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenQuocGia)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Romooc>(entity =>
            {
                entity.HasKey(e => e.MaRomooc)
                    .HasName("PK_ThongTin_Romooc");

                entity.ToTable("Romooc");

                entity.Property(e => e.MaRomooc)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.KetCauSan)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MaLoaiRomooc)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SoGuRomooc)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ThongSoKyThuat)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.MaLoaiRomoocNavigation)
                    .WithMany(p => p.Romoocs)
                    .HasForeignKey(d => d.MaLoaiRomooc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTin_Romooc_PhanLoai_Romooc");
            });

            modelBuilder.Entity<TaiXe>(entity =>
            {
                entity.HasKey(e => e.MaTaiXe)
                    .HasName("PK_ThongTin_TaiXe");

                entity.ToTable("TaiXe");

                entity.Property(e => e.MaTaiXe)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Cccd)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("CCCD");

                entity.Property(e => e.GhiChu).HasColumnType("text");

                entity.Property(e => e.HoVaTen)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LoaiXe)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaNhaThau)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NgaySinh).HasColumnType("date");

                entity.Property(e => e.PhanLoaiTaiXe)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SoDienThoai)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.TrangThai)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TinhThanh>(entity =>
            {
                entity.HasKey(e => e.MaTinh)
                    .HasName("PK_DiaChi_TinhThanh");

                entity.ToTable("TinhThanh");

                entity.Property(e => e.MaTinh).ValueGeneratedNever();

                entity.Property(e => e.PhanLoai)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenTinh)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<VanDon>(entity =>
            {
                entity.HasKey(e => e.MaVanDon)
                    .HasName("PK_ThongTin_VanDon");

                entity.ToTable("VanDon");

                entity.Property(e => e.MaVanDon)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Booking)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CangChuyenTai).HasMaxLength(50);

                entity.Property(e => e.CangDich).HasMaxLength(50);

                entity.Property(e => e.ClpNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CLP_No");

                entity.Property(e => e.ContNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Cont_No");

                entity.Property(e => e.HangTau).HasMaxLength(50);

                entity.Property(e => e.MaDonViVanTai)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaDvt).HasColumnName("MaDVT");

                entity.Property(e => e.MaKh)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaKH");

                entity.Property(e => e.MaLoaiHangHoa)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaLoaiThungHang)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaPtvc)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaPTVC");

                entity.Property(e => e.MaRomooc)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaSoXe)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaTaiXe)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.SealHq)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SEAL_HQ");

                entity.Property(e => e.SealHt)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SEAL_HT");

                entity.Property(e => e.Tau).HasMaxLength(50);

                entity.Property(e => e.TrangThaiDonHang)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.VanDons)
                    .HasForeignKey(d => d.MaKh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTin_VanDonXuatNhap_ThongTin_KhachHang");

                entity.HasOne(d => d.MaRomoocNavigation)
                    .WithMany(p => p.VanDons)
                    .HasForeignKey(d => d.MaRomooc)
                    .HasConstraintName("FK_VanDon_Romooc");

                entity.HasOne(d => d.MaSoXeNavigation)
                    .WithMany(p => p.VanDons)
                    .HasForeignKey(d => d.MaSoXe)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VanDon_XeVanChuyen");

                entity.HasOne(d => d.MaTaiXeNavigation)
                    .WithMany(p => p.VanDons)
                    .HasForeignKey(d => d.MaTaiXe)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VanDon_TaiXe");
            });

            modelBuilder.Entity<XaPhuong>(entity =>
            {
                entity.HasKey(e => e.MaPhuong)
                    .HasName("PK_DiaChi_XaPhuong");

                entity.ToTable("XaPhuong");

                entity.Property(e => e.MaPhuong).ValueGeneratedNever();

                entity.Property(e => e.PhanLoai)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenPhuong)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<XeVanChuyen>(entity =>
            {
                entity.HasKey(e => e.MaSoXe)
                    .HasName("PK_ThongTin_XeVanChuyen");

                entity.ToTable("XeVanChuyen");

                entity.Property(e => e.MaSoXe)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LoaiVanHanh)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MaGps)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaGPS");

                entity.Property(e => e.MaGpsmobile)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaGPSMobile");

                entity.Property(e => e.MaLoaiPhuongTien)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaNhaCungCap)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaTaiSan)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaTaiXeMacDinh)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.NgayHoatDong).HasColumnType("date");

                entity.Property(e => e.PhanLoaiXeVanChuyen)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TrangThai)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.MaNhaCungCapNavigation)
                    .WithMany(p => p.XeVanChuyens)
                    .HasForeignKey(d => d.MaNhaCungCap)
                    .HasConstraintName("FK_XeVanChuyen_NhaCungCap");

                entity.HasOne(d => d.MaTaiXeMacDinhNavigation)
                    .WithMany(p => p.XeVanChuyens)
                    .HasForeignKey(d => d.MaTaiXeMacDinh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTin_XeVanChuyen_ThongTin_TaiXe");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
