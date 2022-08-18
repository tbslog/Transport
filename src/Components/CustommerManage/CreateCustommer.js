import { useState, useEffect } from "react";
import axios from "axios";
import { useForm } from "react-hook-form";

const CreateCustommer = () => {
  const [IsLoading, SetIsLoading] = useState(true);

  const [submitValue, setSubmitValue] = useState({});
  const {
    register,
    reset,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const onSubmit = (data) => {
    console.log(JSON.stringify(data));
    setSubmitValue(data);
  };

  const [ListProvince, SetListProvince] = useState([]);
  const [ListDistrict, SetListDistrict] = useState([]);
  const [ListWard, SetListWard] = useState([]);

  useEffect(() => {
    reset();
    SetIsLoading(true);

    SetListProvince([]);
    SetListDistrict([]);
    SetListWard([]);
    async function getlistProvince() {
      const listProvince = await axios.get(
        "http://localhost:8088/api/address/ListProvinces"
      );

      if (listProvince && listProvince.data && listProvince.data.length > 0) {
        SetListProvince(listProvince.data);
      }
    }

    getlistProvince();
    SetIsLoading(false);
  }, []);

  const HandleChangeProvince = (val) => {
    try {
      SetIsLoading(true);

      if (val === undefined || val === "") {
        SetListDistrict([]);
        SetListWard([]);
        return;
      }
      async function getListDistrict() {
        const listDistrict = await axios.get(
          `http://localhost:8088/api/address/ListDistricts?ProvinceId=${val}`
        );
        if (listDistrict && listDistrict.data && listDistrict.data.length > 0) {
          SetListDistrict(listDistrict.data);
        } else {
          SetListDistrict([]);
        }
      }
      getListDistrict();
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
      async function GetListWard() {
        const listWard = await axios.get(
          `http://localhost:8088/api/address/ListWards?DistrictId=${val}`
        );

        if (listWard && listWard.data && listWard.data.length > 0) {
          SetListWard(listWard.data);
        } else {
          SetListWard([]);
        }
      }
      GetListWard();
      SetIsLoading(false);
    } catch (error) {}
  };

  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">Form Thêm Mới Khách Hàng</h3>
          <div>{IsLoading === true && <div>Loading...</div>}</div>
        </div>

        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="card-body">
            <div className="form-group">
              <label htmlFor="MaKH">Mã khách hàng</label>
              <input
                type="text"
                className="form-control"
                id="MaKH"
                placeholder="Nhập mã khách hàng"
                {...register("MaKH", {
                  required: "Không được để trống",
                  maxLength: {
                    value: 50,
                    message: "Không được vượt quá 50 ký tự",
                  },
                  minLength: {
                    value: 10,
                    message: "Không được ít hơn 10 ký tự",
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
                    value: 5,
                    message: "Không được ít hơn 10 ký tự",
                  },
                })}
              />
              {errors.TenKH && (
                <span className="text-danger">{errors.TenKH.message}</span>
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
                    value: 5,
                    message: "Không được ít hơn 10 ký tự",
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
                    message: "Không được vượt quá 50 ký tự",
                  },
                  minLength: {
                    value: 5,
                    message: "Không được ít hơn 10 ký tự",
                  },
                  pattern: {
                    message: "Không phải số điện thoại",
                    value: /\S+@\S+\.\S+/,
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
                    value: 5,
                    message: "Không được ít hơn 10 ký tự",
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
                        value: 50,
                        message: "Không được vượt quá 50 ký tự",
                      },
                      minLength: {
                        value: 5,
                        message: "Không được ít hơn 10 ký tự",
                      },
                    })}
                  />
                  {errors.SoNha && (
                    <span className="text-danger">{errors.SoNha.message}</span>
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
                    <option selected value="">
                      Chọn tỉnh...
                    </option>
                    {ListProvince &&
                      ListProvince.length > 0 &&
                      ListProvince.map((val) => {
                        return (
                          <option key={val.maTinh} value={val.maTinh}>
                            {val.tenTinh}
                          </option>
                        );
                      })}
                  </select>
                  {errors.maTinh && (
                    <span className="text-danger">{errors.maTinh.message}</span>
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
                    <option selected value="">
                      Chọn Huyện...
                    </option>
                    {ListDistrict &&
                      ListDistrict.length > 0 &&
                      ListDistrict.map((val) => {
                        return (
                          <option key={val.maHuyen} value={val.maHuyen}>
                            {val.tenHuyen}
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
                    <option selected value="">
                      Chọn phường
                    </option>
                    {ListWard &&
                      ListWard.length > 0 &&
                      ListWard.map((val) => {
                        return (
                          <option key={val.maPhuong} value={val.maPhuong}>
                            {val.tenPhuong}
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
                type="button"
                onClick={() => reset()}
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
      </div>
    </>
  );
};

export default CreateCustommer;
