import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import moment from "moment";
import LoadingPage from "../Common/Loading/LoadingPage";

const UpdateTransportLess = (props) => {
  const { getListTransport, selectIdClick, hideModal } = props;

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
  });

  const Validate = {
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
    MaVDKH: {
      required: "Không được để trống",
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
    TongSoKien: {
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
    PTVanChuyen: {
      required: "Không được để trống",
    },
  };

  const [listFirstPoint, setListFirstPoint] = useState([]);
  const [listSecondPoint, setListSecondPoint] = useState([]);
  const [listCus, setListCus] = useState([]);
  const [transportData, setTransportData] = useState({});
  const [transportId, setTransportId] = useState("");
  const [listShipping, setListShipping] = useState([]);
  const [listAccountCus, setListAccountCus] = useState([]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      let getListShipping = await getData("Common/GetListShipping");
      setListShipping(getListShipping);
      const getListCus = await getData(`Customer/GetListCustomerFilter`);
      if (getListCus && getListCus.length > 0) {
        let arrKh = [];
        getListCus
          .filter((x) => x.loaiKH === "KH")
          .map((val) => {
            arrKh.push({
              label: val.maKh + " - " + val.tenKh,
              value: val.maKh,
            });
          });
        setListCus(arrKh);
      } else {
        setListCus([]);
      }

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
        setListFirstPoint(obj);
        setListSecondPoint(obj);
      }
      SetIsLoading(false);
    })();
  }, []);

  useEffect(() => {
    if (
      selectIdClick &&
      listCus &&
      listFirstPoint &&
      listSecondPoint &&
      listShipping &&
      listShipping.length > 0 &&
      Object.keys(selectIdClick).length > 0 &&
      listCus.length > 0 &&
      listFirstPoint.length > 0 &&
      listSecondPoint.length > 0
    ) {
      (async () => {
        SetIsLoading(true);
        handleResetClick();
        let getTransport = await getData(
          `BillOfLading/GetTransportLessById?transportId=${selectIdClick.maVanDon}`
        );
        setTransportData(getTransport);

        setValue("LoaiHinh", getTransport.maPTVC);

        setValue(
          "HangTau",
          {
            ...listShipping.filter((x) => x.value === getTransport.hangTau),
          }[0]
        );

        setValue("TenTau", getTransport.tenTau);
        setValue("MaVDKH", getTransport.maVanDonKH);

        setValue(
          "DiemLayHang",
          {
            ...listFirstPoint.filter(
              (x) => x.value === getTransport.diemLayHang
            ),
          }[0]
        );
        setValue(
          "DiemTraHang",
          {
            ...listSecondPoint.filter(
              (x) => x.value === getTransport.diemTraHang
            ),
          }[0]
        );
        setValue("LoaiVanDon", getTransport.loaiVanDon);
        setValue("TongKhoiLuong", getTransport.tongKhoiLuong);
        setValue("TongTheTich", getTransport.tongTheTich);
        setValue("TongSoKien", getTransport.tongSoKien);
        // setValue(
        //   "MaKH",

        // );

        handleOnChangeCustomer(
          { ...listCus.filter((x) => x.value === getTransport.maKH) }[0]
        );

        setValue("GhiChu", getTransport.ghiChu);
        setValue(
          "TGLayHang",
          !getTransport.thoiGianLayHang
            ? null
            : new Date(getTransport.thoiGianLayHang)
        );
        setValue(
          "TGTraHang",
          !getTransport.thoiGianTraHang
            ? null
            : new Date(getTransport.thoiGianTraHang)
        );
        SetIsLoading(false);
      })();
    }
  }, [selectIdClick, listCus, listSecondPoint, listFirstPoint, listShipping]);

  useEffect(() => {
    if (transportData && Object.keys(transportData).length > 0) {
      if (transportId !== selectIdClick.maVanDon) {
        SetIsLoading(true);
        setTransportId(selectIdClick.maVanDon);
        SetIsLoading(false);
      }
    }
  }, [transportData]);

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

  const handleOnchangeTransportType = (val) => {
    reset();
    setValue("LoaiVanDon", val);
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);

    const update = await postData(
      `BillOfLading/UpdateTransportLess?transportId=${selectIdClick.maVanDon}`,
      {
        AccountId: !data.AccountCus ? null : data.AccountCus.value,
        MaPTVC: data.LoaiHinh,
        HangTau: data.HangTau,
        TenTau: data.TenTau,
        MaVanDonKH: data.MaVDKH,
        DiemDau: data.DiemLayHang.value,
        DiemCuoi: data.DiemTraHang.value,
        LoaiVanDon: data.LoaiVanDon,
        TongKhoiLuong: !data.TongKhoiLuong ? null : data.TongKhoiLuong,
        TongTheTich: !data.TongTheTich ? null : data.TongTheTich,
        TongSoKien: !data.TongSoKien ? null : data.TongSoKien,
        MaKH: data.MaKH.value,
        GhiChu: data.GhiChu,
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
      }
    );

    if (update === 1) {
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
    setListAccountCus([]);
    setValue("AccountCus", null);
    // setListFirstPoint([]);
    // setListSecondPoint([]);
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
                      value={watch("LoaiHinh")}
                    >
                      <option value="">Chọn Loại Hình</option>
                      <option value="LTL">LTL</option>
                      <option value="LCL">LCL</option>
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
                </div>
              </div>

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

export default UpdateTransportLess;
