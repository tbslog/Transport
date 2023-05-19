import { useState, useEffect, useLayoutEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import moment from "moment";
import { ToastError, ToastWarning } from "../Common/FuncToast";
import Cookies from "js-cookie";
import LoadingPage from "../Common/Loading/LoadingPage";

const UpdateTransport = (props) => {
  const { getListTransport, selectIdClick, hideModal } = props;
  const accountType = Cookies.get("AccType");
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
    },
    LoaiHangHoa: {
      required: "Không được để trống",
    },
    TongSoKien: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,6})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
      validate: (value) => {
        if (parseInt(value) < 1) {
          return "Không được nhỏ hơn 1";
        }
      },
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
  };

  const [IsLoading, SetIsLoading] = useState(false);
  const [listCus, setListCus] = useState([]);
  const [listPoint, setListPoint] = useState([]);
  const [listSupplier, setListSupplier] = useState([]);
  const [listVehicleType, setlistVehicleType] = useState([]);
  const [listVehicleTypeTemp, setListVehicleTypeTemp] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listTransportType, setListTransportType] = useState([]);
  const [listShipping, setListShipping] = useState([]);
  const [listAccountCus, setListAccountCus] = useState([]);

  const [totalCBM, setTotalCBM] = useState("");
  const [totalWGT, setTotalWGT] = useState("");
  const [totalPCS, setTotalPCS] = useState("");

  const [dataTransport, setDataTransport] = useState();

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
        setListSupplier(arrSupplier);
      } else {
        setListCus([]);
        setListSupplier([]);
      }

      let getListShipping = await getData("Common/GetListShipping");
      setListShipping(getListShipping);
      let getListVehicleType = await getData("Common/GetListVehicleType");
      setlistVehicleType(getListVehicleType);
      setListVehicleTypeTemp(getListVehicleType);

      let getListGoodsType = await getData("Common/GetListGoodsType");
      setListGoodsType(getListGoodsType);

      let getListTransportType = await getData("Common/GetListTransportType");
      setListTransportType(getListTransportType);

      const getListPoint = await getData(
        "Address/GetListAddressSelect?pointType=&type=Diem"
      );
      if (getListPoint && getListPoint.length > 0) {
        var obj = [];
        getListPoint.map((val) => {
          obj.push({
            value: val.maDiaDiem,
            label: val.maDiaDiem + " - " + val.tenDiaDiem,
          });
        });
        setListPoint(obj);
      }
      SetIsLoading(false);
    })();
  }, []);

  useLayoutEffect(() => {
    if (selectIdClick && Object.keys(selectIdClick).length > 0) {
      (async () => {
        let getTransportById = await getData(
          `BillOfLading/GetTransportById?transportId=${selectIdClick.maVanDon}`
        );

        if (getTransportById && Object.keys(getTransportById).length > 0) {
          setDataTransport(getTransportById);
        } else {
          setDataTransport({});
        }
      })();
    }
  }, [selectIdClick]);

  useLayoutEffect(() => {
    if (
      dataTransport &&
      Object.keys(dataTransport).length > 0 &&
      listSupplier &&
      listPoint &&
      listSupplier.length > 0 &&
      listPoint.length > 0 &&
      listGoodsType &&
      listGoodsType.length > 0 &&
      listVehicleType &&
      listVehicleType.length > 0
    ) {
      SetIsLoading(true);

      setValue("optionHandling", [{}]);

      setValue(
        "DiemLayHang",
        { ...listPoint.filter((x) => x.value === dataTransport.diemDau) }[0]
      );
      setValue(
        "DiemTraHang",
        { ...listPoint.filter((x) => x.value === dataTransport.diemCuoi) }[0]
      );

      let arrData = [];
      dataTransport.arrHandlings.map((val) => {
        arrData.push({
          DonViVanTai: !val.donViVanTai
            ? null
            : {
                ...listSupplier.filter((x) => x.value === val.donViVanTai),
              }[0],
          LoaiHangHoa: val.loaiHangHoa,
          PTVanChuyen: val.ptVanChuyen,
          KhoiLuong: val.khoiLuong,
          TheTich: val.theTich,
          SoKien: val.soKien,
          ContNo: val.contNo,
          ReuseCont: val.reuseCont,
          DiemTraRong: !val.diemTraRong
            ? null
            : {
                ...listPoint.filter((x) => x.value === val.diemTraRong),
              }[0],
          DiemLayRong: !val.diemLayRong
            ? null
            : {
                ...listPoint.filter((x) => x.value === val.diemLayRong),
              }[0],
        });
      });

      for (let i = 0; i <= arrData.length - 1; i++) {
        setValue(`optionHandling.${i}.DonViVanTai`, arrData[i].DonViVanTai);
        setValue(`optionHandling.${i}.LoaiHangHoa`, arrData[i].LoaiHangHoa);
        setValue(`optionHandling.${i}.PTVanChuyen`, arrData[i].PTVanChuyen);
        setValue(`optionHandling.${i}.KhoiLuong`, arrData[i].KhoiLuong);
        setValue(`optionHandling.${i}.TheTich`, arrData[i].TheTich);
        setValue(`optionHandling.${i}.SoKien`, arrData[i].SoKien);
        setValue(`optionHandling.${i}.DiemLayRong`, arrData[i].DiemLayRong);
        setValue(`optionHandling.${i}.DiemTraRong`, arrData[i].DiemTraRong);
        setValue(`optionHandling.${i}.ContNo`, arrData[i].ContNo);
        setValue(`optionHandling.${i}.ReuseCont`, arrData[i].ReuseCont);
      }
      setValue("optionHandling", arrData);
    } else {
      setValue("optionHandling", []);
    }
    SetIsLoading(false);
  }, [listSupplier, listPoint, listGoodsType, listVehicleType, dataTransport]);

  useLayoutEffect(() => {
    if (
      listCus &&
      listCus.length > 0 &&
      dataTransport &&
      listShipping &&
      listShipping.length > 0 &&
      Object.keys(dataTransport).length > 0 &&
      listTransportType &&
      listTransportType.length > 0
    ) {
      setValue("MaVDKH", dataTransport.maVanDonKH);
      setValue("LoaiVanDon", dataTransport.loaiVanDon);
      setValue("TongKhoiLuong", dataTransport.tongKhoiLuong);
      setValue("TongTheTich", dataTransport.tongTheTich);
      setValue("TongSoKien", dataTransport.tongSoKien);
      setValue("LoaiHinh", dataTransport.maPTVC.replaceAll(" ", ""));
      handleOnChangeCustomer(
        { ...listCus.filter((x) => x.value === dataTransport.maKh) }[0]
      );

      if (dataTransport.loaiVanDon === "xuat") {
        setValue(
          "HangTau",
          {
            ...listShipping.filter((x) => x.value === dataTransport.hangTau),
          }[0]
        );
        setValue("TenTau", dataTransport.tenTau);
        setValue(
          "TGLayRong",
          !dataTransport.thoiGianLayRong
            ? null
            : new Date(dataTransport.thoiGianLayRong)
        );
        setValue(
          "TGHaCang",
          !dataTransport.thoiGianHaCang
            ? null
            : new Date(dataTransport.thoiGianHaCang)
        );
      } else {
        setValue(
          "TGTraRong",
          !dataTransport.thoiGianTraRong
            ? null
            : new Date(dataTransport.thoiGianTraRong)
        );
        setValue(
          "TGCoMat",
          !dataTransport.thoiGianCoMat
            ? null
            : new Date(dataTransport.thoiGianCoMat)
        );
        setValue(
          "TGHanLenh",
          !dataTransport.thoiGianHanLenh
            ? null
            : new Date(dataTransport.thoiGianHanLenh)
        );
      }
      setValue("GhiChu", dataTransport.ghiChu);

      setValue(
        "TGTraHang",
        !dataTransport.thoiGianTraHang
          ? null
          : new Date(dataTransport.thoiGianTraHang)
      );
      setValue(
        "TGLayHang",
        !dataTransport.thoiGianLayHang
          ? null
          : new Date(dataTransport.thoiGianLayHang)
      );
      SetIsLoading(false);
    }
  }, [listCus, listTransportType, dataTransport, listShipping]);

  useEffect(() => {
    if (
      watch("optionHandling") &&
      watch("optionHandling").length > 0 &&
      dataTransport &&
      Object.keys(dataTransport).length > 0
    ) {
      handleCheckWeight("PCS");
      handleCheckWeight("KG");
      handleCheckWeight("CBM");
    }
  }, [watch("optionHandling"), dataTransport]);

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

  useLayoutEffect(() => {
    if (listAccountCus && listAccountCus.length > 0) {
      setValue(
        "AccountCus",
        {
          ...listAccountCus.filter((x) => x.value === dataTransport.accountId),
        }[0]
      );
    }
  }, [listAccountCus]);

  const handleOnChangeCustomer = async (val) => {
    if (val && Object.keys(val).length > 0) {
      setListAccountCus([]);
      setValue("AccountCus", null);
      setValue("MaKH", val);
      const getListAcc = await getData(
        `AccountCustomer/GetListAccountSelectByCus?accountId=${val.value}`
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

  const handleOnchangeType = (val) => {
    setValue("LoaiHinh", val);

    if (val) {
      let temp = listVehicleTypeTemp;

      if (val === "FCL") {
        temp = temp.filter((x) => x.phanLoai === "Cont");
      }
      if (val === "FTL") {
        temp = temp.filter((x) => x.phanLoai === "Truck");
      }

      setlistVehicleType(temp);
    } else {
      setlistVehicleType([]);
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

  const onSubmit = async (data) => {
    SetIsLoading(true);

    let arr = [];
    data.optionHandling.map((val) => {
      arr.push({
        DiemTraRong: !val.DiemTraRong ? null : val.DiemTraRong.value,
        DiemLayRong: !val.DiemLayRong ? null : val.DiemLayRong.value,
        LoaiHangHoa: val.LoaiHangHoa,
        PTVanChuyen: val.PTVanChuyen,
        ReuseCont: !val.ReuseCont ? false : val.ReuseCont,
        KhoiLuong: !val.KhoiLuong ? null : val.KhoiLuong,
        TheTich: !val.TheTich ? null : val.TheTich,
        SoKien: !val.SoKien ? null : val.SoKien,
        DonViTinh: "CHUYEN",
      });
    });

    if (arr && arr.length < 1) {
      ToastError("Vui lòng điền đầy đủ thông tin");
      return;
    }

    const Update = await postData(
      `BillOfLading/UpdateTransport?transportId=${dataTransport.maVanDon}`,
      {
        AccountId: !data.AccountCus ? null : data.AccountCus.value,
        MaPTVC: data.LoaiHinh,
        DiemDau: data.DiemLayHang.value,
        DiemCuoi: data.DiemTraHang.value,
        arrHandlings: arr,
        HangTau: data.HangTau,
        TenTau: data.TenTau,
        MaVanDonKH: data.MaVDKH,
        LoaiVanDon: data.LoaiVanDon,
        TongKhoiLuong: !data.TongKhoiLuong ? null : data.TongKhoiLuong,
        TongTheTich: !data.TongTheTich ? null : data.TongTheTich,
        TongSoKien: !data.TongSoKien ? null : data.TongSoKien,
        MaKH: data.MaKH.value,
        GhiChu: data.GhiChu,
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
        ThoiGianLayHang: !data.TGLayHang
          ? null
          : moment(new Date(data.TGLayHang).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
        ThoiGianTraHang: !data.TGTraHang
          ? null
          : moment(new Date(data.TGTraHang).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
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
      }
    );

    if (Update === 1) {
      getListTransport();
      hideModal();
    }

    SetIsLoading(false);
  };

  return (
    <>
      <div className="card card-primary">
        <div>{IsLoading === true && <LoadingPage></LoadingPage>}</div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="LoaiHinh">Loại Hình(*)</label>
                    <select
                      className="form-control"
                      {...register("LoaiHinh", Validate.LoaiHinh)}
                      onChange={(e) => handleOnchangeType(e.target.value)}
                    >
                      <option value="">Chọn Loại Hình</option>
                      <option value="FCL">FCL</option>
                      <option value="FTL">FTL</option>
                      {/* {listTransportType &&
                        listTransportType.length > 0 &&
                        listTransportType.map((val) => {
                          return (
                            <option value={val.maPtvc} key={val.maPtvc}>
                              {val.tenPtvc}
                            </option>
                          );
                        })} */}
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
                    <label htmlFor="LoaiVanDon">Phân Loại Vận Đơn(*)</label>
                    <select
                      className="form-control"
                      {...register("LoaiVanDon", Validate.LoaiVanDon)}
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
                    <label htmlFor="MaKH">Khách Hàng(*)</label>
                    <Controller
                      name="MaKH"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listCus}
                          onChange={(field) => handleOnChangeCustomer(field)}
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
                          options={listPoint}
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
                          options={listPoint}
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
                {/* <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaCungDuong">Cung Đường(*)</label>
                    <Controller
                      name="MaCungDuong"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listRoad}
                          onChange={(field) => handleOnChangeRoad(field)}
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
                </div> */}
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
                    Tổng Số Trọng Lượng :
                    {totalWGT.sumWgt && totalWGT.totalWeight && (
                      <>
                        {totalWGT.sumWgt + "/" + totalWGT.totalWeight}
                        {totalWGT.totalWeight < totalWGT.sumWgt ? (
                          <>
                            <p style={{ color: "red" }}>
                              (Vượt quá tổng trọng lượng của đơn hàng)
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
                    Tổng Số Khối :
                    {totalCBM.sumCBM && totalCBM.totalCBM && (
                      <>
                        {totalCBM.sumCBM + "/" + totalCBM.totalCBM}
                        {totalCBM.totalCBM < totalCBM.sumCBM ? (
                          <>
                            <p style={{ color: "red" }}>
                              (Vượt quá tổng số khối của đơn hàng)
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
                    Tổng Số Kiện:
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
                    {fields &&
                      fields.length > 0 &&
                      fields.map((value, index) => (
                        <tr key={index}>
                          <td>{index + 1}</td>
                          <td>
                            <div className="row">
                              {/* {accountType && accountType === "NV" && (
                                <div className="col-sm-2">
                                  <div className="form-group">
                                    <Controller
                                      name={`optionHandling.${index}.DonViVanTai`}
                                      control={control}
                                      render={({ field }) => (
                                        <Select
                                          {...field}
                                          classNamePrefix={"form-control"}
                                          value={field.value}
                                          options={listSupplier}
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
                                      ?.DonViVanTai && (
                                      <span className="text-danger">
                                        {
                                          errors.optionHandling?.[index]
                                            ?.DonViVanTai.message
                                        }
                                      </span>
                                    )}
                                  </div>
                                </div>
                              )} */}
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
                                        watch(
                                          `optionHandling.${index}.TheTich`
                                        ),
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
                                    {/* <div className="col-sm-2">
                                      <div className="form-group">
                                        <input
                                          type="text"
                                          className="form-control"
                                          placeholder="Mã Container"
                                          id="ContNo"
                                          {...register(
                                            `optionHandling.${index}.ContNo`,
                                            Validate.ContNo
                                          )}
                                        />
                                        {errors.optionHandling?.[index]
                                          ?.ContNo && (
                                          <span className="text-danger">
                                            {
                                              errors.optionHandling?.[index]
                                                ?.ContNo.message
                                            }
                                          </span>
                                        )}
                                      </div>
                                    </div> */}

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
                                                  classNamePrefix={
                                                    "form-control"
                                                  }
                                                  value={field.value}
                                                  options={listPoint}
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
                                                  classNamePrefix={
                                                    "form-control"
                                                  }
                                                  value={field.value}
                                                  options={listPoint}
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
                                  {errors.optionHandling?.[index]
                                    ?.KhoiLuong && (
                                    <span className="text-danger">
                                      {
                                        errors.optionHandling?.[index]
                                          ?.KhoiLuong.message
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
                                  className="btn btn-sm btn-default mx-1"
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
                  type="submit"
                  className="btn btn-primary"
                  style={{ float: "right" }}
                >
                  Cập Nhật
                </button>
              </div>
            </div>
          </form>
        )}
      </div>
    </>
  );
};

export default UpdateTransport;
