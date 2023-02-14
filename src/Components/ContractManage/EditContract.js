import { useState, useEffect, useCallback } from "react";
import { getData, postData, getFile } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import moment from "moment";
import DatePicker from "react-datepicker";
import LoadingPage from "../Common/Loading/LoadingPage";

const EditContract = (props) => {
  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    setValue,
    control,
    watch,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    MaHopDong: {
      required: "Không được để trống",
      maxLength: {
        value: 20,
        message: "Không được vượt quá 20 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TenHopDong: {
      required: "Không được để trống",
      maxLength: {
        value: 50,
        message: "Không được vượt quá 50 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      pattern: {
        value:
          /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    PhanLoaiHopDong: {
      required: "Không được để trống",
    },
    SoHopDongCha: {
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    NgayBatDau: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      minLength: {
        value: 10,
        message: "Không được ít hơn 10 ký tự",
      },
      pettern: {
        value:
          /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/,
        message: "Không phải định dạng ngày",
      },
    },
    NgayKetThuc: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      minLength: {
        value: 10,
        message: "Không được ít hơn 10 ký tự",
      },
      pettern: {
        value:
          /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/,
        message: "Không phải định dạng ngày",
      },
    },
    PTVC: {
      required: "Không được để trống",
    },
    LoaiHinhHopTac: {
      required: "Không được để trống",
    },
    LoaiSPDV: {
      required: "Không được để trống",
    },
    LoaiHinhKho: {
      required: "Không được để trống",
    },
    HinhThucThueKho: {
      required: "Không được để trống",
    },
  };

  const [isContract, setIsContract] = useState(true);
  const [listContractType, setListContractType] = useState([]);
  const [listTransportType, setListTransportType] = useState([]);
  const [listStatusType, setListStatusType] = useState([]);

  const [listProduct, setListProduct] = useState([]);
  const [listWHType, setListWHType] = useState([]);

  const [downloadFile, setDownloadFile] = useState(null);

  useEffect(() => {
    if (
      props &&
      props.selectIdClick &&
      Object.keys(props.selectIdClick).length > 0 &&
      Object.keys(props).length > 0
    ) {
      SetIsLoading(true);

      if (props.selectIdClick.soHopDongCha) {
        setIsContract(false);
        setValue("SoHopDongCha", props.selectIdClick.soHopDongCha);
        setValue("PhuPhi", props.selectIdClick.phuPhi);
      } else {
        setIsContract(true);
      }

      setValue("NgayThanhToan", props.selectIdClick.ngayThanhToan);
      setValue("MaHopDong", props.selectIdClick.maHopDong);
      setValue("TenHopDong", props.selectIdClick.tenHienThi);
      setValue("MaKh", props.selectIdClick.maKh);
      setValue("GhiChu", props.selectIdClick.ghiChu);
      setDownloadFile(props.selectIdClick.file);
      setValue("TrangThai", props.selectIdClick.trangThai);
      setValue("NgayBatDau", new Date(props.selectIdClick.thoiGianBatDau));
      setValue("NgayKetThuc", new Date(props.selectIdClick.thoiGianKetThuc));
      SetIsLoading(false);
    }
  }, [props, props.selectIdClick]);

  useEffect(() => {
    if (
      props &&
      props.listContractType &&
      listProduct &&
      listWHType &&
      listProduct.length > 0 &&
      listWHType.length > 0 &&
      props.listContractType.length > 0
    ) {
      setListContractType(props.listContractType);
      setValue("Account", props.selectIdClick.account);
      setValue("LoaiHinhHopTac", props.selectIdClick.loaiHinhHopTac);
      setValue("LoaiSPDV", props.selectIdClick.loaiSPDV);
      setValue("LoaiHinhKho", props.selectIdClick.loaiHinhKho);
      setValue("HinhThucThueKho", props.selectIdClick.hinhThucThueKho);
    }
  }, [props, props.listContractType, listProduct, listWHType]);

  useEffect(() => {
    SetIsLoading(true);

    (async () => {
      let data = await getData(`Contract/GetListOptionSelect`);

      setListProduct(data.dsSPDV);
      setListWHType(data.dsLoaiHinhKho);
    })();

    setListStatusType(props.listStatus);
    SetIsLoading(false);
  }, []);

  const handleDownloadContact = async () => {
    SetIsLoading(true);
    const getFileDownLoad = getFile(
      `Contract/DownloadFile?fileId=${downloadFile}`,
      watch("TenHopDong")
    );
    SetIsLoading(false);
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);

    var update = await postData(
      `Contract/UpdateContract?Id=${data.MaHopDong}`,
      {
        LoaiHinhHopTac: data.LoaiHinhHopTac,
        MaLoaiSPDV: data.LoaiSPDV,
        MaLoaiHinh: data.LoaiHinhKho,
        HinhThucThue: data.HinhThucThueKho,
        NgayThanhToan: !data.NgayThanhToan ? null : data.NgayThanhToan,
        tenHienThi: data.TenHopDong,
        ThoiGianBatDau: moment(new Date(data.NgayBatDau).toISOString()).format(
          "YYYY-MM-DD"
        ),
        ThoiGianKetThuc: moment(
          new Date(data.NgayKetThuc).toISOString()
        ).format("YYYY-MM-DD"),
        GhiChu: data.GhiChu,
        FileContract: data.FileContact[0],
        FileCosting: data.FileCosting[0],
        TrangThai: data.TrangThai,
      },
      {
        headers: { "Content-Type": "multipart/form-data" },
      }
    );

    if (update === 1) {
      props.getListContract(
        1,
        "",
        "",
        "",
        "",
        props.tabIndex === 0 ? "KH" : "NCC"
      );

      props.hideModal();
    }

    SetIsLoading(false);
  };

  return (
    <>
      <div>
        {IsLoading === true ? (
          <div>
            <LoadingPage></LoadingPage>
          </div>
        ) : (
          <>
            {isContract === true && (
              <>
                <div className="card card-primary">
                  {IsLoading === false && (
                    <form onSubmit={handleSubmit(onSubmit)}>
                      <div className="card-body">
                        <div className="row">
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="MaHopDong">Mã Hợp Đồng(*)</label>
                              <input
                                readOnly
                                autoComplete="false"
                                type="text"
                                className="form-control"
                                id="MaHopDong"
                                placeholder="Nhập mã hợp đồng"
                                {...register("MaHopDong", Validate.MaHopDong)}
                              />
                              {errors.MaHopDong && (
                                <span className="text-danger">
                                  {errors.MaHopDong.message}
                                </span>
                              )}
                            </div>
                          </div>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="TenHopDong">
                                Tên Hợp Đồng(*)
                              </label>
                              <input
                                type="text"
                                className="form-control"
                                id="TenHopDong"
                                placeholder="Nhập tên hợp đồng"
                                {...register("TenHopDong", Validate.TenHopDong)}
                              />
                              {errors.TenHopDong && (
                                <span className="text-danger">
                                  {errors.TenHopDong.message}
                                </span>
                              )}
                            </div>
                          </div>
                        </div>
                        <div className="row">
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="MaKh">Mã Khách Hàng(*)</label>
                              <input
                                type="text "
                                className="form-control"
                                id="MaKh"
                                placeholder="Nhập mã khách hàng"
                                readOnly
                                {...register("MaKh", Validate.MaKh)}
                              />
                              {errors.MaKh && (
                                <span className="text-danger">
                                  {errors.MaKh.message}
                                </span>
                              )}
                            </div>
                          </div>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="NgayThanhToan">
                                Ngày Thanh Toán(*)
                              </label>
                              <input
                                type="text"
                                className="form-control"
                                id="NgayThanhToan"
                                placeholder="Ngày Thanh Toán"
                                {...register(
                                  "NgayThanhToan",
                                  Validate.NgayThanhToan
                                )}
                              />
                              {errors.NgayThanhToan && (
                                <span className="text-danger">
                                  {errors.NgayThanhToan.message}
                                </span>
                              )}
                            </div>
                          </div>
                        </div>
                        <div className="row">
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="LoaiHinhHopTac">
                                Loại Hình Hợp Tác(*)
                              </label>
                              <select
                                className="form-control"
                                {...register(
                                  "LoaiHinhHopTac",
                                  Validate.LoaiHinhHopTac
                                )}
                              >
                                <option value="">Chọn Loại Hình Hợp Tác</option>
                                <option value="TrucTiep">Trực Tiếp</option>
                                <option value="GianTiep">Gián Tiếp</option>
                              </select>
                              {errors.LoaiHinhHopTac && (
                                <span className="text-danger">
                                  {errors.LoaiHinhHopTac.message}
                                </span>
                              )}
                            </div>
                          </div>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="LoaiSPDV">
                                Sản Phẩm Dịch Vụ(*)
                              </label>
                              <select
                                className="form-control"
                                {...register("LoaiSPDV", Validate.LoaiSPDV)}
                              >
                                <option value="">Chọn Sản Phẩm Dịch Vụ</option>
                                {listProduct &&
                                  listProduct.length > 0 &&
                                  listProduct.map((val, index) => {
                                    return (
                                      <option
                                        key={index}
                                        value={val.maLoaiSpdv}
                                      >
                                        {val.tenLoaiSpdv}
                                      </option>
                                    );
                                  })}
                              </select>
                              {errors.LoaiSPDV && (
                                <span className="text-danger">
                                  {errors.LoaiSPDV.message}
                                </span>
                              )}
                            </div>
                          </div>
                          {watch("LoaiSPDV") && watch("LoaiSPDV") === "KHO" && (
                            <>
                              <div className="col col-sm">
                                <div className="form-group">
                                  <label htmlFor="LoaiHinhKho">
                                    Loại Hình Kho(*)
                                  </label>
                                  <select
                                    className="form-control"
                                    {...register(
                                      "LoaiHinhKho",
                                      Validate.LoaiHinhKho
                                    )}
                                  >
                                    <option value="">Chọn Loại Hình Kho</option>
                                    {listWHType &&
                                      listWHType.length > 0 &&
                                      listWHType.map((val, index) => {
                                        return (
                                          <option
                                            key={index}
                                            value={val.maLoaiHinh}
                                          >
                                            {val.tenLoaiHinh}
                                          </option>
                                        );
                                      })}
                                  </select>
                                  {errors.LoaiHinhKho && (
                                    <span className="text-danger">
                                      {errors.LoaiHinhKho.message}
                                    </span>
                                  )}
                                </div>
                              </div>
                              <div className="col col-sm">
                                <div className="form-group">
                                  <label htmlFor="HinhThucThueKho">
                                    Hình Thức Thuê Kho(*)
                                  </label>
                                  <select
                                    className="form-control"
                                    {...register(
                                      "HinhThucThueKho",
                                      Validate.HinhThucThueKho
                                    )}
                                  >
                                    <option value="">
                                      Chọn Hình Thức Thuê Kho
                                    </option>
                                    <option value="CoDinh">Cố Định</option>
                                    <option value="KhongCoDinh">
                                      Không Cố Định
                                    </option>
                                  </select>
                                  {errors.HinhThucThueKho && (
                                    <span className="text-danger">
                                      {errors.HinhThucThueKho.message}
                                    </span>
                                  )}
                                </div>
                              </div>
                            </>
                          )}
                        </div>
                        <div className="row">
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="NgayBatDau">
                                Ngày bắt đầu(*)
                              </label>
                              <div className="input-group ">
                                <Controller
                                  control={control}
                                  name="NgayBatDau"
                                  render={({ field }) => (
                                    <DatePicker
                                      className="form-control"
                                      dateFormat="dd/MM/yyyy"
                                      placeholderText="Chọn ngày bắt đầu"
                                      onChange={(date) => field.onChange(date)}
                                      selected={field.value}
                                    />
                                  )}
                                  rules={Validate.NgayBatDau}
                                />
                                {errors.NgayBatDau && (
                                  <span className="text-danger">
                                    {errors.NgayBatDau.message}
                                  </span>
                                )}
                              </div>
                            </div>
                          </div>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="NgayKetThuc">
                                Ngày kết thúc(*)
                              </label>
                              <div className="input-group ">
                                <Controller
                                  control={control}
                                  name="NgayKetThuc"
                                  render={({ field }) => (
                                    <DatePicker
                                      className="form-control"
                                      dateFormat="dd/MM/yyyy"
                                      placeholderText="Chọn ngày bắt đầu"
                                      onChange={(date) => field.onChange(date)}
                                      selected={field.value}
                                    />
                                  )}
                                  rules={Validate.NgayKetThuc}
                                />
                                {errors.NgayKetThuc && (
                                  <span className="text-danger">
                                    {errors.NgayKetThuc.message}
                                  </span>
                                )}
                              </div>
                            </div>
                          </div>
                        </div>

                        <div className="form-group">
                          <label htmlFor="GhiChu">Ghi Chú</label>
                          <textarea
                            type="text"
                            className="form-control"
                            id="GhiChu"
                            rows={3}
                            placeholder="Nhập ghi chú"
                            {...register("GhiChu")}
                          ></textarea>
                          {errors.GhiChu && (
                            <span className="text-danger">
                              {errors.GhiChu.message}
                            </span>
                          )}
                        </div>

                        <div className="form-group">
                          <label htmlFor="FileContact">
                            Tải lên tệp Hợp Đồng
                          </label>
                          <input
                            type="file"
                            className="form-control-file"
                            {...register("FileContact", Validate.FileContact)}
                          />
                          {errors.FileContact && (
                            <span className="text-danger">
                              {errors.FileContact.message}
                            </span>
                          )}
                        </div>
                        <div className="form-group">
                          <label htmlFor="FileCosting">
                            Tải lên tệp Costing(*)
                          </label>
                          <input
                            type="file"
                            className="form-control-file"
                            {...register("FileCosting", Validate.FileCosting)}
                          />
                          {errors.FileCosting && (
                            <span className="text-danger">
                              {errors.FileCosting.message}
                            </span>
                          )}
                        </div>

                        {/* {downloadFile && downloadFile !== "0" && (
                          <div>
                            <div className="form-group">
                              <label htmlFor="FileContact">
                                Tải về tệp Hợp Đồng
                              </label>
                              <br />
                              <button
                                type="button"
                                className="btn btn-default"
                                onClick={() => handleDownloadContact()}
                              >
                                <i className="fas fa-file-download">
                                  Tải tệp hợp đồng
                                </i>
                              </button>
                            </div>
                          </div>
                        )} */}

                        <div className="form-group">
                          <label htmlFor="TrangThai">Trạng thái(*)</label>
                          <select
                            className="form-control"
                            {...register("TrangThai", Validate.TrangThai)}
                          >
                            <option value="">Chọn trạng thái</option>
                            {listStatusType &&
                              listStatusType.map((val) => {
                                return (
                                  <option
                                    value={val.statusId}
                                    key={val.statusId}
                                  >
                                    {val.statusContent}
                                  </option>
                                );
                              })}
                          </select>
                          {errors.TrangThai && (
                            <span className="text-danger">
                              {errors.TrangThai.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="card-footer">
                        <div>
                          <button
                            type="submit"
                            className="btn btn-primary"
                            style={{ float: "right" }}
                          >
                            Cập nhật
                          </button>
                        </div>
                      </div>
                    </form>
                  )}
                </div>
              </>
            )}
            {isContract === false && (
              <>
                <div className="card card-primary">
                  {IsLoading === false && (
                    <form onSubmit={handleSubmit(onSubmit)}>
                      <div className="card-body">
                        <div className="row">
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="MaHopDong">Mã Hợp Đồng(*)</label>
                              <input
                                readOnly
                                autoComplete="false"
                                type="text"
                                className="form-control"
                                id="MaHopDong"
                                placeholder="Nhập mã hợp đồng"
                                {...register("MaHopDong", Validate.MaHopDong)}
                              />
                              {errors.MaHopDong && (
                                <span className="text-danger">
                                  {errors.MaHopDong.message}
                                </span>
                              )}
                            </div>
                          </div>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="TenHopDong">
                                Tên Hợp Đồng(*)
                              </label>
                              <input
                                type="text"
                                className="form-control"
                                id="TenHopDong"
                                placeholder="Nhập tên hợp đồng"
                                {...register("TenHopDong", Validate.TenHopDong)}
                              />
                              {errors.TenHopDong && (
                                <span className="text-danger">
                                  {errors.TenHopDong.message}
                                </span>
                              )}
                            </div>
                          </div>
                        </div>
                        <div className="row">
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="MaKh">Mã Khách Hàng(*)</label>
                              <input
                                readOnly
                                autoComplete="false"
                                type="text"
                                className="form-control"
                                id="MaKh"
                                placeholder="Nhập mã hợp đồng"
                                {...register("MaKh", Validate.MaKh)}
                              />
                              {errors.MaKh && (
                                <span className="text-danger">
                                  {errors.MaKh.message}
                                </span>
                              )}
                            </div>
                          </div>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="SoHopDongCha">
                                Số hợp đồng cha(*)
                              </label>
                              <input
                                readOnly
                                autoComplete="false"
                                type="text"
                                className="form-control"
                                id="SoHopDongCha"
                                placeholder="Nhập mã hợp đồng"
                                {...register(
                                  "SoHopDongCha",
                                  Validate.SoHopDongCha
                                )}
                              />
                              {errors.SoHopDongCha && (
                                <span className="text-danger">
                                  {errors.SoHopDongCha.message}
                                </span>
                              )}
                            </div>
                          </div>
                        </div>
                        <div className="row">
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="Account">Account</label>
                              <input
                                disabled={true}
                                autoComplete="false"
                                type="text"
                                className="form-control"
                                id="Account"
                                placeholder="Nhập Account"
                                {...register("Account", Validate.Account)}
                              />
                              {errors.Account && (
                                <span className="text-danger">
                                  {errors.Account.message}
                                </span>
                              )}
                            </div>
                          </div>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="LoaiHinhHopTac">
                                Loại Hình Hợp Tác(*)
                              </label>
                              <select
                                className="form-control"
                                {...register(
                                  "LoaiHinhHopTac",
                                  Validate.LoaiHinhHopTac
                                )}
                              >
                                <option value="">Chọn Loại Hình Hợp Tác</option>
                                <option value="TrucTiep">Trực Tiếp</option>
                                <option value="GianTiep">Gián Tiếp</option>
                              </select>
                              {errors.LoaiHinhHopTac && (
                                <span className="text-danger">
                                  {errors.LoaiHinhHopTac.message}
                                </span>
                              )}
                            </div>
                          </div>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="LoaiSPDV">
                                Sản Phẩm Dịch Vụ(*)
                              </label>
                              <select
                                className="form-control"
                                {...register("LoaiSPDV", Validate.LoaiSPDV)}
                              >
                                <option value="">Chọn Sản Phẩm Dịch Vụ</option>
                                {listProduct &&
                                  listProduct.length > 0 &&
                                  listProduct.map((val, index) => {
                                    return (
                                      <option
                                        key={index}
                                        value={val.maLoaiSpdv}
                                      >
                                        {val.tenLoaiSpdv}
                                      </option>
                                    );
                                  })}
                              </select>
                              {errors.LoaiSPDV && (
                                <span className="text-danger">
                                  {errors.LoaiSPDV.message}
                                </span>
                              )}
                            </div>
                          </div>
                          {watch("LoaiSPDV") && watch("LoaiSPDV") === "KHO" && (
                            <>
                              <div className="col col-sm">
                                <div className="form-group">
                                  <label htmlFor="LoaiHinhKho">
                                    Loại Hình Kho(*)
                                  </label>
                                  <select
                                    className="form-control"
                                    {...register(
                                      "LoaiHinhKho",
                                      Validate.LoaiHinhKho
                                    )}
                                  >
                                    <option value="">Chọn Loại Hình Kho</option>
                                    {listWHType &&
                                      listWHType.length > 0 &&
                                      listWHType.map((val, index) => {
                                        return (
                                          <option
                                            key={index}
                                            value={val.maLoaiHinh}
                                          >
                                            {val.tenLoaiHinh}
                                          </option>
                                        );
                                      })}
                                  </select>
                                  {errors.LoaiHinhKho && (
                                    <span className="text-danger">
                                      {errors.LoaiHinhKho.message}
                                    </span>
                                  )}
                                </div>
                              </div>
                              <div className="col col-sm">
                                <div className="form-group">
                                  <label htmlFor="HinhThucThueKho">
                                    Hình Thức Thuê Kho(*)
                                  </label>
                                  <select
                                    className="form-control"
                                    {...register(
                                      "HinhThucThueKho",
                                      Validate.HinhThucThueKho
                                    )}
                                  >
                                    <option value="">
                                      Chọn Hình Thức Thuê Kho
                                    </option>
                                    <option value="CoDinh">Cố Định</option>
                                    <option value="KhongCoDinh">
                                      Không Cố Định
                                    </option>
                                  </select>
                                  {errors.HinhThucThueKho && (
                                    <span className="text-danger">
                                      {errors.HinhThucThueKho.message}
                                    </span>
                                  )}
                                </div>
                              </div>
                            </>
                          )}
                        </div>
                        <div className="row">
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="NgayBatDau">
                                Ngày bắt đầu(*)
                              </label>
                              <div className="input-group ">
                                <Controller
                                  control={control}
                                  name="NgayBatDau"
                                  render={({ field }) => (
                                    <DatePicker
                                      className="form-control"
                                      dateFormat="dd/MM/yyyy"
                                      placeholderText="Chọn ngày bắt đầu"
                                      onChange={(date) => field.onChange(date)}
                                      selected={field.value}
                                    />
                                  )}
                                  rules={Validate.NgayBatDau}
                                />
                                {errors.NgayBatDau && (
                                  <span className="text-danger">
                                    {errors.NgayBatDau.message}
                                  </span>
                                )}
                              </div>
                            </div>
                          </div>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="NgayKetThuc">
                                Ngày kết thúc(*)
                              </label>
                              <div className="input-group ">
                                <Controller
                                  control={control}
                                  name="NgayKetThuc"
                                  render={({ field }) => (
                                    <DatePicker
                                      className="form-control"
                                      dateFormat="dd/MM/yyyy"
                                      placeholderText="Chọn ngày bắt đầu"
                                      onChange={(date) => field.onChange(date)}
                                      selected={field.value}
                                    />
                                  )}
                                  rules={Validate.NgayKetThuc}
                                />
                                {errors.NgayKetThuc && (
                                  <span className="text-danger">
                                    {errors.NgayKetThuc.message}
                                  </span>
                                )}
                              </div>
                            </div>
                          </div>
                        </div>
                        <div className="form-group">
                          <label htmlFor="GhiChu">Ghi Chú</label>
                          <textarea
                            type="text"
                            className="form-control"
                            id="GhiChu"
                            rows={3}
                            placeholder="Nhập ghi chú"
                            {...register("GhiChu")}
                          ></textarea>
                        </div>
                        <div className="form-group">
                          <label htmlFor="FileContact">
                            Tải lên tệp Phục Lục
                          </label>
                          <input
                            type="file"
                            className="form-control-file"
                            {...register("FileContact", Validate.FileContact)}
                          />
                          {errors.FileContact && (
                            <span className="text-danger">
                              {errors.FileContact.message}
                            </span>
                          )}
                          <div className="form-group">
                            <label htmlFor="FileCosting">
                              Tải lên tệp Costing(*)
                            </label>
                            <input
                              type="file"
                              className="form-control-file"
                              {...register("FileCosting", Validate.FileCosting)}
                            />
                            {errors.FileCosting && (
                              <span className="text-danger">
                                {errors.FileCosting.message}
                              </span>
                            )}
                          </div>
                        </div>
                        <div className="form-group">
                          <label htmlFor="TrangThai">Trạng thái(*)</label>
                          <select
                            className="form-control"
                            {...register("TrangThai", Validate.TrangThai)}
                          >
                            <option value="">Chọn trạng thái</option>
                            {listStatusType &&
                              listStatusType.map((val) => {
                                return (
                                  <option
                                    value={val.statusId}
                                    key={val.statusId}
                                  >
                                    {val.statusContent}
                                  </option>
                                );
                              })}
                          </select>
                          {errors.TrangThai && (
                            <span className="text-danger">
                              {errors.TrangThai.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="card-footer">
                        <div>
                          <button
                            type="submit"
                            className="btn btn-primary"
                            style={{ float: "right" }}
                          >
                            Cập nhật
                          </button>
                        </div>
                      </div>
                    </form>
                  )}
                </div>
              </>
            )}
          </>
        )}
      </div>
    </>
  );
};

export default EditContract;
