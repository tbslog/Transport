import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import moment from "moment/moment";

const CreateHandling = (props) => {
  const { selectIdClick, reOpenModal, hideModal, getListTransport } = props;

  const [IsLoading, SetIsLoading] = useState(false);
  const {
    register,
    clearErrors,
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
          DonViVanTai: null,
          HangTau: null,
          TenTau: null,
          XeVanChuyen: null,
          TaiXe: null,
          Romooc: null,
          PTVanChuyen: "",
          LoaiHangHoa: "",
          MaBangGia: null,
          CONTNO: "",
          SEALHQ: "",
          KhoiLuong: null,
          TheTich: null,
          DiemLayRong: null,
          TGLayTraRong: null,
          TGTraRong: null,
          TGCoMat: null,
          TGLech: null,
          TGLayHang: null,
          TGTraHang: null,
          TGKeoCont: null,
          TGCatMang: null,
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
      maxLength: {
        value: 11,
        message: "Không được ít hơn 11 ký tự",
      },
      minLength: {
        value: 11,
        message: "Không được ít hơn 11 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9 ]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt, phải viết hoa",
      },
    },
    SEALHQ: {
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9 ]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt, phải viết hoa",
      },
    },
    SEALNP: {
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

  const [listPoint, setListPoint] = useState([]);
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

      const getListPoint = await getData("address/GetListAddressSelect");
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

      let getListDVT = await getData("Common/GetListDVT");
      let getListVehicleType = await getData("Common/GetListVehicleType");
      let getListGoodsType = await getData("Common/GetListGoodsType");
      let getListTransportType = await getData("Common/GetListTransportType");

      setListDVT(getListDVT);
      setlistVehicleType(getListVehicleType);
      setListGoodsType(getListGoodsType);
      setListPTVC(getListTransportType);

      SetIsLoading(false);
    })();
  }, []);

  useEffect(() => {
    clearErrors();
    if (
      props &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0 &&
      listPoint &&
      listPoint.length > 0
    ) {
      handleOnchangeListRoad(selectIdClick.cungDuong);
      setValue("MaVanDon", selectIdClick.maVanDon);
      setValue("TongKhoiLuong", selectIdClick.tongKhoiLuong);
      setValue("TongTheTich", selectIdClick.tongTheTich);
      setValue(
        "DiemLayHang",
        { ...listPoint.filter((x) => x.value === selectIdClick.diemDau) }[0]
      );
      setValue(
        "DiemTraHang",
        { ...listPoint.filter((x) => x.value === selectIdClick.diemCuoi) }[0]
      );
    }
  }, [props, selectIdClick, listRoad, listPoint]);

  useEffect(() => {
    if (
      selectIdClick &&
      listCustomer &&
      Object.keys(selectIdClick).length > 0 &&
      listCustomer.length > 0
    ) {
      setValue(
        "MaKH",
        { ...listCustomer.filter((x) => x.value === selectIdClick.maKh) }[0]
      );
    }
  }, [selectIdClick, listCustomer]);

  const handleOnchangeListRoad = async (value) => {
    SetIsLoading(true);
    setValue(
      "MaCungDuong",
      { ...listRoad.filter((x) => x.value === value) }[0]
    );

    const getlistData = await getData(
      `BillOfLading/LoadDataHandling?RoadId=${value}`
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
    }
    SetIsLoading(false);
  };

  const handleOnChangeVehicleType = (index, val) => {
    setValue(`optionTransport.${index}.PTVanChuyen`, val);

    if (val.includes("TRUCK")) {
      setValue(`optionTransport.${index}.HangTau`, null);
      setValue(`optionTransport.${index}.TenTau`, null);
      setValue(`optionTransport.${index}.Romooc`, null);
      setValue(`optionTransport.${index}.DiemLayTraRong`, null);
      setValue(`optionTransport.${index}.TGLayTraRong`, null);
      setValue(`optionTransport.${index}.TGKeoCont`, null);
      setValue(`optionTransport.${index}.TGLech`, null);
      setValue(`optionTransport.${index}.TGCatMang`, null);
      setValue(`optionTransport.${index}.SEALHQ`, null);
      setValue(`optionTransport.${index}.CONTNO`, null);
    }
  };

  const handleOnChangeMass = () => {
    // let totalMass = watch("TongKhoiLuong");
    // let listMass = watch("optionTransport");
    // console.log(listMass);
    // const sumMass = listMass.reduce((acc, curr) =>
    //   curr.KhoiLuong.type === "number"
    //     ? acc + Number.parseInt(curr.KhoiLuong, 0)
    //     : 0
    // );
    // totalMass = totalMass - sumMass;
    // setValue("TongKhoiLuong", totalMass);
  };

  const handleOnChangeFilter = () => {
    // let maDVT = watch("DVT");
    // let maPTVC = watch("PTVC");
    // let data = listPriceTable;
    // let tempData = data;
    // let filterByTransportType;
    // let filterByDVT;
    // if (maPTVC !== "") {
    //   filterByTransportType = tempData.filter((x) => x.ptvc === maPTVC);
    // } else {
    //   filterByTransportType = tempData;
    // }
    // tempData = filterByTransportType;
    // if (maDVT !== "") {
    //   filterByDVT = tempData.filter((x) => x.dvt === maDVT);
    // } else {
    //   filterByDVT = tempData;
    // }
    // tempData = filterByDVT;
    // setListPriceTable(tempData);
  };

  const handleOnChangeFilterArr = (index, value, nameVar) => {
    // if (nameVar === "PTVanChuyen") {
    //   setValue(`optionTransport.${index}.PTVanChuyen`, value);
    // }
    // if (nameVar === "LoaiHangHoa") {
    //   setValue(`optionTransport.${index}.LoaiHangHoa`, value);
    // }
    // if (nameVar === "NhaCungCap") {
    //   setValue(
    //     `optionTransport.${index}.NhaCungCap`,
    //     {
    //       ...listNpp.filter((x) => x.value === value),
    //     }[0]
    //   );
    // }
    // if (nameVar === "KhachHang") {
    //   setValue(
    //     `optionTransport.${index}.KhachHang`,
    //     {
    //       ...listCustomer.filter((x) => x.value === value),
    //     }[0]
    //   );
    // }
    // let data = listPriceTable;
    // let tempData = data;
    // let NhaCungCap = watch(`optionTransport.${index}.NhaCungCap`);
    // let PTVanChuyen = watch(`optionTransport.${index}.PTVanChuyen`);
    // let LoaiHangHoa = watch(`optionTransport.${index}.LoaiHangHoa`);
    // let KhachHang = watch(`optionTransport.${index}.KhachHang`);
    // if (nameVar === "NhaCungCap") {
    //   if (NhaCungCap !== null) {
    //     if (NhaCungCap.value === "TBSL") {
    //       tempData = tempData.filter((x) => x.phanLoaiDoiTac === "KH");
    //     } else {
    //       tempData = tempData.filter((x) => x.maDoiTac === NhaCungCap.value);
    //     }
    //   }
    // }
    // if (PTVanChuyen !== "") {
    //   tempData = tempData.filter((x) => x.ptVanChuyen === PTVanChuyen);
    // }
    // if (LoaiHangHoa !== "") {
    //   tempData = tempData.filter((x) => x.loaiHangHoa === LoaiHangHoa);
    // }
    // if (NhaCungCap.value === "TBSL") {
    //   if (KhachHang !== null) {
    //     tempData = tempData.filter((x) => x.maDoiTac === KhachHang.value);
    //   }
    // } else {
    //   tempData = tempData.filter((x) => x.maDoiTac === NhaCungCap.value);
    // }
    // if (tempData && tempData.length === 1) {
    //   setValue(`optionTransport.${index}.GiaThamChieu`, tempData[0].price);
    //   setValue(`optionTransport.${index}.MaBangGia`, tempData[0].maBangGia);
    // } else {
    //   setValue(`optionTransport.${index}.GiaThamChieu`, null);
    //   setValue(`optionTransport.${index}.MaBangGia`, null);
    // }
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);
    let arrHandling = [];

    data.optionTransport.map((val) => {
      arrHandling.push({
        MaSoXe: val.XeVanChuyen.value,
        MaTaiXe: val.TaiXe.value,
        DonViVanTai: val.DonViVanTai.value,
        MaLoaiHangHoa: val.LoaiHangHoa,
        MaDvt: "CHUYEN",
        MaLoaiPhuongTien: val.PTVanChuyen,
        MaPtvc: "Road",
        HangTau: val.HangTau,
        TenTau: val.TenTau,
        MaRomooc: !val.Romooc ? null : val.Romooc.value,
        ContNo: val.CONTNO,
        SealNp: val.SEALNP,
        SealHq: val.SEALHQ,
        KhoiLuong: !val.KhoiLuong ? null : val.KhoiLuong,
        TheTich: !val.TheTich ? null : val.TheTich,
        GhiChu: val.GhiChu,
        DiemLayTraRong: !val.DiemLayTraRong ? null : val.DiemLayTraRong.value,
        ThoiGianHaCong: null,
        ThoiGianLayTraRong: !val.TGLayTraRong
          ? null
          : moment(new Date(val.TGLayTraRong).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
        ThoiGianKeoCong: !val.TGKeoCont
          ? null
          : moment(new Date(val.TGKeoCont).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
        ThoiGianHanLenh: !val.TGLech
          ? null
          : moment(new Date(val.TGLech).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
        ThoiGianCoMat: moment(new Date(val.TGCoMat).toISOString()).format(
          "yyyy-MM-DDTHH:mm:ss.SSS"
        ),
        ThoiGianCatMang: !val.TGCatMang
          ? null
          : moment(new Date(val.TGCatMang).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
        ThoiGianLayHang: moment(new Date(val.TGLayHang).toISOString()).format(
          "yyyy-MM-DDTHH:mm:ss.SSS"
        ),
        ThoiGianTraHang: moment(new Date(val.TGTraHang).toISOString()).format(
          "yyyy-MM-DDTHH:mm:ss.SSS"
        ),
      });
    });

    var dataCreate = {
      maVanDon: data.MaVanDon,
      maCungDuong: data.MaCungDuong.value,
      dieuPhoi: arrHandling,
    };

    var create = await postData(`BillOfLading/CreateHanling`, dataCreate);

    if (create === 1) {
      getListTransport(1);
      hideModal();
    }
    SetIsLoading(false);
  };

  const handleResetClick = () => {
    hideModal();
    reOpenModal(selectIdClick);
  };

  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">
            Form Điều Phối Vận Đơn
            {selectIdClick.loaiVanDon === "nhap" ? " Nhập" : " Xuất"}
          </h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaKH">Khách Hàng</label>
                    <Controller
                      name={`MaKH`}
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
                    {errors.MaKH && (
                      <span className="text-danger">{errors.MaKH.message}</span>
                    )}
                  </div>
                </div>

                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaVanDon">Mã Vận Đơn</label>
                    <input
                      readOnly
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="MaVanDon"
                      {...register(`MaVanDon`)}
                    />
                  </div>
                </div>

                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TongKhoiLuong">Tổng Khối Lượng</label>
                    <input
                      readOnly
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TongKhoiLuong"
                      {...register(`TongKhoiLuong`)}
                    />
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TongTheTich">Tổng Thể Tích</label>
                    <input
                      readOnly
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TongTheTich"
                      {...register(`TongTheTich`)}
                    />
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaCungDuong">Cung Đường</label>
                    <Controller
                      name="MaCungDuong"
                      control={control}
                      render={({ field }) => (
                        <Select
                          isDisabled
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listRoad}
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
                    <label htmlFor="DiemLayHang">Điểm Lấy Hàng</label>
                    <Controller
                      name="DiemLayHang"
                      control={control}
                      render={({ field }) => (
                        <Select
                          isDisabled
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listPoint}
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
                    <label htmlFor="DiemTraHang">Điểm Trả Hàng</label>
                    <Controller
                      name="DiemTraHang"
                      control={control}
                      render={({ field }) => (
                        <Select
                          isDisabled
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
              </div>
              <div className="row" hidden={true}>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="PTVC">Phương thức vận chuyển</label>
                    <select
                      className="form-control"
                      {...register("PTVC", Validate.PTVC)}
                      // onChange={(e) =>
                      //   handleOnChangeFilter(setValue("PTVC", e.target.value))
                      // }
                      value={watch("PTVC")}
                    >
                      <option value="">Chọn phương thức vận chuyển</option>
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
                      <span className="text-danger">{errors.PTVC.message}</span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="DVT">Đơn vị tính</label>
                    <select
                      className="form-control"
                      {...register("DVT", Validate.DVT)}
                      // onChange={(e) =>
                      //   handleOnChangeFilter(setValue("DVT", e.target.value))
                      // }
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
                      <span className="text-danger">{errors.DVT.message}</span>
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
                                <label htmlFor="DonViVanTai">
                                  Đơn Vị Vận Tải
                                </label>
                                <Controller
                                  name={`optionTransport.${index}.DonViVanTai`}
                                  control={control}
                                  render={({ field }) => (
                                    <Select
                                      {...field}
                                      classNamePrefix={"form-control"}
                                      value={field.value}
                                      options={listNpp}
                                    />
                                  )}
                                  rules={{
                                    required: "không được để trống",
                                  }}
                                />
                                {errors.optionTransport?.[index]
                                  ?.DonViVanTai && (
                                  <span className="text-danger">
                                    {
                                      errors.optionTransport?.[index]
                                        ?.DonViVanTai.message
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
                                  value={watch(
                                    `optionTransport.${index}.PTVanChuyen`
                                  )}
                                  onChange={(e) =>
                                    handleOnChangeVehicleType(
                                      index,
                                      e.target.value
                                    )
                                  }
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
                                  value={watch(
                                    `optionTransport.${index}.LoaiHangHoa`
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
                                {errors.optionTransport?.[index]?.SEALNP && (
                                  <span className="text-danger">
                                    {
                                      errors.optionTransport?.[index]?.SEALNP
                                        .message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                            {watch(`optionTransport.${index}.PTVanChuyen`) &&
                              watch(
                                `optionTransport.${index}.PTVanChuyen`
                              ).includes("CONT") && (
                                <>
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
                                  {...register(
                                    `optionTransport.${index}.KhoiLuong`,
                                    Validate.KhoiLuong
                                  )}
                                  onChange={() => handleOnChangeMass()}
                                />
                                {errors.optionTransport?.[index]?.KhoiLuong && (
                                  <span className="text-danger">
                                    {
                                      errors.optionTransport?.[index]?.KhoiLuong
                                        .message
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
                                {errors.optionTransport?.[index]?.TheTich && (
                                  <span className="text-danger">
                                    {
                                      errors.optionTransport?.[index]?.TheTich
                                        .message
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
                                {errors.optionTransport?.[index]?.TaiXe && (
                                  <span className="text-danger">
                                    {
                                      errors.optionTransport?.[index]?.TaiXe
                                        .message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                            {watch(`optionTransport.${index}.PTVanChuyen`) &&
                              watch(
                                `optionTransport.${index}.PTVanChuyen`
                              ).includes("CONT") && (
                                <>
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      <label htmlFor="Romooc">Số Romooc</label>
                                      <Controller
                                        name={`optionTransport.${index}.Romooc`}
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
                                      {errors.optionTransport?.[index]
                                        ?.Romooc && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.Romooc.message
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
                                  Thời Gian Có Mặt
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
                                  {errors.optionTransport?.[index]?.TGCoMat && (
                                    <span className="text-danger">
                                      {
                                        errors.optionTransport?.[index]?.TGCoMat
                                          .message
                                      }
                                    </span>
                                  )}
                                </div>
                              </div>
                            </div>
                            <div className="col col-sm">
                              <div className="form-group">
                                <label htmlFor="TGLayHang">
                                  Thời Gian Lấy Hàng
                                </label>
                                <div className="input-group ">
                                  <Controller
                                    control={control}
                                    name={`optionTransport.${index}.TGLayHang`}
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
                                    ?.TGLayHang && (
                                    <span className="text-danger">
                                      {
                                        errors.optionTransport?.[index]
                                          ?.TGLayHang.message
                                      }
                                    </span>
                                  )}
                                </div>
                              </div>
                            </div>
                            <div className="col col-sm">
                              <div className="form-group">
                                <label htmlFor="TGTraHang">
                                  Thời Gian Trả Hàng
                                </label>
                                <div className="input-group ">
                                  <Controller
                                    control={control}
                                    name={`optionTransport.${index}.TGTraHang`}
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
                                    ?.TGTraHang && (
                                    <span className="text-danger">
                                      {
                                        errors.optionTransport?.[index]
                                          ?.TGTraHang.message
                                      }
                                    </span>
                                  )}
                                </div>
                              </div>
                            </div>
                          </div>
                          {watch(`optionTransport.${index}.PTVanChuyen`) &&
                            watch(
                              `optionTransport.${index}.PTVanChuyen`
                            ).includes("CONT") && (
                              <>
                                <div className="row">
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      {selectIdClick &&
                                        selectIdClick.loaiVanDon &&
                                        selectIdClick.loaiVanDon === "xuat" && (
                                          <label htmlFor="DiemLayTraRong">
                                            Điểm Lấy Rỗng
                                          </label>
                                        )}
                                      {selectIdClick &&
                                        selectIdClick.loaiVanDon &&
                                        selectIdClick.loaiVanDon === "nhap" && (
                                          <label htmlFor="DiemLayTraRong">
                                            Điểm Trả Rỗng
                                          </label>
                                        )}
                                      <Controller
                                        name={`optionTransport.${index}.DiemLayTraRong`}
                                        control={control}
                                        render={({ field }) => (
                                          <Select
                                            {...field}
                                            classNamePrefix={"form-control"}
                                            value={field.value}
                                            options={listPoint}
                                          />
                                        )}
                                        rules={{
                                          required: "không được để trống",
                                        }}
                                      />
                                      {errors.optionTransport?.[index]
                                        ?.DiemLayTraRong && (
                                        <span className="text-danger">
                                          {
                                            errors.optionTransport?.[index]
                                              ?.DiemLayTraRong.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
                                  <div className="col col-sm">
                                    <div className="form-group">
                                      {selectIdClick &&
                                        selectIdClick.loaiVanDon &&
                                        selectIdClick.loaiVanDon === "xuat" && (
                                          <label htmlFor="TGLayTraRong">
                                            Thời Gian Lấy Rỗng
                                          </label>
                                        )}
                                      {selectIdClick &&
                                        selectIdClick.loaiVanDon &&
                                        selectIdClick.loaiVanDon === "nhap" && (
                                          <label htmlFor="TGLayTraRong">
                                            Thời Gian Trả Rỗng
                                          </label>
                                        )}
                                      <div className="input-group ">
                                        <Controller
                                          control={control}
                                          name={`optionTransport.${index}.TGLayTraRong`}
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
                                          ?.TGLayTraRong && (
                                          <span className="text-danger">
                                            {
                                              errors.optionTransport?.[index]
                                                ?.TGLayTraRong.message
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
                                {selectIdClick &&
                                  selectIdClick.loaiVanDon &&
                                  selectIdClick.loaiVanDon === "xuat" && (
                                    <div className="row">
                                      <div className="col col-sm">
                                        <div className="form-group">
                                          <label htmlFor="HangTau">
                                            Hãng Tàu
                                          </label>
                                          <input
                                            autoComplete="false"
                                            type="text"
                                            className="form-control"
                                            id="HangTau"
                                            {...register(
                                              `optionTransport.${index}.HangTau`,
                                              Validate.HangTau
                                            )}
                                          />
                                          {errors.optionTransport?.[index]
                                            ?.HangTau && (
                                            <span className="text-danger">
                                              {
                                                errors.optionTransport?.[index]
                                                  ?.HangTau.message
                                              }
                                            </span>
                                          )}
                                        </div>
                                      </div>
                                      <div className="col col-sm">
                                        <div className="form-group">
                                          <label htmlFor="TenTau">
                                            Tên Tàu
                                          </label>
                                          <input
                                            autoComplete="false"
                                            type="text"
                                            className="form-control"
                                            id="TenTau"
                                            {...register(
                                              `optionTransport.${index}.TenTau`,
                                              Validate.TenTau
                                            )}
                                          />
                                          {errors.optionTransport?.[index]
                                            ?.TenTau && (
                                            <span className="text-danger">
                                              {
                                                errors.optionTransport?.[index]
                                                  ?.TenTau.message
                                              }
                                            </span>
                                          )}
                                        </div>
                                      </div>
                                      <div className="col col-sm">
                                        <div className="form-group">
                                          <label htmlFor="TGCatMang">
                                            Thời Gian Cắt Máng
                                          </label>
                                          <div className="input-group ">
                                            <Controller
                                              control={control}
                                              name={`optionTransport.${index}.TGCatMang`}
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
                                              ?.TGCatMang && (
                                              <span className="text-danger">
                                                {
                                                  errors.optionTransport?.[
                                                    index
                                                  ]?.TGCatMang.message
                                                }
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
    </>
  );
};

export default CreateHandling;
