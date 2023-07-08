import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import moment from "moment";
import Select from "react-select";
import PriceListContract from "./PriceListContract";
import LoadingPage from "../Common/Loading/LoadingPage";

const AddPriceTable = (props) => {
  const { selectIdClick } = props;
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
    defaultValues: {
      optionRoad: [
        {
          DiemDau: null,
          DiemCuoi: null,
          DiemLayTraRong: null,
          AccountId: null,
          DonGia: null,
          LoaiTienTe: null,
          MaDVT: "",
          MaPTVC: "",
          MaLoaiPhuongTien: "",
          MaLoaiHangHoa: "",
          NgayApDung: "",
          NgayHetHieuLuc: "",
        },
      ],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control, // control props comes from useForm (optional: if you are using FormContext)
    name: "optionRoad", // unique name for your Field Array
  });

  const Validate = {
    MaHopDong: {
      required: "Không được để trống",
      maxLength: {
        value: 50,
        message: "Không được vượt quá 50 ký tự",
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
    // NgayApDung: {
    //   maxLength: {
    //     value: 10,
    //     message: "Không được vượt quá 10 ký tự",
    //   },
    //   minLength: {
    //     value: 10,
    //     message: "Không được ít hơn 10 ký tự",
    //   },
    //   pattern: {
    //     value:
    //       /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/,
    //     message: "Không phải định dạng ngày",
    //   },
    // },
    NgayHetHieuLuc: {
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      minLength: {
        value: 10,
        message: "Không được ít hơn 10 ký tự",
      },
      pattern: {
        value:
          /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/,
        message: "Không phải định dạng ngày",
      },
    },
    DonGia: {
      pattern: {
        value: /^[0-9]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
      required: "Không được để trống",
    },

    LoaiTienTe: {
      required: "Không được để trống",
    },

    MaLoaiPhuongTien: {
      required: "Không được để trống",
    },
    MaLoaiHangHoa: {
      required: "Không được để trống",
    },
    MaDVT: {
      required: "Không được để trống",
    },
    MaPTVC: {
      required: "Không được để trống",
    },
    TrangThai: {
      required: "Không được để trống",
    },
    PhanLoaiDoiTac: {
      required: "Không được để trống",
    },
  };

  const [IsLoading, SetIsLoading] = useState(false);
  const [tabIndex, setTabIndex] = useState(0);
  const [listCustomer, setListCustomer] = useState([]);
  const [listVehicleType, setListVehicleType] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listDVT, setListDVT] = useState([]);
  const [listTransportType, setListTransportType] = useState([]);
  const [listContract, setListContract] = useState([]);
  const [listCustomerType, setListCustomerType] = useState([]);
  const [listFPlace, setListFPlace] = useState([]);
  const [listSPlace, setListSPlace] = useState([]);
  const [listEPlace, setListEPlace] = useState([]);
  const [listAccountCus, setListAccountCus] = useState([]);
  const [listPriceTrade, setListPriceTrade] = useState([]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      let getListDVT = await getData("Common/GetListDVT");
      let getListPriceTrade = await getData("Common/LoadDataPriceTrade");
      let getListVehicleType = await getData("Common/GetListVehicleType");
      let getListGoodsType = await getData("Common/GetListGoodsType");
      let getListTransportType = await getData("Common/GetListTransportType");
      const getListPlace = await getData(
        "Address/GetListAddressSelect?pointType=&type="
      );

      let arrPlace = [];
      getListPlace.forEach((val) => {
        arrPlace.push({
          label: val.tenDiaDiem + " - " + val.loaiDiaDiem,
          value: val.maDiaDiem,
        });
      });

      setListFPlace(arrPlace);
      setListSPlace(arrPlace);

      let arrEPlace = [];
      arrEPlace.push({ label: "-- Rỗng --", value: null });

      getListPlace.forEach((val) => {
        arrEPlace.push({
          label: val.tenDiaDiem + " - " + val.loaiDiaDiem,
          value: val.maDiaDiem,
        });
      });

      setListEPlace(arrEPlace);
      setListPriceTrade(getListPriceTrade);
      let getListCustommerType = await getData(`Common/GetListCustommerType`);
      setListCustomerType(getListCustommerType);
      setListVehicleType(getListVehicleType);
      setListGoodsType(getListGoodsType);
      setListDVT(getListDVT);
      setListTransportType(getListTransportType);
      SetIsLoading(false);
    })();
  }, []);

  useEffect(() => {
    (async () => {
      handleOnChangeContractType("KH");
    })();
  }, [listCustomerType]);

  useEffect(() => {
    console.log(selectIdClick);
    if (selectIdClick && Object.keys(selectIdClick).length > 0) {
      setTabIndex(2);
    } else {
      setTabIndex(0);
    }
  }, [selectIdClick]);

  const handleOnchangeListCustomer = (val) => {
    SetIsLoading(true);
    setListContract([]);
    setValue("MaKh", val);
    setValue("MaHopDong", null);
    getListRoadAndContract(val.value);

    SetIsLoading(false);
  };

  const handleOnChangeContractType = async (val) => {
    setValue("PhanLoaiDoiTac", val);
    setValue("AccountCus", null);
    setValue("MaHopDong", null);
    setValue("MaKh", null);
    setListCustomer([]);
    setListAccountCus([]);
    setListContract([]);

    if (val && val !== "") {
      let getListCustomer = await getData(
        `Customer/GetListCustomerOptionSelect`
      );
      if (getListCustomer && getListCustomer.length > 0) {
        getListCustomer = getListCustomer.filter((x) => x.loaiKH === val);
        let obj = [];

        getListCustomer.map((val) => {
          obj.push({
            value: val.maKh,
            label: val.maKh + " - " + val.tenKh,
          });
        });
        setListCustomer(obj);
      }
    } else {
      handleResetClick();
    }
  };

  const getListRoadAndContract = async (MaKh) => {
    SetIsLoading(true);
    let getListContract = await getData(
      `Contract/GetListContractSelect?MaKH=${MaKh}&getListApprove=true`
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
    }

    SetIsLoading(false);
  };

  const handleResetClick = () => {
    reset();
    setValue("MaKh", null);
    setValue("MaHopDong", null);
    setListContract([]);
    setListCustomer([]);
    setListAccountCus([]);
    setValue("AccountCus", null);
  };

  const HandleOnChangesVehicleType = (val) => {
    let listData = listVehicleType;

    if (val === "FCL" || val === "LCL") {
      listData = listVehicleType.filter((x) =>
        x.maLoaiPhuongTien.includes("CONT")
      );
      return listData;
    }

    if (val === "FTL" || val === "LTL") {
      listData = listVehicleType.filter((x) =>
        x.maLoaiPhuongTien.includes("TRUCK")
      );
      return listData;
    }
  };

  const handleOnChangeCustomer = async (val) => {
    if (val && Object.keys(val).length > 0) {
      setListAccountCus([]);
      setValue("AccountCus", null);
      setValue("MaKH", val);
      const getListAcc = await getData(
        `AccountCustomer/GetListAccountSelectByCus?cusId=${val.value}`
      );
      if (getListAcc && getListAcc.length > 0) {
        var obj = [];
        obj.push({ label: "-- Để Trống --", value: null });
        getListAcc.map((val) => {
          obj.push({
            value: val.accountId,
            label: val.accountId + " - " + val.accountName,
          });
        });
        setListAccountCus(obj);
      } else {
        setListAccountCus([]);
      }
    }
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);
    let arr = [];

    data.optionRoad.forEach((val) => {
      arr.push({
        maHopDong: data.MaHopDong.value,
        DiemDau: val.DiemDau.value,
        DiemCuoi: val.DiemCuoi.value,
        AccountId: !data.AccountCus ? null : data.AccountCus.value,
        DiemLayTraRong: !val.DiemLayTraRong
          ? null
          : val.DiemLayTraRong.value === ""
          ? null
          : val.DiemLayTraRong.value,
        maKh: data.MaKh.value,
        maPtvc: val.MaPTVC,
        maLoaiPhuongTien: val.MaLoaiPhuongTien,
        maLoaiDoiTac: data.PhanLoaiDoiTac,
        DonGia: val.DonGia,
        LoaiTienTe: val.LoaiTienTe,
        maDvt: val.MaDVT,
        maLoaiHangHoa: val.MaLoaiHangHoa,
        ngayHetHieuLuc: null,
      });
    });

    const createPriceTable = await postData("PriceTable/CreatePriceTable", arr);
    if (createPriceTable === 1) {
      reset();
    }
    SetIsLoading(false);
  };

  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex);
  };

  return (
    <>
      <Tabs
        selectedIndex={tabIndex}
        onSelect={(index) => HandleOnChangeTabs(index)}
      >
        <TabList>
          <Tab>Tạo Bảng giá</Tab>
          {selectIdClick && Object.keys(selectIdClick).length > 0 && (
            <>
              <Tab>Bảng Giá Hiện Hành</Tab>
              <Tab>Bảng Giá Hợp Đồng</Tab>
            </>
          )}
        </TabList>

        <TabPanel>
          <div className="card card-primary">
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
                        <label htmlFor="PhanLoaiDoiTac">
                          Phân Loại Đối Tác(*)
                        </label>
                        <select
                          className="form-control"
                          {...register(
                            "PhanLoaiDoiTac",
                            Validate.PhanLoaiDoiTac
                          )}
                          onChange={(e) =>
                            handleOnChangeContractType(e.target.value)
                          }
                        >
                          {listCustomerType &&
                            listCustomerType.map((val) => {
                              return (
                                <option value={val.maLoaiKh} key={val.maLoaiKh}>
                                  {val.tenLoaiKh}
                                </option>
                              );
                            })}
                        </select>
                        {errors.PhanLoaiDoiTac && (
                          <span className="text-danger">
                            {errors.PhanLoaiDoiTac.message}
                          </span>
                        )}
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="KhachHang">Chọn Đối Tác(*)</label>
                        <Controller
                          name="MaKh"
                          control={control}
                          render={({ field }) => (
                            <Select
                              {...field}
                              classNamePrefix={"form-control"}
                              value={field.value}
                              options={listCustomer}
                              onChange={(field) => {
                                handleOnchangeListCustomer(field);
                                handleOnChangeCustomer(field);
                              }}
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
                    {watch("PhanLoaiDoiTac") &&
                      watch("PhanLoaiDoiTac") === "KH" && (
                        <div className="col col-sm">
                          <div className="form-group">
                            <label htmlFor="AccountCus">Account</label>
                            <Controller
                              name="AccountCus"
                              control={control}
                              render={({ field }) => (
                                <Select
                                  {...field}
                                  classNamePrefix={"form-control"}
                                  value={field.value}
                                  options={listAccountCus}
                                />
                              )}
                            />
                            {errors.AccountCus && (
                              <span className="text-danger">
                                {errors.AccountCus.message}
                              </span>
                            )}
                          </div>
                        </div>
                      )}

                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="MaHopDong">Hợp Đồng(*)</label>
                        <Controller
                          name="MaHopDong"
                          rules={Validate.MaHopDong}
                          control={control}
                          render={({ field }) => (
                            <Select
                              {...field}
                              classNamePrefix={"form-control"}
                              value={field.value}
                              options={listContract}
                            />
                          )}
                        />
                        {errors.MaHopDong && (
                          <span className="text-danger">
                            {errors.MaHopDong.message}
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                  <br />
                  <table
                    className="table table-sm table-bordered"
                    style={{
                      whiteSpace: "nowrap",
                    }}
                  >
                    <thead>
                      <tr>
                        <th style={{ width: "10px" }}></th>
                        <th style={{ width: "13%" }}>Điểm Đóng Hàng</th>
                        <th style={{ width: "13%" }}>Điểm Trả Hàng</th>
                        <th style={{ width: "13%" }}>Điểm Lấy/Trả Rỗng</th>
                        <th>Đơn Giá (*)</th>
                        <th>Loại Tiền Tệ</th>
                        <th>Đơn vị tính(*)</th>
                        <th style={{ width: "10%" }}>PTVC(*)</th>
                        <th style={{ width: "12%" }}>Loại phương tiện(*)</th>
                        <th>Loại Hàng Hóa(*)</th>
                        {/* <th>Ngày Hết Hiệu Lực</th> */}
                        <th style={{ width: "40px" }}></th>
                      </tr>
                    </thead>
                    <tbody>
                      {fields.map((value, index) => (
                        <tr key={index}>
                          <td>{index + 1}</td>
                          <td>
                            <div className="form-group">
                              <Controller
                                name={`optionRoad.${index}.DiemDau`}
                                control={control}
                                render={({ field }) => (
                                  <Select
                                    {...field}
                                    classNamePrefix={"form-control"}
                                    value={field.value}
                                    options={listFPlace}
                                    placeholder={"Điểm Đóng Hàng"}
                                  />
                                )}
                                rules={{
                                  required: "không được để trống",
                                  validate: (value) => {
                                    if (!value.value) {
                                      return "không được để trống";
                                    }
                                  },
                                }}
                              />
                              {errors.optionRoad?.[index]?.DiemDau && (
                                <span className="text-danger">
                                  {errors.optionRoad?.[index]?.DiemDau.message}
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <Controller
                                name={`optionRoad.${index}.DiemCuoi`}
                                control={control}
                                render={({ field }) => (
                                  <Select
                                    {...field}
                                    classNamePrefix={"form-control"}
                                    value={field.value}
                                    options={listSPlace}
                                    placeholder={"Điểm Hạ Hàng"}
                                  />
                                )}
                                rules={{
                                  required: "không được để trống",
                                  validate: (value) => {
                                    if (!value.value) {
                                      return "không được để trống";
                                    }
                                  },
                                }}
                              />
                              {errors.optionRoad?.[index]?.DiemCuoi && (
                                <span className="text-danger">
                                  {errors.optionRoad?.[index]?.DiemCuoi.message}
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <Controller
                                name={`optionRoad.${index}.DiemLayTraRong`}
                                control={control}
                                render={({ field }) => (
                                  <Select
                                    {...field}
                                    classNamePrefix={"form-control"}
                                    value={field.value}
                                    options={listEPlace}
                                    placeholder={"Điểm Lấy/Trả Rỗng"}
                                  />
                                )}
                              />
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <input
                                type="text"
                                className="form-control"
                                id="DonGia"
                                {...register(
                                  `optionRoad.${index}.DonGia`,
                                  Validate.DonGia
                                )}
                              />
                              {errors.optionRoad?.[index]?.DonGia && (
                                <span className="text-danger">
                                  {errors.optionRoad?.[index]?.DonGia.message}
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <select
                                className="form-control"
                                {...register(
                                  `optionRoad.${index}.LoaiTienTe`,
                                  Validate.LoaiTienTe
                                )}
                              >
                                <option value="">Chọn loại tiền tệ</option>
                                {listPriceTrade &&
                                  listPriceTrade.map((val) => {
                                    return (
                                      <option
                                        value={val.maLoaiTienTe}
                                        key={val.maLoaiTienTe}
                                      >
                                        {val.tenLoaiTienTe}
                                      </option>
                                    );
                                  })}
                              </select>
                              {errors.optionRoad?.[index]?.LoaiTienTe && (
                                <span className="text-danger">
                                  {
                                    errors.optionRoad?.[index]?.LoaiTienTe
                                      .message
                                  }
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <select
                                className="form-control"
                                {...register(
                                  `optionRoad.${index}.MaDVT`,
                                  Validate.MaDVT
                                )}
                              >
                                <option value="">Chọn đơn vị tính</option>
                                {listDVT &&
                                  listDVT.map((val) => {
                                    return (
                                      <option value={val.maDvt} key={val.maDvt}>
                                        {val.tenDvt}
                                      </option>
                                    );
                                  })}
                              </select>
                              {errors.optionRoad?.[index]?.MaDVT && (
                                <span className="text-danger">
                                  {errors.optionRoad?.[index]?.MaDVT.message}
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <select
                                className="form-control"
                                {...register(
                                  `optionRoad.${index}.MaPTVC`,
                                  Validate.MaPTVC
                                )}
                              >
                                <option value="">Chọn PTVC</option>
                                {listTransportType &&
                                  listTransportType.map((val) => {
                                    return (
                                      <option
                                        value={val.maPtvc}
                                        key={val.maPtvc}
                                      >
                                        {val.tenPtvc}
                                      </option>
                                    );
                                  })}
                              </select>
                              {errors.optionRoad?.[index]?.MaPTVC && (
                                <span className="text-danger">
                                  {errors.optionRoad?.[index]?.MaPTVC.message}
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <select
                                className="form-control"
                                {...register(
                                  `optionRoad.${index}.MaLoaiPhuongTien`,
                                  Validate.MaLoaiPhuongTien
                                )}
                              >
                                <option value="">Chọn loại phương tiện</option>
                                {HandleOnChangesVehicleType(
                                  watch(`optionRoad.${index}.MaPTVC`)
                                ) &&
                                  HandleOnChangesVehicleType(
                                    watch(`optionRoad.${index}.MaPTVC`)
                                  ).map((val) => {
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
                              {errors.optionRoad?.[index]?.MaLoaiPhuongTien && (
                                <span className="text-danger">
                                  {
                                    errors.optionRoad?.[index]?.MaLoaiPhuongTien
                                      .message
                                  }
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <select
                                className="form-control"
                                {...register(
                                  `optionRoad.${index}.MaLoaiHangHoa`,
                                  Validate.MaLoaiHangHoa
                                )}
                              >
                                <option value="">Chọn loại hàng hóa</option>
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
                              {errors.optionRoad?.[index]?.MaLoaiHangHoa && (
                                <span className="text-danger">
                                  {
                                    errors.optionRoad?.[index]?.MaLoaiHangHoa
                                      .message
                                  }
                                </span>
                              )}
                            </div>
                          </td>
                          {/* <td>
                            <div className="form-group">
                              <div className="input-group ">
                                <Controller
                                  control={control}
                                  name={`optionRoad.${index}.NgayApDung`}
                                  render={({ field }) => (
                                    <DatePicker
                                      className="form-control"
                                      dateFormat="dd/MM/yyyy"
                                      onChange={(date) => field.onChange(date)}
                                      selected={field.value}
                                    />
                                  )}
                                  rules={Validate.NgayApDung}
                                />
                                {errors.optionRoad?.[index]?.NgayApDung && (
                                  <span className="text-danger">
                                    {
                                      errors.optionRoad?.[index]?.NgayApDung
                                        .message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                          </td> */}
                          {/* <td>
                            <div className="form-group">
                              <div className="input-group ">
                                <Controller
                                  control={control}
                                  name={`optionRoad.${index}.NgayHetHieuLuc`}
                                  render={({ field }) => (
                                    <DatePicker
                                      className="form-control"
                                      dateFormat="dd/MM/yyyy"
                                      onChange={(date) => field.onChange(date)}
                                      selected={field.value}
                                    />
                                  )}
                                  rules={Validate.NgayHetHieuLuc}
                                />
                                {errors.optionRoad?.[index]?.NgayHetHieuLuc && (
                                  <span className="text-danger">
                                    {
                                      errors.optionRoad?.[index]?.NgayHetHieuLuc
                                        .message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                          </td> */}
                          <td>
                            <div className="form-group">
                              {index >= 1 && (
                                <>
                                  <button
                                    type="button"
                                    className="form-control form-control-sm"
                                    onClick={() => remove(index)}
                                  >
                                    <i className="fas fa-minus"></i>
                                  </button>
                                </>
                              )}
                            </div>
                          </td>
                        </tr>
                      ))}
                      <tr>
                        <td colSpan={11}>
                          <button
                            className="form-control form-control-sm"
                            type="button"
                            onClick={() => append(watch("optionRoad")[0])}
                          >
                            <i className="fas fa-plus"></i>
                          </button>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                  <br />
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
        </TabPanel>

        {selectIdClick && Object.keys(selectIdClick).length > 0 && (
          <>
            <div>
              <TabPanel>
                <PriceListContract
                  selectIdClick={selectIdClick}
                  onlyCT={""}
                  title={"Bảng Giá Hiện Hành"}
                />
              </TabPanel>
            </div>

            <div>
              <TabPanel>
                <PriceListContract
                  selectIdClick={selectIdClick}
                  onlyCT={"getByContractOnly"}
                  title={"Bảng Giá Hợp Đồng"}
                />
              </TabPanel>
            </div>
          </>
        )}
      </Tabs>
    </>
  );
};

export default AddPriceTable;
