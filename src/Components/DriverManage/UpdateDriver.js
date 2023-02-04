import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";
import DatePicker from "react-datepicker";
import LoadingPage from "../Common/Loading/LoadingPage";

const UpdateDriver = (props) => {
  const { selectIdClick, getListDriver, listStatus, hideModal } = props;

  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    setValue,
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    MaTaiXe: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 12 ký tự",
      },
      minLength: {
        value: 10,
        message: "Không được ít hơn 12 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TenTaiXe: {
      required: "Không được để trống",
      maxLength: {
        value: 100,
        message: "Không được vượt quá 100 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      pattern: {
        value:
          /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 -,aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$/,
        message: "Tên khách hàng không được chứa ký tự đặc biệt",
      },
    },
    SoDienThoai: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 12 ký tự",
      },
      minLength: {
        value: 10,
        message: "Không được ít hơn 10 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    CCCD: {
      required: "Không được để trống",
      maxLength: {
        value: 12,
        message: "Không được vượt quá 12 ký tự",
      },
      minLength: {
        value: 12,
        message: "Không được ít hơn 12 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Phải là số",
      },
    },
    LoaiXe: {
      required: "Không được bỏ trống",
    },
    TrangThai: {
      required: "Không được bỏ trống",
    },
  };

  const [listVehicleType, setListVehicleType] = useState([]);
  const [listNCC, setListNCC] = useState([]);

  useEffect(() => {
    if (
      props &&
      selectIdClick &&
      listStatus &&
      listStatus.length > 0 &&
      Object.keys(selectIdClick).length > 0
    ) {
      setValue("MaTaiXe", selectIdClick.maTaiXe);
      setValue("CCCD", selectIdClick.cccd);
      setValue("TenTaiXe", selectIdClick.hoVaTen);
      setValue("SoDienThoai", selectIdClick.soDienThoai);
      setValue("NgaySinh", new Date(selectIdClick.ngaySinh));
      setValue("GhiChu", selectIdClick.ghiChu);
      setValue("NCC", {
        ...listNCC.filter((x) => x.value === selectIdClick.maNhaCungCap)[0],
      });
      setValue("LoaiXe", selectIdClick.maLoaiPhuongTien);
      setValue("TrangThai", selectIdClick.trangThai);
    }
  }, [props, selectIdClick, listStatus, listNCC, listVehicleType]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      let getListVehicleType = await getData("Common/GetListVehicleType");
      setListVehicleType(getListVehicleType);

      let getListNCC = await getData(`Customer/GetListCustomerOptionSelect`);
      if (getListNCC && getListNCC.length > 0) {
        getListNCC = getListNCC.filter((x) => x.loaiKH === "NCC");
        let obj = [];

        getListNCC.map((val) => {
          obj.push({
            value: val.maKh,
            label: val.maKh + " - " + val.tenKh,
          });
        });
        setListNCC(obj);
      }
      SetIsLoading(false);
    })();
  }, []);

  const onSubmit = async (data) => {
    SetIsLoading(true);

    const post = await postData(`Driver/EditDriver?driverId=${data.MaTaiXe}`, {
      cccd: data.CCCD,
      hoVaTen: data.TenTaiXe,
      soDienThoai: data.SoDienThoai,
      ngaySinh: new Date(data.NgaySinh).toISOString(),
      ghiChu: data.GhiChu,
      maNhaCungCap: data.NCC.value,
      TaiXeTBS: false,
      maLoaiPhuongTien: data.LoaiXe,
      trangThai: data.TrangThai,
    });

    if (post === 1) {
      getListDriver(1);
      SetIsLoading(false);
      hideModal();
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
                    <label htmlFor="NCC">Đơn Vị Quản Lý(*)</label>
                    <Controller
                      name="NCC"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listNCC}
                        />
                      )}
                      rules={{ required: "không được để trống" }}
                    />
                    {errors.NCC && (
                      <span className="text-danger">{errors.NCC.message}</span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="LoaiXe">Loại Xe Lái(*)</label>
                    <select
                      className="form-control"
                      {...register("LoaiXe", Validate.LoaiXe)}
                    >
                      <option value="">Chọn Loại Xe</option>
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
                    <label htmlFor="MaTaiXe">Mã Tài Xế(*)</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="MaTaiXe"
                      disabled={true}
                      {...register("MaTaiXe", Validate.MaTaiXe)}
                    />
                    {errors.MaTaiXe && (
                      <span className="text-danger">
                        {errors.MaTaiXe.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="TenTaiXe">Tên Tài Xế(*)</label>
                    <input
                      type="text"
                      className="form-control"
                      id="TenTaiXe"
                      {...register("TenTaiXe", Validate.TenTaiXe)}
                    />
                    {errors.TenTaiXe && (
                      <span className="text-danger">
                        {errors.TenTaiXe.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="NgaySinh">Ngày Sinh(*)</label>
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name={`NgaySinh`}
                        render={({ field }) => (
                          <DatePicker
                            className="form-control"
                            dateFormat="dd/MM/yyyy"
                            onChange={(date) => field.onChange(date)}
                            selected={field.value}
                          />
                        )}
                        rules={{
                          required: "không được để trống",
                        }}
                      />
                      {errors.NgaySinh && (
                        <span className="text-danger">
                          {errors.NgaySinh.message}
                        </span>
                      )}
                    </div>
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="CCCD">CCCD(*)</label>
                    <input
                      type="text"
                      className="form-control"
                      id="CCCD"
                      {...register("CCCD", Validate.CCCD)}
                    />
                    {errors.CCCD && (
                      <span className="text-danger">{errors.CCCD.message}</span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="SoDienThoai">Số Điện Thoại(*)</label>
                    <input
                      type="text"
                      className="form-control"
                      id="SoDienThoai"
                      {...register("SoDienThoai", Validate.SoDienThoai)}
                    />
                    {errors.SoDienThoai && (
                      <span className="text-danger">
                        {errors.SoDienThoai.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="form-group">
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
              </div>
              <div className="form-group">
                <label htmlFor="TrangThai">Trạng Thái(*)</label>
                <select
                  className="form-control"
                  {...register("TrangThai", Validate.TrangThai)}
                >
                  <option value="">Chọn Trạng Thái</option>
                  {listStatus &&
                    listStatus.map((val) => {
                      return (
                        <option value={val.statusId} key={val.statusId}>
                          {val.statusContent}
                        </option>
                      );
                    })}
                </select>
                {errors.TrangThai && (
                  <span className="text-danger">
                    {errors.TrangThai.message}
                  </span>
                )}
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

export default UpdateDriver;
