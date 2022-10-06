import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import moment from "moment/moment";

const CreateTransport = (props) => {
  const { getListContract, listContractType, listStatus } = props;

  const [IsLoading, SetIsLoading] = useState(false);
  const {
    register,
    reset,
    setValue,
    watch,
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onSubmit",
    defaultValues: {
      DiemLayRong: { value: 0, label: "Empty" },
    },
  });

  const Validate = {
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
    MaBangGia: {
      required: "Không được để trống",
      valueAsNumber: {
        value: true,
        message: "Không đúng định dạng",
      },
    },
    MaCungDuong: {
      required: "Không được để trống",
    },
    DiemLayHang: { required: "Không được để trống" },
    DiemNhapHang: { required: "Không được để trống" },
    MaDonHang: { required: "Không được để trống" },
    MaTaiXe: { required: "Không được để trống" },
    XeVanChuyen: { required: "Không được để trống" },
    MaRomooc: {},
    CONTNO: {
      required: "Không được để trống",
      maxLength: {
        value: 11,
        message: "Không được ít hơn 11 ký tự",
      },
      minLength: {
        value: 11,
        message: "Không được ít hơn 11 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    SEALHQ: {
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 ]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    SEALHT: {
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 ]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TrongLuong: {
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9.]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TheTich: {
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9.]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TGLayRong: {
      required: "Không được để trống",
      pattern: {
        value:
          /^([1-9]|([012][0-9])|(3[01]))-([0]{0,1}[1-9]|1[012])-\d\d\d\d [012]{0,1}[0-9]:[0-6][0-9]$/,
        message: "Định dạng ngày không đúng",
      },
    },
    TGTraRong: {
      required: "Không được để trống",
      pattern: {
        value:
          /^([1-9]|([012][0-9])|(3[01]))-([0]{0,1}[1-9]|1[012])-\d\d\d\d [012]{0,1}[0-9]:[0-6][0-9]$/,
        message: "Định dạng ngày không đúng",
      },
    },
    TGCoMat: {
      required: "Không được để trống",
      pattern: {
        value:
          /^([1-9]|([012][0-9])|(3[01]))-([0]{0,1}[1-9]|1[012])-\d\d\d\d [012]{0,1}[0-9]:[0-6][0-9]$/,
        message: "Định dạng ngày không đúng",
      },
    },
    TGLech: {
      required: "Không được để trống",
      pattern: {
        value:
          /^([1-9]|([012][0-9])|(3[01]))-([0]{0,1}[1-9]|1[012])-\d\d\d\d [012]{0,1}[0-9]:[0-6][0-9]$/,
        message: "Định dạng ngày không đúng",
      },
    },
  };
  const [tabIndex, setTabIndex] = useState(0);
  const [listDVT, setListDVT] = useState([]);
  const [listPTVC, setListPTVC] = useState([]);
  const [listCustomer, setListCustomer] = useState([]);
  const [listVehicleType, setlistVehicleType] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listPoint, setListPoint] = useState([]);
  const [listDriver, setListDriver] = useState([]);
  const [listVehicle, setListVehicle] = useState([]);
  const [listRomooc, setListRomooc] = useState([]);
  const [listRoad, setListRoad] = useState([]);
  const [listPriceTable, setListPriceTable] = useState([]);
  const [listFilterPriceTable, setListFilterPriceTable] = useState([]);

  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex, reset());
  };

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      const getlistPoint = await getData("address/GetListAddressSelect");
      if (getlistPoint && getlistPoint.length > 0) {
        var obj = [];
        obj.push({ value: 0, label: "Empty" });
        getlistPoint.map((val) => {
          obj.push({
            value: val.maDiaDiem,
            label:
              val.maDiaDiem + " - " + val.tenDiaDiem + " --- " + val.diaChi,
          });
        });
        setListPoint(obj);
      }

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

      let getListDVT = await getData("Common/GetListDVT");
      let getListVehicleType = await getData("Common/GetListVehicleType");
      let getListGoodsType = await getData("Common/GetListGoodsType");
      let getListTransportType = await getData("Common/GetListTransportType");

      setValue("DiemLayRong", { value: 0, label: "Empty" });
      setListDVT(getListDVT);
      setlistVehicleType(getListVehicleType);
      setListGoodsType(getListGoodsType);
      setListPTVC(getListTransportType);

      SetIsLoading(false);
    })();
  }, []);

  const handleOnchangeListCustomer = async (value) => {
    handleResetClick();
    setListPriceTable([]);

    let getListPriceTable = await getData(
      `PriceTable/GetListPriceTableByCustomerId?Id=${value.value}`
    );

    if (getListPriceTable && getListPriceTable.length > 0) {
      setListFilterPriceTable(getListPriceTable);
      setValue("MaKh", value);
    } else {
      setListPriceTable([]);
    }
  };

  const handleOnChangePoint = async () => {
    setListRoad([]);
    setListPriceTable([]);
    setValue("MaCungDuong", null);
    let diemLayRong = watch("DiemLayRong");
    let diemLayHang = watch("DiemLayHang");
    let diemNhapHang = watch("DiemNhapHang");

    if (diemLayHang && diemNhapHang) {
      const getlistRoadByPoint = await getData(
        `Road/GetListRoadByPoint?diemDau=${diemLayHang.value}&diemCuoi=${diemNhapHang.value}&diemLayRong=${diemLayRong.value}`
      );
      if (getlistRoadByPoint && getlistRoadByPoint.length > 0) {
        var obj = [];
        getlistRoadByPoint.map((val) => {
          obj.push({
            value: val.maCungDuong,
            label: val.maCungDuong + " - " + val.tenCungDuong,
          });
        });
        setListRoad(obj);
      } else {
        setListRoad([]);
      }
    }
  };

  const handleOnChangeFilter = () => {
    setListPriceTable([]);
    let maDVT = watch("DVT");
    let maPTVC = watch("PTVC");
    let maPTVanChuyen = watch("PTVanChuyen");
    let maLoaiHH = watch("LoaiHangHoa");
    let maCungDuong = watch("MaCungDuong");

    let data = listFilterPriceTable;
    let tempData = data;

    let filterByTransportType;
    let filterByDVT;
    let filterByVehicleType;
    let filterByGoodsType;
    let filterByRoad;

    if (maPTVC !== "") {
      filterByTransportType = tempData.filter((x) => x.maPTVC === maPTVC);
    } else {
      filterByTransportType = tempData;
    }
    tempData = filterByTransportType;

    if (maDVT !== "") {
      filterByDVT = tempData.filter((x) => x.maDVT === maDVT);
    } else {
      filterByDVT = tempData;
    }
    tempData = filterByDVT;

    if (maPTVanChuyen !== "") {
      filterByVehicleType = tempData.filter(
        (x) => x.maLoaiPhuongTien === maPTVanChuyen
      );
    } else {
      filterByVehicleType = tempData;
    }
    tempData = filterByVehicleType;

    if (maLoaiHH !== "") {
      filterByGoodsType = tempData.filter((x) => x.maLoaiHangHoa === maLoaiHH);
    } else {
      filterByGoodsType = tempData;
    }
    tempData = filterByGoodsType;

    if (maCungDuong) {
      filterByRoad = tempData.filter(
        (x) => x.maCungDuong === maCungDuong.value
      );

      if (filterByRoad && filterByRoad.length > 0) {
        setValue("MaBangGia", filterByRoad[0].id);
      } else {
        setValue("MaBangGia", null);
      }
      setListPriceTable(filterByRoad);
    } else {
      setListPriceTable([]);
    }
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);
    console.log(watch());
    SetIsLoading(false);
  };

  const handleResetClick = () => {
    reset();
    setValue("DiemLayHang", null);
    setValue("DiemNhapHang", null);
    setValue("MaCungDuong", null);
    setListRoad([]);
    setListPriceTable([]);
  };

  return (
    <>
      <Tabs
        selectedIndex={tabIndex}
        onSelect={(index) => HandleOnChangeTabs(index)}
      >
        <TabList>
          <Tab>Tạo Vận Đơn Nhập</Tab>
          <Tab>Tạo Vận Đơn Xuất</Tab>
        </TabList>

        <TabPanel>
          {tabIndex === 0 && (
            <div className="card card-primary">
              <div className="card-header">
                <h3 className="card-title">Form Thêm Mới Vận Đơn Nhập</h3>
              </div>
              <div>{IsLoading === true && <div>Loading...</div>}</div>

              {IsLoading === false && (
                <form onSubmit={handleSubmit(onSubmit)}>
                  <div className="card-body">
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="KhachHang">
                            Khách Hàng/ Nhà cung cấp
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
                                onChange={(field) =>
                                  handleOnchangeListCustomer(field)
                                }
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
                          <label htmlFor="PTVC">Phương thức vận chuyển</label>
                          <select
                            className="form-control"
                            {...register("PTVC")}
                            onChange={(e) =>
                              handleOnChangeFilter(
                                setValue("PTVC", e.target.value)
                              )
                            }
                            value={watch("PTVC")}
                          >
                            <option value="">
                              Chọn phương thức vận chuyển
                            </option>
                            {listPTVC &&
                              listPTVC.map((val) => {
                                return (
                                  <option value={val.maPtvc} key={val.maPtvc}>
                                    {val.tenPtvc}
                                  </option>
                                );
                              })}
                          </select>
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="DVT">Đơn vị tính</label>
                          <select
                            className="form-control"
                            {...register("DVT")}
                            onChange={(e) =>
                              handleOnChangeFilter(
                                setValue("DVT", e.target.value)
                              )
                            }
                            value={watch("DVT")}
                          >
                            <option value="">Chọn Đơn Vị Tính</option>
                            {listDVT &&
                              listDVT.map((val) => {
                                return (
                                  <option value={val.maDvt} key={val.maDvt}>
                                    {val.tenDvt}
                                  </option>
                                );
                              })}
                          </select>
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="PTVanChuyen">
                            Phương tiện vận chuyển
                          </label>
                          <select
                            className="form-control"
                            {...register("PTVanChuyen")}
                            onChange={(e) =>
                              handleOnChangeFilter(
                                setValue("PTVanChuyen", e.target.value)
                              )
                            }
                            value={watch("PTVanChuyen")}
                          >
                            <option value="">
                              Chọn phương Tiện Vận Chuyển
                            </option>
                            {listVehicleType &&
                              listVehicleType.map((val) => {
                                return (
                                  <option
                                    value={val.maLoaiPhuongTien}
                                    key={val.maLoaiPhuongTien}
                                  >
                                    {val.tenLoaiPhuongTien}
                                  </option>
                                );
                              })}
                          </select>
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="LoaiHangHoa">Loại Hàng Hóa</label>
                          <select
                            className="form-control"
                            {...register("LoaiHangHoa")}
                            onChange={(e) =>
                              handleOnChangeFilter(
                                setValue("LoaiHangHoa", e.target.value)
                              )
                            }
                            value={watch("LoaiHangHoa")}
                          >
                            <option value="">Chọn Loại Hàng Hóa</option>
                            {listGoodsType &&
                              listGoodsType.map((val) => {
                                return (
                                  <option
                                    value={val.maLoaiHangHoa}
                                    key={val.maLoaiHangHoa}
                                  >
                                    {val.tenLoaiHangHoa}
                                  </option>
                                );
                              })}
                          </select>
                        </div>
                      </div>
                    </div>
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="DiemLayRong">Điểm Lấy/Trả Rỗng</label>
                          <Controller
                            name="DiemLayRong"
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={listPoint}
                                onChange={(field) =>
                                  handleOnChangePoint(
                                    setValue("DiemLayRong", field)
                                  )
                                }
                              />
                            )}
                            rules={Validate.MaKh}
                          />
                          {errors.DiemLayRong && (
                            <span className="text-danger">
                              {errors.DiemLayRong.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="DiemLayHang">Điểm Lấy Hàng</label>
                          <Controller
                            name="DiemLayHang"
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={listPoint.filter((x) => x.value !== 0)}
                                onChange={(field) =>
                                  handleOnChangePoint(
                                    setValue("DiemLayHang", field)
                                  )
                                }
                              />
                            )}
                            rules={Validate.DiemLayHang}
                          />
                          {errors.DiemLayHang && (
                            <span className="text-danger">
                              {errors.DiemLayHang.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="DiemNhapHang">Điểm nhập hàng</label>
                          <Controller
                            name="DiemNhapHang"
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={listPoint.filter((x) => x.value !== 0)}
                                onChange={(field) =>
                                  handleOnChangePoint(
                                    setValue("DiemNhapHang", field)
                                  )
                                }
                              />
                            )}
                            rules={Validate.DiemNhapHang}
                          />
                          {errors.DiemNhapHang && (
                            <span className="text-danger">
                              {errors.DiemNhapHang.message}
                            </span>
                          )}
                        </div>
                        <div className="col col-sm"></div>
                      </div>
                    </div>
                    <div className="row">
                      <div className="col col-sm">
                        <div className="col col-sm">
                          <div className="form-group">
                            <label htmlFor="MaCungDuong">Cung Đường</label>
                            <Controller
                              name="MaCungDuong"
                              control={control}
                              render={({ field }) => (
                                <Select
                                  {...field}
                                  classNamePrefix={"form-control"}
                                  value={field.value}
                                  options={listRoad}
                                  onChange={(field) =>
                                    handleOnChangeFilter(
                                      setValue("MaCungDuong", field)
                                    )
                                  }
                                />
                              )}
                              rules={Validate.MaCungDuong}
                            />
                            {errors.MaCungDuong && (
                              <span className="text-danger">
                                {errors.MaCungDuong.message}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="col col-sm">
                          <div className="form-group">
                            <label htmlFor="MaBangGia">Bảng giá</label>
                            <select
                              className="form-control"
                              {...register("MaBangGia")}
                            >
                              {listPriceTable &&
                                listPriceTable.map((val) => {
                                  return (
                                    <option value={val.id} key={val.id}>
                                      {val.donGia}
                                    </option>
                                  );
                                })}
                            </select>
                            {errors.MaBangGia && (
                              <span className="text-danger">
                                {errors.MaBangGia.message}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                    </div>
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="MaDonHang">Mã Đơn Hàng</label>
                          <input
                            autoComplete="false"
                            type="text"
                            className="form-control"
                            id="MaDonHang"
                            {...register("MaDonHang", Validate.MaDonHang)}
                          />
                          {errors.MaDonHang && (
                            <span className="text-danger">
                              {errors.MaDonHang.message}
                            </span>
                          )}
                        </div>
                      </div>
                      {/* <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="DonViVanTai">Đơn vị vận tải</label>
                          <input
                            autoComplete="false"
                            type="text"
                            className="form-control"
                            id="DonViVanTai"
                            {...register("DonViVanTai", Validate.DonViVanTai)}
                          />
                          {errors.DonViVanTai && (
                            <span className="text-danger">
                              {errors.DonViVanTai.message}
                            </span>
                          )}
                        </div>
                      </div> */}
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="MaTaiXe">Tài Xế</label>
                          <Controller
                            name="MaTaiXe"
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={null}
                              />
                            )}
                            rules={Validate.MaTaiXe}
                          />
                          {errors.MaTaiXe && (
                            <span className="text-danger">
                              {errors.MaTaiXe.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="XeVanChuyen">Xe Vận Chuyển</label>
                          <Controller
                            name="XeVanChuyen"
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={null}
                              />
                            )}
                            rules={Validate.XeVanChuyen}
                          />
                          {errors.XeVanChuyen && (
                            <span className="text-danger">
                              {errors.XeVanChuyen.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="MaRomooc">Romooc</label>
                          <Controller
                            name="MaRomooc"
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={null}
                              />
                            )}
                            rules={Validate.MaRomooc}
                          />
                          {errors.MaRomooc && (
                            <span className="text-danger">
                              {errors.MaRomooc.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="CONTNO">CONT NO</label>
                          <input
                            autoComplete="false"
                            type="text"
                            className="form-control"
                            id="CONTNO"
                            {...register("CONTNO", Validate.CONTNO)}
                          />
                          {errors.CONTNO && (
                            <span className="text-danger">
                              {errors.CONTNO.message}
                            </span>
                          )}
                        </div>
                      </div>
                      {/* <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="CPLNO">CPL NO</label>
                          <input
                            autoComplete="false"
                            type="text"
                            className="form-control"
                            id="CPLNO"
                            {...register("CPLNO", Validate.CPLNO)}
                          />
                          {errors.CPLNO && (
                            <span className="text-danger">
                              {errors.CPLNO.message}
                            </span>
                          )}
                        </div>
                      </div> */}
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="SEALHQ">SEAL HQ</label>
                          <input
                            autoComplete="false"
                            type="text"
                            className="form-control"
                            id="SEALHQ"
                            {...register("SEALHQ", Validate.SEALHQ)}
                          />
                          {errors.SEALHQ && (
                            <span className="text-danger">
                              {errors.SEALHQ.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="SEALHT">SEAL HT</label>
                          <input
                            autoComplete="false"
                            type="text"
                            className="form-control"
                            id="SEALHT"
                            {...register("SEALHT", Validate.SEALHT)}
                          />
                          {errors.SEALHT && (
                            <span className="text-danger">
                              {errors.SEALHT.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="TrongLuong">Trọng Lượng</label>
                          <input
                            autoComplete="false"
                            type="text"
                            className="form-control"
                            id="TrongLuong"
                            {...register("TrongLuong", Validate.TrongLuong)}
                          />
                          {errors.TrongLuong && (
                            <span className="text-danger">
                              {errors.TrongLuong.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="TheTich">Thể tích</label>
                          <input
                            autoComplete="false"
                            type="text"
                            className="form-control"
                            id="TheTich"
                            {...register("TheTich", Validate.TheTich)}
                          />
                          {errors.TheTich && (
                            <span className="text-danger">
                              {errors.TheTich.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>

                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="TGLayRong">Thời gian lấy rỗng</label>
                          <div className="input-group ">
                            <Controller
                              control={control}
                              name="TGLayRong"
                              render={({ field }) => (
                                <DatePicker
                                  className="form-control"
                                  showTimeSelect
                                  timeFormat="HH:mm"
                                  dateFormat="dd/MM/yyyy HH:mm"
                                  onChange={(date) => field.onChange(date)}
                                  selected={field.value}
                                />
                              )}
                              rules={Validate.TGLayRong}
                            />
                            {errors.TGLayRong && (
                              <span className="text-danger">
                                {errors.TGLayRong.message}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="TGTraRong">Thời Gian Trả Rỗng</label>
                          <div className="input-group ">
                            <Controller
                              control={control}
                              name="TGTraRong"
                              render={({ field }) => (
                                <DatePicker
                                  className="form-control"
                                  showTimeSelect
                                  timeFormat="HH:mm"
                                  dateFormat="dd/MM/yyyy HH:mm"
                                  onChange={(date) => field.onChange(date)}
                                  selected={field.value}
                                />
                              )}
                              rules={Validate.TGTraRong}
                            />
                            {errors.TGTraRong && (
                              <span className="text-danger">
                                {errors.TGTraRong.message}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="TGCoMat">
                            Thời gian có mặt tại kho
                          </label>
                          <div className="input-group ">
                            <Controller
                              control={control}
                              name="TGCoMat"
                              render={({ field }) => (
                                <DatePicker
                                  className="form-control"
                                  showTimeSelect
                                  timeFormat="HH:mm"
                                  dateFormat="dd/MM/yyyy HH:mm"
                                  onChange={(date) => field.onChange(date)}
                                  selected={field.value}
                                />
                              )}
                              rules={Validate.TGCoMat}
                            />
                            {errors.TGCoMat && (
                              <span className="text-danger">
                                {errors.TGCoMat.message}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="TGLech">Thời gian hạn lệch</label>
                          <div className="input-group ">
                            <Controller
                              control={control}
                              name="TGLech"
                              render={({ field }) => (
                                <DatePicker
                                  className="form-control"
                                  showTimeSelect
                                  timeFormat="HH:mm"
                                  dateFormat="dd/MM/yyyy HH:mm"
                                  onChange={(date) => field.onChange(date)}
                                  selected={field.value}
                                />
                              )}
                              rules={Validate.TGLech}
                            />
                            {errors.TGLech && (
                              <span className="text-danger">
                                {errors.TGLech.message}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                    </div>

                    {/* <div className="form-group">
                      <label htmlFor="TrangThai">Trạng thái</label>
                      <select
                        className="form-control"
                        {...register("TrangThai", Validate.TrangThai)}
                      >
                        <option value="">Chọn trạng thái</option>
                        {listStatus &&
                          listStatus.map((val) => {
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
                    </div> */}
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
                <h3 className="card-title">Form Thêm Mới Vận Đơn Xuất</h3>
              </div>
              <div>{IsLoading === true && <div>Loading...</div>}</div>

              {IsLoading === false && (
                <form onSubmit={handleSubmit(onSubmit)}>
                  <div className="card-body">
                    <div className="form-group">
                      <label htmlFor="TrangThai">Trạng thái</label>
                      <select
                        className="form-control"
                        {...register("TrangThai", Validate.TrangThai)}
                      >
                        <option value="">Chọn trạng thái</option>
                        {listStatus &&
                          listStatus.map((val) => {
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

export default CreateTransport;
