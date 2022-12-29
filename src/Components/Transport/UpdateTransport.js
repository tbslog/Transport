import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import moment from "moment";
import { ToastError } from "../Common/FuncToast";

const UpdateTransport = (props) => {
  const { getListTransport, selectIdClick, hideModal } = props;

  const {
    register,
    setValue,
    reset,
    resetField,
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
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
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
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    TongTheTich: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    KhoiLuong: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    TheTich: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    SoKien: {
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
  };

  const [IsLoading, SetIsLoading] = useState(false);
  const [listFirstPoint, setListFirstPoint] = useState([]);
  const [listSecondPoint, setListSecondPoint] = useState([]);
  const [listRoad, setListRoad] = useState([]);
  const [listCus, setListCus] = useState([]);
  const [arrRoad, setArrRoad] = useState([]);
  const [listPoint, setListPoint] = useState([]);
  const [listSupplier, setListSupplier] = useState([]);
  const [listVehicleType, setlistVehicleType] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listTransportType, setListTransportType] = useState([]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      const getListCus = await getData(`Customer/GetListCustomerOptionSelect`);
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

      let getListVehicleType = await getData("Common/GetListVehicleType");
      let getListGoodsType = await getData("Common/GetListGoodsType");
      setlistVehicleType(getListVehicleType);
      setListGoodsType(getListGoodsType);

      let getListTransportType = await getData("Common/GetListTransportType");
      setListTransportType(getListTransportType);

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
      SetIsLoading(false);
    })();
  }, []);

  useEffect(() => {
    if (
      selectIdClick &&
      Object.keys(selectIdClick).length > 0 &&
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
      let arrData = [];
      selectIdClick.arrHandlings.map((val) => {
        arrData.push({
          DonViVanTai: {
            ...listSupplier.filter((x) => x.value === val.donViVanTai),
          }[0],
          LoaiHangHoa: val.loaiHangHoa,
          PTVanChuyen: val.ptVanChuyen,
          KhoiLuong: val.khoiLuong,
          TheTich: val.theTich,
          SoKien: val.soKien,
          DiemLayTraRong: !val.diemLayTraRong
            ? null
            : {
                ...listPoint.filter((x) => x.value === val.diemLayTraRong),
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
        setValue(
          `optionHandling.${i}.DiemLayTraRong`,
          arrData[i].DiemLayTraRong
        );
      }
      setValue("optionHandling", arrData);
    } else {
      setValue("optionHandling", []);
    }

    SetIsLoading(false);
  }, [listSupplier, listPoint, listGoodsType, listVehicleType, selectIdClick]);

  useEffect(() => {
    if (
      listCus &&
      listCus.length > 0 &&
      props &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0 &&
      listTransportType &&
      listTransportType.length > 0
    ) {
      SetIsLoading(true);

      setValue("MaVDKH", selectIdClick.maVanDonKH);
      setValue("LoaiVanDon", selectIdClick.loaiVanDon);
      setValue("TongKhoiLuong", selectIdClick.tongKhoiLuong);
      setValue("TongTheTich", selectIdClick.tongTheTich);
      setValue("TongSoKien", selectIdClick.tongSoKien);
      setValue("LoaiHinh", selectIdClick.maPTVC.replaceAll(" ", ""));
      setValue(
        "MaKH",
        { ...listCus.filter((x) => x.value === selectIdClick.maKh) }[0]
      );
      handleOnChangeCustomer(
        { ...listCus.filter((x) => x.value === selectIdClick.maKh) }[0]
      );

      if (selectIdClick.loaiVanDon === "xuat") {
        setValue("HangTau", selectIdClick.hangTau);
        setValue("TenTau", selectIdClick.tenTau);
        setValue(
          "TGHaCang",
          !selectIdClick.thoiGianHaCang
            ? null
            : new Date(selectIdClick.thoiGianHaCang)
        );
      } else {
        setValue(
          "TGCoMat",
          !selectIdClick.thoiGianCoMat
            ? null
            : new Date(selectIdClick.thoiGianCoMat)
        );
        setValue(
          "TGHanLenh",
          !selectIdClick.thoiGianHanLenh
            ? null
            : new Date(selectIdClick.thoiGianHanLenh)
        );
      }
      setValue("GhiChu", selectIdClick.ghiChu);

      setValue(
        "TGTraHang",
        !selectIdClick.thoiGianTraHang
          ? null
          : new Date(selectIdClick.thoiGianTraHang)
      );
      setValue(
        "TGLayHang",
        !selectIdClick.thoiGianLayHang
          ? null
          : new Date(selectIdClick.thoiGianLayHang)
      );
      setValue(
        "TGLayTraRong",
        !selectIdClick.thoiGianLayTraRong
          ? null
          : new Date(selectIdClick.thoiGianLayTraRong)
      );
      SetIsLoading(false);
    }
  }, [listCus, listTransportType, props, selectIdClick]);

  useEffect(() => {
    if (
      arrRoad &&
      arrRoad.length > 0 &&
      listRoad &&
      listRoad.length > 0 &&
      listFirstPoint &&
      listFirstPoint.length > 0 &&
      listSecondPoint &&
      listSecondPoint.length > 0
    ) {
      SetIsLoading(true);

      setValue(
        "MaCungDuong",
        { ...listRoad.filter((x) => x.value === selectIdClick.cungDuong) }[0]
      );

      handleOnChangeRoad(
        { ...listRoad.filter((x) => x.value === selectIdClick.cungDuong) }[0]
      );
    }
    SetIsLoading(false);
  }, [arrRoad, listRoad, listFirstPoint, listSecondPoint]);

  const handleOnChangeCustomer = async (val) => {
    if (val && Object.keys(val).length > 0) {
      setValue("MaKH", val);
      setValue("MaCungDuong", null);
      setValue("DiemLayHang", null);
      setValue("DiemTraHang", null);

      const getListRoad = await getData(
        `BillOfLading/LoadDataRoadTransportByCusId?id=${val.value}`
      );

      if (getListRoad && Object.keys(getListRoad).length > 0) {
        if (getListRoad.cungDuong && getListRoad.cungDuong.length > 0) {
          let arr = [];
          getListRoad.cungDuong.map((val) => {
            arr.push({
              label: val.tenCungDuong + " - " + val.km + " KM",
              value: val.maCungDuong,
            });
          });
          setArrRoad(getListRoad.cungDuong);
          setListRoad(arr);
        } else {
          setListRoad([]);
        }

        if (getListRoad.diemDau && getListRoad.diemDau.length > 0) {
          let arr = [];
          getListRoad.diemDau.map((val) => {
            arr.push({
              label: val.tenDiaDiem,
              value: val.maDiaDiem,
            });
          });
          setListFirstPoint(arr);
        } else {
          setListFirstPoint([]);
        }

        if (getListRoad.diemCuoi && getListRoad.diemCuoi.length > 0) {
          let arr = [];
          getListRoad.diemCuoi.map((val) => {
            arr.push({
              label: val.tenDiaDiem,
              value: val.maDiaDiem,
            });
          });
          setListSecondPoint(arr);
        } else {
          setListSecondPoint([]);
        }
      }
    }
  };

  const handleOnChangeRoad = (val) => {
    if (val && Object.keys(val).length > 0) {
      setValue("MaCungDuong", val);
      const point = arrRoad.filter((x) => x.maCungDuong === val.value)[0];
      setValue(
        "DiemLayHang",
        listFirstPoint.filter((x) => x.value === point.diemDau)[0]
      );
      setValue(
        "DiemTraHang",
        listSecondPoint.filter((x) => x.value === point.diemCuoi)[0]
      );
    } else {
      setValue("MaCungDuong", null);
    }
  };

  const handleOnChangePoint = () => {
    setValue("MaCungDuong", null);
    var diemdau = watch("DiemLayHang");
    var diemCuoi = watch("DiemTraHang");

    if (
      diemdau &&
      diemCuoi &&
      Object.keys(diemdau).length > 0 &&
      Object.keys(diemCuoi).length > 0
    ) {
      const filterRoad = arrRoad.filter(
        (x) => x.diemDau === diemdau.value && x.diemCuoi === diemCuoi.value
      )[0];

      if (filterRoad && Object.keys(filterRoad).length > 0) {
        setValue(
          "MaCungDuong",
          { ...listRoad.filter((x) => x.value === filterRoad.maCungDuong) }[0]
        );
      }
    } else {
      setValue("MaCungDuong", null);
    }
  };

  const handleOnchangeTransportType = (val) => {
    reset();
    setValue("LoaiVanDon", val);
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);

    let arr = [];
    data.optionHandling.map((val) => {
      arr.push({
        DonViVanTai: !val.DonViVanTai ? null : val.DonViVanTai.value,
        DiemLayTraRong: !val.DiemLayTraRong ? null : val.DiemLayTraRong.value,
        LoaiHangHoa: val.LoaiHangHoa,
        PTVanChuyen: val.PTVanChuyen,
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
      `BillOfLading/UpdateTransport?transportId=${selectIdClick.maVanDon}`,
      {
        MaPTVC: data.LoaiHinh,
        arrHandlings: arr,
        HangTau: data.HangTau,
        TenTau: data.TenTau,
        MaVanDonKH: data.MaVDKH,
        MaCungDuong: data.MaCungDuong.value,
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
        ThoiGianLayTraRong: !data.TGLayTraRong
          ? null
          : moment(new Date(data.TGLayTraRong).toISOString()).format(
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
        <div className="card-header">
          <h3 className="card-title">Form Cập Nhật Vận Đơn</h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>

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
                      value={watch("LoaiHinh")}
                    >
                      <option value="">Chọn Loại Hình</option>
                      {listTransportType &&
                        listTransportType.length > 0 &&
                        listTransportType.map((val) => {
                          return (
                            <option value={val.maPtvc} key={val.maPtvc}>
                              {val.tenPtvc}
                            </option>
                          );
                        })}
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
                    <label htmlFor="MaVDKH">Mã Vận Đơn Của Khách Hàng</label>
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
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="HangTau">Hãng Tàu</label>
                        <input
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="TongThungHang"
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
                  </>
                )}
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="DiemLayHang">Điểm Lấy Hàng(*)</label>
                    <Controller
                      name="DiemLayHang"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listFirstPoint}
                          onChange={(field) =>
                            handleOnChangePoint(
                              setValue(
                                "DiemLayHang",
                                {
                                  ...listFirstPoint.filter(
                                    (x) => x.value === field.value
                                  ),
                                }[0]
                              )
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
                    <label htmlFor="DiemTraHang">Điểm Trả Hàng(*)</label>
                    <Controller
                      name="DiemTraHang"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listSecondPoint}
                          onChange={(field) =>
                            handleOnChangePoint(
                              setValue(
                                "DiemTraHang",
                                {
                                  ...listSecondPoint.filter(
                                    (x) => x.value === field.value
                                  ),
                                }[0]
                              )
                            )
                          }
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
                <div className="col col-sm">
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
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TongKhoiLuong">
                      Tổng Khối Lượng (Đơn Vị Tấn)
                    </label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TongKhoiLuong"
                      {...register(`TongKhoiLuong`, Validate.TongKhoiLuong)}
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
                      Tổng Thể Tích (Đơn Vị m3)
                    </label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TongTheTich"
                      {...register(`TongTheTich`, Validate.TongTheTich)}
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
                          <div className="col-sm">Đơn Vị Vận Tải(*)</div>
                          <div className="col-sm-2">Loại Hàng Hóa(*)</div>
                          <div className="col-sm-2">Loại Phương Tiện(*)</div>
                          {watch(`optionHandling`) &&
                            watch(`optionHandling`).length > 0 &&
                            watch(`optionHandling`).filter((x) =>
                              x.PTVanChuyen.includes("CONT")
                            ).length > 0 && (
                              <div className="col-sm-2">Điểm Lấy Rỗng(*)</div>
                            )}
                          <div className="col-sm-1">Khối Lượng</div>
                          <div className="col-sm-1">Thể Tích</div>
                          <div className="col-sm-1">Số Kiện</div>
                        </div>
                      </th>
                      <th style={{ width: "40px" }}>
                        <button
                          className="form-control form-control-sm"
                          type="button"
                          onClick={() => append(watch(`optionHandling`)[0])}
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
                              <div className="col-sm">
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
                                  <div className="col-sm-2">
                                    <div className="form-group">
                                      <Controller
                                        name={`optionHandling.${index}.DiemLayTraRong`}
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
                                          validate: (value) => {
                                            if (!value.value) {
                                              return "không được để trống";
                                            }
                                          },
                                        }}
                                      />
                                      {errors.optionHandling?.[index]
                                        ?.DiemLayTraRong && (
                                        <span className="text-danger">
                                          {
                                            errors.optionHandling?.[index]
                                              ?.DiemLayTraRong.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </div>
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
                                    placeholder="Thể Tích"
                                    id="TheTich"
                                    {...register(
                                      `optionHandling.${index}.TheTich`,
                                      Validate.TheTich
                                    )}
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
                                  onClick={() => remove(index)}
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
                          <label htmlFor="TGLayTraRong">
                            Thời Gian Lấy Rỗng
                          </label>
                        )}
                        {watch("LoaiVanDon") === "nhap" && (
                          <label htmlFor="TGLayTraRong">
                            Thời Gian Trả Rỗng
                          </label>
                        )}
                        <div className="input-group ">
                          <Controller
                            control={control}
                            name={`TGLayTraRong`}
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
                          {errors.TGLayTraRong && (
                            <span className="text-danger">
                              {errors.TGLayTraRong.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                    {watch("LoaiVanDon") === "xuat" && (
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="TGHaCang">Thời Gian Hạ Cảng(*)</label>
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
