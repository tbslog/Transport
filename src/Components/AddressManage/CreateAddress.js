import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";

const CreateAddress = (props) => {
  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    reset,
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
    SetIsLoading(true);

    SetListProvince([]);
    SetListDistrict([]);
    SetListWard([]);

    (async () => {
      const getListTypeAddress = await getData("address/GetListTypeAddress");

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
      }

      const getlistProvince = await getData("address/ListProvinces");

      if (getlistProvince && getlistProvince.length > 0) {
        let obj = [];
        obj.push({ value: "", label: "Chọn Tỉnh" });
        getlistProvince.map((val) => {
          obj.push({
            value: val.maTinh,
            label: val.maTinh + " - " + val.tenTinh,
          });
        });

        SetListProvince(obj);
      }
    })();

    SetIsLoading(false);
  }, []);

  const HandleChangeProvince = (val) => {
    try {
      SetIsLoading(true);

      setValue("MaHuyen", { value: "", label: "Chọn Huyện" });
      setValue("MaPhuong", { value: "", label: "Chọn Phường" });
      if (val.value === undefined || val.value === "" || val === "") {
        SetListDistrict([]);
        SetListWard([]);
        SetIsLoading(false);
        return;
      }

      (async () => {
        const listDistrict = await getData(
          `address/ListDistricts?ProvinceId=${val.value}`
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
          setValue("MaTinh", val);
        } else {
          SetListDistrict([]);
          SetListWard([]);
        }
      })();

      SetListDistrict([]);
      SetListWard([]);
      SetIsLoading(false);
    } catch (error) {}
  };

  const HandleOnchangeDistrict = (val) => {
    try {
      SetIsLoading(true);

      if (val.value === undefined || val.value === "") {
        SetListWard([]);
        return;
      }

      (async () => {
        const listWard = await getData(
          `address/ListWards?DistrictId=${val.value}`
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

          setValue("MaHuyen", val);
          SetListWard(obj);
        } else {
          SetListWard([]);
        }
      })();

      SetIsLoading(false);
    } catch (error) {}
  };

  const handleResetClick = () => {
    reset();
    setValue("MaLoaiDiaDiem", { value: "", label: "Chọn Loại địa điểm" });
    setValue("MaTinh", { value: "", label: "Chọn Tỉnh" });
    setValue("MaHuyen", { value: "", label: "Chọn Huyện" });
    setValue("MaPhuong", { value: "", label: "Chọn Phường" });
  };

  const onSubmit = async (data, e) => {
    SetIsLoading(true);

    const post = await postData("Address/CreateAddress", {
      tenDiaDiem: data.TenDiaDiem,
      maQuocGia: 1,
      maTinh: data.MaTinh.value,
      maHuyen: data.MaHuyen.value,
      maPhuong: data.MaPhuong.value,
      soNha: data.SoNha,
      diaChiDayDu: "",
      maGps: data.GPS,
      maLoaiDiaDiem: "1",
    });
    if (post === 1) {
      props.getListAddress(1);
      handleResetClick();
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
                  {...register("TenDiaDiem", Validate.TenDiaDiem)}
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

export default CreateAddress;
