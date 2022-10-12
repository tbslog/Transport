import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import moment from "moment/moment";

const AddContract = (props) => {
  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    reset,
    setValue,
    control,
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
        message: "Không được vượt quá 10 ký tự",
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
    MaKh: {
      required: "Không được để trống",
      maxLength: {
        value: 8,
        message: "Không được vượt quá 8 ký tự",
      },
      minLength: {
        value: 8,
        message: "Không được ít hơn 8 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
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
    PhuPhi: {
      pattern: {
        value: /^[0-9]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
    },
  };

  const [tabIndex, setTabIndex] = useState(0);
  const [listContractType, setListContractType] = useState([]);
  const [listTransportType, setListTransportType] = useState([]);
  const [listStatusType, setListStatusType] = useState([]);
  const [listCustomer, setListCustomer] = useState([]);
  const [listContract, setListContract] = useState([]);

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
      console.log(getListCustomer);
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

      const getListTransportType = await getData("Common/GetListTransportType");
      setListTransportType(getListTransportType);
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

    console.log(data);

    var create = await postData(
      "Contract/CreateContract",
      {
        maHopDong: data.MaHopDong,
        soHopDongCha: tabIndex === 0 ? null : data.SoHopDongCha.value,
        tenHienThi: data.TenHopDong,
        maKh: data.MaKh.value,
        thoiGianBatDau: moment(new Date(data.NgayBatDau).toISOString()).format(
          "YYYY-MM-DD"
        ),
        thoiGianKetThuc: moment(
          new Date(data.NgayKetThuc).toISOString()
        ).format("YYYY-MM-DD"),
        ghiChu: data.GhiChu,
        trangThai: data.TrangThai,
        file: data.FileContact[0],
      },
      {
        headers: { "Content-Type": "multipart/form-data" },
      }
    );

    if (create === 1) {
      props.getListContract(1);
      reset();
      props.hideModal();
    }

    SetIsLoading(false);
  };

  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex, reset());
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
              <div>{IsLoading === true && <div>Loading...</div>}</div>

              {IsLoading === false && (
                <form onSubmit={handleSubmit(onSubmit)}>
                  <div className="card-body">
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="KhachHang">
                            Khách Hàng/ Nhà Cung Cấp
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
                          <label htmlFor="MaHopDong">Mã Hợp Đồng</label>
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
                          <label htmlFor="TenHopDong">Tên Hợp Đồng</label>
                          <input
                            type="text"
                            className="form-control"
                            id="TenHopDong"
                            placeholder="Nhập tên khách hàng"
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
                      {/* <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="PhanLoaiHopDong">
                            Phân Loại Hợp Đồng
                          </label>
                          <select
                            className="form-control"
                            {...register(
                              "PhanLoaiHopDong",
                              Validate.PhanLoaiHopDong
                            )}
                          >
                            <option value="">Chọn phân loại HĐ</option>
                            {listContractType &&
                              listContractType.map((val) => {
                                return (
                                  <option
                                    value={val.maLoaiHopDong}
                                    key={val.maLoaiHopDong}
                                  >
                                    {val.tenLoaiHopDong}
                                  </option>
                                );
                              })}
                          </select>
                          {errors.PhanLoaiHopDong && (
                            <span className="text-danger">
                              {errors.PhanLoaiHopDong.message}
                            </span>
                          )}
                        </div>
                      </div> */}
                      {/* <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="PTVC">Phương thức vận chuyển</label>
                          <select
                            className="form-control"
                            {...register("PTVC", Validate.PTVC)}
                          >
                            <option value="">
                              Chọn phương thức vận chuyển
                            </option>
                            {listTransportType &&
                              listTransportType.map((val) => {
                                return (
                                  <option value={val.maPtvc} key={val.maPtvc}>
                                    {val.tenPtvc}
                                  </option>
                                );
                              })}
                          </select>
                          {errors.PTVC && (
                            <span className="text-danger">
                              {errors.PTVC.message}
                            </span>
                          )}
                        </div>
                      </div> */}
                    </div>
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="NgayBatDau">Ngày bắt đầu</label>
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
                          <label htmlFor="NgayKetThuc">Ngày kết thúc</label>
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
                      <input
                        type="text"
                        className="form-control"
                        id="GhiChu"
                        placeholder="Nhập ghi chú"
                        {...register("GhiChu")}
                      />
                      {errors.GhiChu && (
                        <span className="text-danger">
                          {errors.GhiChu.message}
                        </span>
                      )}
                    </div>

                    <div className="form-group">
                      <label htmlFor="FileContact">Tải lên tệp Hợp Đồng</label>
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
                      <label htmlFor="TrangThai">Trạng thái</label>
                      <select
                        className="form-control"
                        {...register("TrangThai", Validate.TrangThai)}
                      >
                        <option value="">Chọn trạng thái</option>
                        {listStatusType &&
                          listStatusType.map((val) => {
                            return (
                              <option value={val.statusId} key={val.statusId}>
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
              <div>{IsLoading === true && <div>Loading...</div>}</div>

              {IsLoading === false && (
                <form onSubmit={handleSubmit(onSubmit)}>
                  <div className="card-body">
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="KhachHang">
                            Khách Hàng/ Nhà Cung Cấp
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
                          <label htmlFor="SoHopDongCha">Số hợp đồng cha</label>
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
                          <label htmlFor="MaHopDong">Mã Phụ Lục Hợp Đồng</label>
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
                            Tên Phụ Lục Hợp Đồng
                          </label>
                          <input
                            type="text"
                            className="form-control"
                            id="TenHopDong"
                            placeholder="Nhập tên khách hàng"
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
                      {/* <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="PhanLoaiHopDong">
                            Phân Loại Hợp Đồng
                          </label>
                          <select
                            className="form-control"
                            {...register(
                              "PhanLoaiHopDong",
                              Validate.PhanLoaiHopDong
                            )}
                          >
                            <option value="">Chọn phân loại HĐ</option>
                            {listContractType &&
                              listContractType.map((val) => {
                                return (
                                  <option
                                    value={val.maLoaiHopDong}
                                    key={val.maLoaiHopDong}
                                  >
                                    {val.tenLoaiHopDong}
                                  </option>
                                );
                              })}
                          </select>
                          {errors.PhanLoaiHopDong && (
                            <span className="text-danger">
                              {errors.PhanLoaiHopDong.message}
                            </span>
                          )}
                        </div>
                      </div> */}
                      {/* <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="PTVC">Phương thức vận chuyển</label>
                          <select
                            className="form-control"
                            {...register("PTVC", Validate.PTVC)}
                          >
                            <option value="">
                              Chọn phương thức vận chuyển
                            </option>
                            {listTransportType &&
                              listTransportType.map((val) => {
                                return (
                                  <option value={val.maPtvc} key={val.maPtvc}>
                                    {val.tenPtvc}
                                  </option>
                                );
                              })}
                          </select>
                          {errors.PTVC && (
                            <span className="text-danger">
                              {errors.PTVC.message}
                            </span>
                          )}
                        </div>
                      </div> */}
                    </div>
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="NgayBatDau">Ngày bắt đầu</label>
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
                          <label htmlFor="NgayKetThuc">Ngày kết thúc</label>
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
                    {/* <div className="form-group">
                      <label htmlFor="PhuPhi">Phụ Phí</label>
                      <input
                        type="text"
                        className="form-control"
                        id="PhuPhi"
                        placeholder="Nhập phụ phí"
                        {...register("PhuPhi", Validate.PhuPhi)}
                      />
                      {errors.PhuPhi && (
                        <span className="text-danger">
                          {errors.PhuPhi.message}
                        </span>
                      )}
                    </div> */}
                    <div className="form-group">
                      <label htmlFor="GhiChu">Ghi Chú</label>
                      <input
                        type="text"
                        className="form-control"
                        id="GhiChu"
                        placeholder="Nhập ghi chú"
                        {...register("GhiChu")}
                      />
                      {errors.GhiChu && (
                        <span className="text-danger">
                          {errors.GhiChu.message}
                        </span>
                      )}
                    </div>

                    <div className="form-group">
                      <label htmlFor="FileContact">Tải lên tệp Hợp Đồng</label>
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
                      <label htmlFor="TrangThai">Trạng thái</label>
                      <select
                        className="form-control"
                        {...register("TrangThai", Validate.TrangThai)}
                      >
                        <option value="">Chọn trạng thái</option>
                        {listStatusType &&
                          listStatusType.map((val) => {
                            return (
                              <option value={val.statusId} key={val.statusId}>
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
