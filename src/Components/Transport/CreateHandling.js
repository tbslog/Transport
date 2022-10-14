import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import moment from "moment/moment";

const CreateHandling = (props) => {
  const { listStatus } = props;

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
      optionTransport: [
        {
          NhaCungCap: null,
          KhachHang: null,
          XeVanChuyen: null,
          TaiXe: null,
          Romooc: null,
          PTVanChuyen: "",
          LoaiHangHoa: "",
          MaBangGia: "",
          GiaThucTe: "",
          CONTNO: "",
          SEALHQ: "",
          SEALHT: "",
          TrongLuong: "",
          TheTich: "",
          TGLayRong: "",
          TGTraRong: "",
          TGCoMat: "",
          TGLech: "",
          TGNhapHang: "",
          TGKeoCont: "",
          GhiChu: "",
        },
      ],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control, // control props comes from useForm (optional: if you are using FormContext)
    name: "optionTransport", // unique name for your Field Array
  });

  const Validate = {
    NhaCungCap: {
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
    MaCungDuong: {
      required: "Không được để trống",
    },
    MaDonHang: { required: "Không được để trống" },
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
      required: "Không được để trống",
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9.]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TheTich: {
      required: "Không được để trống",
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
    GiaThucTe: {
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
      required: "Không được để trống",
    },
    PTVanChuyen: {
      required: "Không được để trống",
    },
    LoaiHangHoa: {
      required: "Không được để trống",
    },
    PTVC: {
      required: "Không được để trống",
    },
    DVT: {
      required: "Không được để trống",
    },
  };

  const [listPriceTable, setListPriceTable] = useState([]);
  const [tabIndex, setTabIndex] = useState(0);
  const [listDVT, setListDVT] = useState([]);
  const [listPTVC, setListPTVC] = useState([]);
  const [listCustomer, setListCustomer] = useState([]);
  const [listNpp, setListNpp] = useState([]);
  const [listVehicleType, setlistVehicleType] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listDriver, setListDriver] = useState([]);
  const [listVehicle, setListVehicle] = useState([]);
  const [listRomooc, setListRomooc] = useState([]);
  const [listRoad, setListRoad] = useState([]);

  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex, reset());
  };

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      let getListRoad = await getData(`Road/GetListRoadOptionSelect`);
      if (getListRoad && getListRoad.length > 0) {
        let obj = [];
        getListRoad.map((val) => {
          obj.push({
            value: val.maCungDuong,
            label: val.maCungDuong + " - " + val.tenCungDuong,
          });
        });
        setListRoad(obj);
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

  const handleOnchangeListRoad = async (value) => {
    SetIsLoading(true);
    setValue("MaCungDuong", value);

    const getlistData = await getData(
      `BillOfLading/LoadDataTransport?RoadId=${value.value}`
    );

    if (getlistData && Object.keys(getlistData).length > 0) {
      let objCustomer = [];
      getlistData.listKhachHang.map((val) => {
        objCustomer.push({
          value: val.maKH,
          label: val.maKH + " - " + val.tenKH,
        });
      });
      setListCustomer(objCustomer);

      let objNpp = [];
      objNpp.push({ value: "TBSL", label: "TBS Logistics" });
      getlistData.listNhaPhanPhoi.map((val) => {
        objNpp.push({
          value: val.maNPP,
          label: val.maNPP + " - " + val.tenNPP,
        });
      });
      setListNpp(objNpp);

      let objDriver = [];
      getlistData.listTaiXe.map((val) => {
        objDriver.push({
          value: val.maTaiXe,
          label: val.maTaiXe + " - " + val.tenTaiXe,
        });
      });
      setListDriver(objDriver);

      let objVehicle = [];
      getlistData.listXeVanChuyen.map((val) => {
        objVehicle.push({
          value: val.maSoXe,
          label: val.maSoXe + " - " + val.maLoaiPhuongTien,
        });
      });
      setListVehicle(objVehicle);

      let objRomooc = [];
      getlistData.listRomooc.map((val) => {
        objRomooc.push({
          value: val.maRomooc,
          label: val.maRomooc + " - " + val.tenLoaiRomooc,
        });
      });
      setListRomooc(objRomooc);

      setListPriceTable(getlistData.bangGiaVanDon);
    }

    SetIsLoading(false);
  };

  const handleOnChangeFilter = () => {
    let maDVT = watch("DVT");
    let maPTVC = watch("PTVC");
    let data = listPriceTable;
    let tempData = data;

    let filterByTransportType;
    let filterByDVT;

    if (maPTVC !== "") {
      filterByTransportType = tempData.filter((x) => x.ptvc === maPTVC);
    } else {
      filterByTransportType = tempData;
    }
    tempData = filterByTransportType;

    if (maDVT !== "") {
      filterByDVT = tempData.filter((x) => x.dvt === maDVT);
    } else {
      filterByDVT = tempData;
    }
    tempData = filterByDVT;

    setListPriceTable(tempData);

    console.log(tempData);
  };

  const handleOnChangeFilterArr = (index, value, nameVar) => {
    if (nameVar === "PTVanChuyen") {
      setValue(`optionTransport.${index}.PTVanChuyen`, value);
    }
    if (nameVar === "LoaiHangHoa") {
      setValue(`optionTransport.${index}.LoaiHangHoa`, value);
    }
    if (nameVar === "NhaCungCap") {
      setValue(
        `optionTransport.${index}.NhaCungCap`,
        {
          ...listNpp.filter((x) => x.value === value),
        }[0]
      );
    }
    if (nameVar === "KhachHang") {
      setValue(
        `optionTransport.${index}.KhachHang`,
        {
          ...listCustomer.filter((x) => x.value === value),
        }[0]
      );
    }

    let data = listPriceTable;
    let tempData = data;

    let NhaPhanPhoi = watch(`optionTransport.${index}.NhaCungCap`);
    let PTVanChuyen = watch(`optionTransport.${index}.PTVanChuyen`);
    let LoaiHangHoa = watch(`optionTransport.${index}.LoaiHangHoa`);
    let KhachHang = watch(`optionTransport.${index}.KhachHang`);

    if (nameVar === "NhaCungCap") {
      if (NhaPhanPhoi !== null) {
        if (NhaPhanPhoi.value === "TBSL") {
          tempData = tempData.filter((x) => x.phanLoaiDoiTac === "KH");
        } else {
          tempData = tempData.filter((x) => x.maDoiTac === NhaPhanPhoi.value);
        }
      }
    }

    if (PTVanChuyen !== "") {
      tempData = tempData.filter((x) => x.ptVanChuyen === PTVanChuyen);
    }

    if (LoaiHangHoa !== "") {
      tempData = tempData.filter((x) => x.loaiHangHoa === LoaiHangHoa);
    }
    if (NhaPhanPhoi.value === "TBSL") {
      if (KhachHang !== null) {
        tempData = tempData.filter((x) => x.maDoiTac === KhachHang.value);
      }
    } else {
      tempData = tempData.filter((x) => x.maDoiTac === NhaPhanPhoi.value);
    }

    if (tempData && tempData.length === 1) {
      setValue(`optionTransport.${index}.MaBangGia`, tempData[0].price);
    } else {
      setValue(`optionTransport.${index}.MaBangGia`, null);
    }
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);
    console.log(data);
    SetIsLoading(false);
  };

  const handleResetClick = () => {
    reset();
    setListNpp([]);
    setListCustomer([]);
    setValue("NhaCungCap", null);
    setValue("MaCungDuong", null);
    setValue("optionTransport", [{}]);
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
                                  handleOnchangeListRoad(field)
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
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="PTVC">Phương thức vận chuyển</label>
                          <select
                            className="form-control"
                            {...register("PTVC", Validate.PTVC)}
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
                          {errors.PTVC && (
                            <span className="text-danger">
                              {errors.PTVC.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="DVT">Đơn vị tính</label>
                          <select
                            className="form-control"
                            {...register("DVT", Validate.DVT)}
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
                          {errors.DVT && (
                            <span className="text-danger">
                              {errors.DVT.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <br />
                    </div>
                    <div>
                      {fields.map((value, index) => (
                        <div className="row" key={index}>
                          <div className="col col-sm">
                            <div className="card card-info">
                              <div className="card-header">
                                <h3 className="card-title">
                                  Thùng Hàng Số {index + 1}
                                </h3>
                                <div className="card-tools">
                                  {index >= 1 && (
                                    <button
                                      type="button"
                                      className="btn btn-tool"
                                      onClick={() => remove(index)}
                                    >
                                      <i className="fas fa-minus"></i>
                                    </button>
                                  )}
                                  <button
                                    type="button"
                                    className="btn btn-tool"
                                    onClick={() => append({})}
                                  >
                                    <i className="fas fa-plus" />
                                  </button>
                                </div>
                              </div>

                              <div className="card-body">
                                <div className="row">
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="NhaCungCap">
                                        Đơn Vị Vận Tải
                                      </label>
                                      <Controller
                                        name={`optionTransport.${index}.NhaCungCap`}
                                        control={control}
                                        render={({ field }) => (
                                          <Select
                                            {...field}
                                            classNamePrefix={"form-control"}
                                            value={field.value}
                                            options={listNpp}
                                            onChange={(field) =>
                                              handleOnChangeFilterArr(
                                                index,
                                                field.value,
                                                "NhaCungCap"
                                              )
                                            }
                                          />
                                        )}
                                        rules={{
                                          required: "không được để trống",
                                        }}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.NhaCungCap && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.NhaCungCap.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="KhachHang">
                                        Khách Hàng
                                      </label>
                                      <Controller
                                        name={`optionTransport.${index}.KhachHang`}
                                        control={control}
                                        render={({ field }) => (
                                          <Select
                                            {...field}
                                            classNamePrefix={"form-control"}
                                            value={field.value}
                                            options={listCustomer}
                                            onChange={(field) =>
                                              handleOnChangeFilterArr(
                                                index,
                                                field.value,
                                                "KhachHang"
                                              )
                                            }
                                          />
                                        )}
                                        rules={{
                                          required: "không được để trống",
                                        }}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.KhachHang && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.KhachHang.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="PTVanChuyen">
                                        Phương tiện vận chuyển
                                      </label>
                                      <select
                                        className="form-control"
                                        {...register(
                                          `optionTransport.${index}.PTVanChuyen`,
                                          Validate.PTVanChuyen
                                        )}
                                        onChange={(e) =>
                                          handleOnChangeFilterArr(
                                            index,
                                            e.target.value,
                                            "PTVanChuyen"
                                          )
                                        }
                                        value={watch(
                                          `optionTransport.${index}.PTVanChuyen`
                                        )}
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
                                      {errors.optionTransport?.[index]
                                        ?.PTVanChuyen && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.PTVanChuyen.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="LoaiHangHoa">
                                        Loại Hàng Hóa
                                      </label>
                                      <select
                                        className="form-control"
                                        {...register(
                                          `optionTransport.${index}.LoaiHangHoa`,
                                          Validate.LoaiHangHoa
                                        )}
                                        onChange={(e) =>
                                          handleOnChangeFilterArr(
                                            index,
                                            e.target.value,
                                            "LoaiHangHoa"
                                          )
                                        }
                                        value={watch(
                                          `optionTransport.${index}.LoaiHangHoa`
                                        )}
                                      >
                                        <option value="">
                                          Chọn Loại Hàng Hóa
                                        </option>
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
                                      {errors.optionTransport?.[index]
                                        ?.LoaiHangHoa && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.LoaiHangHoa.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="MaBangGia">
                                        Giá Tham Chiếu
                                      </label>
                                      <input
                                        autoComplete="false"
                                        type="text"
                                        className="form-control"
                                        id="MaBangGia"
                                        readOnly
                                        {...register(
                                          `optionTransport.${index}.MaBangGia`,
                                          Validate.MaBangGia
                                        )}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.MaBangGia && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.MaBangGia.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="GiaThucTe">
                                        Giá Thực Tế
                                      </label>
                                      <input
                                        autoComplete="false"
                                        type="text"
                                        className="form-control"
                                        id="GiaThucTe"
                                        {...register(
                                          `optionTransport.${index}.GiaThucTe`,
                                          Validate.GiaThucTe
                                        )}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.GiaThucTe && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.GiaThucTe.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
                                </div>
                                <div className="row">
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="SEALNP">SEAL NP</label>
                                      <input
                                        autoComplete="false"
                                        type="text"
                                        className="form-control"
                                        id="SEALNP"
                                        {...register(
                                          `optionTransport.${index}.SEALNP`,
                                          Validate.SEALNP
                                        )}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.SEALNP && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.SEALNP.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
                                  {watch(
                                    `optionTransport.${index}.PTVanChuyen`
                                  ) &&
                                    watch(
                                      `optionTransport.${index}.PTVanChuyen`
                                    ).includes("CONT") && (
                                      <>
                                        <div className="col col-sm">
                                          <div className="form-group">
                                            <label htmlFor="CONTNO">
                                              CONT NO
                                            </label>
                                            <input
                                              autoComplete="false"
                                              type="text"
                                              className="form-control"
                                              id="CONTNO"
                                              {...register(
                                                `optionTransport.${index}.CONTNO`,
                                                Validate.CONTNO
                                              )}
                                            />
                                            {errors.optionTransport?.[index]
                                              ?.CONTNO && (
                                              <span className="text-danger">
                                                {
                                                  errors.optionTransport?.[
                                                    index
                                                  ]?.CONTNO.message
                                                }
                                              </span>
                                            )}
                                          </div>
                                        </div>
                                        <div className="col col-sm">
                                          <div className="form-group">
                                            <label htmlFor="SEALHQ">
                                              SEAL HQ
                                            </label>
                                            <input
                                              autoComplete="false"
                                              type="text"
                                              className="form-control"
                                              id="SEALHQ"
                                              {...register(
                                                `optionTransport.${index}.SEALHQ`,
                                                Validate.SEALHQ
                                              )}
                                            />
                                            {errors.optionTransport?.[index]
                                              ?.SEALHQ && (
                                              <span className="text-danger">
                                                {
                                                  errors.optionTransport?.[
                                                    index
                                                  ]?.SEALHQ.message
                                                }
                                              </span>
                                            )}
                                          </div>
                                        </div>
                                        <div className="col col-sm">
                                          <div className="form-group">
                                            <label htmlFor="SEALHT">
                                              SEAL HT
                                            </label>
                                            <input
                                              autoComplete="false"
                                              type="text"
                                              className="form-control"
                                              id="SEALHT"
                                              {...register(
                                                `optionTransport.${index}.SEALHT`,
                                                Validate.SEALHT
                                              )}
                                            />
                                            {errors.optionTransport?.[index]
                                              ?.SEALHT && (
                                              <span className="text-danger">
                                                {
                                                  errors.optionTransport?.[
                                                    index
                                                  ]?.SEALHT.message
                                                }
                                              </span>
                                            )}
                                          </div>
                                        </div>
                                      </>
                                    )}

                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="TrongLuong">
                                        Trọng Lượng
                                      </label>
                                      <input
                                        autoComplete="false"
                                        type="text"
                                        className="form-control"
                                        id="TrongLuong"
                                        {...register(
                                          `optionTransport.${index}.TrongLuong`,
                                          Validate.TrongLuong
                                        )}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.TrongLuong && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.TrongLuong.message
                                          }
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
                                        {...register(
                                          `optionTransport.${index}.TheTich`,
                                          Validate.TheTich
                                        )}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.TheTich && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.TheTich.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
                                </div>
                                <div className="row">
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="XeVanChuyen">
                                        Xe Vận Chuyển
                                      </label>
                                      <Controller
                                        name={`optionTransport.${index}.XeVanChuyen`}
                                        control={control}
                                        render={({ field }) => (
                                          <Select
                                            {...field}
                                            classNamePrefix={"form-control"}
                                            value={field.value}
                                            options={listVehicle}
                                          />
                                        )}
                                        rules={{
                                          required: "không được để trống",
                                        }}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.XeVanChuyen && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.XeVanChuyen.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="TaiXe">Tài Xế</label>
                                      <Controller
                                        name={`optionTransport.${index}.TaiXe`}
                                        control={control}
                                        render={({ field }) => (
                                          <Select
                                            {...field}
                                            classNamePrefix={"form-control"}
                                            value={field.value}
                                            options={listDriver}
                                          />
                                        )}
                                        rules={{
                                          required: "không được để trống",
                                        }}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.TaiXe && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.TaiXe.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
                                  {watch(
                                    `optionTransport.${index}.PTVanChuyen`
                                  ) &&
                                    watch(
                                      `optionTransport.${index}.PTVanChuyen`
                                    ).includes("CONT") && (
                                      <>
                                        {" "}
                                        <div className="col col-sm">
                                          <div className="form-group">
                                            <label htmlFor="Romooc">
                                              Số Romooc
                                            </label>
                                            <Controller
                                              name={`optionTransport.${index}.Romooc`}
                                              control={control}
                                              render={({ field }) => (
                                                <Select
                                                  {...field}
                                                  classNamePrefix={
                                                    "form-control"
                                                  }
                                                  value={field.value}
                                                  options={listRomooc}
                                                />
                                              )}
                                              rules={{
                                                required: "không được để trống",
                                              }}
                                            />
                                            {errors.optionTransport?.[index]
                                              ?.Romooc && (
                                              <span className="text-danger">
                                                {
                                                  errors.optionTransport?.[
                                                    index
                                                  ]?.Romooc.message
                                                }
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
                                      <label htmlFor="TGCoMat">
                                        Thời gian có mặt
                                      </label>
                                      <div className="input-group ">
                                        <Controller
                                          control={control}
                                          name={`optionTransport.${index}.TGCoMat`}
                                          render={({ field }) => (
                                            <DatePicker
                                              className="form-control"
                                              showTimeSelect
                                              timeFormat="HH:mm"
                                              dateFormat="dd/MM/yyyy HH:mm"
                                              onChange={(date) =>
                                                field.onChange(date)
                                              }
                                              selected={field.value}
                                            />
                                          )}
                                          rules={{
                                            required: "không được để trống",
                                          }}
                                        />
                                        {errors.optionTransport?.[index]
                                          ?.TGCoMat && (
                                          <span className="text-danger">
                                            {
                                              errors.optionTransport?.[index]
                                                ?.TGCoMat.message
                                            }
                                          </span>
                                        )}
                                      </div>
                                    </div>
                                  </div>
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="TGNhapHang">
                                        Thời gian nhập hàng
                                      </label>
                                      <div className="input-group ">
                                        <Controller
                                          control={control}
                                          name={`optionTransport.${index}.TGNhapHang`}
                                          render={({ field }) => (
                                            <DatePicker
                                              className="form-control"
                                              showTimeSelect
                                              timeFormat="HH:mm"
                                              dateFormat="dd/MM/yyyy HH:mm"
                                              onChange={(date) =>
                                                field.onChange(date)
                                              }
                                              selected={field.value}
                                            />
                                          )}
                                          rules={{
                                            required: "không được để trống",
                                          }}
                                        />
                                        {errors.optionTransport?.[index]
                                          ?.TGNhapHang && (
                                          <span className="text-danger">
                                            {
                                              errors.optionTransport?.[index]
                                                ?.TGNhapHang.message
                                            }
                                          </span>
                                        )}
                                      </div>
                                    </div>
                                  </div>
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="TGXuatHang">
                                        Thời gian xuất hàng
                                      </label>
                                      <div className="input-group ">
                                        <Controller
                                          control={control}
                                          name={`optionTransport.${index}.TGXuatHang`}
                                          render={({ field }) => (
                                            <DatePicker
                                              className="form-control"
                                              showTimeSelect
                                              timeFormat="HH:mm"
                                              dateFormat="dd/MM/yyyy HH:mm"
                                              onChange={(date) =>
                                                field.onChange(date)
                                              }
                                              selected={field.value}
                                            />
                                          )}
                                          rules={{
                                            required: "không được để trống",
                                          }}
                                        />
                                        {errors.optionTransport?.[index]
                                          ?.TGXuatHang && (
                                          <span className="text-danger">
                                            {
                                              errors.optionTransport?.[index]
                                                ?.TGXuatHang.message
                                            }
                                          </span>
                                        )}
                                      </div>
                                    </div>
                                  </div>
                                </div>
                                {watch(
                                  `optionTransport.${index}.PTVanChuyen`
                                ) &&
                                  watch(
                                    `optionTransport.${index}.PTVanChuyen`
                                  ).includes("CONT") && (
                                    <>
                                      <div className="row">
                                        <div className="col col-sm">
                                          <div className="form-group">
                                            <label htmlFor="TGLayRong">
                                              Thời gian lấy rỗng
                                            </label>
                                            <div className="input-group ">
                                              <Controller
                                                control={control}
                                                name={`optionTransport.${index}.TGLayRong`}
                                                render={({ field }) => (
                                                  <DatePicker
                                                    className="form-control"
                                                    showTimeSelect
                                                    timeFormat="HH:mm"
                                                    dateFormat="dd/MM/yyyy HH:mm"
                                                    onChange={(date) =>
                                                      field.onChange(date)
                                                    }
                                                    selected={field.value}
                                                  />
                                                )}
                                                rules={{
                                                  required:
                                                    "không được để trống",
                                                }}
                                              />
                                              {errors.optionTransport?.[index]
                                                ?.TGLayRong && (
                                                <span className="text-danger">
                                                  {
                                                    errors.optionTransport?.[
                                                      index
                                                    ]?.TGLayRong.message
                                                  }
                                                </span>
                                              )}
                                            </div>
                                          </div>
                                        </div>
                                        <div className="col col-sm">
                                          <div className="form-group">
                                            <label htmlFor="TGTraRong">
                                              Thời Gian Trả Rỗng
                                            </label>
                                            <div className="input-group ">
                                              <Controller
                                                control={control}
                                                name={`optionTransport.${index}.TGTraRong`}
                                                render={({ field }) => (
                                                  <DatePicker
                                                    className="form-control"
                                                    showTimeSelect
                                                    timeFormat="HH:mm"
                                                    dateFormat="dd/MM/yyyy HH:mm"
                                                    onChange={(date) =>
                                                      field.onChange(date)
                                                    }
                                                    selected={field.value}
                                                  />
                                                )}
                                                rules={{
                                                  required:
                                                    "không được để trống",
                                                }}
                                              />
                                              {errors.optionTransport?.[index]
                                                ?.TGTraRong && (
                                                <span className="text-danger">
                                                  {
                                                    errors.optionTransport?.[
                                                      index
                                                    ]?.TGTraRong.message
                                                  }
                                                </span>
                                              )}
                                            </div>
                                          </div>
                                        </div>
                                      </div>
                                      <div className="row">
                                        <div className="col col-sm">
                                          <div className="form-group">
                                            <label htmlFor="TGLech">
                                              Thời gian hạn lệnh
                                            </label>
                                            <div className="input-group ">
                                              <Controller
                                                control={control}
                                                name={`optionTransport.${index}.TGLech`}
                                                render={({ field }) => (
                                                  <DatePicker
                                                    className="form-control"
                                                    showTimeSelect
                                                    timeFormat="HH:mm"
                                                    dateFormat="dd/MM/yyyy HH:mm"
                                                    onChange={(date) =>
                                                      field.onChange(date)
                                                    }
                                                    selected={field.value}
                                                  />
                                                )}
                                                rules={{
                                                  required:
                                                    "không được để trống",
                                                }}
                                              />
                                              {errors.optionTransport?.[index]
                                                ?.TGLech && (
                                                <span className="text-danger">
                                                  {
                                                    errors.optionTransport?.[
                                                      index
                                                    ]?.TGLech.message
                                                  }
                                                </span>
                                              )}
                                            </div>
                                          </div>
                                        </div>

                                        <div className="col col-sm">
                                          <div className="form-group">
                                            <label htmlFor="TGKeoCont">
                                              Thời Gian Kéo Công
                                            </label>
                                            <div className="input-group ">
                                              <Controller
                                                control={control}
                                                name={`optionTransport.${index}.TGKeoCont`}
                                                render={({ field }) => (
                                                  <DatePicker
                                                    className="form-control"
                                                    showTimeSelect
                                                    timeFormat="HH:mm"
                                                    dateFormat="dd/MM/yyyy HH:mm"
                                                    onChange={(date) =>
                                                      field.onChange(date)
                                                    }
                                                    selected={field.value}
                                                  />
                                                )}
                                                rules={{
                                                  required:
                                                    "không được để trống",
                                                }}
                                              />
                                              {errors.optionTransport?.[index]
                                                ?.TGKeoCont && (
                                                <span className="text-danger">
                                                  {
                                                    errors.optionTransport?.[
                                                      index
                                                    ]?.TGKeoCont.message
                                                  }
                                                </span>
                                              )}
                                            </div>
                                          </div>
                                        </div>
                                      </div>
                                    </>
                                  )}

                                <div className="row">
                                  <div className="col col-12">
                                    <div className="form-group">
                                      <label htmlFor="GhiChu">Ghi Chú</label>
                                      <div className="input-group ">
                                        <textarea
                                          className="form-control"
                                          rows={3}
                                          {...register(
                                            `optionTransport.${index}.GhiChu`
                                          )}
                                        ></textarea>
                                      </div>
                                    </div>
                                  </div>
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                      ))}
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

export default CreateHandling;
