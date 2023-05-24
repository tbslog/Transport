using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

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
		public virtual DbSet<AccountOfCustomer> AccountOfCustomer { get; set; }
		public virtual DbSet<Attachment> Attachment { get; set; }
		public virtual DbSet<BangGia> BangGia { get; set; }
		public virtual DbSet<BangGiaDacBiet> BangGiaDacBiet { get; set; }
		public virtual DbSet<BangQuyDoi> BangQuyDoi { get; set; }
		public virtual DbSet<BoPhan> BoPhan { get; set; }
		public virtual DbSet<ChuoiKhachHang> ChuoiKhachHang { get; set; }
		public virtual DbSet<CungDuong> CungDuong { get; set; }
		public virtual DbSet<DiaDiem> DiaDiem { get; set; }
		public virtual DbSet<DieuPhoi> DieuPhoi { get; set; }
		public virtual DbSet<DonViTinh> DonViTinh { get; set; }
		public virtual DbSet<ExchangeRate> ExchangeRate { get; set; }
		public virtual DbSet<FieldOfFunction> FieldOfFunction { get; set; }
		public virtual DbSet<FunctionsOfSystems> FunctionsOfSystems { get; set; }
		public virtual DbSet<HopDongVaPhuLuc> HopDongVaPhuLuc { get; set; }
		public virtual DbSet<KhachHang> KhachHang { get; set; }
		public virtual DbSet<KhachHangAccount> KhachHangAccount { get; set; }
		public virtual DbSet<KhuVuc> KhuVuc { get; set; }
		public virtual DbSet<LoaiChungTu> LoaiChungTu { get; set; }
		public virtual DbSet<LoaiDiaDiem> LoaiDiaDiem { get; set; }
		public virtual DbSet<LoaiHangHoa> LoaiHangHoa { get; set; }
		public virtual DbSet<LoaiHinhKho> LoaiHinhKho { get; set; }
		public virtual DbSet<LoaiHopDong> LoaiHopDong { get; set; }
		public virtual DbSet<LoaiKhachHang> LoaiKhachHang { get; set; }
		public virtual DbSet<LoaiPhuongTien> LoaiPhuongTien { get; set; }
		public virtual DbSet<LoaiRomooc> LoaiRomooc { get; set; }
		public virtual DbSet<LoaiSpdv> LoaiSpdv { get; set; }
		public virtual DbSet<LoaiTienTe> LoaiTienTe { get; set; }
		public virtual DbSet<Log> Log { get; set; }
		public virtual DbSet<LogGps> LogGps { get; set; }
		public virtual DbSet<LogTimeUsedOfUsers> LogTimeUsedOfUsers { get; set; }
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
		public virtual DbSet<SftPayFor> SftPayFor { get; set; }
		public virtual DbSet<ShippingInfomation> ShippingInfomation { get; set; }
		public virtual DbSet<StatusText> StatusText { get; set; }
		public virtual DbSet<SubFee> SubFee { get; set; }
		public virtual DbSet<SubFeeByContract> SubFeeByContract { get; set; }
		public virtual DbSet<SubFeePrice> SubFeePrice { get; set; }
		public virtual DbSet<SubFeeType> SubFeeType { get; set; }
		public virtual DbSet<TaiXe> TaiXe { get; set; }
		public virtual DbSet<TaiXeTheoChang> TaiXeTheoChang { get; set; }
		public virtual DbSet<TepChungTu> TepChungTu { get; set; }
		public virtual DbSet<TepHopDong> TepHopDong { get; set; }
		public virtual DbSet<ThaoTacTaiXe> ThaoTacTaiXe { get; set; }
		public virtual DbSet<ThongBao> ThongBao { get; set; }
		public virtual DbSet<TinhThanh> TinhThanh { get; set; }
		public virtual DbSet<TrongTaiXe> TrongTaiXe { get; set; }
		public virtual DbSet<UserHasCustomer> UserHasCustomer { get; set; }
		public virtual DbSet<UserHasPermission> UserHasPermission { get; set; }
		public virtual DbSet<UserHasRole> UserHasRole { get; set; }
		public virtual DbSet<ValidateDataByCustomer> ValidateDataByCustomer { get; set; }
		public virtual DbSet<VanDon> VanDon { get; set; }
		public virtual DbSet<XaPhuong> XaPhuong { get; set; }
		public virtual DbSet<XeVanChuyen> XeVanChuyen { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .Build();

                optionsBuilder.UseSqlServer(configuration.GetConnectionString("TMS_Cloud"));
            }
        }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
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

			modelBuilder.Entity<AccountOfCustomer>(entity =>
			{
				entity.HasKey(e => e.MaAccount)
					.HasName("PK__AccountO__0A2B8E3462D97A7E");

				entity.Property(e => e.MaAccount)
					.HasMaxLength(8)
					.IsUnicode(false);

				entity.Property(e => e.Creator)
					.IsRequired()
					.HasMaxLength(30);

				entity.Property(e => e.TenAccount)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Updater).HasMaxLength(30);
			});

			modelBuilder.Entity<Attachment>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Creator).HasMaxLength(30);

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
					.HasMaxLength(30)
					.IsUnicode(false);

				entity.Property(e => e.Updater).HasMaxLength(30);
			});

			modelBuilder.Entity<BangGia>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Approver).HasMaxLength(30);

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");

				entity.Property(e => e.LoaiTienTe)
					.IsRequired()
					.HasMaxLength(5)
					.IsUnicode(false);

				entity.Property(e => e.MaAccount)
					.HasMaxLength(8)
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

				entity.Property(e => e.Updater).HasMaxLength(30);

				entity.HasOne(d => d.MaAccountNavigation)
					.WithMany(p => p.BangGia)
					.HasForeignKey(d => d.MaAccount)
					.HasConstraintName("fk_MaAccountBG_MaAccountCus");

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

			modelBuilder.Entity<ChuoiKhachHang>(entity =>
			{
				entity.HasNoKey();

				entity.Property(e => e.MaChuoi)
					.IsRequired()
					.HasMaxLength(10)
					.IsUnicode(false);

				entity.Property(e => e.TenChuoi)
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

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.GhiChu)
					.IsRequired()
					.HasMaxLength(500);

				entity.Property(e => e.TenCungDuong)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Updater).HasMaxLength(30);
			});

			modelBuilder.Entity<DiaDiem>(entity =>
			{
				entity.HasKey(e => e.MaDiaDiem)
					.HasName("PK_DanhMucDiaDiem");

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.DiaChiDayDu)
					.IsRequired()
					.HasMaxLength(250);

				entity.Property(e => e.LoaiDiaDiem)
					.HasMaxLength(10)
					.IsUnicode(false);

				entity.Property(e => e.MaGps)
					.HasMaxLength(50)
					.IsUnicode(false)
					.HasColumnName("MaGPS");

				entity.Property(e => e.NhomDiaDiem)
					.IsRequired()
					.HasMaxLength(50)
					.IsUnicode(false);

				entity.Property(e => e.SoNha).HasMaxLength(100);

				entity.Property(e => e.TenDiaDiem)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Updater).HasMaxLength(30);

				entity.HasOne(d => d.DiaDiemChaNavigation)
					.WithMany(p => p.InverseDiaDiemChaNavigation)
					.HasForeignKey(d => d.DiaDiemCha)
					.HasConstraintName("fk_DiaDiemCha_MaDiaDiem");

				entity.HasOne(d => d.MaHuyenNavigation)
					.WithMany(p => p.DiaDiem)
					.HasForeignKey(d => d.MaHuyen)
					.HasConstraintName("FK_DiaDiem_QuanHuyen");

				entity.HasOne(d => d.MaPhuongNavigation)
					.WithMany(p => p.DiaDiem)
					.HasForeignKey(d => d.MaPhuong)
					.HasConstraintName("FK_DiaDiem_XaPhuong");

				entity.HasOne(d => d.MaQuocGiaNavigation)
					.WithMany(p => p.DiaDiem)
					.HasForeignKey(d => d.MaQuocGia)
					.HasConstraintName("FK_DiaDiem_QuocGia");

				entity.HasOne(d => d.MaTinhNavigation)
					.WithMany(p => p.DiaDiem)
					.HasForeignKey(d => d.MaTinh)
					.HasConstraintName("FK_DiaDiem_TinhThanh");

				entity.HasOne(d => d.NhomDiaDiemNavigation)
					.WithMany(p => p.DiaDiem)
					.HasForeignKey(d => d.NhomDiaDiem)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_DanhMuc_DiaDiem_DanhMuc_LoaiDiaDiem");
			});

			modelBuilder.Entity<DieuPhoi>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.BangGiaKh).HasColumnName("BangGiaKH");

				entity.Property(e => e.BangGiaNcc).HasColumnName("BangGiaNCC");

				entity.Property(e => e.ContNo)
					.HasMaxLength(50)
					.IsUnicode(false)
					.HasColumnName("Cont_No");

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.DonGiaKh)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("DonGiaKH");

				entity.Property(e => e.DonGiaNcc)
					.HasColumnType("decimal(18, 0)")
					.HasColumnName("DonGiaNCC");

				entity.Property(e => e.DonViVanTai)
					.HasMaxLength(8)
					.IsUnicode(false);

				entity.Property(e => e.LoaiTienTeKh)
					.HasMaxLength(5)
					.IsUnicode(false)
					.HasColumnName("LoaiTienTeKH");

				entity.Property(e => e.LoaiTienTeNcc)
					.HasMaxLength(5)
					.IsUnicode(false)
					.HasColumnName("LoaiTienTeNCC");

				entity.Property(e => e.MaChuyen)
					.IsRequired()
					.HasMaxLength(21)
					.IsUnicode(false);

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

				entity.Property(e => e.MaRomooc)
					.HasMaxLength(50)
					.IsUnicode(false);

				entity.Property(e => e.MaSoXe)
					.HasMaxLength(10)
					.IsUnicode(false);

				entity.Property(e => e.MaTaiXe)
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

				entity.Property(e => e.Updater).HasMaxLength(30);

				entity.HasOne(d => d.BangGiaKhNavigation)
					.WithMany(p => p.DieuPhoiBangGiaKhNavigation)
					.HasForeignKey(d => d.BangGiaKh)
					.HasConstraintName("FK_DieuPhoi_BangGia");

				entity.HasOne(d => d.BangGiaNccNavigation)
					.WithMany(p => p.DieuPhoiBangGiaNccNavigation)
					.HasForeignKey(d => d.BangGiaNcc)
					.HasConstraintName("FK_DieuPhoi_BangGia1");

				entity.HasOne(d => d.DonViVanTaiNavigation)
					.WithMany(p => p.DieuPhoi)
					.HasForeignKey(d => d.DonViVanTai)
					.HasConstraintName("FK_DieuPhoi_KhachHang");

				entity.HasOne(d => d.MaRomoocNavigation)
					.WithMany(p => p.DieuPhoi)
					.HasForeignKey(d => d.MaRomooc)
					.HasConstraintName("FK_DieuPhoi_Romooc");

				entity.HasOne(d => d.MaSoXeNavigation)
					.WithMany(p => p.DieuPhoi)
					.HasForeignKey(d => d.MaSoXe)
					.HasConstraintName("FK_DieuPhoi_XeVanChuyen");

				entity.HasOne(d => d.MaTaiXeNavigation)
					.WithMany(p => p.DieuPhoi)
					.HasForeignKey(d => d.MaTaiXe)
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

			modelBuilder.Entity<ExchangeRate>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.CurrencyCode)
					.IsRequired()
					.HasMaxLength(5)
					.IsUnicode(false);

				entity.Property(e => e.CurrencyName)
					.IsRequired()
					.HasMaxLength(30);
			});

			modelBuilder.Entity<FieldOfFunction>(entity =>
			{
				entity.HasKey(e => e.FieldId)
					.HasName("PK__FieldOfF__C8B6FF27C47BFD81");

				entity.Property(e => e.FieldId)
					.HasMaxLength(10)
					.IsUnicode(false)
					.HasColumnName("FieldID");

				entity.Property(e => e.FieldName)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.FunctionId)
					.IsRequired()
					.HasMaxLength(10)
					.IsUnicode(false);

				entity.HasOne(d => d.Function)
					.WithMany(p => p.FieldOfFunction)
					.HasForeignKey(d => d.FunctionId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__FieldOfFu__Funct__28F7FFC9");
			});

			modelBuilder.Entity<FunctionsOfSystems>(entity =>
			{
				entity.HasKey(e => e.FunctionId)
					.HasName("PK__Function__31ABF918E9C2355D");

				entity.Property(e => e.FunctionId)
					.HasMaxLength(10)
					.IsUnicode(false)
					.HasColumnName("FunctionID");

				entity.Property(e => e.FunctionName)
					.IsRequired()
					.HasMaxLength(50);
			});

			modelBuilder.Entity<HopDongVaPhuLuc>(entity =>
			{
				entity.HasKey(e => e.MaHopDong)
					.HasName("PK_ThongTin_HopDongVaPhuLuc");

				entity.Property(e => e.MaHopDong)
					.HasMaxLength(20)
					.IsUnicode(false);

				entity.Property(e => e.Account).HasMaxLength(50);

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.GhiChu).HasMaxLength(500);

				entity.Property(e => e.HinhThucThue)
					.HasMaxLength(50)
					.IsUnicode(false);

				entity.Property(e => e.LoaiHinhHopTac)
					.HasMaxLength(50)
					.IsUnicode(false);

				entity.Property(e => e.MaHopDongCha)
					.HasMaxLength(20)
					.IsUnicode(false);

				entity.Property(e => e.MaKh)
					.IsRequired()
					.HasMaxLength(8)
					.IsUnicode(false)
					.HasColumnName("MaKH");

				entity.Property(e => e.MaLoaiHinh)
					.HasMaxLength(50)
					.IsUnicode(false);

				entity.Property(e => e.MaLoaiHopDong)
					.IsRequired()
					.HasMaxLength(10)
					.IsUnicode(false);

				entity.Property(e => e.MaLoaiSpdv)
					.HasMaxLength(50)
					.IsUnicode(false)
					.HasColumnName("MaLoaiSPDV");

				entity.Property(e => e.TenHienThi)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.ThoiGianBatDau).HasColumnType("date");

				entity.Property(e => e.ThoiGianKetThuc).HasColumnType("date");

				entity.Property(e => e.Updater).HasMaxLength(30);

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

				entity.Property(e => e.Chuoi).HasMaxLength(50);

				entity.Property(e => e.Creator).HasMaxLength(30);

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

				entity.Property(e => e.Updater).HasMaxLength(30);
			});

			modelBuilder.Entity<KhachHangAccount>(entity =>
			{
				entity.ToTable("KhachHang_Account");

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Creator)
					.IsRequired()
					.HasMaxLength(30);

				entity.Property(e => e.MaAccount)
					.IsRequired()
					.HasMaxLength(8)
					.IsUnicode(false);

				entity.Property(e => e.MaKh)
					.IsRequired()
					.HasMaxLength(8)
					.IsUnicode(false)
					.HasColumnName("MaKH");

				entity.HasOne(d => d.MaAccountNavigation)
					.WithMany(p => p.KhachHangAccount)
					.HasForeignKey(d => d.MaAccount)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__KhachHang__MaAcc__4999D985");

				entity.HasOne(d => d.MaKhNavigation)
					.WithMany(p => p.KhachHangAccount)
					.HasForeignKey(d => d.MaKh)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__KhachHang___MaKH__48A5B54C");
			});

			modelBuilder.Entity<KhuVuc>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.TenKhuVuc)
					.IsRequired()
					.HasMaxLength(50);
			});

			modelBuilder.Entity<LoaiChungTu>(entity =>
			{
				entity.HasKey(e => e.MaLoaiChungTu)
					.HasName("PK__LoaiChun__AC699872F2D68137");

				entity.Property(e => e.MaLoaiDiaDiem)
					.IsRequired()
					.HasMaxLength(50)
					.IsUnicode(false);

				entity.Property(e => e.TenLoaiChungTu)
					.IsRequired()
					.HasMaxLength(150);
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

			modelBuilder.Entity<LoaiHinhKho>(entity =>
			{
				entity.HasKey(e => e.MaLoaiHinh);

				entity.Property(e => e.MaLoaiHinh)
					.HasMaxLength(50)
					.IsUnicode(false);

				entity.Property(e => e.TenLoaiHinh)
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

			modelBuilder.Entity<LoaiSpdv>(entity =>
			{
				entity.HasKey(e => e.MaLoaiSpdv);

				entity.ToTable("LoaiSPDV");

				entity.Property(e => e.MaLoaiSpdv)
					.HasMaxLength(50)
					.IsUnicode(false)
					.HasColumnName("MaLoaiSPDV");

				entity.Property(e => e.TenLoaiSpdv)
					.IsRequired()
					.HasMaxLength(50)
					.HasColumnName("TenLoaiSPDV");
			});

			modelBuilder.Entity<LoaiTienTe>(entity =>
			{
				entity.HasNoKey();

				entity.Property(e => e.MaLoaiTienTe)
					.IsRequired()
					.HasMaxLength(5)
					.IsUnicode(false);

				entity.Property(e => e.TenLoaiTienTe)
					.IsRequired()
					.HasMaxLength(30);
			});

			modelBuilder.Entity<Log>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Message)
					.IsRequired()
					.HasColumnType("text");

				entity.Property(e => e.ModuleName)
					.IsRequired()
					.HasMaxLength(200)
					.IsUnicode(false);
			});

			modelBuilder.Entity<LogGps>(entity =>
			{
				entity.ToTable("LogGPS");

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.MaGps)
					.HasMaxLength(250)
					.HasColumnName("MaGPS");

				entity.Property(e => e.TrangThaiDp).HasColumnName("TrangThaiDP");

				entity.HasOne(d => d.DiemCuoiNavigation)
					.WithMany(p => p.LogGpsDiemCuoiNavigation)
					.HasForeignKey(d => d.DiemCuoi)
					.HasConstraintName("FK__LogGPS__DiemCuoi__02D256E1");

				entity.HasOne(d => d.DiemDauNavigation)
					.WithMany(p => p.LogGpsDiemDauNavigation)
					.HasForeignKey(d => d.DiemDau)
					.HasConstraintName("FK__LogGPS__DiemDau__01DE32A8");

				entity.HasOne(d => d.DiemLayRongNavigation)
					.WithMany(p => p.LogGpsDiemLayRongNavigation)
					.HasForeignKey(d => d.DiemLayRong)
					.HasConstraintName("FK__LogGPS__DiemLayR__7FF5EA36");

				entity.HasOne(d => d.DiemTraRongNavigation)
					.WithMany(p => p.LogGpsDiemTraRongNavigation)
					.HasForeignKey(d => d.DiemTraRong)
					.HasConstraintName("FK__LogGPS__DiemTraR__00EA0E6F");

				entity.HasOne(d => d.MaDieuPhoiNavigation)
					.WithMany(p => p.LogGps)
					.HasForeignKey(d => d.MaDieuPhoi)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__LogGPS__MaDieuPh__7F01C5FD");
			});

			modelBuilder.Entity<LogTimeUsedOfUsers>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Token).IsRequired();

				entity.Property(e => e.UserName)
					.IsRequired()
					.HasMaxLength(30);
			});

			modelBuilder.Entity<NguoiDung>(entity =>
			{
				entity.Property(e => e.Id)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.AccountType)
					.IsRequired()
					.HasMaxLength(10)
					.IsUnicode(false);

				entity.Property(e => e.CreatedTime)
					.HasColumnType("datetime")
					.HasColumnName("Created_Time");

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.HoVaTen)
					.IsRequired()
					.HasMaxLength(40)
					.HasDefaultValueSql("(N'')");

				entity.Property(e => e.MaBoPhan)
					.HasMaxLength(20)
					.IsUnicode(false);

				entity.Property(e => e.MaNhanVien)
					.HasMaxLength(10)
					.IsUnicode(false)
					.HasDefaultValueSql("('')");

				entity.Property(e => e.NguoiTao)
					.IsRequired()
					.HasMaxLength(30);

				entity.Property(e => e.UpdatedTime)
					.HasColumnType("datetime")
					.HasColumnName("Updated_Time");

				entity.Property(e => e.Updater).HasMaxLength(30);

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

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.RoleName)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Updater).HasMaxLength(30);
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

				entity.Property(e => e.Creator).HasMaxLength(30);

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

				entity.Property(e => e.Updater).HasMaxLength(30);

				entity.HasOne(d => d.MaLoaiRomoocNavigation)
					.WithMany(p => p.Romooc)
					.HasForeignKey(d => d.MaLoaiRomooc)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_ThongTin_Romooc_PhanLoai_Romooc");
			});

			modelBuilder.Entity<SfeeByTcommand>(entity =>
			{
				entity.ToTable("SFeeByTcommand");

				entity.Property(e => e.Approver).HasMaxLength(30);

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.IdTcommand).HasColumnName("IdTCommand");

				entity.Property(e => e.PayForId).HasColumnName("payForId");

				entity.Property(e => e.SfId).HasColumnName("sfID");

				entity.Property(e => e.Updater).HasMaxLength(30);

				entity.HasOne(d => d.IdTcommandNavigation)
					.WithMany(p => p.SfeeByTcommand)
					.HasForeignKey(d => d.IdTcommand)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_SFeeByTcommand_DieuPhoi");

				entity.HasOne(d => d.PayFor)
					.WithMany(p => p.SfeeByTcommand)
					.HasForeignKey(d => d.PayForId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__SFeeByTco__payFo__66010E09");

				entity.HasOne(d => d.Sf)
					.WithMany(p => p.SfeeByTcommand)
					.HasForeignKey(d => d.SfId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_SFeeByTcommand_SubFee");
			});

			modelBuilder.Entity<SftPayFor>(entity =>
			{
				entity.HasKey(e => e.PayForId)
					.HasName("PK__SftPayFo__8CBC50A18EC89AE2");

				entity.Property(e => e.PayForId).HasColumnName("payForId");

				entity.Property(e => e.PfDescription)
					.HasMaxLength(300)
					.HasColumnName("pfDescription");

				entity.Property(e => e.PfName)
					.IsRequired()
					.HasMaxLength(30)
					.HasColumnName("pfName");
			});

			modelBuilder.Entity<ShippingInfomation>(entity =>
			{
				entity.HasKey(e => e.ShippingCode)
					.HasName("PK_ShippingInfomation_1");

				entity.Property(e => e.ShippingCode)
					.HasMaxLength(20)
					.IsUnicode(false);

				entity.Property(e => e.ShippingLineName)
					.IsRequired()
					.HasMaxLength(100);
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

				entity.Property(e => e.Creator).HasMaxLength(30);

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

				entity.Property(e => e.Updater).HasMaxLength(30);

				entity.HasOne(d => d.SfTypeNavigation)
					.WithMany(p => p.SubFee)
					.HasForeignKey(d => d.SfType)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_SubFee_SubFeeType");
			});

			modelBuilder.Entity<SubFeeByContract>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.PriceId).HasColumnName("PriceID");

				entity.HasOne(d => d.MaDieuPhoiNavigation)
					.WithMany(p => p.SubFeeByContract)
					.HasForeignKey(d => d.MaDieuPhoi)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_SubFeeByContract_DieuPhoi");

				entity.HasOne(d => d.Price)
					.WithMany(p => p.SubFeeByContract)
					.HasForeignKey(d => d.PriceId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_SubFeeByContract_SubFeePrice");
			});

			modelBuilder.Entity<SubFeePrice>(entity =>
			{
				entity.HasKey(e => e.PriceId);

				entity.Property(e => e.PriceId).HasColumnName("priceID");

				entity.Property(e => e.AccountId)
					.HasMaxLength(8)
					.IsUnicode(false);

				entity.Property(e => e.ApprovedDate).HasColumnName("approvedDate");

				entity.Property(e => e.Approver).HasMaxLength(30);

				entity.Property(e => e.ContractId)
					.IsRequired()
					.HasMaxLength(20)
					.IsUnicode(false)
					.HasColumnName("contractID");

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.CusType)
					.IsRequired()
					.HasMaxLength(10)
					.IsUnicode(false)
					.HasColumnName("cusType");

				entity.Property(e => e.DeactiveDate).HasColumnName("deactiveDate");

				entity.Property(e => e.Description).HasColumnName("description");

				entity.Property(e => e.FirstPlace).HasColumnName("firstPlace");

				entity.Property(e => e.GetEmptyPlace).HasColumnName("getEmptyPlace");

				entity.Property(e => e.GoodsType)
					.HasMaxLength(10)
					.IsUnicode(false)
					.HasColumnName("goodsType");

				entity.Property(e => e.Price).HasColumnName("price");

				entity.Property(e => e.PriceType)
					.IsRequired()
					.HasMaxLength(5)
					.IsUnicode(false)
					.HasColumnName("priceType");

				entity.Property(e => e.SecondPlace).HasColumnName("secondPlace");

				entity.Property(e => e.SfId).HasColumnName("sfID");

				entity.Property(e => e.SfStateByContract)
					.HasColumnName("sfStateByContract")
					.HasDefaultValueSql("((1))")
					.HasComment("0: deactivated, 1: create new, 2: approved, 3: deleted");

				entity.Property(e => e.Status).HasColumnName("status");

				entity.Property(e => e.Updater).HasMaxLength(30);

				entity.Property(e => e.VehicleType)
					.HasMaxLength(10)
					.IsUnicode(false)
					.HasColumnName("vehicleType");

				entity.HasOne(d => d.Account)
					.WithMany(p => p.SubFeePrice)
					.HasForeignKey(d => d.AccountId)
					.HasConstraintName("fk_MaAccountSF_MaAccountCus");

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

				entity.Property(e => e.MaLoaiDiaDiem)
					.HasMaxLength(50)
					.IsUnicode(false);

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

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.GhiChu).HasColumnType("text");

				entity.Property(e => e.HoVaTen)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.MaLoaiPhuongTien)
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

				entity.Property(e => e.Updater).HasMaxLength(30);

				entity.HasOne(d => d.MaNhaCungCapNavigation)
					.WithMany(p => p.TaiXe)
					.HasForeignKey(d => d.MaNhaCungCap)
					.HasConstraintName("FK_TaiXe_KhachHang1");
			});

			modelBuilder.Entity<TaiXeTheoChang>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.MaSoXe)
					.HasMaxLength(10)
					.IsUnicode(false);

				entity.Property(e => e.MaTaiXe)
					.HasMaxLength(12)
					.IsUnicode(false);

				entity.Property(e => e.SoChan).HasDefaultValueSql("((1))");

				entity.HasOne(d => d.MaDieuPhoiNavigation)
					.WithMany(p => p.TaiXeTheoChang)
					.HasForeignKey(d => d.MaDieuPhoi)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__TaiXeTheo__MaDie__3E2826D9");

				entity.HasOne(d => d.MaSoXeNavigation)
					.WithMany(p => p.TaiXeTheoChang)
					.HasForeignKey(d => d.MaSoXe)
					.HasConstraintName("FK_TaiXeTheoChang_XeVanChuyen");

				entity.HasOne(d => d.MaTaiXeNavigation)
					.WithMany(p => p.TaiXeTheoChang)
					.HasForeignKey(d => d.MaTaiXe)
					.HasConstraintName("FK__TaiXeTheo__MaTai__3F1C4B12");
			});

			modelBuilder.Entity<TepChungTu>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Creator)
					.IsRequired()
					.HasMaxLength(30);

				entity.Property(e => e.TenChungTu).IsRequired();

				entity.Property(e => e.Updater).HasMaxLength(30);

				entity.HasOne(d => d.LoaiChungTuNavigation)
					.WithMany(p => p.TepChungTu)
					.HasForeignKey(d => d.LoaiChungTu)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__TepChungT__LoaiC__5E94F66B");

				entity.HasOne(d => d.MaDieuPhoiNavigation)
					.WithMany(p => p.TepChungTu)
					.HasForeignKey(d => d.MaDieuPhoi)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__TepChungT__MaDie__5CACADF9");

				entity.HasOne(d => d.MaHinhAnhNavigation)
					.WithMany(p => p.TepChungTu)
					.HasForeignKey(d => d.MaHinhAnh)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__TepChungT__MaHin__5DA0D232");
			});

			modelBuilder.Entity<TepHopDong>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Creator)
					.IsRequired()
					.HasMaxLength(30);

				entity.Property(e => e.MaHopDong)
					.IsRequired()
					.HasMaxLength(20)
					.IsUnicode(false);

				entity.Property(e => e.Updater).HasMaxLength(30);

				entity.HasOne(d => d.MaHopDongNavigation)
					.WithMany(p => p.TepHopDong)
					.HasForeignKey(d => d.MaHopDong)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__TepHopDon__MaHop__61716316");

				entity.HasOne(d => d.MaTepHongDongNavigation)
					.WithMany(p => p.TepHopDong)
					.HasForeignKey(d => d.MaTepHongDong)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__TepHopDon__MaTep__6265874F");
			});

			modelBuilder.Entity<ThaoTacTaiXe>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Creator)
					.IsRequired()
					.HasMaxLength(30);

				entity.HasOne(d => d.MaDieuPhoiNavigation)
					.WithMany(p => p.ThaoTacTaiXe)
					.HasForeignKey(d => d.MaDieuPhoi)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__ThaoTacTa__MaDie__13FCE2E3");
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

			modelBuilder.Entity<TrongTaiXe>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.DonViTrongTai)
					.IsRequired()
					.HasMaxLength(10)
					.IsUnicode(false);

				entity.Property(e => e.MaLoaiPhuongTien)
					.IsRequired()
					.HasMaxLength(10)
					.IsUnicode(false);
			});

			modelBuilder.Entity<UserHasCustomer>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.AccountId)
					.HasMaxLength(8)
					.IsUnicode(false);

				entity.Property(e => e.CustomerId)
					.IsRequired()
					.HasMaxLength(8)
					.IsUnicode(false);

				entity.Property(e => e.UserId).HasColumnName("UserID");

				entity.HasOne(d => d.Customer)
					.WithMany(p => p.UserHasCustomer)
					.HasForeignKey(d => d.CustomerId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_UserHasCustomer_KhachHang");

				entity.HasOne(d => d.User)
					.WithMany(p => p.UserHasCustomer)
					.HasForeignKey(d => d.UserId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_UserHasCustomer_NguoiDung");
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

			modelBuilder.Entity<ValidateDataByCustomer>(entity =>
			{
				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Creator)
					.IsRequired()
					.HasMaxLength(30);

				entity.Property(e => e.FieldId)
					.IsRequired()
					.HasMaxLength(10)
					.IsUnicode(false);

				entity.Property(e => e.FunctionId)
					.IsRequired()
					.HasMaxLength(10)
					.IsUnicode(false);

				entity.Property(e => e.MaAccount)
					.HasMaxLength(8)
					.IsUnicode(false);

				entity.Property(e => e.MaKh)
					.IsRequired()
					.HasMaxLength(8)
					.IsUnicode(false)
					.HasColumnName("MaKH");

				entity.HasOne(d => d.Field)
					.WithMany(p => p.ValidateDataByCustomer)
					.HasForeignKey(d => d.FieldId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__ValidateD__Field__2EB0D91F");

				entity.HasOne(d => d.Function)
					.WithMany(p => p.ValidateDataByCustomer)
					.HasForeignKey(d => d.FunctionId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__ValidateD__Funct__2DBCB4E6");

				entity.HasOne(d => d.MaAccountNavigation)
					.WithMany(p => p.ValidateDataByCustomer)
					.HasForeignKey(d => d.MaAccount)
					.HasConstraintName("FK__ValidateD__MaAcc__2CC890AD");

				entity.HasOne(d => d.MaKhNavigation)
					.WithMany(p => p.ValidateDataByCustomer)
					.HasForeignKey(d => d.MaKh)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK__ValidateDa__MaKH__2BD46C74");
			});

			modelBuilder.Entity<VanDon>(entity =>
			{
				entity.HasKey(e => e.MaVanDon)
					.HasName("PK_ThongTin_VanDon");

				entity.Property(e => e.MaVanDon)
					.HasMaxLength(10)
					.IsUnicode(false);

				entity.Property(e => e.Creator).HasMaxLength(30);

				entity.Property(e => e.HangTau).HasMaxLength(50);

				entity.Property(e => e.LoaiVanDon)
					.IsRequired()
					.HasMaxLength(20)
					.IsUnicode(false);

				entity.Property(e => e.MaAccount)
					.HasMaxLength(8)
					.IsUnicode(false);

				entity.Property(e => e.MaKh)
					.IsRequired()
					.HasMaxLength(8)
					.IsUnicode(false)
					.HasColumnName("MaKH");

				entity.Property(e => e.MaPtvc)
					.IsRequired()
					.HasMaxLength(10)
					.IsUnicode(false)
					.HasColumnName("MaPTVC");

				entity.Property(e => e.MaVanDonKh)
					.HasMaxLength(50)
					.IsUnicode(false)
					.HasColumnName("MaVanDonKH");

				entity.Property(e => e.Tau).HasMaxLength(50);

				entity.Property(e => e.Updater).HasMaxLength(30);
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

				entity.Property(e => e.Creator).HasMaxLength(30);

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

				entity.Property(e => e.MaNhaCungCap)
					.HasMaxLength(8)
					.IsUnicode(false);

				entity.Property(e => e.MaTaiSan)
					.HasMaxLength(50)
					.IsUnicode(false);

				entity.Property(e => e.MaTaiXeMacDinh)
					.HasMaxLength(12)
					.IsUnicode(false);

				entity.Property(e => e.NgayHoatDong).HasColumnType("date");

				entity.Property(e => e.Updater).HasMaxLength(30);

				entity.HasOne(d => d.MaNhaCungCapNavigation)
					.WithMany(p => p.XeVanChuyen)
					.HasForeignKey(d => d.MaNhaCungCap)
					.HasConstraintName("FK__XeVanChuy__MaNha__6359AB88");

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
