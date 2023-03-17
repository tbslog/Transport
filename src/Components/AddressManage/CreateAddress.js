import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";
import LoadingPage from "../Common/Loading/LoadingPage";

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
  const [listArea, setListArea] = useState([]);

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
      await loadArea();
      const getListTypeAddress = await getData("address/GetListAddressType");
      if (getListTypeAddress && getListTypeAddress.length > 0) {
        let obj = [];

        getListTypeAddress.map((val) => {
          obj.push({
            value: val.maLoaiDiaDiem,
            label: val.maLoaiDiaDiem + " - " + val.tenLoaiDiaDiem,
          });
        });
        SetListTypeAddress(obj);
      }

      const getlistProvince = await getData("address/GetListProvinces");

      if (getlistProvince && getlistProvince.length > 0) {
        let obj = [];
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

  const loadArea = async () => {
    const getListArea = await getData(
      "Address/GetListAddressSelect?pointType=&type=KhuVuc"
    );
    setListArea(getListArea);
  };

  const HandleChangeProvince = (val) => {
    try {
      SetIsLoading(true);

      setValue("MaHuyen", null);
      setValue("MaPhuong", null);
      if (val.value === undefined || val.value === "" || val === "") {
        SetListDistrict([]);
        SetListWard([]);
        SetIsLoading(false);
        return;
      }

      (async () => {
        const listDistrict = await getData(
          `address/GetListDistricts?ProvinceId=${val.value}`
        );

        if (listDistrict && listDistrict.length > 0) {
          var obj = [];

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

      (async () => {
        SetListWard([]);
        setValue("MaPhuong", null);

        const listWard = await getData(
          `address/GetListWards?DistrictId=${val.value}`
        );

        if (listWard && listWard.length > 0) {
          var obj = [];

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
    setValue("MaLoaiDiaDiem", null);
    setValue("MaTinh", null);
    setValue("MaHuyen", null);
    setValue("MaPhuong", null);
  };

  const onSubmit = async (data, e) => {
    SetIsLoading(true);

    console.log(data);
    const post = await postData("Address/CreateAddress", {
      tenDiaDiem: data.TenDiaDiem,
      maQuocGia: 1,
      maTinh: !data.MaTinh ? null : data.MaTinh.value,
      maHuyen: !data.MaHuyen ? null : data.MaHuyen.value,
      maPhuong: !data.MaPhuong ? null : data.MaPhuong.value,
      soNha: data.SoNha,
      diaChiDayDu: "",
      maGps: data.GPS,
      PhanLoaiLoaiDiaDiem: data.MaLoaiDiaDiem.value,
      DiaDiemCha: !data.KhuVuc ? null : data.KhuVuc,
    });

    if (post === 1) {
      await loadArea();
      props.getListAddress(1);
      handleResetClick();
    }
    SetIsLoading(false);
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
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="Tinh">Loại địa điểm(*)</label>
                    <Controller
                      name="MaLoaiDiaDiem"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={ListTypeAddress}
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
                    <label htmlFor="KhuVuc">Khu Vực</label>
                    <select
                      className="form-control"
                      {...register("KhuVuc", Validate.KhuVuc)}
                    >
                      <option value="">-- Địa Điểm Khu Vực --</option>
                      {listArea &&
                        listArea.length > 0 &&
                        listArea.map((val, index) => {
                          return (
                            <option value={val.maDiaDiem} key={index}>
                              {val.tenDiaDiem}
                            </option>
                          );
                        })}
                    </select>
                    {errors.KhuVuc && (
                      <span className="text-danger">
                        {errors.KhuVuc.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="TenDiaDiem">Tên địa điểm(*)</label>
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
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="GPS">Tọa Độ GPS(*)</label>
                    <input
                      type="text"
                      className="form-control"
                      id="GPS"
                      placeholder="Nhập Tọa Độ"
                      {...register("GPS", Validate.MaGPS)}
                    />
                    {errors.GPS && (
                      <span className="text-danger">{errors.GPS.message}</span>
                    )}
                  </div>
                </div>
              </div>

              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="Tinh">Tỉnh(*)</label>
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
                        />
                      )}
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
                    <label htmlFor="SDT">Huyện(*)</label>
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
                        />
                      )}
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
                    <label htmlFor="SDT">Phường(*)</label>
                    <Controller
                      name="MaPhuong"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={ListWard}
                        />
                      )}
                    />
                    {errors.MaPhuong && (
                      <span className="text-danger">
                        {errors.MaPhuong.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
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
                      <span className="text-danger">
                        {errors.SoNha.message}
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
