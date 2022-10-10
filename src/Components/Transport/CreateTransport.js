import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
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
      optionTransport: [{}],
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
    GiaThucTe: {
      required: "Không được để trống",
    },
    PTVanChuyen: {
      required: "Không được để trống",
    },
    LoaiHangHoa: {
      required: "Không được để trống",
    },
    TrongLuong: {
      required: "Không được để trống",
    },
    TheTich: {
      required: "Không được để trống",
    },
    PTVC: { required: "Không được để trống" },
    DVT: { required: "Không được để trống" },
  };

  const [listData, setListData] = useState([]);
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
  const [listPriceTable, setListPriceTable] = useState([]);
  const [listFilterPriceTable, setListFilterPriceTable] = useState([]);

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
    setListPriceTable([]);

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
      getlistData.listNhaPhanPhoi.map((val) => {
        objNpp.push({
          value: val.maNpp,
          label: val.maNpp + " - " + val.tenNpp,
        });
      });
      setListNpp(objNpp);
      setListData(getlistData.bangGiaVanDon);
    }

    SetIsLoading(false);
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
                          <label htmlFor="NhaCungCap">Đơn Vị Vận Tải</label>
                          <Controller
                            name="NhaCungCap"
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={listNpp}
                                // onChange={(field) =>
                                //   handleOnchangeListCustomer(field)
                                // }
                              />
                            )}
                            rules={Validate.NhaCungCap}
                          />
                          {errors.NhaCungCap && (
                            <span className="text-danger">
                              {errors.NhaCungCap.message}
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
                                          handleOnChangeFilter(
                                            setValue(
                                              "PTVanChuyen",
                                              e.target.value
                                            )
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
                                          handleOnChangeFilter(
                                            setValue(
                                              "LoaiHangHoa",
                                              e.target.value
                                            )
                                          )
                                        }
                                        value={watch("LoaiHangHoa")}
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
                                        Giá kham khảo
                                      </label>
                                      <select
                                        className="form-control"
                                        {...register(
                                          `optionTransport.${index}.MaBangGia`,
                                          Validate.MaBangGia
                                        )}
                                      >
                                        {listPriceTable &&
                                          listPriceTable.map((val) => {
                                            return (
                                              <option
                                                value={val.id}
                                                key={val.id}
                                              >
                                                {val.donGia}
                                              </option>
                                            );
                                          })}
                                      </select>
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
                                      <label htmlFor="CONTNO">CONT NO</label>
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
                                            errors.optionTransport?.[index]
                                              ?.CONTNO.message
                                          }
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
                                        {...register(
                                          `optionTransport.${index}.SEALHQ`,
                                          Validate.SEALHQ
                                        )}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.SEALHQ && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.SEALHQ.message
                                          }
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
                                        {...register(
                                          `optionTransport.${index}.SEALHT`,
                                          Validate.SEALHT
                                        )}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.SEALHT && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.SEALHT.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
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
                                            required: "không được để trống",
                                          }}
                                        />
                                        {errors.optionTransport?.[index]
                                          ?.TGLayRong && (
                                          <span className="text-danger">
                                            {
                                              errors.optionTransport?.[index]
                                                ?.TGLayRong.message
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
                                            required: "không được để trống",
                                          }}
                                        />
                                        {errors.optionTransport?.[index]
                                          ?.TGTraRong && (
                                          <span className="text-danger">
                                            {
                                              errors.optionTransport?.[index]
                                                ?.TGTraRong.message
                                            }
                                          </span>
                                        )}
                                      </div>
                                    </div>
                                  </div>
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="TGCoMat">
                                        Thời gian có mặt tại cảng
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
                                </div>
                                <div className="row">
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="TGLech">
                                        Thời gian hạn lệch
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
                                            required: "không được để trống",
                                          }}
                                        />
                                        {errors.optionTransport?.[index]
                                          ?.TGLech && (
                                          <span className="text-danger">
                                            {
                                              errors.optionTransport?.[index]
                                                ?.TGLech.message
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
                                            required: "không được để trống",
                                          }}
                                        />
                                        {errors.optionTransport?.[index]
                                          ?.TGKeoCont && (
                                          <span className="text-danger">
                                            {
                                              errors.optionTransport?.[index]
                                                ?.TGKeoCont.message
                                            }
                                          </span>
                                        )}
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

export default CreateTransport;
