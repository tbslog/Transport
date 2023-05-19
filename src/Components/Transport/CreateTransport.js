import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import moment from "moment";
import { ToastError, ToastWarning } from "../Common/FuncToast";
import LoadingPage from "../Common/Loading/LoadingPage";

const CreateTransport = (props) => {
  const { getListTransport, hideModal } = props;
  const [IsLoading, SetIsLoading] = useState(false);
  const {
    register,
    reset,
    setValue,
    control,
    watch,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
    defaultValues: {
      LoaiVanDon: "nhap",
      optionHandling: [
        {
          DonViVanTai: { label: "Chọn Đơn Vị Vận Tải", value: null },
          LoaiHangHoa: "Normal",
          PTVanChuyen: "",
          KhoiLuong: null,
          TheTich: null,
          SoKien: null,
          DiemTraRong: { label: "Chọn Điểm Trả Rỗng", value: null },
          DiemLayRong: { label: "Chọn Điểm Lấy Rỗng", value: null },
        },
      ],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control, // control props comes from useForm (optional: if you are using FormContext)
    name: "optionHandling", // unique name for your Field Array
  });

  const Validate = {
    MaVDKH: {
      required: "Không được để trống",
    },
    LoaiHinh: {
      required: "Không được để trống",
    },
    MaKH: {
      required: "Không được để trống",
    },
    DiemLayHang: {
      required: "Không được để trống",
    },
    DiemTraHang: {
      required: "Không được để trống",
    },
    LoaiVanDon: {
      required: "Không được để trống",
    },
    MaCungDuong: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      minLength: {
        value: 10,
        message: "Không được ít hơn 10 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    LoaiHangHoa: {
      required: "Không được để trống",
    },
    TongKhoiLuong: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,6})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    TongTheTich: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,6})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    TongSoKien: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,6})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    KhoiLuong: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,6})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    TheTich: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,6})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    SoKien: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,6})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    PTVanChuyen: {
      required: "Không được để trống",
    },
    CONTNO: {
      pattern: {
        value: /([A-Z]{3})([UJZ])(\d{6})(\d)/,
        message: "Mã không không đúng, vui lòng viết hoa",
      },
    },
  };

  const [listCus, setListCus] = useState([]);
  const [listVehicleType, setlistVehicleType] = useState([]);
  const [listVehicleTypeTemp, setListVehicleTypeTemp] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listShipping, setListShipping] = useState([]);

  const [listFPlace, setListFPlace] = useState([]);
  const [listSPlace, setListSPlace] = useState([]);
  const [listEPlace, setListEPlace] = useState([]);
  const [listAccountCus, setListAccountCus] = useState([]);

  const [totalCBM, setTotalCBM] = useState("");
  const [totalWGT, setTotalWGT] = useState("");
  const [totalPCS, setTotalPCS] = useState("");

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      const getListCus = await getData(`Customer/GetListCustomerFilter`);
      if (getListCus && getListCus.length > 0) {
        let arrKh = [];
        let arrSupplier = [];
        getListCus
          .filter((x) => x.loaiKH === "KH")
          .map((val) => {
            arrKh.push({
              label: val.maKh + " - " + val.tenKh,
              value: val.maKh,
            });
          });
        getListCus
          .filter((x) => x.loaiKH === "NCC")
          .map((val) => {
            arrSupplier.push({
              label: val.maKh + " - " + val.tenKh,
              value: val.maKh,
            });
          });
        setListCus(arrKh);
      } else {
        setListCus([]);
      }

      let getListShipping = await getData("Common/GetListShipping");
      setListShipping(getListShipping);
      let getListVehicleType = await getData("Common/GetListVehicleType");
      let getListGoodsType = await getData("Common/GetListGoodsType");
      setlistVehicleType(getListVehicleType);
      setListVehicleTypeTemp(getListVehicleType);
      setListGoodsType(getListGoodsType);

      const getListPlace = await getData(
        "Address/GetListAddressSelect?pointType=&type=Diem"
      );

      let arrPlace = [];
      getListPlace.forEach((val) => {
        arrPlace.push({ label: val.tenDiaDiem, value: val.maDiaDiem });
      });

      setListFPlace(arrPlace);
      setListSPlace(arrPlace);
      setListEPlace(arrPlace);

      // const getListPoint = await getData("address/GetListAddressSelect");
      // if (getListPoint && getListPoint.length > 0) {
      //   var obj = [];
      //   getListPoint.map((val) => {
      //     obj.push({
      //       value: val.maDiaDiem,
      //       label: val.maDiaDiem + " - " + val.tenDiaDiem,
      //     });
      //   });
      //   setListPoint(obj);
      // }
      SetIsLoading(false);
    })();
  }, []);

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

  const handleOnchangeTransportType = (val) => {
    reset();
    setValue("LoaiVanDon", val);
  };

  const handleOnChangeWeight = async (vehicleType, val, type) => {
    if (val && vehicleType && type && val > 0) {
      let getTonnageVehicle = await getData(
        `BillOfLading/LayTrongTaiXe?vehicleType=${vehicleType}`
      );

      if (getTonnageVehicle) {
        let vehicleCBM = getTonnageVehicle.cbm;
        let vehicleWGT = getTonnageVehicle.wgt;
        if (type === "KhoiLuong") {
          if (vehicleWGT < val) {
            return ToastWarning(
              <>
                <p>Vượt Quá trọng tải xe {val + "/" + vehicleWGT}</p>
              </>
            );
          }
        }

        if (type === "TheTich") {
          if (vehicleCBM < val) {
            return ToastWarning(
              <>
                <p>Vượt quá khối lượng xe {val + "/" + vehicleCBM}</p>
              </>
            );
          }
        }
      }
    }
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);

    let arr = [];
    data.optionHandling.map((val) => {
      arr.push({
        DiemTraRong: !val.DiemTraRong ? null : val.DiemTraRong.value,
        DiemLayRong: !val.DiemLayRong ? null : val.DiemLayRong.value,
        LoaiHangHoa: val.LoaiHangHoa,
        PTVanChuyen: val.PTVanChuyen,
        KhoiLuong: !val.KhoiLuong ? null : val.KhoiLuong,
        TheTich: !val.TheTich ? null : val.TheTich,
        SoKien: !val.SoKien ? null : val.SoKien,
        ReuseCont: !val.ReuseCont ? false : val.ReuseCont,
        DonViTinh: "CHUYEN",
      });
    });

    if (arr && arr.length < 1) {
      ToastError("Vui lòng điền đầy đủ thông tin");
      return;
    }

    const create = await postData("BillOfLading/CreateTransport", {
      arrHandlings: arr,
      AccountId: !data.AccountCus ? null : data.AccountCus.value,
      MaPTVC: data.LoaiHinh,
      DiemDau: data.DiemLayHang.value,
      DiemCuoi: data.DiemTraHang.value,
      HangTau: data.HangTau,
      TenTau: data.TenTau,
      MaVanDonKH: data.MaVDKH,
      LoaiVanDon: data.LoaiVanDon,
      TongKhoiLuong: !data.TongKhoiLuong ? null : data.TongKhoiLuong,
      TongTheTich: !data.TongTheTich ? null : data.TongTheTich,
      TongSoKien: !data.TongSoKien ? null : data.TongSoKien,
      MaKH: data.MaKH.value,
      TongThungHang: data.TongThungHang,
      GhiChu: data.GhiChu,
      ThoiGianTraRong: !data.TGTraRong
        ? null
        : moment(new Date(data.TGTraRong).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      ThoiGianLayRong: !data.TGLayRong
        ? null
        : moment(new Date(data.TGLayRong).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      ThoiGianHaCang: !data.TGHaCang
        ? null
        : moment(new Date(data.TGHaCang).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      ThoiGianCoMat: !data.TGCoMat
        ? null
        : moment(new Date(data.TGCoMat).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      ThoiGianHanLenh: !data.TGHanLenh
        ? null
        : moment(new Date(data.TGHanLenh).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      thoiGianLayHang: !data.TGLayHang
        ? null
        : moment(new Date(data.TGLayHang).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      thoiGianTraHang: !data.TGTraHang
        ? null
        : moment(new Date(data.TGTraHang).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
    });

    if (create === 1) {
      getListTransport();
      handleResetClick();
      hideModal();
    }

    SetIsLoading(false);
  };

  const handleResetClick = () => {
    reset();
    setValue("MaKH", null);
    setValue("MaCungDuong", null);
    setValue("DiemLayHang", null);
    setValue("DiemTraHang", null);
    setValue("AccountCus", null);
    setListAccountCus([]);
    // setListFirstPoint([]);
    // setListSecondPoint([]);
    // setListRoad([]);
    // setArrRoad([]);
  };

  const handleOnChangeLH = (val) => {
    setValue("LoaiHinh", val);

    if (val && val === "FCL") {
      let filter = listVehicleTypeTemp.filter((x) =>
        x.maLoaiPhuongTien.includes("CONT")
      );

      if (filter && filter.length > 0) {
        setlistVehicleType(filter);
      }
    } else if (val && val === "FTL") {
      let filter = listVehicleTypeTemp.filter((x) =>
        x.maLoaiPhuongTien.includes("TRUCK")
      );

      if (filter && filter.length > 0) {
        setlistVehicleType(filter);
      }
    }
  };

  const handleCheckWeight = (type) => {
    let dataArr = watch("optionHandling");
    if (type === "PCS") {
      let totalPCS = watch("TongSoKien");
      let sumPCS = 0;

      dataArr.forEach((val) => {
        if (parseFloat(val.SoKien) > 0) {
          sumPCS = sumPCS + parseFloat(val.SoKien);
        }
      });

      if (sumPCS && totalPCS) {
        setTotalPCS({ sumPCS, totalPCS });
      } else {
        setTotalPCS({});
      }
    }

    if (type === "KG") {
      let totalWeight = watch("TongKhoiLuong");
      let sumWgt = 0;

      dataArr.forEach((val) => {
        if (parseFloat(val.KhoiLuong) > 0) {
          sumWgt = sumWgt + parseFloat(val.KhoiLuong);
        }
      });
      if (sumWgt && totalWeight) {
        setTotalWGT({ sumWgt, totalWeight });
      } else {
        setTotalWGT({});
      }
    }

    if (type === "CBM") {
      let totalCBM = watch("TongTheTich");
      let sumCBM = 0;
      dataArr.forEach((val) => {
        if (parseFloat(val.TheTich) > 0) {
          sumCBM = sumCBM + parseFloat(val.TheTich);
        }
      });
      if (sumCBM && totalCBM) {
        setTotalCBM({ sumCBM, totalCBM });
      } else {
        setTotalCBM({});
      }
    }
  };

  return (
    <>
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
                    <label htmlFor="LoaiVanDon">Phân Loại Vận Đơn(*)</label>
                    <select
                      className="form-control"
                      {...register("LoaiVanDon", Validate.LoaiVanDon)}
                      value={watch("LoaiVanDon")}
                      onChange={(e) =>
                        handleOnchangeTransportType(e.target.value)
                      }
                    >
                      <option value="nhap">Vận Đơn Nhập</option>
                      <option value="xuat">Vận Đơn Xuất</option>
                    </select>
                    {errors.LoaiVanDon && (
                      <span className="text-danger">
                        {errors.LoaiVanDon.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="LoaiHinh">Loại Hình(*)</label>
                    <select
                      className="form-control"
                      {...register("LoaiHinh", Validate.LoaiHinh)}
                      onChange={(e) => handleOnChangeLH(e.target.value)}
                    >
                      <option value="">Chọn Loại Hình</option>
                      <option value="FCL">FCL</option>
                      <option value="FTL">FTL</option>
                    </select>
                    {errors.LoaiHinh && (
                      <span className="text-danger">
                        {errors.LoaiHinh.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaKH">Khách Hàng(*)</label>
                    <Controller
                      name="MaKH"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          options={listCus}
                          onChange={(field) => handleOnChangeCustomer(field)}
                          value={field.value}
                        />
                      )}
                      rules={Validate.MaKH}
                    />
                    {errors.MaKH && (
                      <span className="text-danger">{errors.MaKH.message}</span>
                    )}
                  </div>
                </div>
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
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaVDKH">Mã Vận Đơn KH/Booking No(*)</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="MaVDKH"
                      {...register(`MaVDKH`, Validate.MaVDKH)}
                    />
                    {errors.MaVDKH && (
                      <span className="text-danger">
                        {errors.MaVDKH.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                {watch("LoaiVanDon") && watch("LoaiVanDon") === "xuat" && (
                  <>
                    <div className="col-sm">
                      <div className="form-group">
                        <label htmlFor="HangTau">Hãng Tàu</label>
                        <select
                          className="form-control"
                          {...register(`HangTau`, Validate.HangTau)}
                        >
                          <option value={""}>-- Để Trống --</option>
                          {listShipping &&
                            listShipping.map((val) => {
                              return (
                                <option
                                  value={val.shippingCode}
                                  key={val.shippingLineName}
                                >
                                  {val.shippingLineName}
                                </option>
                              );
                            })}
                        </select>
                        {errors.HangTau && (
                          <span className="text-danger">
                            {errors.HangTau.message}
                          </span>
                        )}
                      </div>
                    </div>

                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="TenTau">Tên Tàu</label>
                        <input
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="TenTau"
                          {...register(`TenTau`, Validate.TenTau)}
                        />
                        {errors.TenTau && (
                          <span className="text-danger">
                            {errors.TenTau.message}
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
                    <label htmlFor="DiemLayHang">Điểm Đóng Hàng(*)</label>
                    <Controller
                      name="DiemLayHang"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listFPlace}
                          // onChange={(field) =>
                          //   handleOnChangePoint(
                          //     setValue(
                          //       "DiemLayHang",
                          //       {
                          //         ...listFirstPoint.filter(
                          //           (x) => x.value === field.value
                          //         ),
                          //       }[0]
                          //     )
                          //   )
                          // }
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
                    <label htmlFor="DiemTraHang">Điểm Hạ Hàng(*)</label>
                    <Controller
                      name="DiemTraHang"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listSPlace}
                          // onChange={(field) =>
                          //   handleOnChangePoint(
                          //     setValue(
                          //       "DiemTraHang",
                          //       {
                          //         ...listSecondPoint.filter(
                          //           (x) => x.value === field.value
                          //         ),
                          //       }[0]
                          //     )
                          //   )
                          // }
                        />
                      )}
                      rules={Validate.DiemTraHang}
                    />
                    {errors.DiemTraHang && (
                      <span className="text-danger">
                        {errors.DiemTraHang.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TongKhoiLuong">
                      Tổng Khối Lượng (Đơn Vị KG)
                    </label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TongKhoiLuong"
                      {...register(`TongKhoiLuong`, Validate.TongKhoiLuong)}
                      onChange={(e) => {
                        setValue("TongKhoiLuong", e.target.value);
                        handleCheckWeight("KG");
                      }}
                    />
                    {errors.TongKhoiLuong && (
                      <span className="text-danger">
                        {errors.TongKhoiLuong.message}
                      </span>
                    )}
                  </div>
                  <br />
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TongTheTich">
                      Tổng Số Khối (Đơn Vị CBM)
                    </label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TongTheTich"
                      {...register(`TongTheTich`, Validate.TongTheTich)}
                      onChange={(e) => {
                        setValue("TongTheTich", e.target.value);
                        handleCheckWeight("CBM");
                      }}
                    />
                    {errors.TongTheTich && (
                      <span className="text-danger">
                        {errors.TongTheTich.message}
                      </span>
                    )}
                  </div>
                  <br />
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TongSoKien">
                      Tổng Số Kiện (Đơn Vị PCS)
                    </label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TongSoKien"
                      {...register(`TongSoKien`, Validate.TongSoKien)}
                      onChange={(e) => {
                        setValue("TongSoKien", e.target.value);
                        handleCheckWeight("PCS");
                      }}
                    />
                    {errors.TongSoKien && (
                      <span className="text-danger">
                        {errors.TongSoKien.message}
                      </span>
                    )}
                  </div>
                  <br />
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  <p style={{ fontWeight: "bold" }}>
                    Tổng Khối Lượng Phân Bổ :
                    {totalWGT.sumWgt && totalWGT.totalWeight && (
                      <>
                        {totalWGT.sumWgt + "/" + totalWGT.totalWeight}
                        {totalWGT.totalWeight < totalWGT.sumWgt ? (
                          <>
                            <p style={{ color: "red" }}>
                              (Vượt quá tổng khối lượng của đơn hàng)
                            </p>
                          </>
                        ) : (
                          <></>
                        )}
                      </>
                    )}
                  </p>
                </div>
                <div className="col col-sm">
                  <p style={{ fontWeight: "bold" }}>
                    Tổng Thể Tích Phân Bổ :
                    {totalCBM.sumCBM && totalCBM.totalCBM && (
                      <>
                        {totalCBM.sumCBM + "/" + totalCBM.totalCBM}
                        {totalCBM.totalCBM < totalCBM.sumCBM ? (
                          <>
                            <p style={{ color: "red" }}>
                              (Vượt quá tổng thể tích của đơn hàng)
                            </p>
                          </>
                        ) : (
                          <></>
                        )}
                      </>
                    )}
                  </p>
                </div>
                <div className="col col-sm">
                  <p style={{ fontWeight: "bold" }}>
                    Tổng Số Kiện Phân Bổ:
                    {totalPCS.sumPCS && totalPCS.totalPCS && (
                      <>
                        {totalPCS.sumPCS + "/" + totalPCS.totalPCS}
                        {totalPCS.totalPCS < totalPCS.sumPCS ? (
                          <>
                            <p style={{ color: "red" }}>
                              (Vượt quá tổng Kiện của đơn hàng)
                            </p>
                          </>
                        ) : (
                          <></>
                        )}
                      </>
                    )}
                  </p>
                </div>
              </div>
              <div className="row">
                <table
                  className="table table-sm table-bordered "
                  style={{
                    whiteSpace: "nowrap",
                  }}
                >
                  <thead>
                    <tr>
                      <th style={{ width: "40px" }}></th>
                      <th>
                        <div className="row">
                          <div className="col-sm-2">Loại Hàng Hóa(*)</div>
                          <div className="col-sm-2">Loại Phương Tiện(*)</div>
                          {watch(`optionHandling`) &&
                            watch(`optionHandling`).length > 0 &&
                            watch(`optionHandling`).filter((x) =>
                              x.PTVanChuyen.includes("CONT")
                            ).length > 0 && (
                              <>
                                {watch("LoaiVanDon") &&
                                watch("LoaiVanDon") === "xuat" ? (
                                  <>
                                    <div className="col-sm-2">
                                      Điểm Lấy Rỗng(*)
                                    </div>
                                    <div className="col-sm-1">Reuse CONT</div>
                                  </>
                                ) : (
                                  <div className="col-sm-2">
                                    Điểm trả Rỗng(*)
                                  </div>
                                )}
                              </>
                            )}
                          <div className="col-sm-1">Khối Lượng(KG)</div>
                          <div className="col-sm-1">Số Khối</div>
                          <div className="col-sm-1">Số Kiện</div>
                        </div>
                      </th>
                      <th style={{ width: "40px" }}>
                        <button
                          className="form-control form-control-sm"
                          type="button"
                          onClick={() => {
                            append(watch(`optionHandling`)[0]);
                            handleCheckWeight("PCS");
                            handleCheckWeight("KG");
                            handleCheckWeight("CBM");
                          }}
                        >
                          <i className="fas fa-plus"></i>
                        </button>
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {fields.map((value, index) => (
                      <tr key={index}>
                        <td>{index + 1}</td>
                        <td>
                          <div className="row">
                            <div className="col-sm-2">
                              <div className="form-group">
                                <select
                                  className="form-control"
                                  {...register(
                                    `optionHandling.${index}.LoaiHangHoa`,
                                    Validate.LoaiHangHoa
                                  )}
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
                                {errors.optionHandling?.[index]
                                  ?.LoaiHangHoa && (
                                  <span className="text-danger">
                                    {
                                      errors.optionHandling?.[index]
                                        ?.LoaiHangHoa.message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                            <div className="col-sm-2">
                              <div className="form-group">
                                <select
                                  className="form-control"
                                  {...register(
                                    `optionHandling.${index}.PTVanChuyen`,
                                    Validate.PTVanChuyen
                                  )}
                                  onChange={(e) => {
                                    handleOnChangeWeight(
                                      e.target.value,
                                      watch(
                                        `optionHandling.${index}.KhoiLuong`
                                      ),
                                      "KhoiLuong"
                                    );
                                    handleOnChangeWeight(
                                      e.target.value,
                                      watch(`optionHandling.${index}.TheTich`),
                                      "TheTich"
                                    );
                                    setValue(
                                      `optionHandling.${index}.PTVanChuyen`,
                                      e.target.value
                                    );
                                  }}
                                >
                                  <option value="">Chọn Phương Tiện</option>
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
                                {errors.optionHandling?.[index]
                                  ?.PTVanChuyen && (
                                  <span className="text-danger">
                                    {
                                      errors.optionHandling?.[index]
                                        ?.PTVanChuyen.message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                            {watch(`optionHandling.${index}.PTVanChuyen`) &&
                              watch(
                                `optionHandling.${index}.PTVanChuyen`
                              ).includes("CONT") && (
                                <>
                                  {watch("LoaiVanDon") === "xuat" ? (
                                    <>
                                      <div className="col-sm-2">
                                        <div className="form-group">
                                          <Controller
                                            name={`optionHandling.${index}.DiemLayRong`}
                                            control={control}
                                            render={({ field }) => (
                                              <Select
                                                {...field}
                                                classNamePrefix={"form-control"}
                                                value={field.value}
                                                options={listEPlace}
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
                                          {errors.optionHandling?.[index]
                                            ?.DiemLayRong && (
                                            <span className="text-danger">
                                              {
                                                errors.optionHandling?.[index]
                                                  ?.DiemLayRong.message
                                              }
                                            </span>
                                          )}
                                        </div>
                                      </div>
                                      <div className="col-sm-1">
                                        <div class="form-check">
                                          <input
                                            {...register(
                                              `optionHandling.${index}.ReuseCont`
                                            )}
                                            class="form-check-input"
                                            type="checkbox"
                                            value=""
                                            id={("ReuseCont", index)}
                                          />
                                          <label
                                            class="form-check-label"
                                            for={("ReuseCont", index)}
                                          >
                                            Reuse CONT
                                          </label>
                                        </div>
                                      </div>
                                    </>
                                  ) : (
                                    <>
                                      <div className="col-sm-2">
                                        <div className="form-group">
                                          <Controller
                                            name={`optionHandling.${index}.DiemTraRong`}
                                            control={control}
                                            render={({ field }) => (
                                              <Select
                                                {...field}
                                                classNamePrefix={"form-control"}
                                                value={field.value}
                                                options={listEPlace}
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
                                          {errors.optionHandling?.[index]
                                            ?.DiemTraRong && (
                                            <span className="text-danger">
                                              {
                                                errors.optionHandling?.[index]
                                                  ?.DiemTraRong.message
                                              }
                                            </span>
                                          )}
                                        </div>
                                      </div>
                                    </>
                                  )}
                                </>
                              )}
                            <div className="col-sm-1">
                              <div className="form-group">
                                <input
                                  type="text"
                                  className="form-control"
                                  placeholder="Khối Lượng"
                                  id="KhoiLuong"
                                  {...register(
                                    `optionHandling.${index}.KhoiLuong`,
                                    Validate.KhoiLuong
                                  )}
                                  onChange={(e) => {
                                    handleOnChangeWeight(
                                      watch(
                                        `optionHandling.${index}.PTVanChuyen`
                                      ),
                                      e.target.value,
                                      "KhoiLuong"
                                    );
                                    setValue(
                                      `optionHandling.${index}.KhoiLuong`,
                                      e.target.value
                                    );
                                    handleCheckWeight("KG");
                                  }}
                                />
                                {errors.optionHandling?.[index]?.KhoiLuong && (
                                  <span className="text-danger">
                                    {
                                      errors.optionHandling?.[index]?.KhoiLuong
                                        .message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                            <div className="col-sm-1">
                              <div className="form-group">
                                <input
                                  type="text"
                                  className="form-control"
                                  placeholder="Số Khối"
                                  id="TheTich"
                                  {...register(
                                    `optionHandling.${index}.TheTich`,
                                    Validate.TheTich
                                  )}
                                  onChange={(e) => {
                                    handleOnChangeWeight(
                                      watch(
                                        `optionHandling.${index}.PTVanChuyen`
                                      ),
                                      e.target.value,
                                      "TheTich"
                                    );
                                    setValue(
                                      `optionHandling.${index}.TheTich`,
                                      e.target.value
                                    );
                                    handleCheckWeight("CBM");
                                  }}
                                />
                                {errors.optionHandling?.[index]?.TheTich && (
                                  <span className="text-danger">
                                    {
                                      errors.optionHandling?.[index]?.TheTich
                                        .message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                            <div className="col-sm-1">
                              <div className="form-group">
                                <input
                                  type="text"
                                  className="form-control"
                                  placeholder="Số Kiện"
                                  id="SoKien"
                                  {...register(
                                    `optionHandling.${index}.SoKien`,
                                    Validate.SoKien
                                  )}
                                  onChange={(e) => {
                                    setValue(
                                      `optionHandling.${index}.SoKien`,
                                      e.target.value
                                    );
                                    handleCheckWeight("PCS");
                                  }}
                                />
                                {errors.optionHandling?.[index]?.SoKien && (
                                  <span className="text-danger">
                                    {
                                      errors.optionHandling?.[index]?.SoKien
                                        .message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                          </div>
                        </td>
                        <td>
                          <div className="form-group">
                            {index >= 1 && (
                              <button
                                type="button"
                                className="btn btn-title btn-sm btn-default mx-1"
                                gloss="Xóa Dòng"
                                onClick={() => {
                                  remove(index);
                                  handleCheckWeight("PCS");
                                  handleCheckWeight("KG");
                                  handleCheckWeight("CBM");
                                }}
                              >
                                <i className="fas fa-minus"></i>
                              </button>
                            )}
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              {watch(`optionHandling`) &&
                watch(`optionHandling`).length > 0 &&
                watch(`optionHandling`).filter((x) =>
                  x.PTVanChuyen.includes("CONT")
                ).length > 0 && (
                  <div className="row">
                    <div className="col col-sm">
                      <div className="form-group">
                        {watch("LoaiVanDon") === "xuat" && (
                          <div className="input-group ">
                            <label htmlFor="TGLayRong">
                              Thời Gian Lấy Rỗng
                            </label>
                            <Controller
                              control={control}
                              name={`TGLayRong`}
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
                              // rules={{
                              //   required: "không được để trống",
                              // }}
                            />
                            {errors.TGLayRong && (
                              <span className="text-danger">
                                {errors.TGLayRong.message}
                              </span>
                            )}
                          </div>
                        )}
                        {watch("LoaiVanDon") === "nhap" && (
                          <div className="input-group ">
                            <label htmlFor="TGTraRong">
                              Thời Gian Trả Rỗng
                            </label>
                            <Controller
                              control={control}
                              name={`TGTraRong`}
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
                              // rules={{
                              //   required: "không được để trống",
                              // }}
                            />
                            {errors.TGTraRong && (
                              <span className="text-danger">
                                {errors.TGTraRong.message}
                              </span>
                            )}
                          </div>
                        )}
                      </div>
                    </div>
                    {watch("LoaiVanDon") === "xuat" && (
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="TGHaCang">Thời Gian CUT OFF(*)</label>
                          <div className="input-group ">
                            <Controller
                              control={control}
                              name={`TGHaCang`}
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
                              rules={{
                                required: "không được để trống",
                              }}
                            />
                            {errors.TGHaCang && (
                              <span className="text-danger">
                                {errors.TGHaCang.message}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                    )}
                    {watch("LoaiVanDon") === "nhap" && (
                      <>
                        <div className="col col-sm">
                          <div className="form-group">
                            <label htmlFor="TGCoMat">Thời Gian Có Mặt</label>
                            <div className="input-group ">
                              <Controller
                                control={control}
                                name={`TGCoMat`}
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
                                // rules={{
                                //   required: "không được để trống",
                                // }}
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
                            <label htmlFor="TGHanLenh">
                              Thời Gian Hạn Lệnh(*)
                            </label>
                            <div className="input-group ">
                              <Controller
                                control={control}
                                name={`TGHanLenh`}
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
                                rules={{
                                  required: "không được để trống",
                                }}
                              />
                              {errors.TGHanLenh && (
                                <span className="text-danger">
                                  {errors.TGHanLenh.message}
                                </span>
                              )}
                            </div>
                          </div>
                        </div>
                      </>
                    )}
                  </div>
                )}

              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TGLayHang">Thời Gian Lấy Hàng</label>
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name={`TGLayHang`}
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
                        // rules={{
                        //   required: "không được để trống",
                        // }}
                      />
                      {errors.TGLayHang && (
                        <span className="text-danger">
                          {errors.TGLayHang.message}
                        </span>
                      )}
                    </div>
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TGTraHang">Thời Gian Trả Hàng</label>
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name={`TGTraHang`}
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
                        // rules={{
                        //   required: "không được để trống",
                        // }}
                      />
                      {errors.TGTraHang && (
                        <span className="text-danger">
                          {errors.TGTraHang.message}
                        </span>
                      )}
                    </div>
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-12">
                  <div className="form-group">
                    <label htmlFor="GhiChu">Ghi Chú</label>
                    <div className="input-group ">
                      <textarea
                        className="form-control"
                        rows={3}
                        {...register(`GhiChu`)}
                      ></textarea>
                    </div>
                  </div>
                </div>
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
    </>
  );
};

export default CreateTransport;
