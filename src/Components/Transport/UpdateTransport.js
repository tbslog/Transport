import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import moment from "moment";

const UpdateTransport = (props) => {
  const { getListTransport, selectIdClick, hideModal } = props;

  const {
    register,
    setValue,
    clearErrors,
    control,
    watch,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
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
      required: "Không được để trống",
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    TongTheTich: {
      required: "Không được để trống",
      pattern: {
        value:
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
  };

  const [IsLoading, SetIsLoading] = useState(false);
  const [contractId, setContractId] = useState("");
  const [listFirstPoint, setListFirstPoint] = useState([]);
  const [listSecondPoint, setListSecondPoint] = useState([]);
  const [listRoad, setListRoad] = useState([]);
  const [listCus, setListCus] = useState([]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      const getListPoint = await getData("address/GetListAddressSelect");
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
    clearErrors();
    if (
      props &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0 &&
      listFirstPoint &&
      listSecondPoint &&
      listFirstPoint.length > 0 &&
      listSecondPoint.length > 0
    ) {
      SetIsLoading(true);
      setValue(
        "MaCungDuong",
        {
          ...listRoad.filter((x) => x.value === selectIdClick.maCungDuong),
        }[0]
      );

      setContractId(selectIdClick.maVanDon);
      setValue("MaVanDon", selectIdClick.maVanDon);
      setValue("TongKhoiLuong", selectIdClick.tongKhoiLuong);
      setValue("TongTheTich", selectIdClick.tongTheTich);
      setValue("LoaiVanDon", selectIdClick.loaiVanDon);
      setValue("TGLayHang", new Date(selectIdClick.thoiGianLayHang));
      setValue("TGTraHang", new Date(selectIdClick.thoiGianTraHang));
      setValue("TGLayTraRong", new Date(selectIdClick.thoiGianLayTraRong));
      setValue(
        "DiemLayHang",
        {
          ...listFirstPoint.filter((x) => x.value === selectIdClick.diemDau),
        }[0]
      );
      setValue(
        "DiemTraHang",
        {
          ...listSecondPoint.filter((x) => x.value === selectIdClick.diemCuoi),
        }[0]
      );

      handleOnChangePoint(
        {
          ...listFirstPoint.filter((x) => x.value === selectIdClick.diemDau),
        }[0],
        {
          ...listSecondPoint.filter((x) => x.value === selectIdClick.diemCuoi),
        }[0]
      );

      SetIsLoading(false);
    }
  }, [props, selectIdClick, listFirstPoint, listSecondPoint]);

  useEffect(() => {
    if (
      listRoad &&
      listRoad.length > 0 &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      SetIsLoading(true);
      setValue(
        "MaCungDuong",
        {
          ...listRoad.filter((x) => x.value === selectIdClick.cungDuong),
        }[0]
      );

      handleOnChangeRoad(
        {
          ...listRoad.filter((x) => x.value === selectIdClick.cungDuong),
        }[0]
      );
      SetIsLoading(false);
    }
  }, [selectIdClick, listRoad]);

  useEffect(() => {
    if (listCus && listCus.length > 0) {
      SetIsLoading(true);
      setValue(
        "MaKH",
        {
          ...listCus.filter((x) => x.value === selectIdClick.maKh),
        }[0]
      );
      SetIsLoading(false);
    }
  }, [selectIdClick, listCus]);

  const handleOnChangePoint = async (diemDau, diemCuoi) => {
    SetIsLoading(true);
    setListRoad([]);
    setListCus([]);
    setValue("MaKH", null);
    setValue("MaCungDuong", null);

    if (!diemDau && !diemCuoi) {
      diemDau = watch("DiemLayHang");
      diemCuoi = watch("DiemTraHang");
    }

    if (
      diemDau &&
      diemCuoi &&
      Object.keys(diemDau).length > 0 &&
      Object.keys(diemCuoi).length > 0
    ) {
      let getListRoad = await getData(
        `Road/getListRoadByPoint?diemDau=${diemDau.value}&diemCuoi=${diemCuoi.value}`
      );
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
    } else {
      setListRoad([]);
      setValue("MaCungDuong", null);
    }

    SetIsLoading(false);
  };

  const handleOnChangeRoad = async (cungDuong) => {
    SetIsLoading(true);
    if (!cungDuong) {
      cungDuong = watch("MaCungDuong");
    }

    setListCus([]);
    setValue("MaKH", null);
    if (cungDuong && Object.keys(cungDuong).length > 0) {
      let getListCus = await getData(
        `BillOfLading/LoadDataHandling?RoadId=${cungDuong.value}`
      );

      if (getListCus.listKhachHang && getListCus.listKhachHang.length > 0) {
        let obj = [];
        getListCus.listKhachHang.map((val) => {
          obj.push({
            value: val.maKH,
            label: val.maKH + " - " + val.tenKH,
          });
        });
        setListCus(obj);
      }
    }
    SetIsLoading(false);
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);

    const Update = await postData(
      `BillOfLading/UpdateTransport?transportId=${contractId}`,
      {
        maCungDuong: data.MaCungDuong.value,
        loaiVanDon: data.LoaiVanDon,
        tongThungHang: data.TongThungHang,
        tongKhoiLuong: data.TongKhoiLuong,
        tongTheTich: data.TongTheTich,
        MaKh: data.MaKH.value,
        thoiGianLayHang: moment(new Date(data.TGLayHang).toISOString()).format(
          "yyyy-MM-DDTHH:mm:ss.SSS"
        ),
        thoiGianTraHang: moment(new Date(data.TGTraHang).toISOString()).format(
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
      getListTransport(1);
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
                    <label htmlFor="DiemLayHang">Điểm Lấy Hàng</label>
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
                    <label htmlFor="DiemTraHang">Điểm Trả Hàng</label>
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
                            handleOnChangeRoad(
                              setValue(
                                "MaCungDuong",
                                {
                                  ...listRoad.filter(
                                    (x) => x.value === field.value
                                  ),
                                }[0]
                              )
                            )
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
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaKH">Khách Hàng</label>
                    <Controller
                      name="MaKH"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listCus}
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
                    <label htmlFor="LoaiVanDon">Phân Loại Vận Đơn</label>
                    <select
                      className="form-control"
                      {...register("LoaiVanDon", Validate.LoaiVanDon)}
                      value={watch("LoaiVanDon")}
                    >
                      <option defaultValue={true} value="nhap">
                        Vận Đơn Nhập
                      </option>
                      <option value="xuat">Vận Đơn Xuất</option>
                    </select>
                    {errors.LoaiVanDon && (
                      <span className="text-danger">
                        {errors.LoaiVanDon.message}
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
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    {watch("LoaiVanDon") === "xuat" && (
                      <label htmlFor="TGLayTraRong">Thời Gian Lấy Rỗng</label>
                    )}
                    {watch("LoaiVanDon") === "nhap" && (
                      <label htmlFor="TGLayTraRong">Thời Gian Trả Rỗng</label>
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
                      />
                      {errors.TGLayTraRong && (
                        <span className="text-danger">
                          {errors.TGLayTraRong.message}
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
