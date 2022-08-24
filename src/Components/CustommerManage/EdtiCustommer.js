import { useState, useEffect } from "react";
import axios from "axios";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";

const EditCustommer = (props) => {
  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    setValue,
    formState: { errors },
    handleSubmit,
    reset,
  } = useForm({
    mode: "onChange",
  });

  const [ListProvince, SetListProvince] = useState([]);
  const [ListDistrict, SetListDistrict] = useState([]);
  const [ListWard, SetListWard] = useState([]);

  const onSubmit = async (data) => {
    SetIsLoading(true);

    await axios
      .put(
        `http://localhost:8088/api/Custommer/EdtiCustommer?CustommerId=${data.MaKH}`,
        {
          maKh: data.MaKH.toUpperCase(),
          tenKh: data.TenKH,
          maSoThue: data.MST,
          sdt: data.SDT,
          email: data.Email,
          address: {
            tenDiaDiem: "",
            maQuocGia: 1,
            maTinh: parseInt(data.MaTinh),
            maHuyen: parseInt(data.MaHuyen),
            maPhuong: parseInt(data.MaPhuong),
            soNha: data.SoNha,
            diaChiDayDu: "",
            maGps: data.GPS,
            maLoaiDiaDiem: "1",
          },
        },
        {
          accept: "*/*",
          "Content-Type": "application/json",
        }
      )
      .then(
        (response) => {
          props.getListUser(1);
          toast.success(response.data);
        },
        (error) => {
          toast.success(error.response.data);
        }
      );

    SetIsLoading(false);
  };

  useEffect(() => {
    reset();
    if (
      props &&
      props.selectIdClick &&
      props.Address &&
      Object.keys(props.Address).length > 0 &&
      Object.keys(props.selectIdClick).length > 0
    ) {
      setValue("MaKH", props.selectIdClick.maKh);
      setValue("MST", props.selectIdClick.maSoThue);
      setValue("SDT", props.selectIdClick.sdt);
      setValue("TenKH", props.selectIdClick.tenKh);
      setValue("Email", props.selectIdClick.email);
      setValue("GPS", props.Address.maGps);
      setValue("SoNha", props.Address.sonha);

      async function getAddress() {
        await LoadDistrict(props.Address.matinh);
        await LoadWard(props.Address.mahuyen);
        setValue("MaTinh", props.Address.matinh);
        setValue("MaHuyen", props.Address.mahuyen);
        setValue("MaPhuong", props.Address.maphuong);
      }
      getAddress();
    }
  }, [props.selectIdClick, props.Address]);

  useEffect(() => {
    SetIsLoading(true);

    SetListProvince([]);
    SetListDistrict([]);
    SetListWard([]);

    async function getProvince() {
      const listProvince = await axios.get(
        "http://localhost:8088/api/address/ListProvinces"
      );

      if (listProvince && listProvince.data && listProvince.data.length > 0) {
        SetListProvince(listProvince.data);
      }
    }

    getProvince();
    SetIsLoading(false);
  }, []);

  const LoadDistrict = async (val) => {
    const listDistrict = await axios.get(
      `http://localhost:8088/api/address/ListDistricts?ProvinceId=${val}`
    );
    if (listDistrict && listDistrict.data && listDistrict.data.length > 0) {
      SetListDistrict(listDistrict.data);
    } else {
      SetListDistrict([]);
    }
  };

  const LoadWard = async (val) => {
    const listWard = await axios.get(
      `http://localhost:8088/api/address/ListWards?DistrictId=${val}`
    );

    if (listWard && listWard.data && listWard.data.length > 0) {
      SetListWard(listWard.data);
    } else {
      SetListWard([]);
    }
  };

  const HandleChangeProvince = (val) => {
    try {
      SetListDistrict([]);
      SetListWard([]);
      setValue("MaHuyen", "");
      setValue("MaPhuong", "");
      SetIsLoading(true);
      if (val === undefined || val === "") {
        SetListDistrict([]);
        SetListWard([]);
        return;
      }
      LoadDistrict(val);
      SetIsLoading(false);
    } catch (error) {}
  };

  const HandleOnchangeDistrict = (val) => {
    try {
      SetIsLoading(true);

      if (val === undefined || val === "") {
        SetListWard([]);
        return;
      }
      LoadWard(val);
      SetIsLoading(false);
    } catch (error) {}
  };
  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">Form Cập nhật Khách Hàng</h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)} autocomplete="off">
            <div className="card-body">
              <div className="form-group">
                <label htmlFor="MaKH">Mã khách hàng</label>
                <input
                  autoComplete="false"
                  type="text"
                  className="form-control"
                  id="MaKH"
                  readOnly
                  placeholder="Nhập mã khách hàng"
                  {...register("MaKH", {
                    required: "Không được để trống",
                    maxLength: {
                      value: 8,
                      message: "Không được vượt quá 8 ký tự",
                    },
                    minLength: {
                      value: 8,
                      message: "Không được ít hơn 8 ký tự",
                    },
                    pattern: {
                      value:
                        /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
                      message: "Không được chứa ký tự đặc biệt",
                    },
                  })}
                />
                {errors.MaKH && (
                  <span className="text-danger">{errors.MaKH.message}</span>
                )}
              </div>
              <div className="form-group">
                <label htmlFor="TenKH">Tên khách hàng</label>
                <input
                  type="text"
                  className="form-control"
                  id="TenKH"
                  placeholder="Nhập tên khách hàng"
                  {...register("TenKH", {
                    required: "Không được để trống",
                    maxLength: {
                      value: 50,
                      message: "Không được vượt quá 50 ký tự",
                    },
                    minLength: {
                      value: 1,
                      message: "Không được ít hơn 1 ký tự",
                    },
                    pattern: {
                      value:
                        /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$/,
                      message: "Tên khách hàng không được chứa ký tự đặc biệt",
                    },
                  })}
                />
                {errors.TenKH && (
                  <span className="text-danger">{errors.TenKH.message}</span>
                )}
              </div>
              <div className="form-group">
                <label htmlFor="Email">Địa chỉ Email</label>
                <input
                  type="text "
                  className="form-control"
                  id="Email"
                  placeholder="Nhập địa chỉ Email"
                  {...register("Email", {
                    required: "Không được để trống",
                    maxLength: {
                      value: 100,
                      message: "Không được vượt quá 100 ký tự",
                    },
                    minLength: {
                      value: 3,
                      message: "Không được ít hơn 3 ký tự",
                    },
                    pattern: {
                      value: /^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$/,
                      message: "Không phải Email",
                    },
                  })}
                />
                {errors.Email && (
                  <span className="text-danger">{errors.Email.message}</span>
                )}
              </div>
              <div className="form-group">
                <label htmlFor="MST">Mã số thuế</label>
                <input
                  type="text "
                  className="form-control"
                  id="MST"
                  placeholder="Nhập mã số thuế"
                  {...register("MST", {
                    required: "Không được để trống",
                    maxLength: {
                      value: 50,
                      message: "Không được vượt quá 50 ký tự",
                    },
                    minLength: {
                      value: 1,
                      message: "Không được ít hơn 1 ký tự",
                    },
                    pattern: {
                      value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
                      message: "Mã số thuế chỉ được chứa ký tự là số",
                    },
                  })}
                />
                {errors.MST && (
                  <span className="text-danger">{errors.MST.message}</span>
                )}
              </div>
              <div className="form-group">
                <label htmlFor="SDT">Số điện thoại</label>
                <input
                  type="text"
                  className="form-control"
                  id="SDT"
                  placeholder="Nhập số điện thoại"
                  {...register("SDT", {
                    required: "Không được để trống",
                    maxLength: {
                      value: 50,
                      message: "Không được vượt quá 20 ký tự",
                    },
                    minLength: {
                      value: 10,
                      message: "Không được ít hơn 10 ký tự",
                    },
                    pattern: {
                      value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
                      message: "Số điện thoại chỉ được chứa ký tự là số",
                    },
                  })}
                />
                {errors.SDT && (
                  <span className="text-danger">{errors.SDT.message}</span>
                )}
              </div>
              <div className="form-group">
                <label htmlFor="GPS">Mã GPS</label>
                <input
                  type="text"
                  className="form-control"
                  id="GPS"
                  placeholder="Nhập mã GPS"
                  {...register("GPS", {
                    required: "Không được để trống",
                    maxLength: {
                      value: 50,
                      message: "Không được vượt quá 50 ký tự",
                    },
                    minLength: {
                      value: 1,
                      message: "Không được ít hơn 1 ký tự",
                    },
                  })}
                />
                {errors.GPS && (
                  <span className="text-danger">{errors.GPS.message}</span>
                )}
              </div>
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="Sonha">Số nhà</label>
                    <input
                      type="text"
                      className="form-control"
                      id="Sonha"
                      placeholder="Nhập số nhà"
                      {...register("SoNha", {
                        required: "Không được để trống",
                        maxLength: {
                          value: 100,
                          message: "Không được vượt quá 100 ký tự",
                        },
                        minLength: {
                          value: 1,
                          message: "Không được ít hơn 1 ký tự",
                        },
                      })}
                    />
                    {errors.SoNha && (
                      <span className="text-danger">
                        {errors.SoNha.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="Tinh">Tỉnh</label>
                    <select
                      className="form-control"
                      id="inputGroupSelect01"
                      {...register("MaTinh", {
                        required: "Không được để trống",
                        onChange: (e) => HandleChangeProvince(e.target.value),
                      })}
                    >
                      <option value="">Chọn tỉnh...</option>
                      {ListProvince &&
                        ListProvince.length > 0 &&
                        ListProvince.map((val) => {
                          return (
                            <option key={val.maTinh} value={val.maTinh}>
                              {val.maTinh} - {val.tenTinh}
                            </option>
                          );
                        })}
                    </select>
                    {errors.maTinh && (
                      <span className="text-danger">
                        {errors.maTinh.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="SDT">Huyện</label>
                    <select
                      className="form-control"
                      id="inputGroupSelect01"
                      {...register("MaHuyen", {
                        required: "Không được để trống",
                        onChange: (e) => HandleOnchangeDistrict(e.target.value),
                      })}
                    >
                      <option value="">Chọn Huyện...</option>
                      {ListDistrict &&
                        ListDistrict.length > 0 &&
                        ListDistrict.map((val) => {
                          return (
                            <option key={val.maHuyen} value={val.maHuyen}>
                              {val.maHuyen} - {val.tenHuyen}
                            </option>
                          );
                        })}
                    </select>
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
                    <select
                      className="form-control"
                      id="inputGroupSelect01"
                      {...register("MaPhuong", {
                        required: "Không được để trống",
                      })}
                    >
                      <option value="">Chọn phường</option>
                      {ListWard &&
                        ListWard.length > 0 &&
                        ListWard.map((val) => {
                          return (
                            <option key={val.maPhuong} value={val.maPhuong}>
                              {val.maPhuong} - {val.tenPhuong}
                            </option>
                          );
                        })}
                    </select>
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

export default EditCustommer;
