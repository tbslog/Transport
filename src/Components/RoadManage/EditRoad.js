import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import { ToastWarning } from "../Common/FuncToast";
import Select from "react-select";

const EditRoad = (props) => {
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

  const [listDiemLayRong, setListDiemLayRong] = useState([]);
  const [listAddress, SetListAddress] = useState([]);
  const [listStatus, setListStatus] = useState([]);

  const Validate = {
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
    TenCungDuong: {
      required: "Không được để trống",
      maxLength: {
        value: 100,
        message: "Không được vượt quá 100 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      // pattern: {
      //   value:
      //     /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$/,
      //   message: "Tên khách hàng không được chứa ký tự đặc biệt",
      // },
    },
    MaHopDong: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },

      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    SoKM: {
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
          /^(?:0\.(?:0[0-9]|[0-9]\d?)|[0-9]\d*(?:\.\d{1,2})?)(?:e[+-]?\d+)?$/,
        message: "Phải là số",
      },
    },
    TrangThai: {
      required: "Không được để trống",
    },
  };

  useEffect(() => {
    if (
      props &&
      props.selectIdClick &&
      Object.keys(props.selectIdClick).length > 0
    ) {
      setValue("MaCungDuong", props.selectIdClick.maCungDuong);
      setValue("TenCungDuong", props.selectIdClick.tenCungDuong);
      setValue("MaHopDong", props.selectIdClick.maHopDong);
      setValue("SoKM", props.selectIdClick.km);

      setValue("TrangThai", props.selectIdClick.trangThai);

      setValue("DiemLayRong", {
        ...listAddress.filter(
          (x) => x.value === props.selectIdClick.diemLayRong
        )[0],
      });
      setValue("DiemDau", {
        ...listAddress.filter(
          (x) => x.value === props.selectIdClick.diemDau
        )[0],
      });
      setValue("DiemCuoi", {
        ...listAddress.filter(
          (x) => x.value === props.selectIdClick.diemCuoi
        )[0],
      });
    }
  }, [props, props.selectIdClick, listAddress]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      const getlistAddress = await getData("address/GetListAddressSelect");

      if (getlistAddress && getlistAddress.length > 0) {
        var obj = [];
        getlistAddress.map((val) => {
          obj.push({
            value: val.maDiaDiem,
            label:
              val.maDiaDiem + " - " + val.tenDiaDiem + " --- " + val.diaChi,
          });
        });
        setListStatus(props.listStatus);

        obj.unshift({ value: "", label: "Empty" });
        setListDiemLayRong(obj);
        SetListAddress(obj.filter((x) => x.value !== ""));
      }
    })();

    SetIsLoading(false);
  }, []);

  const HandleOnchangeDiemLayRong = (val) => {
    setValue("DiemLayRong", {
      ...listAddress.filter((x) => x.value === val.value)[0],
    });
  };

  const HandleOnchangeDiemDau = (val) => {
    setValue("DiemDau", {
      ...listAddress.filter((x) => x.value === val.value)[0],
    });
  };

  const HandleOnchangeDiemCuoi = (val) => {
    setValue("DiemCuoi", {
      ...listAddress.filter((x) => x.value === val.value)[0],
    });
  };

  const onSubmit = async (data, e) => {
    SetIsLoading(true);
    const post = await postData(`Road/UpdateRoad?Id=${data.MaCungDuong}`, {
      tenCungDuong: data.TenCungDuong,
      km: data.SoKM,
      diemDau: data.DiemDau.value,
      diemCuoi: data.DiemCuoi.value,
      diemLayRong: data.DiemLayRong.value == "" ? null : data.DiemLayRong.value,
      ghiChu: data.GhiChu,
      trangThai: data.TrangThai,
    });

    if (post === 1) {
      props.getListRoad(1);
      props.hideModal();
    }

    SetIsLoading(false);
  };

  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">Form Cập Nhật Cung Đường</h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaCungDuong">Mã cung đường</label>
                    <input
                      readOnly
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="MaCungDuong"
                      placeholder="Nhập mã cung đường"
                      {...register("MaCungDuong", Validate.MaCungDuong)}
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
                    <label htmlFor="TenCungDuong">Tên cung đường</label>
                    <input
                      type="text"
                      className="form-control"
                      id="TenCungDuong"
                      placeholder="Nhập tên khách hàng"
                      {...register("TenCungDuong", Validate.TenCungDuong)}
                    />
                    {errors.TenCungDuong && (
                      <span className="text-danger">
                        {errors.TenCungDuong.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>

              <div className="form-group">
                <label htmlFor="SoKM">Số KM</label>
                <input
                  type="text "
                  className="form-control"
                  id="SoKM"
                  placeholder="Nhập Số KM"
                  {...register("SoKM", Validate.SoKM)}
                />
                {errors.SoKM && (
                  <span className="text-danger">{errors.SoKM.message}</span>
                )}
              </div>

              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="DiemLayRong">Điểm lấy rỗng</label>
                    <Controller
                      name="DiemLayRong"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          isDisabled={true}
                          value={field.value}
                          options={listDiemLayRong}
                          onChange={(field) => HandleOnchangeDiemLayRong(field)}
                        />
                      )}
                    />
                    {errors.DiemLayRong && (
                      <span className="text-danger">
                        {errors.DiemLayRong.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="DiemDau">Điểm đầu</label>
                    <Controller
                      name="DiemDau"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          isDisabled={true}
                          value={field.value}
                          options={listAddress}
                          onChange={(field) => HandleOnchangeDiemDau(field)}
                        />
                      )}
                      rules={{ required: "không được để trống" }}
                    />
                    {errors.DiemDau && (
                      <span className="text-danger">
                        {errors.DiemDau.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="DiemCuoi">Điểm cuối</label>
                    <Controller
                      name="DiemCuoi"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          isDisabled={true}
                          value={field.value}
                          options={listAddress}
                          onChange={(field) => HandleOnchangeDiemCuoi(field)}
                        />
                      )}
                      rules={{ required: "không được để trống" }}
                    />
                    {errors.DiemCuoi && (
                      <span className="text-danger">
                        {errors.DiemCuoi.message}
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
                <label htmlFor="TrangThai">Trạng Thái</label>
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

export default EditRoad;
