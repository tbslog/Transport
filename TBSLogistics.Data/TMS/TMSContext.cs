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

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<BangGia> BangGia { get; set; }
        public virtual DbSet<BangGiaDacBiet> BangGiaDacBiet { get; set; }
        public virtual DbSet<BangQuyDoi> BangQuyDoi { get; set; }
        public virtual DbSet<BoPhan> BoPhan { get; set; }
        public virtual DbSet<CungDuong> CungDuong { get; set; }
        public virtual DbSet<DiaDiem> DiaDiem { get; set; }
        public virtual DbSet<DieuPhoi> DieuPhoi { get; set; }
        public virtual DbSet<DonViTinh> DonViTinh { get; set; }
        public virtual DbSet<HopDongVaPhuLuc> HopDongVaPhuLuc { get; set; }
        public virtual DbSet<KhachHang> KhachHang { get; set; }
        public virtual DbSet<LoaiDiaDiem> LoaiDiaDiem { get; set; }
        public virtual DbSet<LoaiHangHoa> LoaiHangHoa { get; set; }
        public virtual DbSet<LoaiHopDong> LoaiHopDong { get; set; }
        public virtual DbSet<LoaiKhachHang> LoaiKhachHang { get; set; }
        public virtual DbSet<LoaiPhuongTien> LoaiPhuongTien { get; set; }
        public virtual DbSet<LoaiRomooc> LoaiRomooc { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<NguoiDung> NguoiDung { get; set; }
        public virtual DbSet<NhomKhachHang> NhomKhachHang { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<PhuongThucVanChuyen> PhuongThucVanChuyen { get; set; }
        public virtual DbSet<QuanHuyen> QuanHuyen { get; set; }
        public virtual DbSet<QuocGia> QuocGia { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleHasPermission> RoleHasPermission { get; set; }
        public virtual DbSet<Romooc> Romooc { get; set; }
        public virtual DbSet<SfeeByTcommand> SfeeByTcommand { get; set; }
        public virtual DbSet<StatusText> StatusText { get; set; }
        public virtual DbSet<SubFee> SubFee { get; set; }
        public virtual DbSet<SubFeePrice> SubFeePrice { get; set; }
        public virtual DbSet<SubFeeType> SubFeeType { get; set; }
        public virtual DbSet<TaiXe> TaiXe { get; set; }
        public virtual DbSet<ThongBao> ThongBao { get; set; }
        public virtual DbSet<TinhThanh> TinhThanh { get; set; }
        public virtual DbSet<UserHasPermission> UserHasPermission { get; set; }
        public virtual DbSet<UserHasRole> UserHasRole { get; set; }
        public virtual DbSet<VanDon> VanDon { get; set; }
        public virtual DbSet<XaPhuong> XaPhuong { get; set; }
        public virtual DbSet<XeVanChuyen> XeVanChuyen { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=192.168.3.63;Database=TMS;User Id=haile;Password=123456;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PassWord)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DieuPhoiId).HasColumnName("DieuPhoiID");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FileType)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.FolderName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaHopDong)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.DieuPhoi)
                    .WithMany(p => p.Attachment)
                    .HasForeignKey(d => d.DieuPhoiId)
                    .HasConstraintName("FK_Attachment_DieuPhoi");

                entity.HasOne(d => d.MaHopDongNavigation)
                    .WithMany(p => p.Attachment)
                    .HasForeignKey(d => d.MaHopDong)
                    .HasConstraintName("FK_Attachment_HopDongVaPhuLuc");
            });

            modelBuilder.Entity<BangGia>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.MaCungDuong)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaDvt)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaDVT");

                entity.Property(e => e.MaHopDong)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MaLoaiDoiTac)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaLoaiHangHoa)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaLoaiPhuongTien)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaPtvc)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaPTVC");

                entity.HasOne(d => d.MaCungDuongNavigation)
                    .WithMany(p => p.BangGia)
                    .HasForeignKey(d => d.MaCungDuong)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BangGia_CungDuong");

                entity.HasOne(d => d.MaHopDongNavigation)
                    .WithMany(p => p.BangGia)
                    .HasForeignKey(d => d.MaHopDong)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BangGia_HopDongVaPhuLuc");
            });

            modelBuilder.Entity<BangGiaDacBiet>(entity =>
            {
                entity.HasKey(e => e.MaBangGiaDb)
                    .HasName("PK_ThongTin_BangGiaDacBiet");

                entity.Property(e => e.MaBangGiaDb)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaBangGiaDB");

                entity.Property(e => e.Km).HasColumnName("KM");

                entity.Property(e => e.TenBangGiaDb)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("TenBangGiaDB");
            });

            modelBuilder.Entity<BangQuyDoi>(entity =>
            {
                entity.HasKey(e => e.MaQuyDoi)
                    .HasName("PK_ThongTin_BangQuyDoi");

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

            modelBuilder.Entity<BoPhan>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MaBoPhan)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MoTaCongViec)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.TenBoPhan)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CungDuong>(entity =>
            {
                entity.HasKey(e => e.MaCungDuong)
                    .HasName("PK_ThongTin_CungDuong");

                entity.Property(e => e.MaCungDuong)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.GhiChu)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TenCungDuong)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DiaDiem>(entity =>
            {
                entity.HasKey(e => e.MaDiaDiem)
                    .HasName("PK_DanhMucDiaDiem");

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

                entity.Property(e => e.SoNha).HasMaxLength(100);

                entity.Property(e => e.TenDiaDiem)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.MaHuyenNavigation)
                    .WithMany(p => p.DiaDiem)
                    .HasForeignKey(d => d.MaHuyen)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiaDiem_QuanHuyen");

                entity.HasOne(d => d.MaLoaiDiaDiemNavigation)
                    .WithMany(p => p.DiaDiem)
                    .HasForeignKey(d => d.MaLoaiDiaDiem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DanhMuc_DiaDiem_DanhMuc_LoaiDiaDiem");

                entity.HasOne(d => d.MaPhuongNavigation)
                    .WithMany(p => p.DiaDiem)
                    .HasForeignKey(d => d.MaPhuong)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiaDiem_XaPhuong");

                entity.HasOne(d => d.MaQuocGiaNavigation)
                    .WithMany(p => p.DiaDiem)
                    .HasForeignKey(d => d.MaQuocGia)
                    .HasConstraintName("FK_DiaDiem_QuocGia");

                entity.HasOne(d => d.MaTinhNavigation)
                    .WithMany(p => p.DiaDiem)
                    .HasForeignKey(d => d.MaTinh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiaDiem_TinhThanh");
            });

            modelBuilder.Entity<DieuPhoi>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ContNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Cont_No");

                entity.Property(e => e.DonViVanTai)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.GiaThamChieu).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.HangTau).HasMaxLength(50);

                entity.Property(e => e.IdbangGia).HasColumnName("IDBangGia");

                entity.Property(e => e.MaDvt)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaDVT");

                entity.Property(e => e.MaLoaiHangHoa)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaLoaiPhuongTien)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaPtvc)
                    .IsRequired()
                    .HasMaxLength(10)
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

                entity.Property(e => e.MaVanDon)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SealHq)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SEAL_HQ");

                entity.Property(e => e.SealNp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SEAL_NP");

                entity.Property(e => e.Tau).HasMaxLength(50);

                entity.HasOne(d => d.DonViVanTaiNavigation)
                    .WithMany(p => p.DieuPhoi)
                    .HasForeignKey(d => d.DonViVanTai)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DieuPhoi_KhachHang");

                entity.HasOne(d => d.IdbangGiaNavigation)
                    .WithMany(p => p.DieuPhoi)
                    .HasForeignKey(d => d.IdbangGia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DieuPhoi_BangGia");

                entity.HasOne(d => d.MaRomoocNavigation)
                    .WithMany(p => p.DieuPhoi)
                    .HasForeignKey(d => d.MaRomooc)
                    .HasConstraintName("FK_DieuPhoi_Romooc");

                entity.HasOne(d => d.MaSoXeNavigation)
                    .WithMany(p => p.DieuPhoi)
                    .HasForeignKey(d => d.MaSoXe)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DieuPhoi_XeVanChuyen");

                entity.HasOne(d => d.MaTaiXeNavigation)
                    .WithMany(p => p.DieuPhoi)
                    .HasForeignKey(d => d.MaTaiXe)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DieuPhoi_TaiXe");

                entity.HasOne(d => d.MaVanDonNavigation)
                    .WithMany(p => p.DieuPhoi)
                    .HasForeignKey(d => d.MaVanDon)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DieuPhoi_VanDon");
            });

            modelBuilder.Entity<DonViTinh>(entity =>
            {
                entity.HasKey(e => e.MaDvt)
                    .HasName("PK_PhanLoai_DVT");

                entity.Property(e => e.MaDvt)
                    .HasMaxLength(10)
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

            modelBuilder.Entity<HopDongVaPhuLuc>(entity =>
            {
                entity.HasKey(e => e.MaHopDong)
                    .HasName("PK_ThongTin_HopDongVaPhuLuc");

                entity.Property(e => e.MaHopDong)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.GhiChu).HasMaxLength(500);

                entity.Property(e => e.MaHopDongCha)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MaKh)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("MaKH");

                entity.Property(e => e.MaLoaiHopDong)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaPhuPhi)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenHienThi)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ThoiGianBatDau).HasColumnType("date");

                entity.Property(e => e.ThoiGianKetThuc).HasColumnType("date");

                entity.HasOne(d => d.MaHopDongChaNavigation)
                    .WithMany(p => p.InverseMaHopDongChaNavigation)
                    .HasForeignKey(d => d.MaHopDongCha)
                    .HasConstraintName("FK_HopDongVaPhuLuc_HopDongVaPhuLuc");

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.HopDongVaPhuLuc)
                    .HasForeignKey(d => d.MaKh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HopDongVaPhuLuc_KhachHang");
            });

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.MaKh)
                    .HasName("PK_ThongTinNguoiBan");

                entity.Property(e => e.MaKh)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("MaKH");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaLoaiKh)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaLoaiKH");

                entity.Property(e => e.MaNhomKh)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaNhomKH");

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
            });

            modelBuilder.Entity<LoaiDiaDiem>(entity =>
            {
                entity.HasKey(e => e.MaLoaiDiaDiem)
                    .HasName("PK_DanhMuc_LoaiDiaDiem");

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

                entity.Property(e => e.MaLoaiHangHoa)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenLoaiHangHoa)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<LoaiHopDong>(entity =>
            {
                entity.HasKey(e => e.MaLoaiHopDong);

                entity.Property(e => e.MaLoaiHopDong)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenLoaiHopDong)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<LoaiKhachHang>(entity =>
            {
                entity.HasKey(e => e.MaLoaiKh);

                entity.Property(e => e.MaLoaiKh)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaLoaiKH");

                entity.Property(e => e.TenLoaiKh)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("TenLoaiKH");
            });

            modelBuilder.Entity<LoaiPhuongTien>(entity =>
            {
                entity.HasKey(e => e.MaLoaiPhuongTien)
                    .HasName("PK_PhanLoai_PhuongTien");

                entity.Property(e => e.MaLoaiPhuongTien)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PhanLoai)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenLoaiPhuongTien)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<LoaiRomooc>(entity =>
            {
                entity.HasKey(e => e.MaLoaiRomooc)
                    .HasName("PK_PhanLoai_Romooc");

                entity.Property(e => e.MaLoaiRomooc)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenLoaiRomooc)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.ModuleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Time");

                entity.Property(e => e.HoVaTen)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.MaBoPhan)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MaNhanVien)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.NguoiTao)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Time");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.NguoiDung)
                    .HasForeignKey<NguoiDung>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NguoiDung_Account");
            });

            modelBuilder.Entity<NhomKhachHang>(entity =>
            {
                entity.HasKey(e => e.MaNhomKh);

                entity.Property(e => e.MaNhomKh)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaNhomKH");

                entity.Property(e => e.TenNhomKh)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("TenNhomKH");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Mid)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("MId");

                entity.Property(e => e.ParentId)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ParentID");

                entity.Property(e => e.PermissionName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PhuongThucVanChuyen>(entity =>
            {
                entity.HasKey(e => e.MaPtvc)
                    .HasName("PK_HinhThucVanChuyen");

                entity.Property(e => e.MaPtvc)
                    .HasMaxLength(10)
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

                entity.Property(e => e.MaHuyen).ValueGeneratedNever();

                entity.Property(e => e.PhanLoai)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenHuyen)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<QuocGia>(entity =>
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

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<RoleHasPermission>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RoleHasPermission)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleHasPermission_Permission");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleHasPermission)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleHasPermission_Role");
            });

            modelBuilder.Entity<Romooc>(entity =>
            {
                entity.HasKey(e => e.MaRomooc)
                    .HasName("PK_ThongTin_Romooc");

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
                    .WithMany(p => p.Romooc)
                    .HasForeignKey(d => d.MaLoaiRomooc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTin_Romooc_PhanLoai_Romooc");
            });

            modelBuilder.Entity<SfeeByTcommand>(entity =>
            {
                entity.ToTable("SFeeByTcommand");

                entity.Property(e => e.IdTcommand).HasColumnName("IdTCommand");

                entity.Property(e => e.SfId).HasColumnName("sfID");

                entity.Property(e => e.SfPriceId).HasColumnName("sfPriceId");

                entity.HasOne(d => d.IdTcommandNavigation)
                    .WithMany(p => p.SfeeByTcommand)
                    .HasForeignKey(d => d.IdTcommand)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SFeeByTcommand_DieuPhoi");

                entity.HasOne(d => d.Sf)
                    .WithMany(p => p.SfeeByTcommand)
                    .HasForeignKey(d => d.SfId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SFeeByTcommand_SubFee");

                entity.HasOne(d => d.SfPrice)
                    .WithMany(p => p.SfeeByTcommand)
                    .HasForeignKey(d => d.SfPriceId)
                    .HasConstraintName("FK_SFeeByTcommand_SubFeePrice");
            });

            modelBuilder.Entity<StatusText>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FunctionId)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("FunctionID");

                entity.Property(e => e.LangId)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("LangID");

                entity.Property(e => e.StatusContent)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.StatusId).HasColumnName("StatusID");
            });

            modelBuilder.Entity<SubFee>(entity =>
            {
                entity.Property(e => e.SubFeeId).HasColumnName("subFeeID");

                entity.Property(e => e.Creator)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("creator");

                entity.Property(e => e.SfDescription).HasColumnName("sfDescription");

                entity.Property(e => e.SfName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("sfName");

                entity.Property(e => e.SfState)
                    .HasColumnName("sfState")
                    .HasDefaultValueSql("((1))")
                    .HasComment("0: deactivated, 1: create new, 2: approved, 3: deleted");

                entity.Property(e => e.SfType).HasColumnName("sfType");

                entity.HasOne(d => d.SfTypeNavigation)
                    .WithMany(p => p.SubFee)
                    .HasForeignKey(d => d.SfType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubFee_SubFeeType");
            });

            modelBuilder.Entity<SubFeePrice>(entity =>
            {
                entity.HasKey(e => e.PriceId);

                entity.Property(e => e.PriceId).HasColumnName("priceID");

                entity.Property(e => e.ApprovedDate).HasColumnName("approvedDate");

                entity.Property(e => e.Approver)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("approver");

                entity.Property(e => e.ContractId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("contractID");

                entity.Property(e => e.Creator)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("creator");

                entity.Property(e => e.DeactiveDate).HasColumnName("deactiveDate");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.FirstPlace)
                    .HasColumnName("firstPlace")
                    .HasComment("if the sub-fee is collected on road, need two place; is collected at a place need one place; independent to place  then 2 place-fields are null ");

                entity.Property(e => e.GoodsType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("goodsType");

                entity.Property(e => e.SecondPlace).HasColumnName("secondPlace");

                entity.Property(e => e.SfId).HasColumnName("sfID");

                entity.Property(e => e.SfStateByContract)
                    .HasColumnName("sfStateByContract")
                    .HasDefaultValueSql("((1))")
                    .HasComment("0: deactivated, 1: create new, 2: approved, 3: deleted");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UnitPrice).HasColumnName("unitPrice");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.SubFeePrice)
                    .HasForeignKey(d => d.ContractId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubFeePrice_HopDongVaPhuLuc");

                entity.HasOne(d => d.Sf)
                    .WithMany(p => p.SubFeePrice)
                    .HasForeignKey(d => d.SfId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubFeePrice_SubFee");
            });

            modelBuilder.Entity<SubFeeType>(entity =>
            {
                entity.HasKey(e => e.SfTypeId);

                entity.Property(e => e.SfTypeId).HasColumnName("sfTypeID");

                entity.Property(e => e.SfTypeName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("sfTypeName");
            });

            modelBuilder.Entity<TaiXe>(entity =>
            {
                entity.HasKey(e => e.MaTaiXe)
                    .HasName("PK_ThongTin_TaiXe");

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

                entity.Property(e => e.MaLoaiPhuongTien)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaNhaCungCap)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.NgaySinh).HasColumnType("date");

                entity.Property(e => e.SoDienThoai)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.TaiXeTbs).HasColumnName("TaiXeTBS");

                entity.HasOne(d => d.MaNhaCungCapNavigation)
                    .WithMany(p => p.TaiXe)
                    .HasForeignKey(d => d.MaNhaCungCap)
                    .HasConstraintName("FK_TaiXe_KhachHang1");
            });

            modelBuilder.Entity<ThongBao>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FunctionId)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("FunctionID");

                entity.Property(e => e.LangId)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("LangID");

                entity.Property(e => e.TextContent)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TextId).HasColumnName("TextID");

                entity.Property(e => e.TextType)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TinhThanh>(entity =>
            {
                entity.HasKey(e => e.MaTinh)
                    .HasName("PK_DiaChi_TinhThanh");

                entity.Property(e => e.MaTinh).ValueGeneratedNever();

                entity.Property(e => e.PhanLoai)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenTinh)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserHasPermission>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.UserHasPermission)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserHasPermission_Permission");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserHasPermission)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserHasPermission_NguoiDung");
            });

            modelBuilder.Entity<UserHasRole>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserHasRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserHasRole_Role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserHasRole)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserHasRole_NguoiDung");
            });

            modelBuilder.Entity<VanDon>(entity =>
            {
                entity.HasKey(e => e.MaVanDon)
                    .HasName("PK_ThongTin_VanDon");

                entity.Property(e => e.MaVanDon)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LoaiVanDon)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaCungDuong)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaKh)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("MaKH");
            });

            modelBuilder.Entity<XaPhuong>(entity =>
            {
                entity.HasKey(e => e.MaPhuong)
                    .HasName("PK_DiaChi_XaPhuong");

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

                entity.Property(e => e.MaSoXe)
                    .HasMaxLength(10)
                    .IsUnicode(false);

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
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaTaiSan)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaTaiXeMacDinh)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.NgayHoatDong).HasColumnType("date");

                entity.HasOne(d => d.MaTaiXeMacDinhNavigation)
                    .WithMany(p => p.XeVanChuyen)
                    .HasForeignKey(d => d.MaTaiXeMacDinh)
                    .HasConstraintName("FK_ThongTin_XeVanChuyen_ThongTin_TaiXe");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
