import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";
import DatePicker from "react-datepicker";

const CreateVehicle = (props) => {
  const { getListVehicle, listStatus } = props;

  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    reset,
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
    defaultValues: {
      TaiXeMacDinh: { value: "", label: "Rỗng" },
    },
  });

  const Validate = {
    MaSoXe: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      minLength: {
        value: 6,
        message: "Không được ít hơn 6 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TrongTaiToiDa: {
      maxLength: {
        value: 10,
        message: "Không được vượt quá 12 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TrongTaiToiThieu: {
      maxLength: {
        value: 10,
        message: "Không được vượt quá 12 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TGKhauHao: {
      maxLength: {
        value: 10,
        message: "Không được vượt quá 12 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    LoaiXe: {
      required: "Không được bỏ trống",
    },
    MaGPS: {
      required: "Không được bỏ trống",
    },
    MaGPSMobile: {
      required: "Không được bỏ trống",
    },
    TrangThai: {
      required: "Không được bỏ trống",
    },
  };

  const [listVehicleType, setListVehicleType] = useState([]);
  const [listDriver, setListDriver] = useState([]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      let getListVehicleType = await getData("Common/GetListVehicleType");
      getListVehicleType = getListVehicleType.filter(
        (x) => !x.maLoaiPhuongTien.includes("CONT")
      );
      setListVehicleType(getListVehicleType);

      let getListDriver = await getData(`Driver/GetListSelectDriver`);
      if (getListDriver && getListDriver.length > 0) {
        let obj = [];
        obj.push({ value: null, label: "Rỗng" });
        getListDriver.map((val) => {
          obj.push({
            value: val.maTaiXe,
            label: val.maTaiXe + " - " + val.hoVaTen,
          });
        });
        setListDriver(obj);
      }
      SetIsLoading(false);
    })();
  }, []);

  const onSubmit = async (data) => {
    SetIsLoading(true);
    const post = await postData("Vehicle/CreateVehicle", {
      MaSoXe: data.MaSoXe,
      MaLoaiPhuongTien: data.LoaiXe,
      MaTaiXeMacDinh: !data.TaiXeMacDinh.value ? null : data.TaiXeMacDinh.value,
      TrongTaiToiThieu: !data.TrongTaiToiThieu ? null : data.TrongTaiToiThieu,
      TrongTaiToiDa: !data.TrongTaiToiDa ? null : data.TrongTaiToiDa,
      MaGps: data.MaGPS,
      MaGpsmobile: data.MaGPSMobile,
      ThoiGianKhauHao: data.ThoiGianKhauHao,
      MaTaiSan: data.LoaiXe,
      NgayHoatDong: !data.NgayHoatDong
        ? null
        : new Date(data.NgayHoatDong).toISOString(),
    });

    if (post === 1) {
      getListVehicle(1);
      handleResetClick();
    }
    SetIsLoading(false);
  };

  const handleResetClick = () => {
    reset();
  };

  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">Form Thêm Phương Tiện</h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>
        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TaiXeMacDinh">Tài Xế Mặc Định</label>
                    <Controller
                      name="TaiXeMacDinh"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listDriver}
                        />
                      )}
                      rules={Validate.TaiXeMacDinh}
                    />
                    {errors.TaiXeMacDinh && (
                      <span className="text-danger">
                        {errors.TaiXeMacDinh.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="LoaiXe">Phân Loại Xe(*)</label>
                    <select
                      className="form-control"
                      {...register("LoaiXe", Validate.LoaiXe)}
                    >
                      <option value="">Chọn Loại Xe</option>
                      <option value="CONT">Container</option>
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
                    {errors.LoaiXe && (
                      <span className="text-danger">
                        {errors.LoaiXe.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="MaSoXe">Mã Số Xe(*)</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="MaSoXe"
                      {...register("MaSoXe", Validate.MaSoXe)}
                    />
                    {errors.MaSoXe && (
                      <span className="text-danger">
                        {errors.MaSoXe.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="TrongTaiToiThieu">
                      Trọng Tải Tối Thiểu
                    </label>
                    <input
                      type="text"
                      className="form-control"
                      id="TrongTaiToiThieu"
                      {...register(
                        "TrongTaiToiThieu",
                        Validate.TrongTaiToiThieu
                      )}
                    />
                    {errors.TrongTaiToiThieu && (
                      <span className="text-danger">
                        {errors.TrongTaiToiThieu.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="TrongTaiToiDa">Trọng Tải Tối Đa</label>
                    <input
                      type="text"
                      className="form-control"
                      id="TrongTaiToiDa"
                      {...register("TrongTaiToiDa", Validate.TrongTaiToiDa)}
                    />
                    {errors.TrongTaiToiDa && (
                      <span className="text-danger">
                        {errors.TrongTaiToiDa.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="MaGPS">Mã GPS(*)</label>
                    <input
                      type="text"
                      className="form-control"
                      id="MaGPS"
                      {...register("MaGPS", Validate.MaGPS)}
                    />
                    {errors.MaGPS && (
                      <span className="text-danger">
                        {errors.MaGPS.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="MaGPSMobile">Mã GPS Mobile(*)</label>
                    <input
                      type="text"
                      className="form-control"
                      id="MaGPSMobile"
                      {...register("MaGPSMobile", Validate.MaGPSMobile)}
                    />
                    {errors.MaGPSMobile && (
                      <span className="text-danger">
                        {errors.MaGPSMobile.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="MaTaiSan">Mã Tài Sản</label>
                    <input
                      type="text"
                      className="form-control"
                      id="MaTaiSan"
                      {...register("MaTaiSan", Validate.MaTaiSan)}
                    />
                    {errors.MaTaiSan && (
                      <span className="text-danger">
                        {errors.MaTaiSan.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="TGKhauHao">Thời Gian Khấu Hao</label>
                    <input
                      type="text"
                      className="form-control"
                      id="TGKhauHao"
                      {...register("TGKhauHao", Validate.TGKhauHao)}
                    />
                    {errors.TGKhauHao && (
                      <span className="text-danger">
                        {errors.TGKhauHao.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="NgayHoatDong">Ngày Hoạt Động</label>
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name={`NgayHoatDong`}
                        render={({ field }) => (
                          <DatePicker
                            className="form-control"
                            dateFormat="dd/MM/yyyy"
                            onChange={(date) => field.onChange(date)}
                            selected={field.value}
                          />
                        )}
                      />
                      {errors.NgayHoatDong && (
                        <span className="text-danger">
                          {errors.NgayHoatDong.message}
                        </span>
                      )}
                    </div>
                  </div>
                </div>
              </div>
              {/* <div className="form-group">
                <label htmlFor="GhiChu">Ghi Chú</label>
                <input
                  type="text"
                  className="form-control"
                  id="GhiChu"
                  placeholder="Nhập ghi chú"
                  {...register("GhiChu")}
                />
                {errors.GhiChu && (
                  <span className="text-danger">{errors.GhiChu.message}</span>
                )}
              </div> */}
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

export default CreateVehicle;
