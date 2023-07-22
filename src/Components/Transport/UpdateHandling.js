import { useState, useEffect, useRef } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import Select from "react-select";
import moment from "moment";
import LoadingPage from "../Common/Loading/LoadingPage";
import { ToastWarning } from "../Common/FuncToast";
import Cookies from "js-cookie";
import HandlingImage from "../FileManager/HandlingImage";
import AddSubFeeByHandling from "./AddSubFeeByHandling";
import UpdateTransport from "./UpdateTransport";

const UpdateHandling = (props) => {
  const { getlistData, selectIdClick, hideModal } = props;
  const accountType = Cookies.get("AccType");
  const {
    register,
    reset,
    setValue,
    control,
    watch,
    validate,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
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
      validate: (value) => {
        if (!value.value) {
          return "Không được để trống";
        }
      },
    },
    maTaiXe: {
      // required: "Không được để trống",
      // validate: (val) => {
      //   if (!val.value) {
      //     return "Không được để trống";
      //   }
      // },
    },
    maSoXe: {
      // required: "Không được để trống",
      // validate: (val) => {
      //   if (!val.value) {
      //     return "Không được để trống";
      //   }
      // },
    },
    XeVanChuyenTxt: {},
    diemLayRong: {
      required: "Không được để trống",
      validate: (val) => {
        if (!val.value) {
          return "Không được để trống";
        }
      },
    },
    CONTNO: {
      pattern: {
        value: /([A-Z]{3})([UJZ])(\d{6})(\d)/,
        message: "Mã CONT NO không đúng, Phải viết HOA",
      },
    },
    // SEALHQ: {
    //   pattern: {
    //     value: /^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9 ]+(?<![_.])$/,
    //     message: "Không được chứa ký tự đặc biệt, phải viết hoa",
    //   },
    // },
    // SEALHT: {
    //   pattern: {
    //     value: /^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9 ]+(?<![_.])$/,
    //     message: "Không được chứa ký tự đặc biệt, phải viết hoa",
    //   },
    // },
    // SEALNP: {
    //   pattern: {
    //     value: /^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9 ]+(?<![_.])$/,
    //     message: "Không được chứa ký tự đặc biệt, phải viết hoa",
    //   },
    // },
    KhoiLuong: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,6})?)(?:e[+-]?\d+)?$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TheTich: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,6})?)(?:e[+-]?\d+)?$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    SoKien: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,6})?)(?:e[+-]?\d+)?$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    PTVanChuyen: {
      required: "Không được để trống",
    },
    LoaiHangHoa: {
      required: "Không được để trống",
    },
    HangTau: {
      required: "Không được để trống",
    },
    TenTau: {
      required: "Không được để trống",
    },
  };

  const [listPoint, setListPoint] = useState([]);
  const [IsLoading, SetIsLoading] = useState(false);
  const [listCustomer, setListCustomer] = useState([]);
  const [listVehicleType, setlistVehicleType] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listDriver, setListDriver] = useState([]);
  const [listVehicle, setListVehicle] = useState([]);
  const [listRomooc, setListRomooc] = useState([]);
  const [transportType, setTransportType] = useState("");
  const [data, setData] = useState({});
  const [roadDetail, setRoadDetail] = useState({});
  const [listSupplier, setListSupplier] = useState([]);
  const [listPlace, setListPlace] = useState([]);

  const [wgtVehicle, setWgtVehicle] = useState();
  const [cbmVehicle, setCbmVehicle] = useState();

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();
  const [title, setTitle] = useState("");

  const [isCheckVehicle, setIsCheckVehicle] = useState(false);

  useEffect(() => {
    (async () => {
      SetIsLoading(true);
      let getListVehicleType = await getData("Common/GetListVehicleType");
      let getListGoodsType = await getData("Common/GetListGoodsType");

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

      const getListPlace = await getData(
        "Address/GetListAddressSelect?pointType=&type=Diem"
      );
      let arrPlace = [];
      arrPlace.push({ label: "-- Rỗng --", value: null });
      getListPlace.forEach((val) => {
        arrPlace.push({ label: val.tenDiaDiem, value: val.maDiaDiem });
      });
      setListPlace(arrPlace);

      let getListNCC = await getData(`Customer/GetListCustomerOptionSelect`);
      if (getListNCC && getListNCC.length > 0) {
        let listSup = getListNCC.filter((x) => x.loaiKH === "NCC");
        let objSup = [];

        listSup.map((val) => {
          objSup.push({
            value: val.maKh,
            label: val.maKh + " - " + val.tenKh,
          });
        });
        setListSupplier(objSup);

        let objCus = [];
        let listCus = getListNCC.filter((x) => x.loaiKH === "KH");
        listCus.map((val) => {
          objCus.push({
            value: val.maKh,
            label: val.maKh + " - " + val.tenKh,
          });
        });
        setListCustomer(objCus);
      }

      let getListDriver = await getData(`Driver/GetListSelectDriver`);
      if (getListDriver && getListDriver.length > 0) {
        let obj = [];
        getListDriver.map((val) => {
          obj.push({
            value: val.maTaiXe,
            label: val.maTaiXe + " - " + val.hoVaTen,
          });
        });
        setListDriver(obj);
      }

      let getRomooc = await getData(`Romooc/GetListRomoocSelect`);
      if (getRomooc && getRomooc.length > 0) {
        let obj = [];
        obj.push({ label: "--Rỗng--", value: null });
        getRomooc.map((val) => {
          obj.push({
            value: val.maRomooc,
            label: val.maRomooc + " - " + val.romoocType,
          });
        });
        setListRomooc(obj);
      }

      loadDataVehicle();

      setlistVehicleType(getListVehicleType);
      setListGoodsType(getListGoodsType);
      SetIsLoading(false);
    })();
  }, []);

  const loadDataVehicle = async () => {
    let listVehicle = await getData("Vehicle/GetListVehicleSelect");
    if (listVehicle && listVehicle.length > 0) {
      let arr = [];
      listVehicle.map((val) => {
        arr.push({
          label: val.text,
          value: val.vehicleId,
        });
      });
      setListVehicle(arr);
    }
  };

  const showModalForm = () => {
    const modal = new Modal(parseExceptionModal.current, {
      keyboard: false,
      backdrop: "static",
    });
    setModal(modal);
    modal.show();
  };
  const handleShowModal = () => {
    showModalForm();
  };

  useEffect(() => {
    if (
      props &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0 &&
      listVehicleType &&
      listGoodsType &&
      listVehicleType.length > 0 &&
      listGoodsType.length > 0
    ) {
      handleResetClick();
      setIsCheckVehicle(false);
      (async () => {
        let data = await getData(
          `BillOfLading/GetHandlingById?id=${selectIdClick.maDieuPhoi}`
        );
        setIsCheckVehicle(false);

        if (data && Object.keys(data).length > 0) {
          setData(data);
        } else {
          setData({});
        }
      })();
    }
  }, [props, selectIdClick, listVehicleType, listGoodsType, listPoint]);

  useEffect(() => {
    if (
      Object.keys(data).length > 0 &&
      data &&
      listDriver &&
      listRomooc &&
      listVehicle
    ) {
      setValueData(data);
    }
  }, [data, listDriver, listRomooc, listVehicle]);

  const setValueData = (data) => {
    setRoadDetail(data.cungDuong);

    setValue(
      "KhachHang",
      {
        ...listCustomer.filter((x) => x.value === data.maKh),
      }[0]
    );

    setValue(
      "NhaCungCap",
      !data.donViVanTai
        ? { label: "Select...", value: null }
        : {
            ...listSupplier.filter((x) => x.value === data.donViVanTai),
          }[0]
    );

    setValue(
      "DiemLayRong",
      !data.diemLayRong
        ? { label: "Select...", value: null }
        : {
            ...listPoint.filter((x) => x.value === data.diemLayRong),
          }[0]
    );

    setValue(
      "DiemTraRong",
      !data.diemTraRong
        ? { label: "Select...", value: null }
        : {
            ...listPoint.filter((x) => x.value === data.diemTraRong),
          }[0]
    );

    setValue(
      "TaiXe",
      !data.maTaiXe
        ? { label: "Select...", value: null }
        : {
            ...listDriver.filter((x) => x.value === data.maTaiXe),
          }[0]
    );
    setValue(
      "XeVanChuyen",
      !data.maSoXe
        ? { label: "Select...", value: null }
        : {
            ...listVehicle.filter((x) => x.value === data.maSoXe),
          }[0]
    );

    setValue("Account", data.accountName);
    setValue("TongKhoiLuong", data.tongKhoiLuong);
    setValue("TongTheTich", data.tongTheTich);
    setValue("GhiChuVanDon", data.ghiChuVanDon);
    setValue("CONTNO", data.contNo);
    setValue("SEALNP", data.sealNp);
    setValue("SEALHQ", data.sealHq);
    setValue("GhiChu", data.ghiChu);
    setTransportType(data.phanLoaiVanDon);
    setValue("PTVanChuyen", data.ptVanChuyen);
    setValue("LoaiHangHoa", data.maLoaiHangHoa);
    setValue("KhoiLuong", data.khoiLuong);
    setValue("TheTich", data.theTich);
    setValue("SoKien", data.soKien);
    setValue("ReuseCont", data.reuseCont);
    setValue("HangTau", data.hangTau);
    setValue("TenTau", data.tenTau);
    setValue(
      "TGLayRong",
      !data.thoiGianLayTraRong ? null : new Date(data.thoiGianLayRong)
    );

    setValue(
      "TGTraRong",
      !data.thoiGianLayTraRong ? null : new Date(data.thoiGianTraRong)
    );

    setValue(
      "TGHaCang",
      !data.thoiGianHaCang ? null : new Date(data.thoiGianHaCang)
    );
    setValue(
      "TGCoMat",
      !data.thoiGianCoMat ? null : new Date(data.thoiGianCoMat)
    );
    setValue(
      "TGHanLenh",
      !data.thoiGianHanLenh ? null : new Date(data.thoiGianHanLenh)
    );
    setValue(
      "TGLayHang",
      !data.thoiGianLayHang ? null : new Date(data.thoiGianLayHang)
    );
    setValue(
      "TGTraHang",
      !data.thoiGianTraHang ? null : new Date(data.thoiGianTraHang)
    );

    setValue(
      "TGTraRongThucTe",
      !data.thoiGianLayTraRongThucTe
        ? null
        : new Date(data.ThoiGianTraRongThucTe)
    );

    setValue(
      "TGLayRongThucTe",
      !data.thoiGianLayTraRongThucTe
        ? null
        : new Date(data.ThoiGianLayRongThucTe)
    );

    setValue(
      "TGCoMatThucTe",
      !data.thoiGianCoMatThucTe ? null : new Date(data.thoiGianCoMatThucTe)
    );
    setValue(
      "TGLayHangThucTe",
      !data.thoiGianLayHangThucTe ? null : new Date(data.thoiGianLayHangThucTe)
    );
    setValue(
      "TGTraHangThucTe",
      !data.thoiGianTraHangThucTe ? null : new Date(data.thoiGianTraHangThucTe)
    );

    SetIsLoading(false);
  };

  const handleResetClick = () => {
    reset();
    setData({});
    setValue("NhaCungCap", null);
    setValue("KhachHang", null);
    setValue("TaiXe", null);
    setValue("XeVanChuyen", null);
    setValue("Romooc", null);
  };

  const handleOnChangeWeight = async (vehicleType) => {
    if (vehicleType) {
      setValue("PTVanChuyen", vehicleType);
      let getTonnageVehicle = await getData(
        `BillOfLading/LayTrongTaiXe?vehicleType=${vehicleType}`
      );

      if (getTonnageVehicle) {
        let vehicleCBM = getTonnageVehicle.cbm;
        let vehicleWGT = getTonnageVehicle.wgt;
        setWgtVehicle(vehicleWGT);
        setCbmVehicle(vehicleCBM);
      } else {
        setWgtVehicle({});
        setCbmVehicle({});
      }
    }
  };

  const handleOnCheckVehicle = (val) => {
    if (val && val === true) {
      setIsCheckVehicle(true);
      setValue("XeVanChuyenTxt", "");
    } else {
      setIsCheckVehicle(false);
    }
  };

  const onSubmit = async (data) => {
    console.log(data);
    SetIsLoading(true);

    let vehicleID;
    if (isCheckVehicle === true) {
      vehicleID = !data.XeVanChuyenTxt ? null : data.XeVanChuyenTxt;
    } else {
      vehicleID = !data.XeVanChuyen
        ? null
        : !data.XeVanChuyen.value
        ? null
        : data.XeVanChuyen.value;
    }

    let dataUpdate = {
      DonViVanTai: data.NhaCungCap.value,
      PTVanChuyen: data.PTVanChuyen,
      LoaiHangHoa: data.LoaiHangHoa,
      DonViTinh: "CHUYEN",
      DiemLayRong: !data.DiemLayRong ? null : data.DiemLayRong.value,
      DiemTraRong: !data.DiemTraRong ? null : data.DiemTraRong.value,
      MaSoXe: vehicleID,
      ReuseCont: !data.ReuseCont ? false : data.ReuseCont,
      MaTaiXe: !data.TaiXe ? null : !data.TaiXe.value ? null : data.TaiXe.value,
      MaRomooc: !data.Romooc
        ? null
        : !data.Romooc.value
        ? null
        : data.Romooc.value,
      ContNo: data.CONTNO,
      SealNp: data.SEALNP,
      SealHq: data.SEALHQ,
      KhoiLuong: !data.KhoiLuong ? null : data.KhoiLuong,
      TheTich: !data.TheTich ? null : data.TheTich,
      SoKien: !data.SoKien ? null : data.SoKien,
      GhiChu: data.GhiChu,
      ThoiGianTraRongThucTe: !data.ThoiGianTraRongThucTe
        ? null
        : moment(new Date(data.ThoiGianTraRongThucTe).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      ThoiGianLayRongThucTe: !data.ThoiGianLayRongThucTe
        ? null
        : moment(new Date(data.ThoiGianLayRongThucTe).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      ThoiGianCoMatThucTe: !data.TGCoMatThucTe
        ? null
        : moment(new Date(data.TGCoMatThucTe).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      ThoiGianLayHangThucTe: !data.TGLayHangThucTe
        ? null
        : moment(new Date(data.TGLayHangThucTe).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      ThoiGianTraHangThucTe: !data.TGTraHangThucTe
        ? null
        : moment(new Date(data.TGTraHangThucTe).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
    };

    let update = await postData(
      `BillOfLading/UpdateHandling?id=${selectIdClick.maDieuPhoi}`,
      dataUpdate
    );

    if (update === 1) {
      setIsCheckVehicle(false);
      loadDataVehicle();
      getlistData();
      hideModal();
    }

    SetIsLoading(false);
  };

  const handleOnClickChangeSplace = async () => {
    if (selectIdClick && Object.keys(selectIdClick).length > 0) {
      let place = watch("listPlace");

      if (place && Object.keys(place).length > 0 && place.value) {
        SetIsLoading(true);
        let changePlace = await postData(
          "BillOfLading/ChangeSecondPlaceHandling",
          {
            transportId: selectIdClick.maVanDon,
            handlingId: selectIdClick.maDieuPhoi,
            newSecondPlace: place.value,
          }
        );

        setValue(
          "listPlace",
          listPlace.find((x) => x.value === null)
        );

        if (changePlace === 1) {
          modal.hide();
          getlistData();
        }
        SetIsLoading(false);
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
                    <label htmlFor="LoaiHinh">Loại Hình</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="Account"
                      value={data.maPTVC}
                      readOnly
                    />
                  </div>
                </div>
                {accountType && accountType === "NV" && (
                  <>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="KhachHang">Khách Hàng(*)</label>
                        <Controller
                          name={`KhachHang`}
                          AccountName
                          control={control}
                          render={({ field }) => (
                            <Select
                              isDisabled={true}
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
                        {errors.KhachHang && (
                          <span className="text-danger">
                            {errors.KhachHang.message}
                          </span>
                        )}
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="Account">Account</label>
                        <input
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="Account"
                          value={data.accountName}
                          readOnly
                        />
                      </div>
                    </div>
                  </>
                )}

                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="CungDuong">Điểm Đóng Hàng(*)</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="CungDuong"
                      readOnly
                      value={roadDetail.diemLayHang}
                    />
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="CungDuong">Điểm Hạ Hàng(*)</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="CungDuong"
                      readOnly
                      value={roadDetail.diemTraHang}
                    />
                  </div>
                </div>
              </div>

              <div className="row">
                {transportType && transportType === "xuat" && (
                  <>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="HangTau">Hãng Tàu</label>
                        <input
                          disabled={true}
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="HangTau"
                          {...register(`HangTau`)}
                        />
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="TenTau">Tên Tàu</label>
                        <input
                          disabled={true}
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="TenTau"
                          {...register(`TenTau`)}
                        />
                      </div>
                    </div>
                  </>
                )}
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TongKhoiLuong">Tổng Khối Lượng</label>

                    <input
                      {...register(`TongKhoiLuong`)}
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TongKhoiLuong"
                      readOnly
                    />
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TongTheTich">Tổng Thể Tích </label>
                    <input
                      {...register(`TongTheTich`)}
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TongTheTich"
                      readOnly
                    />
                  </div>
                </div>
                {watch(`PTVanChuyen`) &&
                  watch(`PTVanChuyen`).includes("CONT") && (
                    <>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="TGLayTraRong">
                            Thời Gian Lấy/Trả Rỗng(*)
                          </label>
                          <div className="input-group ">
                            <Controller
                              control={control}
                              name={`TGLayTraRong`}
                              render={({ field }) => (
                                <DatePicker
                                  disabled={true}
                                  className="form-control"
                                  showTimeSelect
                                  timeFormat="HH:mm"
                                  dateFormat="dd/MM/yyyy HH:mm"
                                  onChange={(date) => field.onChange(date)}
                                  selected={field.value}
                                />
                              )}
                            />
                          </div>
                        </div>
                      </div>
                      {transportType && transportType === "xuat" ? (
                        <>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="TGHaCang">
                                Thời Gian Hạ Cảng(*)
                              </label>
                              <div className="input-group ">
                                <Controller
                                  control={control}
                                  name={`TGHaCang`}
                                  render={({ field }) => (
                                    <DatePicker
                                      disabled={true}
                                      className="form-control"
                                      showTimeSelect
                                      timeFormat="HH:mm"
                                      dateFormat="dd/MM/yyyy HH:mm"
                                      onChange={(date) => field.onChange(date)}
                                      selected={field.value}
                                    />
                                  )}
                                />
                              </div>
                            </div>
                          </div>
                        </>
                      ) : (
                        <>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="TGCoMat">
                                Thời Gian Có Mặt(*)
                              </label>
                              <div className="input-group ">
                                <Controller
                                  control={control}
                                  name={`TGCoMat`}
                                  render={({ field }) => (
                                    <DatePicker
                                      disabled={true}
                                      className="form-control"
                                      showTimeSelect
                                      timeFormat="HH:mm"
                                      dateFormat="dd/MM/yyyy HH:mm"
                                      onChange={(date) => field.onChange(date)}
                                      selected={field.value}
                                    />
                                  )}
                                />
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
                                      disabled={true}
                                      className="form-control"
                                      showTimeSelect
                                      timeFormat="HH:mm"
                                      dateFormat="dd/MM/yyyy HH:mm"
                                      onChange={(date) => field.onChange(date)}
                                      selected={field.value}
                                    />
                                  )}
                                />
                              </div>
                            </div>
                          </div>
                        </>
                      )}
                    </>
                  )}

                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TGLayHang">Thời Gian Lấy Hàng(*)</label>
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name={`TGLayHang`}
                        render={({ field }) => (
                          <DatePicker
                            disabled={true}
                            className="form-control"
                            showTimeSelect
                            timeFormat="HH:mm"
                            dateFormat="dd/MM/yyyy HH:mm"
                            onChange={(date) => field.onChange(date)}
                            selected={field.value}
                          />
                        )}
                      />
                    </div>
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TGTraHang">Thời Gian Trả Hàng(*)</label>
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name={`TGTraHang`}
                        render={({ field }) => (
                          <DatePicker
                            disabled={true}
                            className="form-control"
                            showTimeSelect
                            timeFormat="HH:mm"
                            dateFormat="dd/MM/yyyy HH:mm"
                            onChange={(date) => field.onChange(date)}
                            selected={field.value}
                          />
                        )}
                      />
                    </div>
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-12">
                  <div className="form-group">
                    <label htmlFor="GhiChuVanDon">Ghi Chú Vận Đơn</label>
                    <div className="input-group ">
                      <textarea
                        readOnly={true}
                        className="form-control"
                        rows={3}
                        {...register(`GhiChuVanDon`)}
                      ></textarea>
                    </div>
                  </div>
                </div>
              </div>

              <div className="row">
                {watch(`PTVanChuyen`) &&
                  watch(`PTVanChuyen`).includes("CONT") && (
                    <>
                      <div className="col col-sm">
                        <div className="form-group">
                          {transportType && transportType === "xuat" && (
                            <>
                              <label htmlFor="DiemLayRong">
                                Điểm Lấy Rỗng(*)
                              </label>
                              <Controller
                                name={`DiemLayRong`}
                                control={control}
                                render={({ field }) => (
                                  <Select
                                    {...field}
                                    isDisabled={
                                      accountType && accountType === "NV"
                                        ? false
                                        : true
                                    }
                                    classNamePrefix={"form-control"}
                                    value={field.value}
                                    options={listPoint}
                                  />
                                )}
                                rules={Validate.diemLayRong}
                              />
                              {errors.DiemLayRong && (
                                <span className="text-danger">
                                  {errors.DiemLayRong.message}
                                </span>
                              )}
                            </>
                          )}
                          {transportType && transportType === "nhap" && (
                            <>
                              <label htmlFor="DiemTraRong">
                                Điểm Trả Rỗng(*)
                              </label>
                              <Controller
                                name={`DiemTraRong`}
                                control={control}
                                render={({ field }) => (
                                  <Select
                                    {...field}
                                    isDisabled={
                                      accountType && accountType === "NV"
                                        ? false
                                        : true
                                    }
                                    classNamePrefix={"form-control"}
                                    value={field.value}
                                    options={listPoint}
                                  />
                                )}
                                rules={Validate.diemLayRong}
                              />
                              {errors.DiemTraRong && (
                                <span className="text-danger">
                                  {errors.DiemTraRong.message}
                                </span>
                              )}
                            </>
                          )}
                        </div>
                      </div>
                    </>
                  )}
                {accountType && accountType === "NV" && (
                  <>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="NhaCungCap">Đơn Vị Vận Tải(*)</label>
                        <Controller
                          name={`NhaCungCap`}
                          control={control}
                          render={({ field }) => (
                            <Select
                              {...field}
                              classNamePrefix={"form-control"}
                              value={field.value}
                              options={listSupplier}
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
                  </>
                )}
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="PTVanChuyen">
                      Phương tiện vận chuyển(*)
                    </label>
                    <select
                      className="form-control"
                      {...register(`PTVanChuyen`, Validate.PTVanChuyen)}
                      value={watch(`PTVanChuyen`)}
                      disabled={
                        accountType && accountType === "NV" ? false : true
                      }
                      onChange={(e) => handleOnChangeWeight(e.target.value)}
                    >
                      <option value="">Chọn phương Tiện Vận Chuyển</option>
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
                    {errors.PTVanChuyen && (
                      <span className="text-danger">
                        {errors.PTVanChuyen.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="LoaiHangHoa">Loại Hàng Hóa(*)</label>
                    <select
                      className="form-control"
                      {...register(`LoaiHangHoa`, Validate.LoaiHangHoa)}
                      value={watch(`LoaiHangHoa`)}
                      disabled={
                        accountType && accountType === "NV" ? false : true
                      }
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
                    {errors.LoaiHangHoa && (
                      <span className="text-danger">
                        {errors.LoaiHangHoa.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="KhoiLuong">Khối Lượng(KG)</label>

                    <input
                      readOnly={true}
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="KhoiLuong"
                      {...register(`KhoiLuong`, Validate.KhoiLuong)}
                    />
                    {errors.KhoiLuong && (
                      <span className="text-danger">
                        {errors.KhoiLuong.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TheTich">Số Khối</label>

                    <input
                      readOnly={true}
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TheTich"
                      {...register(`TheTich`, Validate.TheTich)}
                    />
                    {errors.TheTich && (
                      <span className="text-danger">
                        {errors.TheTich.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="SoKien">Số Kiện</label>
                    <input
                      readOnly={true}
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="SoKien"
                      {...register(`SoKien`, Validate.SoKien)}
                    />
                    {errors.SoKien && (
                      <span className="text-danger">
                        {errors.SoKien.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  {watch("KhoiLuong") && wgtVehicle && (
                    <>
                      {watch("KhoiLuong") > wgtVehicle && (
                        <>
                          {watch("KhoiLuong") + "/" + wgtVehicle}
                          <p style={{ color: "red" }}>
                            (Vượt quá tổng trọng lượng của Xe )
                          </p>
                        </>
                      )}
                    </>
                  )}
                </div>
                <div className="col col-sm">
                  {watch("TheTich") && cbmVehicle && (
                    <>
                      {watch("TheTich") > cbmVehicle && (
                        <>
                          {watch("TheTich") + "/" + cbmVehicle}
                          <p style={{ color: "red" }}>
                            (Vượt quá tổng thể tích của Xe)
                          </p>
                        </>
                      )}
                    </>
                  )}
                </div>
                <div className="col col-sm"></div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="XeVanChuyen">
                      Xe Vận Chuyển (
                      <input
                        type="checkbox"
                        onChange={(e) => handleOnCheckVehicle(e.target.checked)}
                      />
                      Nhập thủ công)
                    </label>

                    {!isCheckVehicle || isCheckVehicle === false ? (
                      <>
                        <Controller
                          name={`XeVanChuyen`}
                          control={control}
                          render={({ field }) => (
                            <Select
                              {...field}
                              classNamePrefix={"form-control"}
                              value={field.value}
                              options={listVehicle}
                            />
                          )}
                          rules={Validate.maSoXe}
                        />
                        {errors.XeVanChuyen && (
                          <span className="text-danger">
                            {errors.XeVanChuyen.message}
                          </span>
                        )}
                      </>
                    ) : (
                      <>
                        <input
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="XeVanChuyenTxt"
                          {...register(`XeVanChuyenTxt`, Validate.maSoXe)}
                        />
                        {errors.XeVanChuyenTxt && (
                          <span className="text-danger">
                            {errors.XeVanChuyenTxt.message}
                          </span>
                        )}
                      </>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TaiXe">Tài Xế</label>
                    <Controller
                      name={`TaiXe`}
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listDriver}
                        />
                      )}
                      rules={Validate.maTaiXe}
                    />
                    {errors.TaiXe && (
                      <span className="text-danger">
                        {errors.TaiXe.message}
                      </span>
                    )}
                  </div>
                </div>
                {watch(`PTVanChuyen`) &&
                  watch(`PTVanChuyen`).includes("CONT") && (
                    <>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="Romooc">Số Romooc</label>
                          <Controller
                            name={`Romooc`}
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={listRomooc}
                              />
                            )}
                          />
                          {errors.Romooc && (
                            <span className="text-danger">
                              {errors.Romooc.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </>
                  )}
              </div>
              <div className="row">
                {watch(`PTVanChuyen`) &&
                watch(`PTVanChuyen`).includes("CONT") ? (
                  <>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="CONTNO">CONT NO</label>
                        <input
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="CONTNO"
                          {...register(`CONTNO`, Validate.CONTNO)}
                        />
                        {errors.CONTNO && (
                          <span className="text-danger">
                            {errors.CONTNO.message}
                          </span>
                        )}
                      </div>
                    </div>

                    {accountType &&
                      accountType === "NV" &&
                      transportType &&
                      transportType === "xuat" && (
                        <div className="col col-sm">
                          <div className="form-group">
                            <label htmlFor="ReuseCont">Reuse CONT</label>
                            <select
                              className="form-control"
                              {...register(`ReuseCont`)}
                            >
                              <option value={true}>Có</option>
                              <option value={false}>Không</option>
                            </select>
                          </div>
                        </div>
                      )}

                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="SEALNP">SEAL NP/Hãng Tàu</label>
                        <input
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="SEALNP"
                          {...register(`SEALNP`, Validate.SEALNP)}
                        />
                        {errors.SEALNP && (
                          <span className="text-danger">
                            {errors.SEALNP.message}
                          </span>
                        )}
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="SEALHQ">SEAL HQ</label>
                        <input
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="SEALHQ"
                          {...register(`SEALHQ`, Validate.SEALHQ)}
                        />
                        {errors.SEALHQ && (
                          <span className="text-danger">
                            {errors.SEALHQ.message}
                          </span>
                        )}
                      </div>
                    </div>
                  </>
                ) : (
                  <>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="SEALNP">SEAL NP</label>
                        <input
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="SEALNP"
                          {...register(`SEALNP`, Validate.SEALNP)}
                        />
                        {errors.SEALNP && (
                          <span className="text-danger">
                            {errors.SEALNP.message}
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
                    <label htmlFor="TGLayHangThucTe">
                      Thời Gian Lấy Hàng Thực Tế
                    </label>
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name={`TGLayHangThucTe`}
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
                      />
                      {errors.TGLayHangThucTe && (
                        <span className="text-danger">
                          {errors.TGLayHangThucTe.message}
                        </span>
                      )}
                    </div>
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TGTraHangThucTe">
                      Thời Gian Trả Hàng Thực Tế
                    </label>
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name={`TGTraHangThucTe`}
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
                      />
                      {errors.TGTraHangThucTe && (
                        <span className="text-danger">
                          {errors.TGTraHangThucTe.message}
                        </span>
                      )}
                    </div>
                  </div>
                </div>
              </div>
              {watch(`PTVanChuyen`) &&
                watch(`PTVanChuyen`).includes("CONT") && (
                  <>
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          {transportType && transportType === "xuat" && (
                            <>
                              <label htmlFor="TGLayRongThucTe">
                                Thời Gian Lấy Rỗng Thực Tế
                              </label>
                              <Controller
                                control={control}
                                name={`TGLayRongThucTe`}
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
                              />
                              {errors.TGLayRongThucTe && (
                                <span className="text-danger">
                                  {errors.TGLayRongThucTe.message}
                                </span>
                              )}
                            </>
                          )}
                          {transportType && transportType === "nhap" && (
                            <>
                              <label htmlFor="TGTraRongThucTe">
                                Thời Gian Trả Rỗng Thực Tế
                              </label>
                              <Controller
                                control={control}
                                name={`TGTraRongThucTe`}
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
                              />
                              {errors.TGTraRongThucTe && (
                                <span className="text-danger">
                                  {errors.TGTraRongThucTe.message}
                                </span>
                              )}
                            </>
                          )}
                        </div>
                      </div>
                      {transportType && transportType === "xuat" ? (
                        // <div className="col col-sm">
                        //   <div className="form-group">
                        //     <label htmlFor="TGHaCangThucTe">
                        //       Thời Gian Hạ Cảng Thực Tế(*)
                        //     </label>
                        //     <div className="input-group ">
                        //       <Controller
                        //         control={control}
                        //         name={`TGHaCangThucTe`}
                        //         render={({ field }) => (
                        //           <DatePicker
                        //             className="form-control"
                        //             showTimeSelect
                        //             timeFormat="HH:mm"
                        //             dateFormat="dd/MM/yyyy HH:mm"
                        //             onChange={(date) => field.onChange(date)}
                        //             selected={field.value}
                        //           />
                        //         )}
                        //       />
                        //       {errors.TGHaCangThucTe && (
                        //         <span className="text-danger">
                        //           {errors.TGHaCangThucTe.message}
                        //         </span>
                        //       )}
                        //     </div>
                        //   </div>
                        // </div>
                        <></>
                      ) : (
                        <>
                          <div className="col col-sm">
                            <div className="form-group">
                              <label htmlFor="TGCoMatThucTe">
                                Thời Gian Có Mặt Thực Tế
                              </label>
                              <div className="input-group ">
                                <Controller
                                  control={control}
                                  name={`TGCoMatThucTe`}
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
                                />
                                {errors.TGCoMatThucTe && (
                                  <span className="text-danger">
                                    {errors.TGCoMatThucTe.message}
                                  </span>
                                )}
                              </div>
                            </div>
                          </div>
                        </>
                      )}
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
                        {...register(`GhiChu`)}
                      ></textarea>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div className="card-footer">
              <div className="row">
                <div className="col col-6">
                  <>
                    <button
                      onClick={() =>
                        handleShowModal(
                          SetShowModal("addSubFee"),
                          setTitle("Thêm Mới Phụ Phí Phát Sinh")
                        )
                      }
                      type="button"
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Phụ Phí Phát Sinh"
                    >
                      <i className="fas fa-file-invoice-dollar"></i>
                    </button>
                  </>
                  <>
                    <button
                      onClick={() =>
                        handleShowModal(
                          SetShowModal("Image"),
                          setTitle("Quản Lý Chứng Từ Chuyến")
                        )
                      }
                      type="button"
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Xem Hình Ảnh"
                    >
                      <i className="fas fa-image"></i>
                    </button>
                    <>
                      {data.maPTVC === "FCL" || data.maPTVC === "FTL" ? (
                        <>
                          <button
                            onClick={() =>
                              handleShowModal(
                                SetShowModal("EditTransport"),
                                setTitle("Tách Chuyến ")
                              )
                            }
                            type="button"
                            className="btn btn-title btn-sm btn-default mx-1"
                            gloss="Tách Chuyến"
                          >
                            <i className="fas fa-sliders-h"></i>
                          </button>
                        </>
                      ) : (
                        <>
                          <button
                            disabled={true}
                            type="button"
                            className="btn btn-sm btn-default mx-1"
                          >
                            <i className="fas fa-sliders-h"></i>
                          </button>
                        </>
                      )}
                    </>
                    <>
                      <button
                        onClick={() =>
                          handleShowModal(
                            SetShowModal("ChangeSecondPlace"),
                            setTitle("Đổi Điểm Hạ Hàng")
                          )
                        }
                        type="button"
                        className="btn btn-title btn-sm btn-default mx-1"
                        gloss="Đổi Điểm Hạ Hàng"
                      >
                        <i className="fas fa-exchange-alt"></i>
                      </button>
                    </>
                  </>
                </div>
                <div className="col col-6">
                  <button
                    type="submit"
                    className="btn btn-primary"
                    style={{ float: "right" }}
                  >
                    Xác Nhận
                  </button>
                </div>
              </div>
            </div>
          </form>
        )}
      </div>
      <div
        className="modal fade"
        id="modal-xl"
        data-backdrop="static"
        ref={parseExceptionModal}
        aria-labelledby="parseExceptionModal"
        backdrop="static"
      >
        <div
          className="modal-dialog modal-dialog-scrollable"
          style={{ maxWidth: "90%" }}
        >
          <div className="modal-content">
            <div className="modal-header">
              <h5>{title}</h5>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                onClick={() => modal.hide()}
                aria-label="Close"
              >
                <span aria-hidden="true">×</span>
              </button>
            </div>
            <div className="modal-body">
              <>
                {ShowModal === "Image" && (
                  <HandlingImage
                    dataClick={selectIdClick}
                    hideModal={modal.hide()}
                    checkModal={modal}
                  />
                )}
                {ShowModal === "addSubFee" && (
                  <AddSubFeeByHandling dataClick={selectIdClick} />
                )}
                {ShowModal === "EditTransport" && (
                  <UpdateTransport
                    getListTransport={getlistData}
                    selectIdClick={selectIdClick}
                    hideModal={modal.hide()}
                  />
                )}
                {ShowModal === "ChangeSecondPlace" && (
                  <>
                    {IsLoading && IsLoading === true ? (
                      <div>
                        <LoadingPage></LoadingPage>
                      </div>
                    ) : (
                      <>
                        <div className="row">
                          <div className="col col-3">
                            <div className="form-group">
                              <Controller
                                name="listPlace"
                                control={control}
                                render={({ field }) => (
                                  <Select
                                    {...field}
                                    className="basic-multi-select"
                                    classNamePrefix={"form-control"}
                                    value={field.value}
                                    options={listPlace}
                                    placeholder="Địa Điểm"
                                  />
                                )}
                              />
                            </div>
                          </div>
                          <div className="col col-3">
                            <button
                              onClick={() => handleOnClickChangeSplace()}
                              type="submit"
                              className="btn btn-primary"
                              style={{ float: "left" }}
                            >
                              Xác Nhận
                            </button>
                          </div>
                        </div>
                        <div style={{ height: "25vh" }}></div>
                      </>
                    )}
                  </>
                )}
              </>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default UpdateHandling;
