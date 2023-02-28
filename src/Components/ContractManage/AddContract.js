import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import moment from "moment/moment";
import LoadingPage from "../Common/Loading/LoadingPage";

const AddContract = (props) => {
  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    reset,
    setValue,
    watch,
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    MaKh: {
      required: "Không được để trống",
    },
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
      // pattern: {
      //   value:
      //     /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$/,
      //   message: "Không được chứa ký tự đặc biệt",
      // },
    },
    PhanLoaiHopDong: {
      required: "Không được để trống",
    },
    SoHopDongCha: {
      required: "Không được để trống",
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
    TrangThai: {
      required: "Không được để trống",
    },
    NgayThanhToan: {
      required: "Không được để trống",
      maxLength: {
        value: 2,
        message: "Không được vượt quá 2 ký tự",
      },
      pattern: {
        value: /^[0-9]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
      validate: (value) => {
        if (parseInt(value) > 31 || parseInt(value) < 1) {
          return "Không tồn tại ngày " + value;
        }
      },
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

  const [tabIndex, setTabIndex] = useState(0);
  const [listContractType, setListContractType] = useState([]);
  const [listStatusType, setListStatusType] = useState([]);
  const [listCustomer, setListCustomer] = useState([]);
  const [listContract, setListContract] = useState([]);
  const [listProduct, setListProduct] = useState([]);
  const [listWHType, setListWHType] = useState([]);

  useEffect(() => {
    if (props && props.listContractType) {
      setListContractType(props.listContractType);
    }
  }, [props, props.listContractType]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      let getListCustomer = await getData(
        `Customer/GetListCustomerOptionSelect`
      );
      if (getListCustomer && getListCustomer.length > 0) {
        let obj = [];

        getListCustomer.map((val) => {
          obj.push({
            value: val.maKh,
            label: val.maKh + " - " + val.tenKh + " (" + val.loaiKH + ")",
          });
        });
        setListCustomer(obj);
      }

      let data = await getData(`Contract/GetListOptionSelect`);

      setListProduct(data.dsSPDV);
      setListWHType(data.dsLoaiHinhKho);

      setListStatusType(props.listStatus);
      SetIsLoading(false);
    })();
  }, []);

  const getListContract = async (MaKh) => {
    SetIsLoading(true);
    setValue("SoHopDongCha", null);
    setListContract([]);
    let getListContract = await getData(
      `Contract/GetListContractSelect?MaKH=${MaKh}&getChild=false`
    );

    if (getListContract && getListContract.length > 0) {
      let obj = [];
      getListContract.map((val) => {
        obj.push({
          value: val.maHopDong,
          label: val.maHopDong + " - " + val.tenHienThi,
        });
      });
      setListContract(obj);
    } else {
      setListContract([]);
    }

    SetIsLoading(false);
  };

  const handleResetClick = () => {
    reset();
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);
    var create = await postData(
      "Contract/CreateContract",
      {
        LoaiHinhHopTac: data.LoaiHinhHopTac,
        MaLoaiSPDV: data.LoaiSPDV,
        MaLoaiHinh: data.LoaiHinhKho,
        HinhThucThue: data.HinhThucThueKho,
        maHopDong: data.MaHopDong,
        soHopDongCha: tabIndex === 0 ? null : data.SoHopDongCha.value,
        Account: tabIndex === 0 ? null : data.Account,
        tenHienThi: data.TenHopDong,
        maKh: data.MaKh.value,
        NgayThanhToan: tabIndex === 1 ? 0 : data.NgayThanhToan,
        thoiGianBatDau: moment(new Date(data.NgayBatDau).toISOString()).format(
          "YYYY-MM-DD"
        ),
        thoiGianKetThuc: moment(
          new Date(data.NgayKetThuc).toISOString()
        ).format("YYYY-MM-DD"),
        GhiChu: data.GhiChu,
        FileContract: data.FileContact[0],
        FileCosting: data.FileCosting[0],
      },
      {
        headers: { "Content-Type": "multipart/form-data" },
      }
    );

    if (create === 1) {
      props.getListContract(
        1,
        "",
        "",
        "",
        "",
        props.tabIndex === 0 ? "KH" : "NCC"
      );
      reset();
    }

    SetIsLoading(false);
  };

  const HandleOnChangeTabs = (tabIndex) => {
    reset();
    setTabIndex(tabIndex);
  };

  return (
    <>
      <Tabs
        selectedIndex={tabIndex}
        onSelect={(index) => HandleOnChangeTabs(index)}
      >
        <TabList>
          <Tab>Tạo Hợp Đồng Chính</Tab>
          <Tab>Tạo Phụ Lục Hợp Đồng</Tab>
        </TabList>

        <TabPanel>
          {tabIndex === 0 && (
            <div className="card card-primary">
              <div className="card-header">
                <h3 className="card-title">Form Thêm Mới Hợp Đồng</h3>
              </div>
              <div>
                {IsLoading === true && (
                  <div>
                    <LoadingPage></LoadingPage>
                  </div>
                )}
              </div>

              {IsLoading === false && (
                <form onSubmit={handleSubmit(onSubmit)}>
                  <div className="card-body">
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="KhachHang">
                            Khách Hàng/ Nhà Cung Cấp(*)
                          </label>
                          <Controller
                            name="MaKh"
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={listCustomer}
                              />
                            )}
                            rules={Validate.MaKh}
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
                          <label htmlFor="MaHopDong">Mã Hợp Đồng(*)</label>
                          <input
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
                          <label htmlFor="TenHopDong">Tên Hợp Đồng(*)</label>
                          <input
                            type="text"
                            className="form-control"
                            id="TenHopDong"
                            placeholder="Nhập Tên Hợp Đồng"
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
                          <label htmlFor="LoaiSPDV">Sản Phẩm Dịch Vụ(*)</label>
                          <select
                            className="form-control"
                            {...register("LoaiSPDV", Validate.LoaiSPDV)}
                          >
                            <option value="">Chọn Sản Phẩm Dịch Vụ</option>
                            {listProduct &&
                              listProduct.length > 0 &&
                              listProduct.map((val, index) => {
                                return (
                                  <option key={index} value={val.maLoaiSpdv}>
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
                          <label htmlFor="NgayBatDau">Ngày bắt đầu(*)</label>
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
                          <label htmlFor="NgayKetThuc">Ngày kết thúc(*)</label>
                          <div className="input-group ">
                            <Controller
                              control={control}
                              name="NgayKetThuc"
                              render={({ field }) => (
                                <DatePicker
                                  className="form-control"
                                  dateFormat="dd/MM/yyyy"
                                  placeholderText="Chọn Ngày kết thúc"
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
                        Tải lên tệp Hợp Đồng(*)
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
                  </div>
                  <div className="card-footer">
                    <div>
                      <button
                        type="button"
                        onClick={() => handleResetClick()}
                        className="btn btn-warning"
                      >
                        Làm mới
                      </button>
                      <button
                        type="submit"
                        className="btn btn-primary"
                        style={{ float: "right" }}
                      >
                        Thêm mới
                      </button>
                    </div>
                  </div>
                </form>
              )}
            </div>
          )}
        </TabPanel>
        <TabPanel>
          {tabIndex === 1 && (
            <div className="card card-primary">
              <div className="card-header">
                <h3 className="card-title">Form Thêm Mới Phục Lục Hợp Đồng</h3>
              </div>
              <div>
                {IsLoading === true && (
                  <div>
                    <LoadingPage></LoadingPage>
                  </div>
                )}
              </div>

              {IsLoading === false && (
                <form onSubmit={handleSubmit(onSubmit)}>
                  <div className="card-body">
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="KhachHang">
                            Khách Hàng/ Nhà Cung Cấp(*)
                          </label>
                          <Controller
                            name="MaKh"
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                onChange={(field) =>
                                  getListContract(
                                    field.value,
                                    setValue("MaKh", field)
                                  )
                                }
                                value={field.value}
                                options={listCustomer}
                              />
                            )}
                            rules={Validate.MaKh}
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
                            Số Hợp Đồng Cha(*)
                          </label>
                          <Controller
                            name="SoHopDongCha"
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={listContract}
                              />
                            )}
                            rules={Validate.SoHopDongCha}
                          />
                          {errors.SoHopDongCha && (
                            <span className="text-danger">
                              {errors.SoHopDongCha.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="MaHopDong">
                            Mã Phụ Lục Hợp Đồng(*)
                          </label>
                          <input
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
                            Tên Phụ Lục Hợp Đồng(*)
                          </label>
                          <input
                            type="text"
                            className="form-control"
                            id="TenHopDong"
                            placeholder="Nhập tên phụ lục hợp đồng"
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
                          <label htmlFor="Account">Account</label>
                          <input
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
                          <label htmlFor="LoaiSPDV">Sản Phẩm Dịch Vụ(*)</label>
                          <select
                            className="form-control"
                            {...register("LoaiSPDV", Validate.LoaiSPDV)}
                          >
                            <option value="">Chọn Sản Phẩm Dịch Vụ</option>
                            {listProduct &&
                              listProduct.length > 0 &&
                              listProduct.map((val, index) => {
                                return (
                                  <option key={index} value={val.maLoaiSpdv}>
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
                          <label htmlFor="NgayBatDau">Ngày bắt đầu(*)</label>
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
                          <label htmlFor="NgayKetThuc">Ngày kết thúc(*)</label>
                          <div className="input-group ">
                            <Controller
                              control={control}
                              name="NgayKetThuc"
                              render={({ field }) => (
                                <DatePicker
                                  className="form-control"
                                  dateFormat="dd/MM/yyyy"
                                  placeholderText="Chọn Ngày kết thúc"
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
                        Tải Lên Tệp Phụ Lục(*)
                      </label>
                      <input
                        type="file"
                        className="form-control-file"
                        {...register("FileContact", {
                          required: "Không được bỏ trống",
                        })}
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
                        {...register("FileCosting")}
                      />
                      {errors.FileCosting && (
                        <span className="text-danger">
                          {errors.FileCosting.message}
                        </span>
                      )}
                    </div>
                  </div>
                  <div className="card-footer">
                    <div>
                      <button
                        type="button"
                        onClick={() => handleResetClick()}
                        className="btn btn-warning"
                      >
                        Làm mới
                      </button>
                      <button
                        type="submit"
                        className="btn btn-primary"
                        style={{ float: "right" }}
                      >
                        Thêm mới
                      </button>
                    </div>
                  </div>
                </form>
              )}
            </div>
          )}
        </TabPanel>
      </Tabs>
    </>
  );
};

export default AddContract;
