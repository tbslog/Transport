import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
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
      Object.keys(props.selectIdClick).length > 0 &&
      Object.keys(props).length > 0
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
  }, [props, props.selectIdClick]);

  useEffect(() => {
    SetIsLoading(true);

    SetListProvince([]);
    SetListDistrict([]);
    SetListWard([]);

    SetIsLoading(false);
  }, []);

  const LoadTypeAddress = async (id) => {
    (async () => {
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

        setValue("MaLoaiDiaDiem", { ...obj.filter((x) => x.value === id)[0] });
      }
    })();
  };

  const LoadProvince = async (tinh) => {
    (async () => {
      const listProvince = await getData("address/GetListProvinces");
      if (listProvince && listProvince.length > 0) {
        var obj = [];
        listProvince.map((val) => {
          obj.push({
            value: val.maTinh,
            label: val.maTinh + " - " + val.tenTinh,
          });
        });
        SetListProvince(obj);
        setValue("MaTinh", { ...obj.filter((x) => x.value === tinh) }[0]);
      }
    })();
  };

  const LoadDistrict = async (tinh, huyen) => {
    (async () => {
      const listDistrict = await getData(
        `address/GetListDistricts?ProvinceId=${tinh}`
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

        setValue("MaHuyen", { ...obj.filter((x) => x.value === huyen) }[0]);
      } else {
        SetListDistrict([]);
      }
    })();
  };

  const LoadWard = async (huyen, phuong) => {
    (async () => {
      const listWard = await getData(
        `address/GetListWards?DistrictId=${huyen}`
      );

      if (listWard && listWard.length > 0) {
        var obj = [];
        listWard.map((val) => {
          obj.push({
            value: val.maPhuong,
            label: val.maPhuong + " - " + val.tenPhuong,
          });
        });

        SetListWard(obj);
        setValue("MaPhuong", { ...obj.filter((x) => x.value === phuong) }[0]);
      } else {
        SetListWard([]);
      }
    })();
  };

  const HandleChangeProvince = (val) => {
    try {
      setValue("MaHuyen", null);
      setValue("MaPhuong", null);
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

    const Update = await postData(
      `Address/EditAddress?Id=${data.MaDiaDiem}`,
      {
        tenDiaDiem: data.TenDiaDiem,
        maQuocGia: 1,
        maTinh: data.MaTinh.value,
        maHuyen: data.MaHuyen.value,
        maPhuong: data.MaPhuong.value,
        soNha: data.SoNha,
        diaChiDayDu: "",
        maGps: data.GPS,
        maLoaiDiaDiem: data.MaLoaiDiaDiem.value,
      },
      {
        accept: "*/*",
        "Content-Type": "application/json",
      }
    );

    if (Update === 1) {
      props.getListAddress(1);
      props.hideModal();
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
                </div>
                <div className="col col-sm">
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
                </div>
              </div>

              <div className="row">
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
