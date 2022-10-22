import { useState, useEffect } from "react";
import { getData, postData, getDataCustom } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import "../../Css/UploadFile.scss";
import Select from "react-select";

const CreateCustommer = (props) => {
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

  const [listStatusType, setListStatusType] = useState([]);
  const [listCustomerGroup, setListCustomerGroup] = useState([]);
  const [listCustomerType, setListCustomerType] = useState([]);

  const [ListProvince, SetListProvince] = useState([]);
  const [ListDistrict, SetListDistrict] = useState([]);
  const [ListWard, SetListWard] = useState([]);

  const onSubmit = async (data, e) => {
    SetIsLoading(true);

    const post = await postData("Customer/CreateCustomer", {
      maKh: data.MaKH.toUpperCase(),
      tenKh: data.TenKH,
      maSoThue: data.MST,
      sdt: data.SDT,
      email: data.Email,
      trangThai: data.TrangThai,
      nhomKh: data.NhomKH,
      loaiKh: data.LoaiKH,
      address: {
        tenDiaDiem: "",
        maQuocGia: 1,
        maTinh: data.MaTinh.value,
        maHuyen: data.MaHuyen.value,
        maPhuong: data.MaPhuong.value,
        soNha: data.SoNha,
        diaChiDayDu: "",
        maGps: data.GPS,
        maLoaiDiaDiem: data.MaLoaiDiaDiem.value,
      },
    });
    if (post === 1) {
      props.getListUser(1);
      reset();
    }

    SetIsLoading(false);
  };

  useEffect(() => {
    if (props.listCusGroup && props.listCusType) {
      setListCustomerGroup(props.listCusGroup);
      setListCustomerType(props.listCusType);
    }
  }, [props.listCusGroup, props.listCusType]);

  useEffect(() => {
    SetIsLoading(true);

    SetListProvince([]);
    SetListDistrict([]);
    SetListWard([]);

    (async () => {
      const getlistProvince = await getData("address/GetListProvinces");

      if (getlistProvince && getlistProvince.length > 0) {
        var obj = [];
        getlistProvince.map((val) => {
          obj.push({
            value: val.maTinh,
            label: val.maTinh + " - " + val.tenTinh,
          });
        });

        SetListProvince(obj);
        setListStatusType(props.listStatus);
      }
    })();

    SetIsLoading(false);
  }, []);

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

      if (val.value === undefined || val.value === "") {
        SetListWard([]);
        return;
      }

      (async () => {
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
    setValue("MaTinh", null);
    setValue("MaHuyen", null);
    setValue("MaPhuong", null);
  };

  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">Form Thêm Mới Khách Hàng</h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="MaKH">Mã khách hàng</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="MaKH"
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
                </div>
                <div className="col-sm">
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
                          message:
                            "Tên khách hàng không được chứa ký tự đặc biệt",
                        },
                      })}
                    />
                    {errors.TenKH && (
                      <span className="text-danger">
                        {errors.TenKH.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>

              <div className="row">
                <div className="col-sm">
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
                      <span className="text-danger">
                        {errors.Email.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
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
                          value:
                            /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
                          message: "Mã số thuế chỉ được chứa ký tự là số",
                        },
                      })}
                    />
                    {errors.MST && (
                      <span className="text-danger">{errors.MST.message}</span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
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
                          value:
                            /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
                          message: "Số điện thoại chỉ được chứa ký tự là số",
                        },
                      })}
                    />
                    {errors.SDT && (
                      <span className="text-danger">{errors.SDT.message}</span>
                    )}
                  </div>
                </div>
              </div>

              <div className="row">
                <div className="col-sm">
                  {" "}
                  <div className="form-group">
                    <label htmlFor="NhomKH">Nhóm khách hàng</label>
                    <select
                      className="form-control"
                      {...register("NhomKH", {
                        required: "Không được để trống",
                      })}
                    >
                      <option value="">Chọn Nhóm khách hàng</option>
                      {listCustomerGroup &&
                        listCustomerGroup.map((val) => {
                          return (
                            <option value={val.maNhomKh} key={val.maNhomKh}>
                              {val.tenNhomKh}
                            </option>
                          );
                        })}
                    </select>
                    {errors.NhomKH && (
                      <span className="text-danger">
                        {errors.NhomKH.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  {" "}
                  <div className="form-group">
                    <label htmlFor="LoaiKH">Phân Loại Đối Tác</label>
                    <select
                      className="form-control"
                      {...register("LoaiKH", {
                        required: "Không được để trống",
                      })}
                    >
                      <option value="">Chọn Phân Loại Đối Tác</option>
                      {listCustomerType &&
                        listCustomerType.map((val) => {
                          return (
                            <option value={val.maLoaiKh} key={val.maLoaiKh}>
                              {val.tenLoaiKh}
                            </option>
                          );
                        })}
                    </select>
                    {errors.LoaiKH && (
                      <span className="text-danger">
                        {errors.LoaiKH.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>

              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="Tinh">Phân Loại Địa Điểm</label>
                    <Controller
                      name="MaLoaiDiaDiem"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={props.listTypeAddress}
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
                    <label htmlFor="GPS">Tọa Độ GPS</label>
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
                </div>
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
              </div>
              <div className="form-group">
                <label htmlFor="TrangThai">Trạng thái</label>
                <select
                  className="form-control"
                  {...register("TrangThai", {
                    required: "Không được để trống",
                  })}
                >
                  <option value="">Chọn trạng thái</option>
                  {listStatusType &&
                    listStatusType.map((val) => {
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

export default CreateCustommer;
