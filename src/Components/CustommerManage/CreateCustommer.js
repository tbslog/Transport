import { useState, useEffect } from "react";
import { postData } from "../Common/FuncAxios";
import { useForm } from "react-hook-form";
import "../../Css/UploadFile.scss";
import { Watch } from "react-loader-spinner";
import LoadingPage from "../Common/Loading/LoadingPage";

const CreateCustommer = (props) => {
  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    watch,
    reset,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    MaKH: {
      required: "Không được để trống",
      maxLength: {
        value: 50,
        message: "Không được vượt quá 50 ký tự",
      },
      minLength: {
        value: 7,
        message: "Không được ít hơn 7 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    Chuoi: {
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
    TenKH: {
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
    },
    Email: {
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
    },
    MST: {
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
    },
    SDT: {
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
    },
  };

  const [listStatusType, setListStatusType] = useState([]);
  const [listCustomerGroup, setListCustomerGroup] = useState([]);
  const [listCustomerType, setListCustomerType] = useState([]);

  useEffect(() => {
    if (props.listCusGroup && props.listCusType) {
      SetIsLoading(true);
      setListCustomerGroup(props.listCusGroup);
      setListCustomerType(props.listCusType);
      setListStatusType(props.listStatus);

      SetIsLoading(false);
    }
  }, [props.listCusGroup, props.listCusType, props.listStatus]);

  const handleResetClick = () => {
    reset();
  };

  const onSubmit = async (data, e) => {
    SetIsLoading(true);
    const post = await postData("Customer/CreateCustomer", {
      MaKH: data.MaKH,
      tenKh: data.TenKH,
      maSoThue: data.MST,
      sdt: data.SDT,
      email: data.Email,
      trangThai: data.TrangThai,
      nhomKh: data.NhomKH,
      loaiKh: data.LoaiKH,
      chuoi: data.Chuoi,
    });
    if (post === 1) {
      props.getListUser();
      reset();
    }

    SetIsLoading(false);
  };

  return (
    <>
      <div className="card card-primary">
        <div>
          {IsLoading === true && (
            <>
              <LoadingPage></LoadingPage>
            </>
          )}
        </div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="LoaiKH">Phân Loại Đối Tác(*)</label>
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

                {watch("LoaiKH") && watch("LoaiKH") === "KH" && (
                  <>
                    <div className="col-sm">
                      <div className="form-group">
                        <label htmlFor="Chuoi">Chuỗi(*)</label>
                        <input
                          type="text"
                          className="form-control"
                          id="Chuoi"
                          placeholder="Nhập tên chuỗi"
                          {...register("Chuoi", Validate.Chuoi)}
                        />
                        {errors.Chuoi && (
                          <span className="text-danger">
                            {errors.Chuoi.message}
                          </span>
                        )}
                      </div>
                    </div>
                  </>
                )}
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="NhomKH">Nhóm khách hàng(*)</label>
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
              </div>
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="MaKH">Mã Đối Tác(*)</label>
                    <input
                      type="text"
                      className="form-control"
                      id="MaKH"
                      placeholder="Nhập tên đối tác"
                      {...register("MaKH", Validate.MaKH)}
                    />
                    {errors.MaKH && (
                      <span className="text-danger">{errors.MaKH.message}</span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="TenKH">Tên đối tác(*)</label>
                    <input
                      type="text"
                      className="form-control"
                      id="TenKH"
                      placeholder="Nhập tên đối tác"
                      {...register("TenKH", Validate.TenKH)}
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
                    <label htmlFor="Email">Địa chỉ Email(*)</label>
                    <input
                      type="text "
                      className="form-control"
                      id="Email"
                      placeholder="Nhập địa chỉ Email"
                      {...register("Email", Validate.Email)}
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
                    <label htmlFor="MST">Mã số thuế(*)</label>
                    <input
                      type="text "
                      className="form-control"
                      id="MST"
                      placeholder="Nhập mã số thuế"
                      {...register("MST", Validate.MST)}
                    />
                    {errors.MST && (
                      <span className="text-danger">{errors.MST.message}</span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="SDT">Số điện thoại(*)</label>
                    <input
                      type="text"
                      className="form-control"
                      id="SDT"
                      placeholder="Nhập số điện thoại"
                      {...register("SDT", Validate.SDT)}
                    />
                    {errors.SDT && (
                      <span className="text-danger">{errors.SDT.message}</span>
                    )}
                  </div>
                </div>
              </div>

              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TrangThai">Trạng thái(*)</label>
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
