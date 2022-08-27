import { useState, useEffect } from "react";
import { getData, putData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import { ToastWarning } from "../Common/FuncToast";
import Select from "react-select";

const EditAddress = (props) => {
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

  const [ListTypeAddress, SetListTypeAddress] = useState([]);
  const [ListProvince, SetListProvince] = useState([]);
  const [ListDistrict, SetListDistrict] = useState([]);
  const [ListWard, SetListWard] = useState([]);

  const Validate = {
    TenDiaDiem: {
      required: "Không được để trống",
      maxLength: {
        value: 50,
        message: "Không được vượt quá 50 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
    },
    MaGPS: {
      required: "Không được để trống",
      maxLength: {
        value: 50,
        message: "Không được vượt quá 50 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
    },
    SoNha: {
      required: "Không được để trống",
      maxLength: {
        value: 100,
        message: "Không được vượt quá 100 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
    },
  };

  useEffect(() => {
    if (
      props &&
      props.selectIdClick &&
      Object.keys(props.selectIdClick).length > 0
    ) {
      setValue("MaDiaDiem", props.selectIdClick.maDiaDiem);
      setValue("TenDiaDiem", props.selectIdClick.tenDiaDiem);
      setValue("GPS", props.selectIdClick.maGps);
      setValue("SoNha", props.selectIdClick.soNha);

      LoadTypeAddress(props.selectIdClick.loaiDiaDiem);
      LoadProvince(props.selectIdClick.maTinh);
      LoadDistrict(props.selectIdClick.maTinh, props.selectIdClick.maHuyen);
      LoadWard(props.selectIdClick.maHuyen, props.selectIdClick.maPhuong);
    }
  }, [props.selectIdClick]);

  useEffect(() => {
    SetIsLoading(true);

    SetListProvince([]);
    SetListDistrict([]);
    SetListWard([]);

    SetIsLoading(false);
  }, []);

  const LoadTypeAddress = async (id) => {
    (async () => {
      const getListTypeAddress = await getData(
        "http://localhost:8088/api/address/GetListTypeAddress"
      );

      if (getListTypeAddress && getListTypeAddress.length > 0) {
        let obj = [];
        obj.push({
          value: "",
          label: "Chọn Loại địa điểm",
        });
        getListTypeAddress.map((val) => {
          obj.push({
            value: val.maLoaiDiaDiem,
            label: val.maLoaiDiaDiem + " - " + val.tenLoaiDiaDiem,
          });
        });
        SetListTypeAddress(obj);

        setValue(
          "MaLoaiDiaDiem",
          obj.filter((x) => x.value === id)
        );
      }
    })();
  };

  const LoadProvince = async (tinh) => {
    (async () => {
      const listProvince = await getData(
        "http://localhost:8088/api/address/ListProvinces"
      );
      if (listProvince && listProvince.length > 0) {
        var obj = [];
        obj.push({ value: "", label: "Chọn Tỉnh" });
        listProvince.map((val) => {
          obj.push({
            value: val.maTinh,
            label: val.maTinh + " - " + val.tenTinh,
          });
        });
        SetListProvince(obj);
        setValue(
          "MaTinh",
          obj.filter((x) => x.value === tinh)
        );
      }
    })();
  };

  const LoadDistrict = async (tinh, huyen) => {
    (async () => {
      const listDistrict = await getData(
        `http://localhost:8088/api/address/ListDistricts?ProvinceId=${tinh}`
      );
      if (listDistrict && listDistrict.length > 0) {
        var obj = [];
        obj.push({ value: "", label: "Chọn Huyện" });
        listDistrict.map((val) => {
          obj.push({
            value: val.maHuyen,
            label: val.maHuyen + " - " + val.tenHuyen,
          });
        });
        SetListDistrict(obj);

        setValue(
          "MaHuyen",
          obj.filter((x) => x.value === huyen)
        );
      } else {
        SetListDistrict([]);
      }
    })();
  };

  const LoadWard = async (huyen, phuong) => {
    (async () => {
      const listWard = await getData(
        `http://localhost:8088/api/address/ListWards?DistrictId=${huyen}`
      );

      if (listWard && listWard.length > 0) {
        var obj = [];
        obj.push({ value: "", label: "Chọn Phường" });
        listWard.map((val) => {
          obj.push({
            value: val.maPhuong,
            label: val.maPhuong + " - " + val.tenPhuong,
          });
        });

        SetListWard(obj);
        setValue(
          "MaPhuong",
          obj.filter((x) => x.value === phuong)
        );
      } else {
        SetListWard([]);
      }
    })();
  };

  const HandleChangeProvince = (val) => {
    try {
      setValue("MaHuyen", { value: "", label: "Chọn Huyện" });
      setValue("MaPhuong", { value: "", label: "Chọn Phường" });
      SetListDistrict([]);
      SetListWard([]);
      if (val.value === undefined || val.value === "") {
        return;
      }

      SetIsLoading(true);
      setValue("MaTinh", val);
      LoadDistrict(val.value, "");
      SetIsLoading(false);
    } catch (error) {}
  };

  const HandleOnchangeDistrict = (val) => {
    try {
      if (val.value === undefined || val.value === "") {
        return;
      }
      SetIsLoading(true);
      LoadWard(val.value, "");
      setValue("MaHuyen", val);
      SetIsLoading(false);
    } catch (error) {}
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);

    const Update = await putData(
      `http://localhost:8088/api/Address/EditAddress?id=${data.MaDiaDiem}`,
      {
        tenDiaDiem: data.TenDiaDiem,
        maQuocGia: 1,
        maTinh: data.MaTinh[0].value,
        maHuyen: data.MaHuyen[0].value,
        maPhuong: data.MaPhuong[0].value,
        soNha: data.SoNha,
        diaChiDayDu: "",
        maGps: data.GPS,
        maLoaiDiaDiem: data.MaLoaiDiaDiem[0].value,
      },
      {
        accept: "*/*",
        "Content-Type": "application/json",
      }
    );

    if (Update === 1) {
      props.getListAddress(1);
    }
    SetIsLoading(false);
  };

  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">Form Thêm Mới Địa Điểm</h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="form-group">
                <label htmlFor="TenDiaDiem">Tên địa điểm</label>
                <input
                  type="text"
                  className="form-control"
                  id="TenDiaDiem"
                  placeholder="Nhập tên địa điểm"
                  {...register("TenDiaDiem", Validate.MaGPS)}
                />
                {errors.TenDiaDiem && (
                  <span className="text-danger">
                    {errors.TenDiaDiem.message}
                  </span>
                )}
              </div>
              <div className="form-group">
                <label htmlFor="GPS">Mã GPS</label>
                <input
                  type="text"
                  className="form-control"
                  id="GPS"
                  placeholder="Nhập mã GPS"
                  {...register("GPS", Validate.MaGPS)}
                />
                {errors.GPS && (
                  <span className="text-danger">{errors.GPS.message}</span>
                )}
              </div>
              <div className="form-group">
                <label htmlFor="Sonha">Số nhà</label>
                <input
                  type="text"
                  className="form-control"
                  id="Sonha"
                  placeholder="Nhập số nhà"
                  {...register("SoNha", Validate.SoNha)}
                />
                {errors.SoNha && (
                  <span className="text-danger">{errors.SoNha.message}</span>
                )}
              </div>
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="Tinh">Loại địa điểm</label>
                    <Controller
                      name="MaLoaiDiaDiem"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={ListTypeAddress}
                          defaultValue={{
                            value: "",
                            label: "Chọn Loại địa điểm",
                          }}
                        />
                      )}
                      rules={{ required: "không được để trống" }}
                    />
                    {errors.MaLoaiDiaDiem && (
                      <span className="text-danger">
                        {errors.MaLoaiDiaDiem.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="Tinh">Tỉnh</label>
                    <Controller
                      name="MaTinh"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={ListProvince}
                          onChange={(field) => HandleChangeProvince(field)}
                          defaultValue={{ value: "", label: "Chọn Tỉnh" }}
                        />
                      )}
                      rules={{ required: "không được để trống" }}
                    />
                    {errors.MaTinh && (
                      <span className="text-danger">
                        {errors.MaTinh.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="SDT">Huyện</label>
                    <Controller
                      name="MaHuyen"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={ListDistrict}
                          onChange={(field) => HandleOnchangeDistrict(field)}
                          defaultValue={{ value: "", label: "Chọn Huyện" }}
                        />
                      )}
                      rules={{ required: "không được để trống" }}
                    />
                    {errors.MaHuyen && (
                      <span className="text-danger">
                        {errors.MaHuyen.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="SDT">Phường</label>
                    <Controller
                      name="MaPhuong"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={ListWard}
                          defaultValue={{ value: "", label: "Chọn Phường" }}
                        />
                      )}
                      rules={{ required: "không được để trống" }}
                    />
                    {errors.MaPhuong && (
                      <span className="text-danger">
                        {errors.MaPhuong.message}
                      </span>
                    )}
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

export default EditAddress;
