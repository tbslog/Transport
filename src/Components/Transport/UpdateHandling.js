import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import moment from "moment";

const UpdateHandling = (props) => {
  const { getlistData, selectIdClick, hideModal } = props;
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
      PTVanChuyen: null,
      XeVanChuyen: null,
      TaiXe: null,
      TenTau: null,
      HangTau: null,
      Romooc: null,
      CONTNO: null,
      SEALNP: null,
      SEALHQ: null,
      KhoiLuong: null,
      TheTich: null,
      TGLayRong: null,
      TGKeoCont: null,
      TGLech: null,
      TGCatMang: null,
      TGTraRong: null,
      TGLayHang: null,
      TGTraHang: null,
      TGCoMat: null,
      GhiChu: null,
    },
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
    MaDonHang: { required: "Không được để trống" },
    CONTNO: {
      required: "Không được để trống",
      maxLength: {
        value: 11,
        message: "Không được ít hơn 11 ký tự",
      },
      minLength: {
        value: 11,
        message: "Không được nhiều hơn 11 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9 ]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt, phải viết hoa",
      },
    },
    SEALHQ: {
      required: "Không được để trống",
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9 ]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt, phải viết hoa",
      },
    },
    SEALHT: {
      required: "Không được để trống",
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9 ]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt, phải viết hoa",
      },
    },
    SEALNP: {
      required: "Không được để trống",
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9 ]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt, phải viết hoa",
      },
    },
    KhoiLuong: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TheTich: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
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
    TGLayHang: {
      required: "Không được để trống",
    },
    TGTraHang: {
      required: "Không được để trống",
    },
    TGCatMang: {
      required: "Không được để trống",
    },
    TGKeoCont: {
      required: "Không được để trống",
    },
    GiaThucTe: {
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
      required: "Không được để trống",
    },
    GiaThamChieu: {
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
    HangTau: {
      required: "Không được để trống",
    },
    TenTau: {
      required: "Không được để trống",
    },
    TGCatMang: {
      required: "Không được để trống",
    },
    // PTVC: {
    //   required: "Không được để trống",
    // },
    // DVT: {
    //   required: "Không được để trống",
    // },
  };

  const [IsLoading, SetIsLoading] = useState(false);
  const [listPriceTable, setListPriceTable] = useState([]);
  const [listCustomer, setListCustomer] = useState([]);
  const [listNpp, setListNpp] = useState([]);
  const [listVehicleType, setlistVehicleType] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listDriver, setListDriver] = useState([]);
  const [listVehicle, setListVehicle] = useState([]);
  const [listRomooc, setListRomooc] = useState([]);
  const [transportType, setTransportType] = useState("");
  const [data, setData] = useState({});
  const [roadDetail, setRoadDetail] = useState({});

  useEffect(() => {
    (async () => {
      let getListVehicleType = await getData("Common/GetListVehicleType");
      let getListGoodsType = await getData("Common/GetListGoodsType");

      setlistVehicleType(getListVehicleType);
      setListGoodsType(getListGoodsType);
    })();
  }, []);

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
      SetIsLoading(true);
      handleResetClick();
      (async () => {
        let data = await getData(
          `BillOfLading/GetHandlingById?id=${selectIdClick.maDieuPhoi}`
        );
        setData(data);
        handleOnchangeListRoad(data);

        SetIsLoading(false);
      })();
    }
  }, [props, selectIdClick, listVehicleType, listGoodsType]);

  useEffect(() => {
    if (
      Object.keys(data).length > 0 &&
      data &&
      listCustomer &&
      listDriver &&
      listNpp &&
      listRomooc &&
      listVehicle &&
      listCustomer.length > 0 &&
      listDriver.length > 0 &&
      listNpp.length > 0 &&
      listRomooc.length > 0 &&
      listVehicle.length > 0
    ) {
      setValueData(data);
    }
  }, [data, listCustomer, listDriver, listNpp, listRomooc, listVehicle]);

  const setValueData = (data) => {
    setRoadDetail(data.cungDuong);
    setValue(
      "NhaCungCap",
      {
        ...listNpp.filter((x) => x.value === data.nhaCungCap),
      }[0]
    );

    setValue(
      "KhachHang",
      {
        ...listCustomer.filter((x) => x.value === data.maKh),
      }[0]
    );
    setTransportType(data.phanLoaiVanDon);
    setValue("PTVanChuyen", data.ptVanChuyen);
    setValue("LoaiHangHoa", data.loaiHangHoa);
    setValue("GiaThamChieu", data.giaThamChieu);
    setValue("GiaThucTe", data.giaThucTe);
    setValue("KhoiLuong", data.khoiLuong);
    setValue("TheTich", data.theTich);
    setValue(
      "TaiXe",
      {
        ...listDriver.filter((x) => x.value === data.maTaiXe),
      }[0]
    );
    setValue(
      "XeVanChuyen",
      {
        ...listVehicle.filter((x) => x.value === data.maSoXe),
      }[0]
    );

    setValue("GhiChu", data.ghiChu);

    setValue("TGCoMat", new Date(data.thoiGianCoMat));
    setValue("TGLayHang", new Date(data.thoiGianLayHang));
    setValue("TGTraHang", new Date(data.thoiGianTraHang));
    setValue("SEALNP", data.sealNp);

    if (data.ptVanChuyen.includes("CONT")) {
      setValue("CONTNO", data.contNo);
      setValue("SEALHQ", data.sealHq);
      setValue(
        "Romooc",
        {
          ...listRomooc.filter((x) => x.value === data.maRomooc),
        }[0]
      );

      setValue(
        "TGLayRong",
        data.thoiGianLayRong === null ? null : new Date(data.thoiGianLayRong)
      );
      setValue(
        "TGTraRong",
        data.thoiGianTraRong === null ? null : new Date(data.thoiGianTraRong)
      );
      setValue(
        "TGLech",
        data.thoiGianHanLenh === null ? null : new Date(data.thoiGianHanLenh)
      );
      setValue(
        "TGKeoCont",
        data.thoiGianKeoCong === null ? null : new Date(data.thoiGianKeoCong)
      );
    }

    if (data.phanLoaiVanDon === "xuat") {
      setValue("HangTau", data.hangTau);
      setValue("TenTau", data.tenTau);
      setValue(
        "TGCatMang",
        data.thoiGianCatMang === null ? null : new Date(data.thoiGianCatMang)
      );
    }
  };

  const handleOnchangeListRoad = async (value) => {
    SetIsLoading(true);
    const getlistData = await getData(
      `BillOfLading/LoadDataHandling?RoadId=${value.maCungDuong}`
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
      getlistData.listXeVanChuyen
        .filter((x) => x.maLoaiPhuongTien === value.ptVanChuyen)
        .map((val) => {
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

  const handleOnChangeFilterArr = (value, nameVar) => {
    if (nameVar === "PTVanChuyen") {
      setValue(`PTVanChuyen`, value);
    }
    if (nameVar === "LoaiHangHoa") {
      setValue(`LoaiHangHoa`, value);
    }
    if (nameVar === "NhaCungCap") {
      setValue(
        `NhaCungCap`,
        {
          ...listNpp.filter((x) => x.value === value),
        }[0]
      );
    }
    if (nameVar === "KhachHang") {
      setValue(
        `KhachHang`,
        {
          ...listCustomer.filter((x) => x.value === value),
        }[0]
      );
    }

    let data = listPriceTable;
    let tempData = data;

    let NhaCungCap = watch(`NhaCungCap`);
    let PTVanChuyen = watch(`PTVanChuyen`);
    let LoaiHangHoa = watch(`LoaiHangHoa`);
    let KhachHang = watch(`KhachHang`);

    if (nameVar === "NhaCungCap") {
      if (NhaCungCap !== null) {
        if (NhaCungCap.value === "TBSL") {
          tempData = tempData.filter((x) => x.phanLoaiDoiTac === "KH");
        } else {
          tempData = tempData.filter((x) => x.maDoiTac === NhaCungCap.value);
        }
      }
    }

    if (PTVanChuyen !== "") {
      tempData = tempData.filter((x) => x.ptVanChuyen === PTVanChuyen);
    }

    if (LoaiHangHoa !== "") {
      tempData = tempData.filter((x) => x.loaiHangHoa === LoaiHangHoa);
    }
    if (NhaCungCap.value === "TBSL") {
      if (KhachHang !== null) {
        tempData = tempData.filter((x) => x.maDoiTac === KhachHang.value);
      }
    } else {
      tempData = tempData.filter((x) => x.maDoiTac === NhaCungCap.value);
    }

    if (tempData && tempData.length === 1) {
      setValue(`GiaThamChieu`, tempData[0].price);
      setValue(`MaBangGia`, tempData[0].maBangGia);
    } else {
      setValue(`GiaThamChieu`, null);
      setValue(`MaBangGia`, null);
    }
  };

  const handleResetClick = () => {
    reset();
    setData({});
    setListCustomer([]);
    setListDriver([]);
    setListNpp([]);
    setListRomooc([]);
    setListVehicle([]);
    setValue("NhaCungCap", null);
    setValue("KhachHang", null);
    setValue("TaiXe", null);
    setValue("XeVanChuyen", null);
    setValue("Romooc", null);
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);

    let dataUpdate = {
      ptVanChuyen: data.PTVanChuyen,
      maSoXe: data.XeVanChuyen.value,
      maTaiXe: data.TaiXe.value,
      tenTau: data.TenTau,
      hangTau: data.HangTau,
      maRomooc: data.Romooc === null ? null : data.Romooc.value,
      contNo: data.CONTNO,
      sealNp: data.SEALNP,
      sealHq: data.SEALHQ,
      khoiLuong: data.KhoiLuong === "" ? null : data.KhoiLuong,
      theTich: data.TheTich === "" ? null : data.TheTich,
      thoiGianLayRong:
        data.TGLayRong === null
          ? null
          : moment(new Date(data.TGLayRong).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
      thoiGianKeoCong:
        data.TGKeoCont === null
          ? null
          : moment(new Date(data.TGKeoCont).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
      thoiGianHanLenh:
        data.TGLech === null
          ? null
          : moment(new Date(data.TGLech).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
      thoiGianCatMang:
        data.TGCatMang === null
          ? null
          : moment(new Date(data.TGCatMang).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
      thoiGianTraRong:
        data.TGTraRong === null
          ? null
          : moment(new Date(data.TGTraRong).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
      thoiGianLayHang: moment(new Date(data.TGLayHang).toISOString()).format(
        "yyyy-MM-DDTHH:mm:ss.SSS"
      ),
      thoiGianTraHang: moment(new Date(data.TGTraHang).toISOString()).format(
        "yyyy-MM-DDTHH:mm:ss.SSS"
      ),
      thoiGianCoMat: moment(new Date(data.TGCoMat).toISOString()).format(
        "yyyy-MM-DDTHH:mm:ss.SSS"
      ),
      ghiChu: data.GhiChu,
    };

    let update = await postData(
      `BillOfLading/UpdateHandling?id=${selectIdClick.maDieuPhoi}`,
      dataUpdate
    );

    if (update === 1) {
      getlistData(selectIdClick.maVanDon);
      hideModal();
    }

    SetIsLoading(false);
  };

  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">Form Cập Nhật Điều Phối</h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="CungDuong">Cung Đường</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="CungDuong"
                      readOnly
                      value={
                        roadDetail.maCungDuong + " - " + roadDetail.tenCungDuong
                      }
                    />
                  </div>
                </div>
                {watch(`PTVanChuyen`) && watch(`PTVanChuyen`).includes("CONT") && (
                  <div className="col col-sm">
                    <div className="form-group">
                      <label htmlFor="CungDuong">Điểm Lấy Rỗng</label>
                      <input
                        autoComplete="false"
                        type="text"
                        className="form-control"
                        id="CungDuong"
                        readOnly
                        value={roadDetail.diemLayRong}
                      />
                    </div>
                  </div>
                )}

                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="CungDuong">Điểm Lấy Hàng</label>
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
                    <label htmlFor="CungDuong">Điểm Trả Hàng</label>
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
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="NhaCungCap">Đơn Vị Vận Tải</label>
                    <Controller
                      name={`NhaCungCap`}
                      control={control}
                      render={({ field }) => (
                        <Select
                          isDisabled={true}
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listNpp}
                          onChange={(field) =>
                            handleOnChangeFilterArr(field.value, "NhaCungCap")
                          }
                        />
                      )}
                      rules={{
                        required: "không được để trống",
                      }}
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
                    <label htmlFor="KhachHang">Khách Hàng</label>
                    <Controller
                      name={`KhachHang`}
                      control={control}
                      render={({ field }) => (
                        <Select
                          isDisabled={true}
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listCustomer}
                          onChange={(field) =>
                            handleOnChangeFilterArr(field.value, "KhachHang")
                          }
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
                    <label htmlFor="PTVanChuyen">Phương tiện vận chuyển</label>
                    <select
                      className="form-control"
                      {...register(`PTVanChuyen`, Validate.PTVanChuyen)}
                      onChange={(e) =>
                        handleOnChangeFilterArr(e.target.value, "PTVanChuyen")
                      }
                      value={watch(`PTVanChuyen`)}
                      disabled
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
                    <label htmlFor="LoaiHangHoa">Loại Hàng Hóa</label>
                    <select
                      className="form-control"
                      {...register(`LoaiHangHoa`, Validate.LoaiHangHoa)}
                      onChange={(e) =>
                        handleOnChangeFilterArr(e.target.value, "LoaiHangHoa")
                      }
                      value={watch(`LoaiHangHoa`)}
                      disabled
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
                <div className="col col-sm" hidden={true}>
                  <div className="form-group">
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="MaBangGia"
                      readOnly
                      {...register(`MaBangGia`)}
                    />
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="GiaThamChieu">Giá Tham Chiếu</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="GiaThamChieu"
                      readOnly
                      {...register(`GiaThamChieu`, Validate.GiaThamChieu)}
                    />
                    {errors.GiaThamChieu && (
                      <span className="text-danger">
                        {errors.GiaThamChieu.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="GiaThucTe">Giá Thực Tế</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="GiaThucTe"
                      {...register(`GiaThucTe`, Validate.GiaThucTe)}
                      disabled
                    />
                    {errors.GiaThucTe && (
                      <span className="text-danger">
                        {errors.GiaThucTe.message}
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
                      {...register(`SEALNP`, Validate.SEALNP)}
                    />
                    {errors.SEALNP && (
                      <span className="text-danger">
                        {errors.SEALNP.message}
                      </span>
                    )}
                  </div>
                </div>
                {watch(`PTVanChuyen`) && watch(`PTVanChuyen`).includes("CONT") && (
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
                )}

                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="KhoiLuong">Khối Lượng</label>
                    <input
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
                    <label htmlFor="TheTich">Thể tích</label>
                    <input
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
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="XeVanChuyen">Xe Vận Chuyển</label>
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
                      rules={{
                        required: "không được để trống",
                      }}
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
                      rules={{
                        required: "không được để trống",
                      }}
                    />
                    {errors.TaiXe && (
                      <span className="text-danger">
                        {errors.TaiXe.message}
                      </span>
                    )}
                  </div>
                </div>
                {watch(`PTVanChuyen`) && watch(`PTVanChuyen`).includes("CONT") && (
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
                          rules={{
                            required: "không được để trống",
                          }}
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
                        rules={{
                          required: "không được để trống",
                        }}
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
                        rules={{
                          required: "không được để trống",
                        }}
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
                        rules={{
                          required: "không được để trống",
                        }}
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
              {watch(`PTVanChuyen`) && watch(`PTVanChuyen`).includes("CONT") && (
                <>
                  <div className="row">
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="TGLayRong">Thời gian lấy rỗng</label>
                        <div className="input-group ">
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
                            rules={{
                              required: "không được để trống",
                            }}
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
                            rules={{
                              required: "không được để trống",
                            }}
                          />
                          {errors.TGTraRong && (
                            <span className="text-danger">
                              {errors.TGTraRong.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                  <div className="row">
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="TGLech">Thời gian hạn lệnh</label>
                        <div className="input-group ">
                          <Controller
                            control={control}
                            name={`TGLech`}
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
                          {errors.TGLech && (
                            <span className="text-danger">
                              {errors.TGLech.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="TGKeoCont">Thời Gian Kéo Công</label>
                        <div className="input-group ">
                          <Controller
                            control={control}
                            name={`TGKeoCont`}
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
                          {errors.TGKeoCont && (
                            <span className="text-danger">
                              {errors.TGKeoCont.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                  {transportType && transportType === "xuat" && (
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="HangTau">Hãng Tàu</label>
                          <input
                            autoComplete="false"
                            type="text"
                            className="form-control"
                            id="HangTau"
                            {...register(`HangTau`, Validate.HangTau)}
                          />
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
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="TGCatMang">Thời Gian Cắt Máng</label>
                          <div className="input-group ">
                            <Controller
                              control={control}
                              name={`TGCatMang`}
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
                            {errors.TGCatMang && (
                              <span className="text-danger">
                                {errors.TGCatMang.message}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                    </div>
                  )}
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
  );
};

export default UpdateHandling;
