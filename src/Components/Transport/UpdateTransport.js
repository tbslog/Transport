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
    reset,
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
    TongThungHang: {
      required: "Không được để trống",
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
  const [arrRoad, setArrRoad] = useState([]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      const getListCus = await getData(
        `Customer/GetListCustomerOptionSelect?type=KH`
      );

      if (getListCus && getListCus.length > 0) {
        let arr = [];
        getListCus.map((val) => {
          arr.push({ label: val.maKh + " - " + val.tenKh, value: val.maKh });
        });
        setListCus(arr);
      } else {
        setListCus([]);
      }
      SetIsLoading(false);
    })();
  }, []);

  useEffect(() => {
    if (
      listCus &&
      listCus.length > 0 &&
      props &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      setValue("MaVDKH", selectIdClick.maVanDonKH);
      setValue("LoaiVanDon", selectIdClick.loaiVanDon);
      setValue("TongKhoiLuong", selectIdClick.tongKhoiLuong);
      setValue("TongTheTich", selectIdClick.tongTheTich);
      setValue("LoaiThungHang", selectIdClick.loaiThungHang);
      setValue(
        "MaKH",
        { ...listCus.filter((x) => x.value === selectIdClick.maKh) }[0]
      );

      handleOnChangeCustomer(
        { ...listCus.filter((x) => x.value === selectIdClick.maKh) }[0]
      );

      setValue("TongThungHang", selectIdClick.tongThungHang);

      if (selectIdClick.loaiVanDon === "xuat") {
        setValue("TGHaCang", new Date(selectIdClick.thoiGianHaCang));
      } else {
        setValue("TGCoMat", new Date(selectIdClick.thoiGianCoMat));
        setValue("TGHanLenh", new Date(selectIdClick.thoiGianHanLenh));
      }

      setValue("TGTraHang", new Date(selectIdClick.thoiGianTraHang));
      setValue("TGLayHang", new Date(selectIdClick.thoiGianLayHang));
      setValue("TGLayTraRong", new Date(selectIdClick.thoiGianLayTraRong));
    }
  }, [listCus, props, selectIdClick]);

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
      setValue(
        "MaCungDuong",
        { ...listRoad.filter((x) => x.value === selectIdClick.cungDuong) }[0]
      );

      handleOnChangeRoad(
        { ...listRoad.filter((x) => x.value === selectIdClick.cungDuong) }[0]
      );
    }
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

  const handleOnChangeVehicleType = (val) => {
    setValue("LoaiThungHang", val);

    setValue("TGHaCang", null);
    setValue("TGCoMat", null);
    setValue("TGHanLenh", null);
    setValue("TGLayTraRong", null);
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

    const Update = await postData(
      `BillOfLading/UpdateTransport?transportId=${selectIdClick.maVanDon}`,
      {
        LoaiThungHang: data.LoaiThungHang,
        HangTau: data.HangTau,
        TenTau: data.TenTau,
        MaVanDonKH: data.MaVDKH,
        MaCungDuong: data.MaCungDuong.value,
        LoaiVanDon: data.LoaiVanDon,
        TongKhoiLuong: data.TongKhoiLuong,
        TongTheTich: data.TongTheTich,
        MaKH: data.MaKH.value,
        TongThungHang: data.TongThungHang,
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
        ThoiGianLayHang: moment(new Date(data.TGLayHang).toISOString()).format(
          "yyyy-MM-DDTHH:mm:ss.SSS"
        ),
        ThoiGianTraHang: moment(new Date(data.TGTraHang).toISOString()).format(
          "yyyy-MM-DDTHH:mm:ss.SSS"
        ),
        ThoiGianLayTraRong: moment(
          new Date(data.TGLayTraRong).toISOString()
        ).format("yyyy-MM-DDTHH:mm:ss.SSS"),
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
                    <label htmlFor="LoaiVanDon">Phân Loại Vận Đơn</label>
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
                    <label htmlFor="LoaiThungHang">Loại Thùng Hàng</label>
                    <select
                      className="form-control"
                      {...register("LoaiThungHang", Validate.LoaiThungHang)}
                      value={watch("LoaiThungHang")}
                      onChange={(e) =>
                        handleOnChangeVehicleType(e.target.value)
                      }
                    >
                      <option value={""}>--Chọn Loại Thùng Hàng--</option>
                      <option value="CONT">CONT</option>
                      <option value="TRUCK">TRUCK</option>
                      <option value="CONT&TRUCK">CONT & TRUCK</option>
                    </select>
                    {errors.LoaiThungHang && (
                      <span className="text-danger">
                        {errors.LoaiThungHang.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TongThungHang">Tổng Số Thùng Hàng</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TongThungHang"
                      {...register(`TongThungHang`, Validate.TongThungHang)}
                    />
                    {errors.TongThungHang && (
                      <span className="text-danger">
                        {errors.TongThungHang.message}
                      </span>
                    )}
                  </div>
                  <br />
                </div>
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
              {watch("LoaiThungHang") &&
                watch("LoaiThungHang").includes("CONT") && (
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
                            rules={{
                              required: "không được để trống",
                            }}
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
                          <label htmlFor="TGHaCang">Thời Gian Hạ Cảng</label>
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
                            <label htmlFor="TGHanLenh">
                              Thời Gian Hạn Lệnh
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
