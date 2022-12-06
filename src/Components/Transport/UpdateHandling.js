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
    CONTNO: {
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
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9 ]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt, phải viết hoa",
      },
    },
    SEALHT: {
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
      required: "Không được để trống",
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TheTich: {
      required: "Không được để trống",
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
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

  useEffect(() => {
    (async () => {
      SetIsLoading(true);
      let getListVehicleType = await getData("Common/GetListVehicleType");
      let getListGoodsType = await getData("Common/GetListGoodsType");

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

      let getDataHandling = await getData("BillOfLading/LoadDataHandling");
      if (getDataHandling && Object.keys(getDataHandling).length > 0) {
        if (
          getDataHandling.listNhaPhanPhoi &&
          getDataHandling.listNhaPhanPhoi.length > 0
        ) {
          let arr = [];
          getDataHandling.listNhaPhanPhoi.map((val) => {
            arr.push({ label: val.tenNPP, value: val.maNPP });
          });
          setListSupplier(arr);
        }

        if (
          getDataHandling.listKhachHang &&
          getDataHandling.listKhachHang.length > 0
        ) {
          let arr = [];
          getDataHandling.listKhachHang.map((val) => {
            arr.push({ label: val.tenKH, value: val.maKH });
          });
          setListCustomer(arr);
        }

        if (
          getDataHandling.listXeVanChuyen &&
          getDataHandling.listXeVanChuyen.length > 0
        ) {
          let arr = [];
          getDataHandling.listXeVanChuyen.map((val) => {
            arr.push({
              label: val.maSoXe + " - " + val.maLoaiPhuongTien,
              value: val.maSoXe,
            });
          });
          setListVehicle(arr);
        }

        if (getDataHandling.listTaiXe && getDataHandling.listTaiXe.length > 0) {
          let arr = [];
          getDataHandling.listTaiXe.map((val) => {
            arr.push({
              label: val.maTaiXe + " - " + val.tenTaiXe,
              value: val.maTaiXe,
            });
          });
          setListDriver(arr);
        }

        if (
          getDataHandling.listRomooc &&
          getDataHandling.listRomooc.length > 0
        ) {
          let arr = [];
          getDataHandling.listRomooc.map((val) => {
            arr.push({
              label: val.maRomooc + " - " + val.tenLoaiRomooc,
              value: val.maRomooc,
            });
          });
          setListRomooc(arr);
        }
      }
      setlistVehicleType(getListVehicleType);
      setListGoodsType(getListGoodsType);
      SetIsLoading(false);
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
      handleResetClick();
      (async () => {
        let data = await getData(
          `BillOfLading/GetHandlingById?id=${selectIdClick.maDieuPhoi}`
        );

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
      listVehicle &&
      listDriver.length > 0 &&
      listRomooc.length > 0 &&
      listVehicle.length > 0
    ) {
      setValueData(data);
    }
  }, [data, listDriver, listRomooc, listVehicle]);

  const setValueData = (data) => {
    setRoadDetail(data.cungDuong);

    setValue(
      "NhaCungCap",
      {
        ...listSupplier.filter((x) => x.value === data.donViVanTai),
      }[0]
    );

    setValue(
      "KhachHang",
      {
        ...listCustomer.filter((x) => x.value === data.maKh),
      }[0]
    );

    setValue(
      "DiemLayTraRong",
      {
        ...listPoint.filter((x) => x.value === data.diemLayRong),
      }[0]
    );
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
    setValue("HangTau", data.hangTau);
    setValue("TenTau", data.tenTau);
    setValue(
      "TGLayTraRong",
      !data.thoiGianLayTraRong ? null : new Date(data.thoiGianLayTraRong)
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
    setValue("TGLayHang", new Date(data.thoiGianLayHang));
    setValue("TGTraHang", new Date(data.thoiGianTraHang));

    setValue(
      "TGLayTraRongThucTe",
      !data.thoiGianLayTraRongThucTe
        ? null
        : new Date(data.thoiGianLayTraRongThucTe)
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

  const onSubmit = async (data) => {
    SetIsLoading(true);

    let dataUpdate = {
      DonViVanTai: data.NhaCungCap.value,
      PTVanChuyen: data.PTVanChuyen,
      MaPtvc: "Road",
      LoaiHangHoa: data.LoaiHangHoa,
      DonViTinh: "CHUYEN",
      DiemLayTraRong: !data.DiemLayTraRong ? null : data.DiemLayTraRong.value,
      MaSoXe: data.XeVanChuyen.value,
      MaTaiXe: data.TaiXe.value,
      MaRomooc: !data.Romooc ? null : data.Romooc.value,
      ContNo: data.CONTNO,
      SealNp: data.SEALNP,
      SealHq: data.SEALHQ,
      KhoiLuong: !data.KhoiLuong ? null : data.KhoiLuong,
      TheTich: !data.TheTich ? null : data.TheTich,
      GhiChu: data.GhiChu,
      ThoiGianLayTraRongThucTe: !data.TGLayTraRongThucTe
        ? null
        : moment(new Date(data.TGLayTraRongThucTe).toISOString()).format(
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
      getlistData();
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
                    <label htmlFor="KhachHang">Khách Hàng(*)</label>
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
                    <label htmlFor="CungDuong">Cung Đường(*)</label>
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
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="CungDuong">Điểm Lấy Hàng(*)</label>
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
                    <label htmlFor="CungDuong">Điểm Trả Hàng(*)</label>
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
                    <div className="col col-sm">
                      <div className="form-group">
                        {transportType && transportType === "xuat" && (
                          <label htmlFor="DiemLayTraRong">
                            Điểm Lấy Rỗng(*)
                          </label>
                        )}
                        {transportType && transportType === "nhap" && (
                          <label htmlFor="DiemLayTraRong">
                            Điểm Trả Rỗng(*)
                          </label>
                        )}
                        <Controller
                          name={`DiemLayTraRong`}
                          control={control}
                          render={({ field }) => (
                            <Select
                              isDisabled={true}
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
                        {errors.DiemLayTraRong && (
                          <span className="text-danger">
                            {errors.DiemLayTraRong.message}
                          </span>
                        )}
                      </div>
                    </div>
                  )}
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="NhaCungCap">Đơn Vị Vận Tải(*)</label>
                    <Controller
                      name={`NhaCungCap`}
                      control={control}
                      render={({ field }) => (
                        <Select
                          isDisabled={true}
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listSupplier}
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
                    <label htmlFor="PTVanChuyen">
                      Phương tiện vận chuyển(*)
                    </label>
                    <select
                      className="form-control"
                      {...register(`PTVanChuyen`, Validate.PTVanChuyen)}
                      value={watch(`PTVanChuyen`)}
                      disabled={true}
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
                      disabled={true}
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
                    <label htmlFor="KhoiLuong">Khối Lượng(*)</label>
                    <input
                      disabled={true}
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
                    <label htmlFor="TheTich">Thể tích(*)</label>
                    <input
                      disabled={true}
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
                    <label htmlFor="SoKhoi">Số Khối(*)</label>
                    <input
                      disabled={true}
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="SoKhoi"
                      {...register(`SoKhoi`, Validate.SoKhoi)}
                    />
                    {errors.SoKhoi && (
                      <span className="text-danger">
                        {errors.SoKhoi.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="XeVanChuyen">Xe Vận Chuyển(*)</label>
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
                    <label htmlFor="TaiXe">Tài Xế(*)</label>
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
                  watch(`PTVanChuyen`).includes("CONT") && (
                    <>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="CONTNO">CONT NO(*)</label>
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
              </div>

              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TGLayHangThucTe">
                      Thời Gian Lấy Hàng Thực Tế(*)
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
                      Thời Gian Trả Hàng Thực Tế(*)
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
                            <label htmlFor="TGLayTraRongThucTe">
                              Thời Gian Lấy Rỗng Thực Tế(*)
                            </label>
                          )}
                          {transportType && transportType === "nhap" && (
                            <label htmlFor="TGLayTraRongThucTe">
                              Thời Gian Trả Rỗng Thực Tế(*)
                            </label>
                          )}

                          <div className="input-group ">
                            <Controller
                              control={control}
                              name={`TGLayTraRongThucTe`}
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
                            {errors.TGLayTraRongThucTe && (
                              <span className="text-danger">
                                {errors.TGLayTraRongThucTe.message}
                              </span>
                            )}
                          </div>
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
                                Thời Gian Có Mặt Thực Tế(*)
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
              <div>
                <button
                  type="submit"
                  className="btn btn-primary"
                  style={{ float: "right" }}
                >
                  Xác Nhận
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
