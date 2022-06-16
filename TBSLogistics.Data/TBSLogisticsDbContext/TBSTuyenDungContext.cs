using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TBSTuyenDungContext : DbContext
    {
        public TBSTuyenDungContext()
        {
        }

        public TBSTuyenDungContext(DbContextOptions<TBSTuyenDungContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BoPhan> BoPhans { get; set; }
        public virtual DbSet<CapBac> CapBacs { get; set; }
        public virtual DbSet<DanhMuc> DanhMucs { get; set; }
        public virtual DbSet<NguoiDung> NguoiDungs { get; set; }
        public virtual DbSet<NguoiDungTheoDanhMuc> NguoiDungTheoDanhMucs { get; set; }
        public virtual DbSet<NhansuBoPhanLienQuan> NhansuBoPhanLienQuans { get; set; }
        public virtual DbSet<NhansuDiaChi> NhansuDiaChis { get; set; }
        public virtual DbSet<NhansuKinhNghiemLamViec> NhansuKinhNghiemLamViecs { get; set; }
        public virtual DbSet<NhansuKyNangChuyenMon> NhansuKyNangChuyenMons { get; set; }
        public virtual DbSet<NhansuNguoiGioiThieu> NhansuNguoiGioiThieus { get; set; }
        public virtual DbSet<NhansuPhuongTienDiLai> NhansuPhuongTienDiLais { get; set; }
        public virtual DbSet<NhansuSucKhoe> NhansuSucKhoes { get; set; }
        public virtual DbSet<NhansuThamChieu> NhansuThamChieus { get; set; }
        public virtual DbSet<NhansuThongTinGiaDinh> NhansuThongTinGiaDinhs { get; set; }
        public virtual DbSet<NhansuThongTinNhanVien> NhansuThongTinNhanViens { get; set; }
        public virtual DbSet<NhansuVanBang> NhansuVanBangs { get; set; }
        public virtual DbSet<NhansuVisaHoChieu> NhansuVisaHoChieus { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleHasPermission> RoleHasPermissions { get; set; }
        public virtual DbSet<TuyendungDanhGiaUv> TuyendungDanhGiaUvs { get; set; }
        public virtual DbSet<TuyendungDiaChiUngVien> TuyendungDiaChiUngViens { get; set; }
        public virtual DbSet<TuyendungKetQuaDanhGium> TuyendungKetQuaDanhGia { get; set; }
        public virtual DbSet<TuyendungKhoaHuanLuyenUv> TuyendungKhoaHuanLuyenUvs { get; set; }
        public virtual DbSet<TuyendungKinhNghiemLamViecUv> TuyendungKinhNghiemLamViecUvs { get; set; }
        public virtual DbSet<TuyendungKyNangVpuv> TuyendungKyNangVpuvs { get; set; }
        public virtual DbSet<TuyendungNgoaiNguUv> TuyendungNgoaiNguUvs { get; set; }
        public virtual DbSet<TuyendungPheDuyetUngVien> TuyendungPheDuyetUngViens { get; set; }
        public virtual DbSet<TuyendungPheDuyetYeuCauTd> TuyendungPheDuyetYeuCauTds { get; set; }
        public virtual DbSet<TuyendungThamChieuUv> TuyendungThamChieuUvs { get; set; }
        public virtual DbSet<TuyendungThongTinGiaDinhUv> TuyendungThongTinGiaDinhUvs { get; set; }
        public virtual DbSet<TuyendungThongTinKhacUv> TuyendungThongTinKhacUvs { get; set; }
        public virtual DbSet<TuyendungThongTinUngVien> TuyendungThongTinUngViens { get; set; }
        public virtual DbSet<TuyendungTieuChiDanhGium> TuyendungTieuChiDanhGia { get; set; }
        public virtual DbSet<TuyendungVanBangUv> TuyendungVanBangUvs { get; set; }
        public virtual DbSet<TuyendungYeuCauTuyenDung> TuyendungYeuCauTuyenDungs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserHasPermission> UserHasPermissions { get; set; }
        public virtual DbSet<UserHasRole> UserHasRoles { get; set; }
        public virtual DbSet<ViTriCongViec> ViTriCongViecs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-1H1LMDC\\MSSQLSERVER01;Database=TBSTuyenDung;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<BoPhan>(entity =>
            {
                entity.ToTable("BoPhan");

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

            modelBuilder.Entity<CapBac>(entity =>
            {
                entity.ToTable("CapBac");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MaCapBac)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.TenCapBac)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<DanhMuc>(entity =>
            {
                entity.ToTable("DanhMuc");

                entity.HasIndex(e => e.IdnguoiDung, "IX_DanhMuc_IDNguoiDung");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_date");

                entity.Property(e => e.Icon)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");

                entity.Property(e => e.TenDanhMuc)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_date");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("URL");

                entity.HasOne(d => d.IdnguoiDungNavigation)
                    .WithMany(p => p.DanhMucs)
                    .HasForeignKey(d => d.IdnguoiDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DanhMuc_NguoiDung");
            });

            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.ToTable("NguoiDung");

                entity.Property(e => e.Id).HasColumnName("ID");

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

                entity.Property(e => e.MaCapBac)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.MaNhanVien)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.NguoiTao)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Time");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<NguoiDungTheoDanhMuc>(entity =>
            {
                entity.ToTable("NguoiDungTheoDanhMuc");

                entity.HasIndex(e => e.IddanhMuc, "IX_NguoiDungTheoDanhMuc_DanhMucId");

                entity.HasIndex(e => e.IdnguoiDung, "IX_NguoiDungTheoDanhMuc_NguoiDungId");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IddanhMuc).HasColumnName("IDDanhMuc");

                entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");

                entity.HasOne(d => d.IddanhMucNavigation)
                    .WithMany(p => p.NguoiDungTheoDanhMucs)
                    .HasForeignKey(d => d.IddanhMuc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NguoiDungTheoDanhMuc_DanhMuc");

                entity.HasOne(d => d.IdnguoiDungNavigation)
                    .WithMany(p => p.NguoiDungTheoDanhMucs)
                    .HasForeignKey(d => d.IdnguoiDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NguoiDungTheoDanhMuc_NguoiDung");
            });

            modelBuilder.Entity<NhansuBoPhanLienQuan>(entity =>
            {
                entity.HasKey(e => e.MaNhanVien);

                entity.ToTable("nhansu_BoPhanLienQuan");

                entity.Property(e => e.MaNhanVien).ValueGeneratedNever();

                entity.Property(e => e.CachTinhLuong)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaBoPhan)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaChucVu)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaViTriCongViec)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NoiLamViec)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TenBoPhan)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.TenChucVu)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.TenViTriCongViec)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.TinhChatCv)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TinhChatCV");

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithOne(p => p.NhansuBoPhanLienQuan)
                    .HasForeignKey<NhansuBoPhanLienQuan>(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nhansu_BoPhanLienQuan_nhansu_ThongTinNhanVien");
            });

            modelBuilder.Entity<NhansuDiaChi>(entity =>
            {
                entity.ToTable("nhansu_DiaChi");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DiaChi)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PhanLoaiCho)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PhanLoaiDiaChi)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.QuanHuyen)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.TinhThanhPho)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.XaPhuong)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.NhansuDiaChis)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nhansu_DiaChi_nhansu_ThongTinNhanVien");
            });

            modelBuilder.Entity<NhansuKinhNghiemLamViec>(entity =>
            {
                entity.ToTable("nhansu_KinhNghiemLamViec");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ChucVu)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DiaChiCongTy)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MucLuongCuoi)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenCongTy)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ThoiGianCongTac)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.NhansuKinhNghiemLamViecs)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nhansu_KinhNghiemLamViec_nhansu_ThongTinNhanVien");
            });

            modelBuilder.Entity<NhansuKyNangChuyenMon>(entity =>
            {
                entity.ToTable("nhansu_KyNangChuyenMon");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DonViHuanLuyen)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.NoiDung).HasMaxLength(100);

                entity.Property(e => e.TenKyNang)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.XepLoai)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.NhansuKyNangChuyenMons)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nhansu_TayNghe_nhansu_ThongTinNhanVien");
            });

            modelBuilder.Entity<NhansuNguoiGioiThieu>(entity =>
            {
                entity.ToTable("nhansu_NguoiGioiThieu");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.QuanHe)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SoThe)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenNguoiGioiThieu)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.NhansuNguoiGioiThieus)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nhansu_NguoiGioiThieu_nhansu_ThongTinNhanVien");
            });

            modelBuilder.Entity<NhansuPhuongTienDiLai>(entity =>
            {
                entity.HasKey(e => e.MaNhanVien)
                    .HasName("PK__nhansu_P__3214EC270AB52971");

                entity.ToTable("nhansu_PhuongTienDiLai");

                entity.Property(e => e.MaNhanVien).ValueGeneratedNever();

                entity.Property(e => e.BienSo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LoaiPhuongTien)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenTuyenXe).HasMaxLength(20);

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithOne(p => p.NhansuPhuongTienDiLai)
                    .HasForeignKey<NhansuPhuongTienDiLai>(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nhansu_PhuongTienDiLai_nhansu_ThongTinNhanVien");
            });

            modelBuilder.Entity<NhansuSucKhoe>(entity =>
            {
                entity.HasKey(e => e.MaNhanVien)
                    .HasName("PK__nhansu_S__3214EC27E527CED3");

                entity.ToTable("nhansu_SucKhoe");

                entity.Property(e => e.MaNhanVien).ValueGeneratedNever();

                entity.Property(e => e.GhiChu).HasMaxLength(100);

                entity.Property(e => e.MaBenhVien)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayKhamSucKhoe)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenBenhVien)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.TheTrang)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithOne(p => p.NhansuSucKhoe)
                    .HasForeignKey<NhansuSucKhoe>(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nhansu_SucKhoe_nhansu_ThongTinNhanVien");
            });

            modelBuilder.Entity<NhansuThamChieu>(entity =>
            {
                entity.ToTable("nhansu_ThamChieu");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ChucVu)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DiaChiCongTy)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.DienThoai)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NguoiLienHe)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenCongTy)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.NhansuThamChieus)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nhansu_ThamChieu_nhansu_ThongTinNhanVien");
            });

            modelBuilder.Entity<NhansuThongTinGiaDinh>(entity =>
            {
                entity.ToTable("nhansu_ThongTinGiaDinh");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DiaChi)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.HoVaTen)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.NamSinh)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgheNghiep)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.QuanHe)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.NhansuThongTinGiaDinhs)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nhansu_ThongTinGiaDinh_nhansu_ThongTinNhanVien");
            });

            modelBuilder.Entity<NhansuThongTinNhanVien>(entity =>
            {
                entity.ToTable("nhansu_ThongTinNhanVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Cccd)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CCCD");

                entity.Property(e => e.DanToc)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmailCongTy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GioiTinh)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.HinhThucPhongVan)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Hinhanh)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.HoVaTen)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");

                entity.Property(e => e.MaSoThue)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.NgayBatDauLam)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayCapCccd)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("NgayCapCCCD");

                entity.Property(e => e.NgayKyHopDong1Nam)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayKyHopDong3Nam)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayKyHopDongVoThoiHan)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayPhongVan)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgaySinh)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayVaoCongDoan)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayVaoDang)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayVaoDoan)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayVaoLam)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NguoiTiepNhan)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NoiCap)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.NoiSinh)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.QuocTich)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Sdt)
                    .IsRequired()
                    .HasMaxLength(13)
                    .IsUnicode(false)
                    .HasColumnName("SDT");

                entity.Property(e => e.SoAnSinhXaHoi)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SoTaiKhoanNh)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SoTaiKhoanNH");

                entity.Property(e => e.TenNganHang)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ThoiGianNghi)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TinhTrangHonNhan)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TonGiao)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.TrinhDoHocVan)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<NhansuVanBang>(entity =>
            {
                entity.ToTable("nhansu_VanBang");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ChuyenNghanh).HasMaxLength(50);

                entity.Property(e => e.LoaiBangCap).HasMaxLength(50);

                entity.Property(e => e.MaBangCap)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgonNgu).HasMaxLength(30);

                entity.Property(e => e.NoiDaoTao).HasMaxLength(50);

                entity.Property(e => e.PhanLoai)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenBangCap)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.ThoiGianTotNghiep)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.XepLoai).HasMaxLength(50);

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.NhansuVanBangs)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nhansu_VanBang_nhansu_ThongTinNhanVien");
            });

            modelBuilder.Entity<NhansuVisaHoChieu>(entity =>
            {
                entity.ToTable("nhansu_VisaHoChieu");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MaSo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayHetHan)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayLam)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PhanLoai)
                    .IsRequired()
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.NhansuVisaHoChieus)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nhansu_VisaHoChieu_nhansu_ThongTinNhanVien");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission");

                entity.HasIndex(e => e.MId, "IX_Permission")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedTime).HasColumnName("Created_Time");

                entity.Property(e => e.MId).HasColumnName("mID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PearentId).HasColumnName("PearentID");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedTime).HasColumnName("Updated_Time");

                entity.HasOne(d => d.Pearent)
                    .WithMany(p => p.InversePearent)
                    .HasPrincipalKey(p => p.MId)
                    .HasForeignKey(d => d.PearentId)
                    .HasConstraintName("FK_Permission_Permission");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedTime).HasColumnName("Created_Time");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedTime).HasColumnName("Updated_Time");
            });

            modelBuilder.Entity<RoleHasPermission>(entity =>
            {
                entity.ToTable("Role_Has_Permission");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedTime).HasColumnName("Created_Time");

                entity.Property(e => e.PermissionId).HasColumnName("PermissionID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RoleHasPermissions)
                    .HasPrincipalKey(p => p.MId)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Role_Has_Permission_Permission1");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleHasPermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Role_Has_Permission_Role");
            });

            modelBuilder.Entity<TuyendungDanhGiaUv>(entity =>
            {
                entity.ToTable("tuyendung_DanhGiaUV");

                entity.HasIndex(e => e.IdnguoiDung, "IX_DanhGiaUV_IDNguoiDung");

                entity.HasIndex(e => e.IdungVien, "IX_DanhGiaUV_IDUngVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DeXuatHl)
                    .HasMaxLength(250)
                    .HasColumnName("DeXuatHL");

                entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");

                entity.Property(e => e.IdungVien).HasColumnName("IDUngVien");

                entity.Property(e => e.LyDoDeNghi).HasMaxLength(250);

                entity.Property(e => e.NhanXetThemKndp)
                    .HasMaxLength(250)
                    .HasColumnName("NhanXetThemKNDP");

                entity.Property(e => e.NhanXetThemTph)
                    .HasMaxLength(250)
                    .HasColumnName("NhanXetThemTPH");

                entity.Property(e => e.ThoiGian).HasColumnType("datetime");

                entity.HasOne(d => d.IdnguoiDungNavigation)
                    .WithMany(p => p.TuyendungDanhGiaUvs)
                    .HasForeignKey(d => d.IdnguoiDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DanhGiaUV_NguoiDung");

                entity.HasOne(d => d.IdungVienNavigation)
                    .WithMany(p => p.TuyendungDanhGiaUvs)
                    .HasForeignKey(d => d.IdungVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DanhGiaUV_ThongTinUngVien");
            });

            modelBuilder.Entity<TuyendungDiaChiUngVien>(entity =>
            {
                entity.ToTable("tuyendung_DiaChiUngVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DiaChi).HasMaxLength(100);

                entity.Property(e => e.IdungVien).HasColumnName("IDUngVien");

                entity.Property(e => e.QuanHuyen).HasMaxLength(20);

                entity.Property(e => e.TinhThanhPho).HasMaxLength(20);

                entity.Property(e => e.XaPhuong).HasMaxLength(20);

                entity.HasOne(d => d.IdungVienNavigation)
                    .WithMany(p => p.TuyendungDiaChiUngViens)
                    .HasForeignKey(d => d.IdungVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiaChiUngVien_ThongTinUngVien");
            });

            modelBuilder.Entity<TuyendungKetQuaDanhGium>(entity =>
            {
                entity.ToTable("tuyendung_KetQuaDanhGia");

                entity.HasIndex(e => e.IddanhGia, "IX_KetQuaDanhGia_IDDanhGia");

                entity.HasIndex(e => e.Idtcdg, "IX_KetQuaDanhGia_IDTCDG");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DiemDg).HasColumnName("DiemDG");

                entity.Property(e => e.GhiChu).HasMaxLength(250);

                entity.Property(e => e.IddanhGia).HasColumnName("IDDanhGia");

                entity.Property(e => e.Idtcdg).HasColumnName("IDTCDG");

                entity.HasOne(d => d.IddanhGiaNavigation)
                    .WithMany(p => p.TuyendungKetQuaDanhGia)
                    .HasForeignKey(d => d.IddanhGia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KetQuaDanhGia_DanhGiaUV");

                entity.HasOne(d => d.IdtcdgNavigation)
                    .WithMany(p => p.TuyendungKetQuaDanhGia)
                    .HasForeignKey(d => d.Idtcdg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KetQuaDanhGia_TieuChiDanhGia");
            });

            modelBuilder.Entity<TuyendungKhoaHuanLuyenUv>(entity =>
            {
                entity.ToTable("tuyendung_KhoaHuanLuyenUV");

                entity.HasIndex(e => e.IdungVien, "IX_KhoaHuanLuyenUV_IDUngVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ChungChi).HasMaxLength(50);

                entity.Property(e => e.DonViToChuc).HasMaxLength(50);

                entity.Property(e => e.IdungVien).HasColumnName("IDUngVien");

                entity.Property(e => e.TenKhoaHoc).HasMaxLength(50);

                entity.Property(e => e.ThoiGianHoc).HasMaxLength(50);

                entity.HasOne(d => d.IdungVienNavigation)
                    .WithMany(p => p.TuyendungKhoaHuanLuyenUvs)
                    .HasForeignKey(d => d.IdungVien)
                    .HasConstraintName("FK_KhoaHuanLuyenUV_ThongTinUngVien");
            });

            modelBuilder.Entity<TuyendungKinhNghiemLamViecUv>(entity =>
            {
                entity.ToTable("tuyendung_KinhNghiemLamViecUV");

                entity.HasIndex(e => e.IdungVien, "IX_KinhNghiemLamViecUV_IDUngVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ChucVu).HasMaxLength(50);

                entity.Property(e => e.ChucVuCapTren).HasMaxLength(50);

                entity.Property(e => e.DiaChiCongTy).HasMaxLength(50);

                entity.Property(e => e.HoTenCapTrenTrucTiep).HasMaxLength(50);

                entity.Property(e => e.IdungVien).HasColumnName("IDUngVien");

                entity.Property(e => e.LyDoNghiViec).HasMaxLength(50);

                entity.Property(e => e.MucLuongCuoi).HasMaxLength(50);

                entity.Property(e => e.NghanhNgheKd)
                    .HasMaxLength(50)
                    .HasColumnName("NghanhNgheKD");

                entity.Property(e => e.TenCongTy).HasMaxLength(50);

                entity.Property(e => e.ThoiGianCongTac).HasMaxLength(50);

                entity.HasOne(d => d.IdungVienNavigation)
                    .WithMany(p => p.TuyendungKinhNghiemLamViecUvs)
                    .HasForeignKey(d => d.IdungVien)
                    .HasConstraintName("FK_KinhNghiemLamViecUV_ThongTinUngVien");
            });

            modelBuilder.Entity<TuyendungKyNangVpuv>(entity =>
            {
                entity.ToTable("tuyendung_KyNangVPUV");

                entity.HasIndex(e => e.IdungVien, "IX_KyNangVPUV_IDUngVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ChuongTrinh).HasMaxLength(50);

                entity.Property(e => e.IdungVien).HasColumnName("IDUngVien");

                entity.Property(e => e.TbsdanhGia)
                    .HasMaxLength(50)
                    .HasColumnName("TBSDanhGia");

                entity.Property(e => e.VanBang).HasMaxLength(50);

                entity.Property(e => e.XepLoai).HasMaxLength(50);

                entity.HasOne(d => d.IdungVienNavigation)
                    .WithMany(p => p.TuyendungKyNangVpuvs)
                    .HasForeignKey(d => d.IdungVien)
                    .HasConstraintName("FK_KyNangVPUV_ThongTinUngVien");
            });

            modelBuilder.Entity<TuyendungNgoaiNguUv>(entity =>
            {
                entity.ToTable("tuyendung_NgoaiNguUV");

                entity.HasIndex(e => e.IdungVien, "IX_NgoaiNguUV_IDUngVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdungVien).HasColumnName("IDUngVien");

                entity.Property(e => e.NgonNgu).HasMaxLength(50);

                entity.Property(e => e.TbsdanhGia)
                    .HasMaxLength(50)
                    .HasColumnName("TBSDanhGia");

                entity.Property(e => e.VanBang).HasMaxLength(50);

                entity.Property(e => e.XepLoai).HasMaxLength(50);

                entity.HasOne(d => d.IdungVienNavigation)
                    .WithMany(p => p.TuyendungNgoaiNguUvs)
                    .HasForeignKey(d => d.IdungVien)
                    .HasConstraintName("FK_NgoaiNguUV_ThongTinUngVien");
            });

            modelBuilder.Entity<TuyendungPheDuyetUngVien>(entity =>
            {
                entity.ToTable("tuyendung_PheDuyetUngVien");

                entity.HasIndex(e => e.IdnguoiDung, "IX_PheDuyetUngVien_IDNguoiDung");

                entity.HasIndex(e => e.IdungVien, "IX_PheDuyetUngVien_IDUngVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");

                entity.Property(e => e.IdungVien).HasColumnName("IDUngVien");

                entity.Property(e => e.LuongPheDuyet).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LyDo)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.IdnguoiDungNavigation)
                    .WithMany(p => p.TuyendungPheDuyetUngViens)
                    .HasForeignKey(d => d.IdnguoiDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PheDuyetUngVien_NguoiDung");

                entity.HasOne(d => d.IdungVienNavigation)
                    .WithMany(p => p.TuyendungPheDuyetUngViens)
                    .HasForeignKey(d => d.IdungVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PheDuyetUngVien_ThongTinUngVien");
            });

            modelBuilder.Entity<TuyendungPheDuyetYeuCauTd>(entity =>
            {
                entity.ToTable("tuyendung_PheDuyetYeuCauTD");

                entity.HasIndex(e => e.IdnguoiDung, "IX_PheDuyet_NguoiDungId");

                entity.HasIndex(e => e.YeuCauId, "IX_PheDuyet_YeuCauId");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");

                entity.Property(e => e.LyDo).IsRequired();

                entity.HasOne(d => d.IdnguoiDungNavigation)
                    .WithMany(p => p.TuyendungPheDuyetYeuCauTds)
                    .HasForeignKey(d => d.IdnguoiDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PheDuyet_NguoiDung");

                entity.HasOne(d => d.YeuCau)
                    .WithMany(p => p.TuyendungPheDuyetYeuCauTds)
                    .HasForeignKey(d => d.YeuCauId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PheDuyet_YeuCauTuyenDung");
            });

            modelBuilder.Entity<TuyendungThamChieuUv>(entity =>
            {
                entity.ToTable("tuyendung_ThamChieuUV");

                entity.HasIndex(e => e.IdungVien, "IX_ThamChieuUV_IDUngVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ChucVu).HasMaxLength(50);

                entity.Property(e => e.DiaChiCongTy).HasMaxLength(250);

                entity.Property(e => e.DienThoai)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdungVien).HasColumnName("IDUngVien");

                entity.Property(e => e.NguoiLienHe).HasMaxLength(50);

                entity.Property(e => e.TenCongTy).HasMaxLength(100);

                entity.HasOne(d => d.IdungVienNavigation)
                    .WithMany(p => p.TuyendungThamChieuUvs)
                    .HasForeignKey(d => d.IdungVien)
                    .HasConstraintName("FK_ThamChieuUV_ThongTinUngVien");
            });

            modelBuilder.Entity<TuyendungThongTinGiaDinhUv>(entity =>
            {
                entity.ToTable("tuyendung_ThongTinGiaDinhUV");

                entity.HasIndex(e => e.IdungVien, "IX_ThongTinGiaDinhUV_IDUngVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DiaChi).HasMaxLength(255);

                entity.Property(e => e.HoVaTen).HasMaxLength(30);

                entity.Property(e => e.IdungVien).HasColumnName("IDUngVien");

                entity.Property(e => e.NamSinh)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgheNghiep).HasMaxLength(50);

                entity.Property(e => e.QuanHe).HasMaxLength(20);

                entity.HasOne(d => d.IdungVienNavigation)
                    .WithMany(p => p.TuyendungThongTinGiaDinhUvs)
                    .HasForeignKey(d => d.IdungVien)
                    .HasConstraintName("FK_ThongTinGiaDinhUV_ThongTinUngVien");
            });

            modelBuilder.Entity<TuyendungThongTinKhacUv>(entity =>
            {
                entity.ToTable("tuyendung_ThongTinKhacUV");

                entity.HasIndex(e => e.IdungVien, "IX_ThongTinKhacUV_IDUngVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BietThongTinTuyenDungQua).HasMaxLength(50);

                entity.Property(e => e.DaLamTaiTbs).HasColumnName("DaLamTaiTBS");

                entity.Property(e => e.HoatDongUaThich).HasMaxLength(100);

                entity.Property(e => e.IdungVien).HasColumnName("IDUngVien");

                entity.Property(e => e.LyDoNopDonVaoTbs)
                    .HasMaxLength(50)
                    .HasColumnName("LyDoNopDonVaoTBS");

                entity.Property(e => e.MonTheThaoUaThich).HasMaxLength(100);

                entity.Property(e => e.MucDoLuyenTap).HasMaxLength(50);

                entity.Property(e => e.QuanHe1).HasMaxLength(50);

                entity.Property(e => e.QuanHe2).HasMaxLength(50);

                entity.Property(e => e.QuanHe3).HasMaxLength(50);

                entity.Property(e => e.SdtnguoiThan1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SDTNguoiThan1");

                entity.Property(e => e.SdtnguoiThan2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SDTNguoiThan2");

                entity.Property(e => e.SdtnguoiThan3)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SDTNguoiThan3");

                entity.Property(e => e.ThongTinNguoiQuenCongTy).HasMaxLength(100);

                entity.Property(e => e.ThongTinNguoiQuenCongTyDoiThu).HasMaxLength(100);

                entity.Property(e => e.ThongTinViTriCu).HasMaxLength(100);

                entity.HasOne(d => d.IdungVienNavigation)
                    .WithMany(p => p.TuyendungThongTinKhacUvs)
                    .HasForeignKey(d => d.IdungVien)
                    .HasConstraintName("FK_ThongTinKhacUV_ThongTinUngVien");
            });

            modelBuilder.Entity<TuyendungThongTinUngVien>(entity =>
            {
                entity.ToTable("tuyendung_ThongTinUngVien");

                entity.HasIndex(e => e.IdnguoiDung, "IX_ThongTinUngVien_IDNguoiDung");

                entity.HasIndex(e => e.IdviTriTuyenDung, "IX_ThongTinUngVien_IDViTriTuyenDung");

                entity.HasIndex(e => e.IdyeuCau, "IX_ThongTinUngVien_IDYeuCau");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Cccd)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CCCD");

                entity.Property(e => e.Cv)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CV");

                entity.Property(e => e.DanToc)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GioiTinh)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.HinhThucPhongVan)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Hinhanh)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.HoVaTen)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");

                entity.Property(e => e.IdviTriTuyenDung).HasColumnName("IDViTriTuyenDung");

                entity.Property(e => e.IdyeuCau).HasColumnName("IDYeuCau");

                entity.Property(e => e.LuongYeuCau).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NgayBatDauLam)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayCapCccd)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("NgayCapCCCD");

                entity.Property(e => e.NgayHenPhongVan)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgaySinh)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NguoiTiepNhan)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NoiCap)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.NoiSinh)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.QuocTich)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Sdt)
                    .IsRequired()
                    .HasMaxLength(13)
                    .IsUnicode(false)
                    .HasColumnName("SDT");

                entity.Property(e => e.ThoiGian).HasColumnType("datetime");

                entity.Property(e => e.TinhTrangHonNhan)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TonGiao)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.TrinhDoHocVan)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdnguoiDungNavigation)
                    .WithMany(p => p.TuyendungThongTinUngViens)
                    .HasForeignKey(d => d.IdnguoiDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTinUngVien_NguoiDung");

                entity.HasOne(d => d.IdviTriTuyenDungNavigation)
                    .WithMany(p => p.TuyendungThongTinUngViens)
                    .HasForeignKey(d => d.IdviTriTuyenDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTinUngVien_ViTriTuyenDung");

                entity.HasOne(d => d.IdyeuCauNavigation)
                    .WithMany(p => p.TuyendungThongTinUngViens)
                    .HasForeignKey(d => d.IdyeuCau)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTinUngVien_YeuCauTuyenDung");
            });

            modelBuilder.Entity<TuyendungTieuChiDanhGium>(entity =>
            {
                entity.ToTable("tuyendung_TieuChiDanhGia");

                entity.HasIndex(e => e.IdnguoiDung, "IX_TieuChiDanhGia_IDNguoiDung");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.GhiChuTcdg)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("GhiChuTCDG");

                entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");

                entity.Property(e => e.Stt).HasColumnName("STT");

                entity.Property(e => e.TenTcdg)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("TenTCDG");

                entity.Property(e => e.ThoiGian).HasColumnType("datetime");

                entity.HasOne(d => d.IdnguoiDungNavigation)
                    .WithMany(p => p.TuyendungTieuChiDanhGia)
                    .HasForeignKey(d => d.IdnguoiDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TieuChiDanhGia_NguoiDung");
            });

            modelBuilder.Entity<TuyendungVanBangUv>(entity =>
            {
                entity.ToTable("tuyendung_VanBangUV");

                entity.HasIndex(e => e.IdungVien, "IX_VanBangUV_IDUngVien");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BangCap).HasMaxLength(50);

                entity.Property(e => e.ChuyenNghanh).HasMaxLength(100);

                entity.Property(e => e.IdungVien).HasColumnName("IDUngVien");

                entity.Property(e => e.NamTotNghiep).HasMaxLength(50);

                entity.Property(e => e.TenTruong).HasMaxLength(50);

                entity.Property(e => e.XepLoai).HasMaxLength(50);

                entity.HasOne(d => d.IdungVienNavigation)
                    .WithMany(p => p.TuyendungVanBangUvs)
                    .HasForeignKey(d => d.IdungVien)
                    .HasConstraintName("FK_VanBangUV_ThongTinUngVien");
            });

            modelBuilder.Entity<TuyendungYeuCauTuyenDung>(entity =>
            {
                entity.ToTable("tuyendung_YeuCauTuyenDung");

                entity.HasIndex(e => e.IdviTriTuyenDung, "IX_YeuCauTuyenDung_IDViTriTuyenDung");

                entity.HasIndex(e => e.IdnguoiDung, "IX_YeuCauTuyenDung_NguoiDungId");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_time");

                entity.Property(e => e.GhiChu).HasMaxLength(500);

                entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");

                entity.Property(e => e.IdviTriTuyenDung).HasColumnName("IDViTriTuyenDung");

                entity.Property(e => e.LoaiNguyenNhanTd)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("LoaiNguyenNhanTD");

                entity.Property(e => e.LyDoTuyen).IsRequired();

                entity.Property(e => e.MaBoPhan)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.NguoiDeXuat)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.ThoiGianBatDauTd).HasColumnName("ThoiGianBatDauTD");

                entity.Property(e => e.ThoiGianKetThucTd).HasColumnName("ThoiGianKetThucTD");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_time");

                entity.Property(e => e.YeuCauChuyenMon).IsRequired();

                entity.HasOne(d => d.IdnguoiDungNavigation)
                    .WithMany(p => p.TuyendungYeuCauTuyenDungs)
                    .HasForeignKey(d => d.IdnguoiDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_YeuCauTuyenDung_NguoiDung1");

                entity.HasOne(d => d.IdviTriTuyenDungNavigation)
                    .WithMany(p => p.TuyendungYeuCauTuyenDungs)
                    .HasForeignKey(d => d.IdviTriTuyenDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_YeuCauTuyenDung_ViTriTuyenDung");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.PassWord)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.User)
                    .HasForeignKey<User>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_NguoiDung");
            });

            modelBuilder.Entity<UserHasPermission>(entity =>
            {
                entity.ToTable("User_Has_Permission");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedTime).HasColumnName("Created_time");

                entity.Property(e => e.PermissionId).HasColumnName("PermissionID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.UserHasPermissions)
                    .HasPrincipalKey(p => p.MId)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Has_Permission_Permission1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserHasPermissions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Has_Permission_User");
            });

            modelBuilder.Entity<UserHasRole>(entity =>
            {
                entity.ToTable("User_Has_Role");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedTime).HasColumnName("Created_Time");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserHasRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Has_Role_Role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserHasRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Has_Role_User");
            });

            modelBuilder.Entity<ViTriCongViec>(entity =>
            {
                entity.ToTable("ViTriCongViec");

                entity.HasIndex(e => e.IdnguoiDung, "IX_ViTriTuyenDung_IDNguoiDung");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Time");

                entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");

                entity.Property(e => e.MaBoPhan)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MaCapBac)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.NoiLamViec)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.ThoiGianThongBaoKq).HasColumnName("ThoiGianThongBaoKQ");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Time");

                entity.Property(e => e.ViTriCongViec1)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("ViTriCongViec");

                entity.HasOne(d => d.IdnguoiDungNavigation)
                    .WithMany(p => p.ViTriCongViecs)
                    .HasForeignKey(d => d.IdnguoiDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ViTriTuyenDung_NguoiDung");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
